﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WebLinter
{
    public abstract class LinterBase
    {
        public LinterBase(ISettings settings)
        {
            Settings = settings;
        }

        public LintingResult Run(params string[] files)
        {
            Result = new LintingResult(files);

            if (!IsEnabled)
                return Result;

            List<FileInfo> fileInfos = new List<FileInfo>();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                {
                    Result.Errors.Add(new LintingError(fileInfo.FullName) { Message = "The file doesn't exist" });
                    return Result;
                }

                fileInfos.Add(fileInfo);
            }

            return Lint(fileInfos.ToArray());
        }

        protected virtual LintingResult Lint(params FileInfo[] files)
        {
            string args = GetArguments(files);
            string output, error;
            RunProcess($"{Name}.cmd", out output, out error, args, files);

            if (!string.IsNullOrEmpty(output))
            {
                ParseErrors(output);
            }
            else if (!string.IsNullOrEmpty(error))
            {
                Result.Errors.Add(new LintingError(files.First().FullName) { Message = error });
            }

            return Result;
        }

        public string Name { get; set; }

        public string HelpLinkFormat { get; set; }

        protected virtual string ConfigFileName { get; set; }

        protected virtual string ErrorMatch { get; set; }

        protected virtual bool IsEnabled { get; set; }

        protected ISettings Settings { get; }

        protected LintingResult Result { get; private set; }

        protected void RunProcess(string command, out string output, out string error, string arguments = "", params FileInfo[] files)
        {
            string fileArg = string.Join(" ", files.Select(f => $"\"{f.FullName}\""));

            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = FindWorkingDirectory(files[0]),
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{Path.Combine(LinterFactory.ExecutionPath, $"node_modules\\.bin\\{command}")}\" {arguments} {fileArg}\"",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            ModifyPathVariable(start);

            Process p = Process.Start(start);
            var stdout = p.StandardOutput.ReadToEndAsync();
            var stderr = p.StandardError.ReadToEndAsync();
            p.WaitForExit();

            output = stdout.Result.Trim();
            error = stderr.Result.Trim();
        }

        private static void ModifyPathVariable(ProcessStartInfo start)
        {
            string path = start.EnvironmentVariables["PATH"];

            string toolsDir = Environment.GetEnvironmentVariable("VS140COMNTOOLS");

            if (Directory.Exists(toolsDir))
            {
                string parent = Directory.GetParent(toolsDir).Parent.FullName;
                path += ";" + Path.Combine(parent, @"IDE\Extensions\Microsoft\Web Tools\External");
            }

            start.UseShellExecute = false;
            start.EnvironmentVariables["PATH"] = path;
        }

        protected virtual string GetArguments(FileInfo[] files)
        {
            return string.Empty;
        }

        protected virtual string FindWorkingDirectory(FileInfo file)
        {
            var dir = file.Directory;

            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, ConfigFileName);
                if (File.Exists(rc))
                    return dir.FullName;

                dir = dir.Parent;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        protected abstract void ParseErrors(string output);

        public override bool Equals(Object obj)
        {
            LinterBase lb = obj as LinterBase;
            if (lb == null)
                return false;
            else
                return Name.Equals(lb.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

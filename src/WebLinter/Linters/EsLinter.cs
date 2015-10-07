﻿using System.IO;
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        public EsLinter(ISettings settings) : base(settings)
        {
            Name = "ESLint";
            ConfigFileName = ".eslintrc";
            ErrorMatch = "Error";
            IsEnabled = Settings.ESLintEnable;
            HelpLinkFormat = "http://eslint.org/docs/rules/{0}";
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["filePath"].Value<string>();

                foreach (JObject error in obj["messages"])
                {
                    var le = new LintingError(fileName);
                    le.Message = error["message"].Value<string>();
                    le.LineNumber = error["line"].Value<int>() - 1;
                    le.ColumnNumber = error["column"].Value<int>() - 1;
                    le.IsError = error["severity"].Value<int>() == 2;
                    le.ErrorCode = error["ruleId"].Value<string>();
                    le.Provider = this;
                    Result.Errors.Add(le);
                }
            }
        }
    }
}

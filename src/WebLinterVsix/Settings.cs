﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using WebLinter;

namespace WebLinterVsix
{
    public class Settings : DialogPage, ISettings
    {
        public Settings()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            // General
            IgnoreFolderNames = @"\node_modules\,\bower_components\,\typings\,\lib\,.min.";
            IgnoreNestedFiles = true;

            // Linters
            CoffeeLintEnable = true;
            CssLintEnable = true;
            ESLintEnable = true;
            TSLintEnable = true;
        }

        public override void ResetSettings()
        {
            SetDefaults();
            base.ResetSettings();
        }

        // General
        [Category("General")]
        [DisplayName("Ignore patterns")]
        [Description("A comma-separated list of strings. Any file containing one of the strings in the path will be ignored.")]
        [DefaultValue(@"\node_modules\,\bower_components\,\typings\,\lib\,.min.")]
        public string IgnoreFolderNames { get; set; }

        [Category("General")]
        [DisplayName("Ignore nested files")]
        [Description("Nested files are files that are nested under other files in Solution Explorer.")]
        [DefaultValue(true)]
        public bool IgnoreNestedFiles { get; set; }

        // Linters
        [Category("Linters")]
        [DisplayName("Enable CoffeeLint")]
        [Description("CoffeeLint is a linter for CoffeeScript files")]
        [DefaultValue(true)]
        public bool CoffeeLintEnable { get; set; }

        [Category("Linters")]
        [DisplayName("Enable CSS Lint")]
        [Description("CSS Lint is a linter for CSS files")]
        [DefaultValue(true)]
        public bool CssLintEnable { get; set; }

        [Category("Linters")]
        [DisplayName("Enable ESLint")]
        [Description("ESLint is a linter JavaScript and JSX files")]
        [DefaultValue(true)]
        public bool ESLintEnable { get; set; }

        [Category("Linters")]
        [DisplayName("Enable TSLint")]
        [Description("TSLint is a linter for TypeScript files")]
        [DefaultValue(true)]
        public bool TSLintEnable { get; set; }

        public IEnumerable<string> GetIgnorePatterns()
        {
            var raw = IgnoreFolderNames.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pattern in raw)
            {
                yield return pattern;
            }
        }
    }
}

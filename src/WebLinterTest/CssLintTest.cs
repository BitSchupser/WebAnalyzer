﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class CsslintTest
    {
        [TestMethod, TestCategory("CssLint")]
        public void Standard()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/csslint/a.css");
            var first = result.First();
            Assert.IsTrue(first.HasErrors);
            Assert.IsFalse(result.First().Errors.First().IsError, "Severity is not 'warning'");
            Assert.IsFalse(string.IsNullOrEmpty(first.Errors.First().FileName));
            Assert.AreEqual(1, first.Errors.Count);
        }

        [TestMethod, TestCategory("CssLint")]
        public void Multiple()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/csslint/a.css", "../../artifacts/csslint/b.css");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName));
            Assert.AreEqual(2, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("CssLint")]
        public void FileNotExist()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/csslint/doesntexist.css");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}

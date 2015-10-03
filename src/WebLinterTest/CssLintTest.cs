﻿using System;
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
            var result = LinterFactory.Lint("../../artifacts/csslint/a.css");
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(3, result.Errors.Count);
        }

        [TestMethod, TestCategory("CssLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/csslint/doesntexist.css");
            Assert.IsTrue(result.HasErrors);
        }
    }
}

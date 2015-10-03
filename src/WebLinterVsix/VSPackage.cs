﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebLinterVsix
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", WebLinter.Constants.VERSION, IconResourceID = 400)]
    [ProvideOptionPage(typeof(Settings), "Web", "Linters", 101, 111, true, new[] { "jshint", "tslint", "coffeelint", "csslint" }, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(Constants.PRODUCT_ID)]
    public sealed class VSPackage : Package
    {
        public static DTE2 Dte;
        public static Dispatcher Dispatcher;
        public static Package Package;
        public static Settings Settings;

        protected override void Initialize()
        {
            Dte = GetService(typeof(DTE)) as DTE2;
            Dispatcher = Dispatcher.CurrentDispatcher;
            Package = this;
            Settings = (Settings)GetDialogPage(typeof(Settings));

            Logger.Initialize(this, Constants.VSIX_NAME);

            base.Initialize();
        }
    }
}

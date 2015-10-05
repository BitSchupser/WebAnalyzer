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
    [ProvideOptionPage(typeof(Settings), "Web", "Linters", 101, 111, true, new[] { "eslint", "tslint", "coffeelint", "csslint" }, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(PackageGuids.guidVSPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class WebLinterPackage : Package
    {
        public static DTE2 Dte;
        public static Dispatcher Dispatcher;
        public static Package Package;
        public static Settings Settings;

        protected override void Initialize()
        {
            Package = this;
            Dte = GetService(typeof(DTE)) as DTE2;
            Dispatcher = Dispatcher.CurrentDispatcher;
            Settings = (Settings)GetDialogPage(typeof(Settings));

            Logger.Initialize(this, Constants.VSIX_NAME);
            LintFilesCommand.Initialize(this);
            CleanErrorsCommand.Initialize(this);
            EditConfigFilesCommand.Initialize(this);
            ResetConfigFilesCommand.Initialize(this);

            base.Initialize();
        }
    }

    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    public sealed class WebLinterInitPackage : Package
    {
        protected override void Initialize()
        {
            // Delay execution until VS is idle.
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                // Then execute in a background thread.
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        WebLinter.LinterFactory.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}

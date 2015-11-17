//------------------------------------------------------------------------------
// <copyright file="ExpressionViewerControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Extension
{
    using Microsoft.CodeAnalysis.MSBuild;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Windows.Threading;
    using System.Composition;
    using Microsoft.VisualStudio.Shell;


    /// <summary>
    /// Interaction logic for ExpressionViewerControl.
    /// </summary>
    public partial class ExpressionViewerControl : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionViewerControl"/> class.
        /// </summary>
        public ExpressionViewerControl()
        {
            this.InitializeComponent();
        }

        public void SetText(string content)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
               {
                   textBox.Text = content;
               }));
        }

        private void ExpressionViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Setup(GlobalServiceProvider, this);
        }

        private void ExpressionViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            Finish();
        }

        // Need a cleaner way to initialize this property
        public static IServiceProvider GlobalServiceProvider { get; set; }

        private Runner Runner { get ; set; }

        /// <summary>
        /// Needs to be called everytime the control is made visible
        /// </summary>
        private void Setup(IServiceProvider serviceProvider, ExpressionViewerControl view)
        {
            var sourceMonitor = new SourceMonitor(serviceProvider);
            var viewController = new ExpressionViewController(view);
            var viewGenerator = new ViewGenerator();
            Runner = new Runner(sourceMonitor, viewController, viewGenerator);
        }

        /// <summary>
        /// Needs to be called everythime the control is hidden
        /// </summary>
        private void Finish()
        {
            if (Runner != null)
            {
                Runner.Dispose();
                Runner = null;
            }
        }
    }
}
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

        // Need a cleaner way to initialize this property
        public static IServiceProvider GlobalServiceProvider { get; set; }

        private void ExpressionViewer_Initialized(object sender, EventArgs e)
        {
            Setup(GlobalServiceProvider, this);
        }

        private void Setup(IServiceProvider serviceProvider, ExpressionViewerControl view)
        {
            var sourceMonitor = new SourceMonitor(serviceProvider);
            var viewController = new ExpressionViewController(view);
            var viewGenerator = new ViewGenerator();
            var runner = new Runner(sourceMonitor, viewController, viewGenerator);
        }

        private void ExpressionViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
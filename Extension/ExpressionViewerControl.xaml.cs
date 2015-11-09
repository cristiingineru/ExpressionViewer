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

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            var generator = new ViewGenerator();

            var solutionPath = @"..\..\..\TestSolutions\ManualTesting\ManualTesting.sln";
            textBox.Text = await generator.GenerateViewAsync(solutionPath);
        }
    }
}
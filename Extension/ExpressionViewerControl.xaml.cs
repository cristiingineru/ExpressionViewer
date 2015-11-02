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


    /// <summary>
    /// Interaction logic for ExpressionViewerControl.
    /// </summary>
    public partial class ExpressionViewerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionViewerControl"/> class.
        /// </summary>
        public ExpressionViewerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var toBeDisplayed = "Error";

            using (var tempDirectory = new TempDirectory())
            {
                try
                {
                    var workspace2 = MSBuildWorkspace.Create();
                    var task2 = workspace2.OpenSolutionAsync(@"..\..\..\ManualTesting\ManualTesting.sln");
                    task2.Wait();
                    var solution2 = task2.Result;
                    var newSolution2 = solution2.GetIsolatedSolution();

                    var expressionSearcher = new ExpressionSearcher();
                    var expressionBuilder = new ExpressionBuilder();
                    var sourceTask = expressionSearcher.FindSource(newSolution2);
                    sourceTask.Wait();
                    var source = sourceTask.Result;
                    var targetTask2 = expressionSearcher.FindTarget(newSolution2);
                    targetTask2.Wait();
                    var target2 = targetTask2.Result;
                    var instrumentationTask = expressionBuilder.BuildWrapper(source);
                    instrumentationTask.Wait();
                    var instrumentation = instrumentationTask.Result;

                    var newCompilation = expressionSearcher.ReplaceNodeInCompilation(target2.Compilation, target2.Node, instrumentation);

                    var instrumentedDll = Path.Combine(tempDirectory.FullName, Path.GetRandomFileName() + ".dll");
                    var emitResult = newCompilation.Emit(instrumentedDll);
                    var assemblyRunner = new AssemblyRunner();
                    var result = assemblyRunner.Run(instrumentedDll, "WrapperClass", "WrapperMethod");

                    toBeDisplayed = result;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    toBeDisplayed = exception.ToString();
                }
            }

            textBox.Text = toBeDisplayed;
        }
    }
}
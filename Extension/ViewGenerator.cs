using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class ViewGenerator : IViewGenerator
    {
        public async Task<string> GenerateViewAsync(string solutionPath, string activeDocument = null, int cursorPosition = -1)
        {
            var view = string.Empty;

            using (var tempDirectory = new TempDirectory())
            {
                try
                {
                    var workspace = MSBuildWorkspace.Create();
                    var solution = await workspace.OpenSolutionAsync(solutionPath);
                    var newSolution = solution.GetIsolatedSolution();

                    var expressionSearcher = new ExpressionSearcher();
                    var expressionBuilder = new ExpressionBuilder();
                    var source = await expressionSearcher.FindSource(newSolution, activeDocument, cursorPosition);
                    var target = await expressionSearcher.FindTarget(newSolution);
                    var instrumentation = await expressionBuilder.BuildWrapper(source);

                    var newCompilation = expressionSearcher.InsertNodeInCompilation(target.Compilation, target.Node, instrumentation);

                    var instrumentedDll = Path.Combine(tempDirectory.FullName, Path.GetRandomFileName() + ".dll");
                    var emitResult = newCompilation.Emit(instrumentedDll);
                    if (emitResult.Success)
                    {
                        using (var assemblyRunner = new AssemblyRunner())
                        {
                            var result = assemblyRunner.Run(instrumentedDll, "WrapperClass", "WrapperMethod");
                            view = result;
                        }
                    }
                    else
                    {
                        view = string.Join(Environment.NewLine, emitResult.Diagnostics);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                    view = exception.ToString();
                }
            }

            return view;
        }
    }
}

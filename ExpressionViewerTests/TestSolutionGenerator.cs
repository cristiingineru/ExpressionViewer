using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    class TestSolutionGenerator
    {
        public static Solution EmptySolution()
        {
            var id = SolutionId.CreateNewId();
            var version = new VersionStamp();
            var solutionInfo = SolutionInfo.Create(id, version);
            var workspace = new AdhocWorkspace();
            var solution = workspace.AddSolution(solutionInfo);

            return solution;
        }

        public static Solution DefaultSolution()
        {
            var defaultContent = @"
            namespace SimpleNamespace {
                public class SimpleClass() {
                    public void Do() {
                        var x = ""text"".ToString().ToString();
                    }
                }
            }";
            var defaultSolution = SingleFileSolution(defaultContent);

            return defaultSolution;
        }

        public static Solution SingleFileSolution(string fileContent)
        {
            var id = SolutionId.CreateNewId();
            var version = new VersionStamp();
            var solutionInfo = SolutionInfo.Create(id, version);
            var workspace = new AdhocWorkspace();
            var solution = workspace.AddSolution(solutionInfo);

            var projectId = ProjectId.CreateNewId();
            var projectInfo = NewProjectInfo(projectId);
            solution = solution.AddProject(projectInfo);
            var project = solution.GetProject(projectId);

            var root = CSharpSyntaxTree.ParseText(fileContent).GetRoot();
            var documentName = DefaultActiveDocument();
            var document = project.AddDocument(documentName, root);

            return document.Project.Solution;
        }

        public static ProjectInfo NewProjectInfo(ProjectId projectId)
        {
            var version = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, version, "no name", "assembly.dll", "C#");

            return projectInfo;
        }

        public static string DefaultActiveDocument()
        {
            return "file.cs";
        }
    }
}

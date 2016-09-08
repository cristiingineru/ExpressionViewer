using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    public static class SingleFileSolution
    {
        public static Solution New(string fileContent)
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
            var documentName = ActiveDocument();
            //var document = project.AddDocument(documentName, root);

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            project = project.AddMetadataReferences(
        new[]
                    {
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                        MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
                    });

            //return document.Project.Solution;

            var document = project.AddDocument(documentName, root);
            return project.Solution;
        }

        private static ProjectInfo NewProjectInfo(ProjectId projectId)
        {
            var version = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, version, "no name", "assembly.dll", "C#");

            return projectInfo;
        }

        public static string ActiveDocument()
        {
            return "file.cs";
        }
    }
}

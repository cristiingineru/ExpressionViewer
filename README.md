# ExpressionViewer
Visual Studio extension for evaluating expressions at edit time

# How to play with it
TBD

# How to extend it
1. Install Visual Studio 2015 Comunity
2. Start VS -> File -> New Project -> Extensibility -> Download the .NET Compiler Platform SDK
3. Clone this repo
4. Open ExpressionViewer.sln
5. Tools -> NuGet Package Manager -> Package Manager Console -> make sure the default project is Extension
6. Install base Roslyn package: Install-Package Microsoft.CodeAnalysis.Common -Version 1.0.0
7. Install Roslyn for C# projects: Install-Package Microsoft.CodeAnalysis.CSharp.Workspaces -Version 1.0.0
8. Change the default project to ExpressionViewerTests
9. Install the same two packages again
10. Just press the Run button or run tests

# Useful tools
## Syntax Visualizer
A small dialog that can be used to interactively inspect the syntax tree of your C# code. It cames with .NET Compiler Platform SDK.

## Roslyn Quoter
It can be used to generate code that builds a C# syntax tree.
http://roslynquoter.azurewebsites.net/

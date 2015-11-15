# ExpressionViewer
Visual Studio extension for evaluating expressions at edit time

![preview](https://raw.githubusercontent.com/cristiingineru/ExpressionViewer/master/images/preview.png)


# How to play with it
1. Install Visual Studio 2015 Comunity
2. Tools -> Extensions and Updates... -> Online -> search for "expression viewer"
  * or look in the Visual Studio galery: https://visualstudiogallery.msdn.microsoft.com/9e4feaed-0365-49ef-905a-66bd0e414618

# How to extend it
1. Install Visual Studio 2015 Comunity
2. Start VS -> File -> New Project -> Extensibility
  * Download the .NET Compiler Platform SDK
3. Clone this repo
  * git clone https://github.com/cristiingineru/ExpressionViewer.git
4. Open ExpressionViewer.sln
5. Tools -> NuGet Package Manager -> Package Manager Console
  * make sure the default project is Extension
6. Install base Roslyn package
  * Install-Package Microsoft.CodeAnalysis.Common -Version 1.0.0
7. Install Roslyn for C# projects
  * Install-Package Microsoft.CodeAnalysis.CSharp.Workspaces -Version 1.0.0
8. Change the default project to ExpressionViewerTests
9. Install the same two packages again
10. Just press the Run button or run tests

# Useful tools
## Syntax Visualizer
A small dialog that can be used to interactively inspect the syntax tree of your C# code. It cames with .NET Compiler Platform SDK.

## Roslyn Quoter
It can be used to generate code that builds a C# syntax tree. It's usefull when a large piece of code needs to be generated or when someone is not familiar with the Roslyn API.
http://roslynquoter.azurewebsites.net/

## ILSpy
.NET assembly decompiler and browser.
http://ilspy.net/

//------------------------------------------------------------------------------
// <copyright file="ExpressionViewer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Extension
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using System.Composition;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("0b046503-dc9e-4f16-a73b-78887829301f")]
    public class ExpressionViewer : ToolWindowPane
    {
        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionViewer"/> class.
        /// </summary>
        public ExpressionViewer() : base(null)
        {
            this.Caption = "ExpressionViewer";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ExpressionViewerControl();
        }
    }
}

using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public struct AjaxMinSymbolForJavaScript : IJavaScriptSymbol
	{
		public int ScriptStartLine { get; set; }

		public int ScriptStartColumn { get; set; }

		public int ScriptEndLine { get; set; }

		public int ScriptEndColumn { get; set; }

		public int SourceStartLine { get; set; }

		public int SourceStartColumn { get; set; }

		public int SourceEndLine { get; set; }

		public int SourceEndColumn { get; set; }

		public uint SourceFileId { get; set; }

		public int ParentSymbolIndex { get; set; }

		public int FunctionNameIndex { get; set; }
	}
}

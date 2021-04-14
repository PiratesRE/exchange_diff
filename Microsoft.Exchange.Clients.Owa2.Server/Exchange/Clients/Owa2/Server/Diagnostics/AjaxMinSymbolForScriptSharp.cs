using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public struct AjaxMinSymbolForScriptSharp : IJavaScriptSymbol
	{
		public int ScriptStartLine { get; set; }

		public int ScriptStartColumn { get; set; }

		public int ScriptEndLine { get; set; }

		public int ScriptEndColumn { get; set; }

		public int SourceStartPosition { get; set; }

		public int SourceEndPosition { get; set; }

		public uint SourceFileId { get; set; }

		public int ParentSymbolIndex { get; set; }
	}
}

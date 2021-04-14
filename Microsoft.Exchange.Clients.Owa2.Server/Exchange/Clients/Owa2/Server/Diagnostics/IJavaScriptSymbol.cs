using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public interface IJavaScriptSymbol
	{
		int ScriptStartLine { get; }

		int ScriptStartColumn { get; }

		int ScriptEndLine { get; }

		int ScriptEndColumn { get; }

		uint SourceFileId { get; set; }

		int ParentSymbolIndex { get; set; }
	}
}

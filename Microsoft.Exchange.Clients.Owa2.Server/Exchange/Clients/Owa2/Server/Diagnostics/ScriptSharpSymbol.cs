using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public struct ScriptSharpSymbol
	{
		public int ScriptStartPosition { get; set; }

		public int ScriptEndPosition { get; set; }

		public int SourceStartLine { get; set; }

		public uint SourceFileId { get; set; }

		public int FunctionNameIndex { get; set; }

		public int ParentSymbol { get; set; }
	}
}

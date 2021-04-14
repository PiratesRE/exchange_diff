using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal struct ScriptSharpSymbolWrapper : IJavaScriptSymbol
	{
		public ScriptSharpSymbolWrapper(ScriptSharpSymbol symbol)
		{
			this.symbol = symbol;
		}

		public int ScriptStartLine
		{
			get
			{
				return 0;
			}
		}

		public int ScriptStartColumn
		{
			get
			{
				return this.symbol.ScriptStartPosition;
			}
		}

		public int ScriptEndLine
		{
			get
			{
				return 0;
			}
		}

		public int ScriptEndColumn
		{
			get
			{
				return this.symbol.ScriptEndPosition;
			}
		}

		public uint SourceFileId
		{
			get
			{
				return this.symbol.SourceFileId;
			}
			set
			{
				this.symbol.SourceFileId = value;
			}
		}

		public int ParentSymbolIndex
		{
			get
			{
				return this.symbol.ParentSymbol;
			}
			set
			{
				this.symbol.ParentSymbol = value;
			}
		}

		public int FunctionNameIndex
		{
			get
			{
				return this.symbol.FunctionNameIndex;
			}
			set
			{
				this.symbol.FunctionNameIndex = value;
			}
		}

		public ScriptSharpSymbol InnerSymbol
		{
			get
			{
				return this.symbol;
			}
			set
			{
				this.symbol = value;
			}
		}

		private ScriptSharpSymbol symbol;
	}
}

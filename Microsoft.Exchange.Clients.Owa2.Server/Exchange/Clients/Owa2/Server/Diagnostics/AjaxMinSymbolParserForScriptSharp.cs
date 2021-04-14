using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class AjaxMinSymbolParserForScriptSharp : AjaxMinSymbolParser<AjaxMinSymbolForScriptSharp>
	{
		protected override AjaxMinSymbolForScriptSharp ParseSymbolData(string[] columns, ClientWatsonFunctionNamePool functionNamePool)
		{
			return new AjaxMinSymbolForScriptSharp
			{
				ScriptStartLine = int.Parse(columns[0]),
				ScriptStartColumn = int.Parse(columns[1]),
				ScriptEndLine = int.Parse(columns[2]),
				ScriptEndColumn = int.Parse(columns[3]),
				SourceStartPosition = int.Parse(columns[4]),
				SourceEndPosition = int.Parse(columns[5]),
				SourceFileId = uint.Parse(columns[10])
			};
		}
	}
}

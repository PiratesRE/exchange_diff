using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal interface IConsolidationSymbolsMap
	{
		bool SkipChecksumValidation { get; set; }

		bool Search(string scriptName, int line, int column, out string sourceFile, out Tuple<int, int> preConsolidationPosition);

		bool HasSymbolsLoadedForScript(string scriptName);
	}
}

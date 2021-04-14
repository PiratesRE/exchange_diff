using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public interface IJavaScriptSymbolsMap<T> where T : IJavaScriptSymbol
	{
		bool Search(string scriptName, T javaScriptSymbol, out T symbolFound);

		string GetSourceFilePathFromId(uint id);

		string GetFunctionName(int index);

		bool HasSymbolsLoadedForScript(string scriptName);
	}
}

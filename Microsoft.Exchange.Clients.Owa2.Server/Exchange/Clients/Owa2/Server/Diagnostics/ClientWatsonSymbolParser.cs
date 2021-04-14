using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal abstract class ClientWatsonSymbolParser<T> where T : IJavaScriptSymbol
	{
		public abstract bool TryParseSymbolData(string value, ClientWatsonFunctionNamePool functionNamePool, out T javaScriptSymbol);

		public virtual bool ExtractScriptPackageName(string symbolFilePath, string scriptFilePath, out string scriptFileName)
		{
			bool result;
			try
			{
				scriptFileName = Path.GetFileNameWithoutExtension(scriptFilePath);
				if (string.IsNullOrEmpty(scriptFileName))
				{
					result = false;
				}
				else
				{
					if (scriptFileName.EndsWith(".min"))
					{
						scriptFileName = scriptFileName.Substring(0, scriptFileName.Length - 4);
					}
					result = true;
				}
			}
			catch (ArgumentException)
			{
				scriptFileName = null;
				result = false;
			}
			return result;
		}
	}
}

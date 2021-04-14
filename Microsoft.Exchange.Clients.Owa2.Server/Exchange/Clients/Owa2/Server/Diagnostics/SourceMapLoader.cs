using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Xml;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal abstract class SourceMapLoader<T> where T : IJavaScriptSymbol
	{
		protected SourceMapLoader(IEnumerable<string> symbolMapFiles)
		{
			this.symbolMapFiles = symbolMapFiles;
		}

		public JavaScriptSymbolsMap<T> Load()
		{
			Dictionary<string, List<T>> symbolMaps = new Dictionary<string, List<T>>(20, StringComparer.InvariantCultureIgnoreCase);
			Dictionary<uint, string> sourceFileIdMap = new Dictionary<uint, string>(1024);
			ClientWatsonFunctionNamePool clientWatsonFunctionNamePool = new ClientWatsonFunctionNamePool();
			foreach (string filePath in this.symbolMapFiles)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				Exception ex = null;
				try
				{
					this.LoadSymbolsMapFromFile(filePath, symbolMaps, sourceFileIdMap, clientWatsonFunctionNamePool);
				}
				catch (XmlException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (SecurityException ex4)
				{
					ex = ex4;
				}
				finally
				{
					stopwatch.Stop();
				}
				SymbolMapLoadLogEvent logEvent;
				if (ex == null)
				{
					logEvent = SymbolMapLoadLogEvent.CreateForSuccess(filePath, stopwatch.Elapsed);
				}
				else
				{
					logEvent = SymbolMapLoadLogEvent.CreateForError(filePath, ex, stopwatch.Elapsed);
				}
				OwaServerLogger.AppendToLog(logEvent);
			}
			return new JavaScriptSymbolsMap<T>(symbolMaps, sourceFileIdMap, clientWatsonFunctionNamePool.ToArray());
		}

		protected abstract void LoadSymbolsMapFromFile(string filePath, Dictionary<string, List<T>> symbolMaps, Dictionary<uint, string> sourceFileIdMap, ClientWatsonFunctionNamePool functionNamePool);

		protected const int ScriptSymbolsInitialCapacity = 1024;

		private const int ScriptMapFilesInitialCapacity = 20;

		private const int SourceFileMapInitialCapacity = 1024;

		private readonly IEnumerable<string> symbolMapFiles;
	}
}

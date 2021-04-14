using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class JavaScriptSymbolsMap<T> : IJavaScriptSymbolsMap<T> where T : IJavaScriptSymbol
	{
		public JavaScriptSymbolsMap(IDictionary<string, List<T>> symbolMaps, IDictionary<uint, string> sourceFileIdMap, string[] functionNames)
		{
			this.symbolMaps = symbolMaps;
			this.sourceFileIdMap = sourceFileIdMap;
			this.functionNames = functionNames;
		}

		public bool Search(string scriptName, T javaScriptSymbol, out T symbolFound)
		{
			symbolFound = default(T);
			List<T> list;
			if (!this.symbolMaps.TryGetValue(scriptName, out list))
			{
				return false;
			}
			int num = list.BinarySearch(javaScriptSymbol, JavaScriptSymbolComparer<T>.Instance);
			if (num < 0)
			{
				num = ~num;
			}
			if (num >= list.Count)
			{
				return false;
			}
			bool result = false;
			T t;
			for (;;)
			{
				t = list[num];
				if (t.ScriptStartLine < javaScriptSymbol.ScriptStartLine || (t.ScriptStartLine == javaScriptSymbol.ScriptStartLine && t.ScriptStartColumn <= javaScriptSymbol.ScriptStartColumn))
				{
					break;
				}
				num = t.ParentSymbolIndex;
				if (num < 0)
				{
					return result;
				}
			}
			result = true;
			symbolFound = t;
			return result;
		}

		public string GetSourceFilePathFromId(uint id)
		{
			string result;
			if (this.sourceFileIdMap.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public string GetFunctionName(int index)
		{
			return this.functionNames[index];
		}

		public bool HasSymbolsLoadedForScript(string scriptName)
		{
			return this.symbolMaps.ContainsKey(scriptName);
		}

		internal HashSet<string> GetScriptNames()
		{
			return new HashSet<string>(this.symbolMaps.Keys, StringComparer.InvariantCultureIgnoreCase);
		}

		internal IEnumerable<T> GetSymbolsLoadedForScript(string scriptName)
		{
			List<T> map;
			if (this.symbolMaps.TryGetValue(scriptName, out map))
			{
				foreach (T symbol in map)
				{
					yield return symbol;
				}
			}
			yield break;
		}

		private readonly IDictionary<string, List<T>> symbolMaps;

		private readonly IDictionary<uint, string> sourceFileIdMap;

		private readonly string[] functionNames;
	}
}

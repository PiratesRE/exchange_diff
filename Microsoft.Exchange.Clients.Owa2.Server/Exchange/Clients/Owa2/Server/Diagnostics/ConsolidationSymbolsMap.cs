using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal sealed class ConsolidationSymbolsMap : IConsolidationSymbolsMap
	{
		public ConsolidationSymbolsMap(string symbolsMapFolderPath, string owaVersion)
		{
			this.symbolsMapFolder = symbolsMapFolderPath;
			this.symbolMaps = new Dictionary<string, List<ConsolidationSymbolsMap.ConsolidationSymbol>>(21, StringComparer.InvariantCultureIgnoreCase);
			this.scriptsOutOfSync = new List<string>();
			this.sourceFileIds = new List<string>(40);
			this.owaVersion = owaVersion;
		}

		public bool SkipChecksumValidation { get; set; }

		private string ScriptsPath
		{
			get
			{
				if (this.scriptsPath == null)
				{
					this.scriptsPath = ResourcePathBuilderUtilities.GetScriptResourcesRootFolderPath(ExchangeSetupContext.InstallPath, ResourcePathBuilderUtilities.GetResourcesRelativeFolderPath(this.owaVersion));
				}
				return this.scriptsPath;
			}
		}

		public bool Search(string scriptName, int line, int column, out string sourceFile, out Tuple<int, int> preConsolidationPosition)
		{
			this.AssureSymbolsAreLoaded();
			preConsolidationPosition = null;
			sourceFile = null;
			ConsolidationSymbolsMap.ConsolidationSymbol item = default(ConsolidationSymbolsMap.ConsolidationSymbol);
			item.ScriptEndLine = line;
			item.ScriptStartLine = line;
			item.ScriptEndColumn = column;
			item.ScriptStartColumn = column;
			List<ConsolidationSymbolsMap.ConsolidationSymbol> list;
			if (!this.symbolMaps.TryGetValue(scriptName, out list))
			{
				return false;
			}
			List<ConsolidationSymbolsMap.ConsolidationSymbol> list2 = this.symbolMaps[scriptName];
			int num = list2.BinarySearch(item, ConsolidationSymbolsMap.ConsolidationSymbolComparer.Instance);
			if (num < 0)
			{
				return false;
			}
			ConsolidationSymbolsMap.ConsolidationSymbol consolidationSymbol = list2[num];
			int num2 = line - consolidationSymbol.ScriptStartLine;
			int num3 = column - consolidationSymbol.ScriptStartColumn;
			preConsolidationPosition = new Tuple<int, int>(consolidationSymbol.SourceStartLine + num2, consolidationSymbol.SourceStartColumn + num3);
			sourceFile = this.sourceFileIds[consolidationSymbol.SourceFileId];
			return true;
		}

		public bool HasSymbolsLoadedForScript(string scriptName)
		{
			this.AssureSymbolsAreLoaded();
			return this.symbolMaps.ContainsKey(scriptName) || this.scriptsOutOfSync.Contains(scriptName);
		}

		private void AssureSymbolsAreLoaded()
		{
			if (this.symbolsLoaded)
			{
				return;
			}
			lock (this.loadLock)
			{
				try
				{
					this.UnsafeAssureSymbolsAreLoaded();
				}
				catch (IOException e)
				{
					OwaServerLogger.AppendToLog(SymbolMapLoadLogEvent.CreateForError(e));
				}
				finally
				{
					this.symbolsLoaded = true;
				}
			}
		}

		private void UnsafeAssureSymbolsAreLoaded()
		{
			if (this.symbolsLoaded)
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (string path in ConsolidationSymbolsMap.ConsolidationMapFileNames)
			{
				string text = Path.Combine(this.symbolsMapFolder, path);
				using (TextReader textReader = new StreamReader(text, Encoding.UTF8))
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(40, StringComparer.InvariantCultureIgnoreCase);
					string text2;
					while ((text2 = textReader.ReadLine()) != null)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							if (text2.StartsWith("#"))
							{
								string[] array = text2.Split(new char[]
								{
									' ',
									','
								});
								string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(array[1]);
								if (this.VerifyChecksum(fileNameWithoutExtension, array[2]))
								{
									this.symbolMaps.Add(fileNameWithoutExtension, new List<ConsolidationSymbolsMap.ConsolidationSymbol>(1024));
								}
								else
								{
									this.scriptsOutOfSync.Add(fileNameWithoutExtension);
								}
							}
							else
							{
								string[] array2 = text2.Split(new char[]
								{
									','
								});
								string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(array2[0]);
								int count;
								if (!dictionary.TryGetValue(fileNameWithoutExtension2, out count))
								{
									count = this.sourceFileIds.Count;
									dictionary.Add(fileNameWithoutExtension2, count);
									this.sourceFileIds.Add(fileNameWithoutExtension2);
								}
								string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(array2[5]);
								if (this.symbolMaps.ContainsKey(fileNameWithoutExtension3))
								{
									ConsolidationSymbolsMap.ConsolidationSymbol item = new ConsolidationSymbolsMap.ConsolidationSymbol
									{
										SourceStartLine = int.Parse(array2[1]),
										SourceStartColumn = int.Parse(array2[2]),
										SourceEndLine = int.Parse(array2[3]),
										SourceEndColumn = int.Parse(array2[4]),
										ScriptStartLine = int.Parse(array2[6]),
										ScriptStartColumn = int.Parse(array2[7]),
										ScriptEndLine = int.Parse(array2[8]),
										ScriptEndColumn = int.Parse(array2[9]),
										SourceFileId = count
									};
									this.symbolMaps[fileNameWithoutExtension3].Add(item);
								}
							}
						}
					}
				}
				OwaServerLogger.AppendToLog(SymbolMapLoadLogEvent.CreateForSuccess(text, stopwatch.Elapsed));
			}
			foreach (List<ConsolidationSymbolsMap.ConsolidationSymbol> list in this.symbolMaps.Values)
			{
				list.TrimExcess();
			}
		}

		private bool VerifyChecksum(string scriptName, string checksum)
		{
			if (this.SkipChecksumValidation)
			{
				return true;
			}
			string path = Path.Combine(this.ScriptsPath, scriptName + ".js");
			if (!File.Exists(path))
			{
				return false;
			}
			bool result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (HashAlgorithm hashAlgorithm = MD5.Create())
				{
					byte[] value = hashAlgorithm.ComputeHash(fileStream);
					result = BitConverter.ToString(value).Equals(checksum);
				}
			}
			return result;
		}

		private const string ScriptExtension = ".js";

		private const int EstimatedNumberOfPreConsolidationScripts = 40;

		private const int EstimatedNumberOfPostConsolidationScripts = 21;

		private const int EstimatedNumberOfSymbolsPerScript = 1024;

		private static readonly string[] ConsolidationMapFileNames = new string[]
		{
			"preboot_slabmanifest_consolidation.csv",
			"slabmanifest_consolidation.csv"
		};

		private readonly string symbolsMapFolder;

		private readonly IDictionary<string, List<ConsolidationSymbolsMap.ConsolidationSymbol>> symbolMaps;

		private readonly List<string> scriptsOutOfSync;

		private readonly List<string> sourceFileIds;

		private readonly object loadLock = new object();

		private string scriptsPath;

		private bool symbolsLoaded;

		private readonly string owaVersion;

		private enum ConsolidationSymbolData
		{
			SourceFileName,
			SourceStartLine,
			SourceStartColumn,
			SourceEndLine,
			SourceEndColumn,
			ScriptFileName,
			ScriptStartLine,
			ScriptStartColumn,
			ScriptEndLine,
			ScriptEndColumn
		}

		private struct ConsolidationSymbol
		{
			public int SourceStartLine { get; set; }

			public int SourceStartColumn { get; set; }

			public int SourceEndLine { get; set; }

			public int SourceEndColumn { get; set; }

			public int ScriptStartColumn { get; set; }

			public int ScriptStartLine { get; set; }

			public int ScriptEndColumn { get; set; }

			public int ScriptEndLine { get; set; }

			public int SourceFileId { get; set; }
		}

		private class ConsolidationSymbolComparer : IComparer<ConsolidationSymbolsMap.ConsolidationSymbol>
		{
			private ConsolidationSymbolComparer()
			{
			}

			public static ConsolidationSymbolsMap.ConsolidationSymbolComparer Instance
			{
				get
				{
					return ConsolidationSymbolsMap.ConsolidationSymbolComparer.instance;
				}
			}

			public int Compare(ConsolidationSymbolsMap.ConsolidationSymbol x, ConsolidationSymbolsMap.ConsolidationSymbol y)
			{
				if (x.ScriptStartLine > y.ScriptStartLine || (x.ScriptStartLine == y.ScriptStartLine && x.ScriptStartColumn > y.ScriptStartColumn))
				{
					return 1;
				}
				if (x.ScriptEndLine < y.ScriptEndLine || (x.ScriptEndLine == y.ScriptEndLine && x.ScriptEndColumn < y.ScriptEndColumn))
				{
					return -1;
				}
				return 0;
			}

			private const int Less = -1;

			private const int Greater = 1;

			private const int Contain = 0;

			private static readonly ConsolidationSymbolsMap.ConsolidationSymbolComparer instance = new ConsolidationSymbolsMap.ConsolidationSymbolComparer();
		}
	}
}

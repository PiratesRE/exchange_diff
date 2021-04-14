using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics
{
	public class ConfigurationDocument
	{
		public static int TraceTypesCount
		{
			get
			{
				return Enum.GetValues(typeof(TraceType)).Length / 2;
			}
		}

		public static ConfigurationDocument LoadFromFile(string configFileName)
		{
			ConfigurationDocument configurationDocument = new ConfigurationDocument();
			return ConfigurationDocument.LoadFromFile(configFileName, configurationDocument, new ConfigurationDocument.LineProcessor(configurationDocument.ProcessLine));
		}

		public static ConfigurationDocument LoadFaultInjectionFromFile(string configFileName)
		{
			ConfigurationDocument configurationDocument = new ConfigurationDocument();
			return ConfigurationDocument.LoadFromFile(configFileName, configurationDocument, new ConfigurationDocument.LineProcessor(configurationDocument.ProcessFaultInjectionLine));
		}

		public void GetEnabledTypes(BitArray destArray, bool addToExisting)
		{
			for (int i = 0; i < this.enabledTraceTypes.Length; i++)
			{
				if (addToExisting)
				{
					if (this.enabledTraceTypes[i])
					{
						destArray[i] = true;
					}
				}
				else
				{
					destArray[i] = this.enabledTraceTypes[i];
				}
			}
		}

		public List<TraceComponentInfo> EnabledComponentsList
		{
			get
			{
				return this.enabledComponentsList;
			}
		}

		internal List<TraceComponentInfo> BypassFilterEnabledComponentsList
		{
			get
			{
				return this.bypassFilterComponentsList;
			}
		}

		internal Dictionary<string, List<string>> CustomParameters
		{
			get
			{
				return this.customParameters;
			}
		}

		internal FaultInjectionConfig FaultInjectionConfig
		{
			get
			{
				return this.faultInjectionConfig;
			}
		}

		internal bool FilteredTracingEnabled
		{
			get
			{
				return this.filteredTracingEnabled;
			}
		}

		internal bool InMemoryTracingEnabled
		{
			get
			{
				return this.inMemoryTracingEnabled;
			}
		}

		internal bool ConsoleTracingEnabled
		{
			get
			{
				return this.consoleTracingEnabled;
			}
		}

		internal uint FileContentHash { get; private set; }

		internal bool SystemDiagnosticsTracingEnabled
		{
			get
			{
				return this.systemDiagnosticsTracingEnabled;
			}
		}

		private static ConfigurationDocument LoadFromFile(string path, ConfigurationDocument configDoc, ConfigurationDocument.LineProcessor processLine)
		{
			try
			{
				if (File.Exists(path))
				{
					configDoc.LoadLines(path, processLine);
				}
				else
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "File {0} does not exist", new object[]
					{
						path
					});
				}
			}
			catch (IOException)
			{
				configDoc = new ConfigurationDocument();
				InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Clearing trace settings due to IOException opening file {0}", new object[]
				{
					path
				});
			}
			catch (UnauthorizedAccessException)
			{
				InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Trace settings unchanged due to UnauthorizedAccessException opening file {0}", new object[]
				{
					path
				});
			}
			return configDoc;
		}

		protected void ProcessLine(string line)
		{
			string text = null;
			TraceComponentInfo traceComponentInfo = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(34351, 0L, "Processing line: {0}", new object[]
			{
				line
			});
			for (;;)
			{
				bool flag7 = false;
				string nextLexem = this.GetNextLexem(ref line, ref flag7);
				if (flag7)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					goto IL_1C4;
				}
				if (nextLexem == ",")
				{
					goto Block_3;
				}
				if (nextLexem == ":")
				{
					goto Block_4;
				}
				if (string.Equals(nextLexem, "bypassFilter", StringComparison.OrdinalIgnoreCase))
				{
					flag4 = true;
				}
				else
				{
					text = nextLexem;
				}
			}
			this.ReportError("Low level scanning error in the statement header.", new object[0]);
			goto IL_1C4;
			Block_3:
			this.ReportError("Bogus comma in the statement header.", new object[0]);
			goto IL_1C4;
			Block_4:
			if (string.IsNullOrEmpty(text))
			{
				this.ReportError("Colon is not preceded with component name.", new object[0]);
			}
			else if (text.Equals("TraceTypes", StringComparison.OrdinalIgnoreCase) || text.Equals("TraceLevels", StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			else if (text.Equals("FilteredTracing", StringComparison.OrdinalIgnoreCase))
			{
				flag2 = true;
			}
			else if (text.Equals("InMemoryTracing", StringComparison.OrdinalIgnoreCase))
			{
				flag3 = true;
			}
			else if (text.Equals("ConsoleTracing", StringComparison.OrdinalIgnoreCase))
			{
				flag5 = true;
			}
			else if (text.Equals("SystemDiagnosticsTracing", StringComparison.OrdinalIgnoreCase))
			{
				flag6 = true;
			}
			else
			{
				traceComponentInfo = this.CreateComponentIfNeccessary(text, flag4);
				if (traceComponentInfo == null)
				{
					List<string> list = null;
					if (!this.customParameters.TryGetValue(text, out list))
					{
						list = new List<string>();
						this.customParameters[text] = list;
					}
					InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "Added line as a custom parameter name,value = {0},{1}", new object[]
					{
						text,
						line
					});
					list.Add(line.Trim());
				}
			}
			IL_1C4:
			if (flag)
			{
				BitArray bitArray = new BitArray(ConfigurationDocument.TraceTypesCount + 1);
				if (this.ReadTraceTypes(line, ref bitArray))
				{
					this.enabledTraceTypes = bitArray;
					return;
				}
			}
			else if (flag2)
			{
				if (line.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase))
				{
					this.filteredTracingEnabled = true;
					return;
				}
			}
			else if (flag3)
			{
				if (line.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase))
				{
					this.inMemoryTracingEnabled = true;
					return;
				}
			}
			else if (flag5)
			{
				if (line.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase))
				{
					this.consoleTracingEnabled = this.GetConsoleTracingEnabled();
					return;
				}
			}
			else if (flag6)
			{
				if (line.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase))
				{
					this.systemDiagnosticsTracingEnabled = true;
					return;
				}
			}
			else
			{
				if (traceComponentInfo != null)
				{
					this.AddParsedComponentFromConfigFile(traceComponentInfo, line, flag4);
					return;
				}
				this.ReportError("Line could not be parsed", new object[0]);
			}
		}

		protected void ProcessFaultInjectionLine(string line)
		{
			InternalBypassTrace.FaultInjectionConfigurationTracer.TraceDebug(50735, 0L, "Processing line: {0}", new object[]
			{
				line
			});
			string text = null;
			FaultInjectionType type = FaultInjectionType.None;
			string empty = string.Empty;
			uint key = 0U;
			List<string> parameters = null;
			for (;;)
			{
				bool flag = false;
				string nextLexem = this.GetNextLexem(ref line, ref flag);
				if (flag)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					goto IL_A4;
				}
				if (nextLexem == ",")
				{
					if (!this.ReadFaultInjectionTagComponent(ref line, ref empty))
					{
						goto Block_4;
					}
				}
				else
				{
					if (nextLexem == ":")
					{
						goto IL_A4;
					}
					text = nextLexem;
				}
			}
			this.ReportError("Low level scanning error in the statement header.", new object[0]);
			return;
			Block_4:
			this.ReportError("Failed to read fault injection tag component.", new object[0]);
			return;
			IL_A4:
			if (string.IsNullOrEmpty(text))
			{
				this.ReportError("Colon is not preceded with component name.", new object[0]);
				return;
			}
			TraceComponentInfo traceComponentInfo = this.CreateComponentIfNeccessary(text, true);
			if (traceComponentInfo == null)
			{
				this.ReportError("Not a component.", new object[0]);
				return;
			}
			if (!this.ReadFaultInjectionType(ref line, ref type))
			{
				this.ReportError("Failed to read fault injection type.", new object[0]);
				return;
			}
			if (!this.ReadFaultInjectionLid(ref line, ref key))
			{
				this.ReportError("Failed to read fault injection LID.", new object[0]);
				return;
			}
			if (!this.ReadFaultInjectionParameters(ref line, ref parameters))
			{
				this.ReportError("Failed to read fault injection parameters.", new object[0]);
				return;
			}
			lock (this.FaultInjectionConfig)
			{
				FaultInjectionTagComponentConfig faultInjectionTagComponentConfig = null;
				FaultInjectionComponentConfig faultInjectionComponentConfig = null;
				if (!this.FaultInjectionConfig.TryGetValue(traceComponentInfo.ComponentGuid, out faultInjectionTagComponentConfig))
				{
					faultInjectionTagComponentConfig = new FaultInjectionTagComponentConfig();
					this.FaultInjectionConfig.Add(traceComponentInfo.ComponentGuid, faultInjectionTagComponentConfig);
				}
				if (!faultInjectionTagComponentConfig.TryGetValue(empty, out faultInjectionComponentConfig))
				{
					faultInjectionComponentConfig = new FaultInjectionComponentConfig();
					faultInjectionTagComponentConfig.Add(empty, faultInjectionComponentConfig);
				}
				faultInjectionComponentConfig[key] = new FaultInjectionPointConfig(type, parameters);
			}
		}

		protected void LoadLines(string configFileName, ConfigurationDocument.LineProcessor processLine)
		{
			Stream stream = null;
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "Trying to load trace file: {0}", new object[]
			{
				configFileName
			});
			int num = 20;
			while (num >= 0 && stream == null)
			{
				try
				{
					stream = this.GetStreamFromFile(configFileName);
					break;
				}
				catch (FileNotFoundException)
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "File does not exist", new object[0]);
					throw;
				}
				catch (UnauthorizedAccessException)
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Failed to load file, UnauthorizedAccessException", new object[0]);
					if (num == 0)
					{
						throw;
					}
				}
				catch (SecurityException)
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Failed to load file, SecurityException", new object[0]);
					if (num == 0)
					{
						throw;
					}
				}
				catch (IOException)
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Failed to load file, IOException", new object[0]);
					if (num == 0)
					{
						throw;
					}
				}
				InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Retrying file load in 500 ms", new object[0]);
				Thread.Sleep(500);
				num--;
			}
			byte[] array;
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				array = binaryReader.ReadBytes((int)stream.Length);
			}
			this.FileContentHash = TraceConfigSync.ComputeContentHash(array);
			using (TextReader textReader = new StringReader(ConfigurationDocument.Encoding.GetString(array, 0, array.Length)))
			{
				string processedLine;
				while ((processedLine = textReader.ReadLine()) != null)
				{
					this.currentLine++;
					processLine(processedLine);
				}
			}
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "Done reading config file: {0}, {1} lines were processed", new object[]
			{
				configFileName,
				this.currentLine
			});
		}

		private void AddParsedComponentFromConfigFile(TraceComponentInfo componentInfo, string currentLine, bool bypassFilterModifierSet)
		{
			Dictionary<int, TraceTagInfo> dictionary = new Dictionary<int, TraceTagInfo>();
			List<TraceComponentInfo> list = bypassFilterModifierSet ? this.bypassFilterComponentsList : this.enabledComponentsList;
			Dictionary<string, TraceComponentInfo> dictionary2 = bypassFilterModifierSet ? this.bypassFilterEnabledComponentsIndex : this.enabledComponentsIndex;
			if (this.ReadComponentTags(currentLine, componentInfo.PrettyName, dictionary))
			{
				if (!dictionary2.ContainsKey(componentInfo.PrettyName))
				{
					list.Add(componentInfo);
					dictionary2.Add(componentInfo.PrettyName, componentInfo);
				}
				componentInfo.TagInfoList = new TraceTagInfo[dictionary.Count];
				dictionary.Values.CopyTo(componentInfo.TagInfoList, 0);
			}
		}

		private string GetNextLexem(ref string line, ref bool error)
		{
			line = line.TrimStart(new char[0]);
			if (string.IsNullOrEmpty(line))
			{
				return string.Empty;
			}
			if (line[0] == ':' || line[0] == ',')
			{
				string result = new string(line[0], 1);
				line = line.Substring(1);
				return result;
			}
			char c = line[0];
			if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
			{
				int i = 1;
				while (i < line.Length)
				{
					c = line[i++];
					if (c != '_' && c != '.' && c != '-' && (c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9'))
					{
						i--;
						break;
					}
				}
				string result2 = line.Substring(0, i);
				line = line.Substring(i);
				return result2;
			}
			if (c == '-' || c == '+' || char.IsDigit(c))
			{
				int j = 1;
				while (j < line.Length)
				{
					if (!char.IsDigit(line, j++))
					{
						j--;
						break;
					}
				}
				string result3 = line.Substring(0, j);
				line = line.Substring(j);
				return result3;
			}
			error = true;
			line = line.Substring(1);
			return string.Empty;
		}

		private string GetNextString(ref string line)
		{
			line = line.TrimStart(new char[0]);
			if (string.IsNullOrEmpty(line))
			{
				return string.Empty;
			}
			int num = 0;
			int num2 = 0;
			int i = 0;
			bool flag = false;
			while (i < line.Length)
			{
				if (line[i] == '"')
				{
					if (flag)
					{
						num2++;
						i--;
						break;
					}
					flag = true;
					i = (num = i + 1);
				}
				else if (flag)
				{
					i++;
				}
				else if (char.IsWhiteSpace(line[i++]))
				{
					i--;
					break;
				}
			}
			string result = line.Substring(num, i);
			line = line.Substring(num + i + num2);
			return result;
		}

		private TraceComponentInfo CreateComponentIfNeccessary(string name, bool bypassFilterList)
		{
			Dictionary<string, TraceComponentInfo> dictionary = bypassFilterList ? this.bypassFilterEnabledComponentsIndex : this.enabledComponentsIndex;
			if (dictionary.ContainsKey(name))
			{
				return dictionary[name];
			}
			TraceComponentInfo traceComponentInfo = null;
			if (AvailableTraces.InnerDictionary.TryGetValue(name, out traceComponentInfo))
			{
				return new TraceComponentInfo(traceComponentInfo.PrettyName, traceComponentInfo.ComponentGuid, null);
			}
			InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "{0} not a known component (may be custom parameter)", new object[]
			{
				name
			});
			return null;
		}

		protected bool ReadTraceTypes(string line, ref BitArray traceTypes)
		{
			for (;;)
			{
				bool flag = false;
				string nextLexem = this.GetNextLexem(ref line, ref flag);
				if (flag)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					return true;
				}
				if (!(nextLexem == ","))
				{
					if (nextLexem == ":")
					{
						goto Block_3;
					}
					TraceType traceTypeByName = this.GetTraceTypeByName(nextLexem);
					if (traceTypeByName != TraceType.None)
					{
						traceTypes[(int)traceTypeByName] = true;
					}
				}
			}
			this.ReportError("Low level scanning error in the trace types statement.", new object[0]);
			return false;
			Block_3:
			this.ReportError("Bogus colon in the trace types statement.", new object[0]);
			return false;
		}

		protected bool ReadFaultInjectionType(ref string line, ref FaultInjectionType faultInjectionType)
		{
			string nextLexem;
			for (;;)
			{
				bool flag = false;
				nextLexem = this.GetNextLexem(ref line, ref flag);
				if (flag)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					return true;
				}
				if (!(nextLexem == ","))
				{
					goto Block_2;
				}
			}
			this.ReportError("Low level scanning error in the trace types statement.", new object[0]);
			return false;
			Block_2:
			if (nextLexem == ":")
			{
				this.ReportError("Bogus colon in the trace types statement.", new object[0]);
				return false;
			}
			faultInjectionType = this.GetFaultInjectionTypeByName(nextLexem);
			return true;
		}

		protected bool ReadFaultInjectionTagComponent(ref string line, ref string tagComponent)
		{
			int num = line.IndexOf(':');
			if (num >= 0)
			{
				tagComponent = line.Substring(0, num);
				line = line.Substring(num);
				return true;
			}
			this.ReportError("Bogus tagComponent definitions.", new object[0]);
			return false;
		}

		protected bool ReadFaultInjectionLid(ref string line, ref uint faultInjectionLid)
		{
			string nextLexem;
			for (;;)
			{
				bool flag = false;
				nextLexem = this.GetNextLexem(ref line, ref flag);
				if (flag)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					return true;
				}
				if (!(nextLexem == ","))
				{
					goto Block_2;
				}
			}
			this.ReportError("Low level scanning error in the trace types statement.", new object[0]);
			return false;
			Block_2:
			if (nextLexem == ":")
			{
				this.ReportError("Bogus colon in the trace types statement.", new object[0]);
				return false;
			}
			try
			{
				faultInjectionLid = uint.Parse(nextLexem);
			}
			catch (FormatException)
			{
				return false;
			}
			return true;
		}

		protected bool ReadFaultInjectionParameters(ref string line, ref List<string> faultInjectionParameters)
		{
			faultInjectionParameters = new List<string>();
			for (;;)
			{
				string nextString = this.GetNextString(ref line);
				if (string.IsNullOrEmpty(nextString))
				{
					break;
				}
				faultInjectionParameters.Add(nextString);
			}
			return true;
		}

		private bool ReadComponentTags(string line, string componentName, Dictionary<int, TraceTagInfo> tags)
		{
			for (;;)
			{
				bool flag = false;
				string nextLexem = this.GetNextLexem(ref line, ref flag);
				if (flag)
				{
					break;
				}
				if (string.IsNullOrEmpty(nextLexem))
				{
					return true;
				}
				if (!(nextLexem == ","))
				{
					if (nextLexem == ":")
					{
						goto Block_3;
					}
					if (string.Compare(nextLexem, "All", StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						bool flag2 = false;
						TraceTagInfo[] tagInfoList = AvailableTraces.InnerDictionary[componentName].TagInfoList;
						TraceTagInfo[] array = tagInfoList;
						int i = 0;
						while (i < array.Length)
						{
							TraceTagInfo traceTagInfo = array[i];
							if (traceTagInfo != null && string.Compare(nextLexem, traceTagInfo.PrettyName, StringComparison.CurrentCultureIgnoreCase) == 0)
							{
								flag2 = true;
								if (!tags.ContainsKey(traceTagInfo.NumericValue))
								{
									tags.Add(traceTagInfo.NumericValue, traceTagInfo);
									break;
								}
								break;
							}
							else
							{
								i++;
							}
						}
						if (!flag2)
						{
							this.ReportError("Unrecognized tag will be ignored: {0}", new object[]
							{
								nextLexem
							});
						}
					}
				}
			}
			this.ReportError("Low level scanning error in the list of tags.", new object[0]);
			return false;
			Block_3:
			this.ReportError("Bogus colon in the list of tags.", new object[0]);
			return false;
		}

		protected TraceType GetTraceTypeByName(string typeName)
		{
			TraceType result;
			try
			{
				result = (TraceType)Enum.Parse(typeof(TraceType), typeName, true);
			}
			catch (ArgumentException)
			{
				this.ReportError("Invalid trace type: {0}", new object[]
				{
					typeName
				});
				result = TraceType.None;
			}
			return result;
		}

		protected FaultInjectionType GetFaultInjectionTypeByName(string typeName)
		{
			FaultInjectionType result;
			try
			{
				result = (FaultInjectionType)Enum.Parse(typeof(FaultInjectionType), typeName, true);
			}
			catch (ArgumentException)
			{
				this.ReportError("Bogus string for fault injection: {0}", new object[]
				{
					typeName
				});
				result = FaultInjectionType.None;
			}
			return result;
		}

		protected void ReportError(string errorMsgFmt, params object[] args)
		{
			InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, errorMsgFmt, args);
		}

		private Stream GetStreamFromFile(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		private bool GetConsoleTracingEnabled()
		{
			return Environment.UserInteractive;
		}

		private BitArray enabledTraceTypes = new BitArray(ConfigurationDocument.TraceTypesCount + 1);

		private List<TraceComponentInfo> enabledComponentsList = new List<TraceComponentInfo>();

		private List<TraceComponentInfo> bypassFilterComponentsList = new List<TraceComponentInfo>();

		private int currentLine;

		protected Dictionary<string, TraceComponentInfo> enabledComponentsIndex = new Dictionary<string, TraceComponentInfo>(StringComparer.OrdinalIgnoreCase);

		protected Dictionary<string, TraceComponentInfo> bypassFilterEnabledComponentsIndex = new Dictionary<string, TraceComponentInfo>(StringComparer.OrdinalIgnoreCase);

		protected Dictionary<string, List<string>> customParameters = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		private FaultInjectionConfig faultInjectionConfig = new FaultInjectionConfig();

		protected bool filteredTracingEnabled;

		protected bool inMemoryTracingEnabled;

		private bool consoleTracingEnabled;

		private bool systemDiagnosticsTracingEnabled;

		internal static readonly Encoding Encoding = Encoding.ASCII;

		protected delegate void LineProcessor(string processedLine);
	}
}

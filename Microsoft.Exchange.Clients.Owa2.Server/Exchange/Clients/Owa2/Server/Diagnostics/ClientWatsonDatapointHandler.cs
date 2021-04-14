using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class ClientWatsonDatapointHandler
	{
		internal ClientWatsonDatapointHandler(RequestDetailsLogger logger, string owaVersion) : this(ClientWatsonParametersFactory.GetInstance(owaVersion), logger)
		{
		}

		internal ClientWatsonDatapointHandler(IClientWatsonParameters parameters, RequestDetailsLogger logger)
		{
			this.parameters = parameters;
			this.logger = logger;
		}

		public void ReportWatsonEvents(UserContext userContext, IList<ClientLogEvent> watsonEvents, Datapoint contextHeader, Datapoint[] context)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num = 0;
			int num2 = 0;
			List<ClientWatsonDatapointHandler.ClientWatsonReportLogEvent> list = new List<ClientWatsonDatapointHandler.ClientWatsonReportLogEvent>(watsonEvents.Count);
			List<WatsonClientReport> list2 = new List<WatsonClientReport>(watsonEvents.Count);
			foreach (ClientLogEvent clientLogEvent in watsonEvents)
			{
				string ctqName;
				clientLogEvent.TryGetValue<string>("ctq", out ctqName);
				ClientWatsonReportData reportData;
				ProcessingResult errorResult = this.TryExtractClientWatsonData(clientLogEvent, out reportData);
				if (errorResult.Code != ResultCode.Success)
				{
					list.Add(new ClientWatsonDatapointHandler.ClientWatsonReportLogEvent(clientLogEvent.Time, userContext, ctqName, reportData.IsUnhandledException, errorResult));
					num2++;
				}
				else
				{
					int hashCode = reportData.OriginalCallStack.GetHashCode();
					bool flag = this.parameters.IsErrorOverReportQuota(hashCode);
					if (flag)
					{
						string[] watsonParameters;
						if (ClientWatsonDatapointHandler.cachedFlavor == null)
						{
							ClientWatsonDatapointHandler.CreateClientWatsonReportAndUpdateCache(reportData, out watsonParameters);
						}
						else
						{
							watsonParameters = WatsonClientReport.BuildWatsonParameters(ClientWatsonDatapointHandler.cachedFlavor, ClientWatsonDatapointHandler.cachedVersion, reportData.TraceComponent, reportData.FunctionName, reportData.ExceptionType, reportData.NormalizedCallStack, reportData.CallStackHash);
						}
						list.Add(new ClientWatsonDatapointHandler.ClientWatsonReportLogEvent(clientLogEvent.Time, userContext, ctqName, watsonParameters, reportData.IsUnhandledException, false));
						num++;
					}
					else
					{
						string[] watsonParameters;
						WatsonClientReport item = ClientWatsonDatapointHandler.CreateClientWatsonReportAndUpdateCache(reportData, out watsonParameters);
						list2.Add(item);
						list.Add(new ClientWatsonDatapointHandler.ClientWatsonReportLogEvent(clientLogEvent.Time, userContext, ctqName, watsonParameters, reportData.IsUnhandledException, true));
					}
				}
			}
			string extraData = ClientWatsonDatapointHandler.BuildExtraDataString(userContext, contextHeader, context, list);
			foreach (WatsonClientReport report in list2)
			{
				this.parameters.ReportAction(report, extraData);
			}
			this.logger.Set(LogDatapointMetadata.WatsonDatapointSkipped, num);
			this.logger.Set(LogDatapointMetadata.WatsonDatapointFailed, num2);
			this.logger.Set(LogDatapointMetadata.WatsonReportingElapsed, stopwatch.ElapsedMilliseconds);
		}

		private static string BuildExtraDataString(UserContext userContext, Datapoint contextHeader, Datapoint[] context, List<ClientWatsonDatapointHandler.ClientWatsonReportLogEvent> eventsToLog)
		{
			string text = ClientWatsonDatapointHandler.FormatClientLogDataForWatson(contextHeader, context);
			StringBuilder stringBuilder = new StringBuilder(text.Length + "------------------------ClientLogs------------------------".Length + "---------------Watson events on this session--------------".Length);
			string text2 = (userContext != null) ? userContext.LogEventCommonData.Features : null;
			string text3 = (userContext != null) ? userContext.LogEventCommonData.Flights : null;
			stringBuilder.AppendLine("-------------------Features & Flights---------------------");
			stringBuilder.Append("Features: ");
			stringBuilder.AppendLine(string.IsNullOrEmpty(text2) ? "None" : text2);
			stringBuilder.Append("Flights: ");
			stringBuilder.AppendLine(string.IsNullOrEmpty(text3) ? "None" : text3);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("---------------Watson events on this session--------------");
			if (userContext != null)
			{
				foreach (string value in userContext.GetClientWatsonHistory())
				{
					stringBuilder.AppendLine(value);
					stringBuilder.AppendLine();
				}
			}
			string[] array = new string[eventsToLog.Count];
			int num = 0;
			foreach (ClientWatsonDatapointHandler.ClientWatsonReportLogEvent clientWatsonReportLogEvent in eventsToLog)
			{
				OwaClientLogger.AppendToLog(clientWatsonReportLogEvent);
				string text4 = string.Format("{0}{1}{2}", clientWatsonReportLogEvent, Environment.NewLine, clientWatsonReportLogEvent.GetWatsonUrl());
				array[num++] = text4;
				stringBuilder.AppendLine(text4);
				stringBuilder.AppendLine();
			}
			if (userContext != null)
			{
				Array.Sort<string>(array);
				userContext.SaveToClientWatsonHistory(array);
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("------------------------ClientLogs------------------------");
			stringBuilder.AppendLine(text);
			return stringBuilder.ToString();
		}

		public ProcessingResult TryExtractClientWatsonData(ClientLogEvent watsonEvent, out ClientWatsonReportData reportData)
		{
			reportData = default(ClientWatsonReportData);
			string text;
			if (watsonEvent.TryGetValue<string>("BuildType", out text))
			{
				return new ProcessingResult
				{
					Code = ResultCode.DebugBuild
				};
			}
			bool flag = watsonEvent.TryGetValue<string>("est", out reportData.OriginalCallStack);
			if (!flag && !watsonEvent.TryGetValue<string>("st", out reportData.OriginalCallStack))
			{
				return new ProcessingResult
				{
					Code = ResultCode.NoStackTrace
				};
			}
			reportData.IsUnhandledException = new bool?(flag);
			if (!reportData.OriginalCallStack.Contains(this.parameters.OwaVersion))
			{
				return new ProcessingResult
				{
					Code = ResultCode.VersionMismatch
				};
			}
			if (!watsonEvent.TryGetValue<string>("s", out reportData.TraceComponent))
			{
				return new ProcessingResult
				{
					Code = ResultCode.NoTraceComponent
				};
			}
			StringBuilder stringBuilder = new StringBuilder(reportData.OriginalCallStack.Length);
			bool flag2 = true;
			string functionName = null;
			string packageName = null;
			foreach (string frame in ClientWatsonDatapointHandler.EnumerateFrames(reportData.OriginalCallStack))
			{
				CallStackFrameInfo callStackFrameInfo;
				ProcessingResult result = this.TryProcessCallStackFrame(frame, out callStackFrameInfo);
				if (result.Code != ResultCode.Success)
				{
					return result;
				}
				if (flag2)
				{
					functionName = callStackFrameInfo.SanitizedFunctionName;
					packageName = callStackFrameInfo.PackageName;
				}
				if (reportData.FunctionName == null && !ClientWatsonDatapointHandler.ExternalScriptRegex.IsMatch(callStackFrameInfo.PackageName) && (flag || !flag2))
				{
					reportData.FunctionName = callStackFrameInfo.SanitizedFunctionName;
					reportData.PackageName = callStackFrameInfo.PackageName;
				}
				callStackFrameInfo.UpdateHash(ref reportData.CallStackHash);
				stringBuilder.AppendFormat(callStackFrameInfo.ToString(), new object[0]);
				flag2 = false;
			}
			if (!flag2)
			{
				reportData.NormalizedCallStack = stringBuilder.ToString();
				if (!watsonEvent.TryGetValue<string>("em", out reportData.ExceptionMessage))
				{
					reportData.ExceptionMessage = "unknown";
				}
				if (!watsonEvent.TryGetValue<string>("en", out reportData.ExceptionType))
				{
					string text2;
					if (!flag && watsonEvent.TryGetValue<string>("f", out text2) && !string.IsNullOrEmpty(text2))
					{
						reportData.ExceptionType = ClientWatsonDatapointHandler.SanitizeExceptionType(text2);
						reportData.ExceptionMessage = ClientWatsonDatapointHandler.FormatExceptionMessage(text2, watsonEvent);
					}
					else
					{
						reportData.ExceptionType = "unknown";
					}
				}
				if (string.IsNullOrEmpty(reportData.FunctionName))
				{
					reportData.FunctionName = functionName;
					reportData.PackageName = packageName;
				}
				return new ProcessingResult
				{
					Code = ResultCode.Success
				};
			}
			return new ProcessingResult
			{
				Code = ResultCode.NoFrames
			};
		}

		public ProcessingResult TryProcessCallStackFrame(string frame, out CallStackFrameInfo callStackFrameInfo)
		{
			List<Regex> regexList = new List<Regex>
			{
				ClientWatsonDatapointHandler.ChromeAndIEStackFrameRegex,
				ClientWatsonDatapointHandler.IOS7StackFrameRegex
			};
			callStackFrameInfo = null;
			Match match;
			if (!ClientWatsonDatapointHandler.TryMatchAny(frame, regexList, out match))
			{
				return new ProcessingResult
				{
					Code = ResultCode.NoMatch,
					RawErrorFrame = frame
				};
			}
			string text = match.Groups["package"].Value;
			int num = text.LastIndexOf('/') + 1;
			if (num > 0)
			{
				text = text.Substring(num, text.Length - num);
			}
			int num2 = int.Parse(match.Groups["line"].Value);
			int num3 = int.Parse(match.Groups["column"].Value);
			string text2 = match.Groups["function"].Value.TrimEnd(new char[0]);
			string text3;
			Tuple<int, int> tuple;
			if (this.parameters.ConsolidationSymbolsMap.HasSymbolsLoadedForScript(text))
			{
				if (!this.parameters.ConsolidationSymbolsMap.Search(text, num2, num3, out text3, out tuple))
				{
					return new ProcessingResult
					{
						Code = ResultCode.FailedToDeConsolidate,
						Package = text,
						Function = text2,
						Line = num2,
						Column = num3
					};
				}
			}
			else
			{
				text3 = text;
				tuple = new Tuple<int, int>(num2, num3);
			}
			if (this.parameters.MinificationSymbolsMapForScriptSharp.HasSymbolsLoadedForScript(text3))
			{
				ResultCode resultCode = this.TryDeMinifyAndDeObfuscate(text3, tuple.Item1, tuple.Item2, out callStackFrameInfo);
				if (resultCode != ResultCode.Success)
				{
					return new ProcessingResult
					{
						Code = resultCode,
						SourceType = FrameSourceType.ScriptSharp,
						Package = text3,
						Function = text2,
						Line = tuple.Item1,
						Column = tuple.Item2
					};
				}
			}
			else if (this.parameters.MinificationSymbolsMapForJavaScript.HasSymbolsLoadedForScript(text3))
			{
				if (!this.TryDeMinify(text3, tuple.Item1, tuple.Item2, out callStackFrameInfo))
				{
					return new ProcessingResult
					{
						Code = ResultCode.FailedToDeMinify,
						SourceType = FrameSourceType.JavaScript,
						Package = text3,
						Function = text2,
						Line = tuple.Item1,
						Column = tuple.Item2
					};
				}
			}
			else
			{
				callStackFrameInfo = new CallStackFrameInfo(true)
				{
					StartLine = tuple.Item1,
					EndLine = tuple.Item1,
					StartColumn = tuple.Item2,
					EndColumn = tuple.Item2,
					FunctionName = text2,
					FileName = text3 + ".js",
					FolderPath = string.Empty
				};
			}
			callStackFrameInfo.PackageName = text3;
			return new ProcessingResult
			{
				Code = ResultCode.Success
			};
		}

		public static IEnumerable<string> EnumerateFrames(string rawCallStack)
		{
			int start = 0;
			int end;
			string frame;
			while ((end = rawCallStack.IndexOf(Environment.NewLine, start)) >= 0)
			{
				frame = rawCallStack.Substring(start, end - start);
				if (!string.IsNullOrEmpty(frame))
				{
					yield return frame;
				}
				start = end + Environment.NewLine.Length;
			}
			frame = rawCallStack.Substring(start, rawCallStack.Length - start);
			if (!string.IsNullOrEmpty(frame))
			{
				yield return frame;
			}
			yield break;
		}

		private static WatsonClientReport CreateClientWatsonReportAndUpdateCache(ClientWatsonReportData reportData, out string[] watsonParameters)
		{
			WatsonClientReport watsonClientReport = new WatsonClientReport(reportData.TraceComponent, reportData.FunctionName, reportData.ExceptionMessage, reportData.ExceptionType, reportData.OriginalCallStack, reportData.NormalizedCallStack, reportData.CallStackHash, reportData.PackageName);
			watsonParameters = watsonClientReport.GetWatsonParameters();
			ClientWatsonDatapointHandler.cachedFlavor = watsonParameters[0];
			ClientWatsonDatapointHandler.cachedVersion = watsonParameters[1];
			return watsonClientReport;
		}

		private static string FormatClientLogDataForWatson(Datapoint contextHeader, Datapoint[] context)
		{
			StringBuilder stringBuilder = new StringBuilder((context.Length + 1) * 512);
			stringBuilder.AppendLine("MachineName: " + Environment.MachineName);
			contextHeader.AppendTo(stringBuilder);
			stringBuilder.AppendLine();
			foreach (Datapoint datapoint in context)
			{
				datapoint.AppendTo(stringBuilder);
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		private static string SanitizeExceptionType(string value)
		{
			if (value.Length >= 400)
			{
				return value.Substring(0, 400);
			}
			return value;
		}

		private static string FormatExceptionMessage(string formatStr, ClientLogEvent watsonEvent)
		{
			List<object> list = new List<object>(ClientWatsonDatapointHandler.FormatStringArgumentKeys.Length);
			foreach (string text in ClientWatsonDatapointHandler.FormatStringArgumentKeys)
			{
				string item;
				if (!watsonEvent.TryGetValue<string>(text, out item) && !formatStr.Contains("{" + text + "}"))
				{
					break;
				}
				list.Add(item);
			}
			string result;
			try
			{
				result = string.Format(formatStr, list.ToArray());
			}
			catch (FormatException)
			{
				result = string.Format("FAILED TO FORMAT EXCEPTION MESSAGE: {0} - Number of args: {1}", formatStr, list.Count);
			}
			return result;
		}

		private static string GetRelativeFilePath(string sourceFilePath)
		{
			int num = sourceFilePath.IndexOf("sources", StringComparison.InvariantCultureIgnoreCase);
			if (num >= 0)
			{
				sourceFilePath = sourceFilePath.Substring(num);
			}
			return sourceFilePath;
		}

		private static bool TryMatchAny(string frame, List<Regex> regexList, out Match match)
		{
			match = null;
			while (regexList.Count > 0)
			{
				match = regexList[0].Match(frame);
				if (match.Success)
				{
					return true;
				}
				regexList.RemoveAt(0);
			}
			return false;
		}

		private bool TryDeMinify(string package, int line, int column, out CallStackFrameInfo callStackFrameInfo)
		{
			AjaxMinSymbolForJavaScript javaScriptSymbol = default(AjaxMinSymbolForJavaScript);
			javaScriptSymbol.ScriptEndLine = line;
			javaScriptSymbol.ScriptStartLine = line;
			javaScriptSymbol.ScriptEndColumn = column;
			javaScriptSymbol.ScriptStartColumn = column;
			AjaxMinSymbolForJavaScript ajaxMinSymbolForJavaScript;
			if (!this.parameters.MinificationSymbolsMapForJavaScript.Search(package, javaScriptSymbol, out ajaxMinSymbolForJavaScript))
			{
				callStackFrameInfo = null;
				return false;
			}
			callStackFrameInfo = new CallStackFrameInfo
			{
				StartLine = ajaxMinSymbolForJavaScript.SourceStartLine,
				StartColumn = ajaxMinSymbolForJavaScript.SourceStartColumn,
				EndLine = ajaxMinSymbolForJavaScript.SourceEndLine,
				EndColumn = ajaxMinSymbolForJavaScript.SourceEndColumn,
				FunctionName = this.parameters.MinificationSymbolsMapForJavaScript.GetFunctionName(ajaxMinSymbolForJavaScript.FunctionNameIndex),
				FileName = ClientWatsonDatapointHandler.GetRelativeFilePath(this.parameters.MinificationSymbolsMapForJavaScript.GetSourceFilePathFromId(ajaxMinSymbolForJavaScript.SourceFileId)),
				FolderPath = this.parameters.ExchangeSourcesPath
			};
			return true;
		}

		private ResultCode TryDeMinifyAndDeObfuscate(string package, int line, int column, out CallStackFrameInfo callStackFrameInfo)
		{
			AjaxMinSymbolForScriptSharp javaScriptSymbol = default(AjaxMinSymbolForScriptSharp);
			javaScriptSymbol.ScriptEndLine = line;
			javaScriptSymbol.ScriptStartLine = line;
			javaScriptSymbol.ScriptEndColumn = column;
			javaScriptSymbol.ScriptStartColumn = column;
			AjaxMinSymbolForScriptSharp ajaxMinSymbolForScriptSharp;
			if (!this.parameters.MinificationSymbolsMapForScriptSharp.Search(package, javaScriptSymbol, out ajaxMinSymbolForScriptSharp))
			{
				callStackFrameInfo = null;
				return ResultCode.FailedToDeMinify;
			}
			ScriptSharpSymbolWrapper javaScriptSymbol2 = new ScriptSharpSymbolWrapper(new ScriptSharpSymbol
			{
				ScriptStartPosition = ajaxMinSymbolForScriptSharp.SourceStartPosition,
				ScriptEndPosition = ajaxMinSymbolForScriptSharp.SourceEndPosition
			});
			ScriptSharpSymbolWrapper scriptSharpSymbolWrapper;
			if (!this.parameters.ObfuscationSymbolsMap.Search(package, javaScriptSymbol2, out scriptSharpSymbolWrapper))
			{
				callStackFrameInfo = null;
				return ResultCode.FailedToDeObfuscate;
			}
			ScriptSharpSymbol innerSymbol = scriptSharpSymbolWrapper.InnerSymbol;
			callStackFrameInfo = new CallStackFrameInfo
			{
				StartLine = innerSymbol.SourceStartLine,
				FunctionName = this.parameters.ObfuscationSymbolsMap.GetFunctionName(innerSymbol.FunctionNameIndex),
				FileName = ClientWatsonDatapointHandler.GetRelativeFilePath(this.parameters.ObfuscationSymbolsMap.GetSourceFilePathFromId(innerSymbol.SourceFileId)),
				FolderPath = this.parameters.ExchangeSourcesPath
			};
			return ResultCode.Success;
		}

		internal const string ExceptionNameKey = "en";

		internal const string ExceptionMessageKey = "em";

		internal const string ExceptionStackKey = "est";

		internal const string ErrorStackFromCallerKey = "st";

		internal const string TraceComponentKey = "s";

		internal const string FormatStringKey = "f";

		internal const string CtqNameKey = "ctq";

		internal const string BuildTypeKey = "BuildType";

		internal const string UnknownValue = "unknown";

		internal const string ErrorMessageFormatFailed = "FAILED TO FORMAT EXCEPTION MESSAGE: {0} - Number of args: {1}";

		private const int MaxWatsonParameterLength = 400;

		private const int EstimatedDatapointLength = 512;

		private const string FlightsLogsHeader = "-------------------Features & Flights---------------------";

		private const string WatsonEventsLogsHeader = "---------------Watson events on this session--------------";

		private const string ExtraDataClientLogsHeader = "------------------------ClientLogs------------------------";

		internal static readonly string[] FormatStringArgumentKeys = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5"
		};

		internal static readonly Regex ExternalScriptRegex = new Regex("(jquery|MicrosoftAjax|microsoft\\.natural|osfruntime|rteinit|ms\\.rte).*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex ChromeAndIEStackFrameRegex = new Regex("^\\s*at\\s+(Function\\.)?(\\$.*\\.)?(?<function>[^\\[\\(]*).*\\(?(?<package>(file|http).*)\\.js:(?<line>\\d+):(?<column>\\d+)\\)?\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex IOS7StackFrameRegex = new Regex("^\\s*(?<function>[^@]*)@?(?<package>(file|http).*)\\.js:(?<line>\\d+):(?<column>\\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly IClientWatsonParameters parameters;

		private readonly RequestDetailsLogger logger;

		private static string cachedFlavor;

		private static string cachedVersion;

		private class ClientWatsonReportLogEvent : ILogEvent
		{
			public ClientWatsonReportLogEvent(string clientTimestamp, UserContext userContext, string ctqName, string[] watsonParameters, bool? isScriptError, bool reported)
			{
				if (userContext != null)
				{
					ExchangePrincipal exchangePrincipal = userContext.ExchangePrincipal;
					if (exchangePrincipal != null)
					{
						this.mailboxGuid = exchangePrincipal.MailboxInfo.MailboxGuid;
						this.tenantGuid = exchangePrincipal.MailboxInfo.OrganizationId.GetTenantGuid();
					}
					this.logEventCommonData = userContext.LogEventCommonData;
				}
				this.clientTimestamp = clientTimestamp;
				this.ctqName = ctqName;
				this.watsonParameters = watsonParameters;
				this.isScriptError = isScriptError;
				this.reported = reported;
			}

			public ClientWatsonReportLogEvent(string clientTimestamp, UserContext userContext, string ctqName, bool? isScriptError, ProcessingResult errorResult) : this(clientTimestamp, userContext, ctqName, new string[0], isScriptError, false)
			{
				this.errorResult = new ProcessingResult?(errorResult);
			}

			public string EventId
			{
				get
				{
					return "ClientWatsonReport";
				}
			}

			public ICollection<KeyValuePair<string, object>> GetEventData()
			{
				if (this.eventData != null)
				{
					return this.eventData;
				}
				this.eventData = new Dictionary<string, object>(this.watsonParameters.Length + 1)
				{
					{
						"R",
						this.reported ? 1 : 0
					}
				};
				if (this.mailboxGuid != default(Guid))
				{
					this.eventData.Add("MG", this.mailboxGuid);
				}
				if (this.tenantGuid != default(Guid))
				{
					this.eventData.Add("TG", this.tenantGuid);
				}
				if (this.ctqName != null)
				{
					this.eventData.Add("ctq", this.ctqName);
				}
				for (int i = 1; i <= this.watsonParameters.Length; i++)
				{
					this.eventData.Add("P" + i, this.watsonParameters[i - 1] ?? string.Empty);
				}
				if (this.isScriptError != null)
				{
					this.eventData.Add("SE", this.isScriptError.Value ? 1 : 0);
				}
				if (this.logEventCommonData != null)
				{
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "pl", this.logEventCommonData.Platform);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "brn", this.logEventCommonData.Browser);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "brv", this.logEventCommonData.BrowserVersion);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "osn", this.logEventCommonData.OperatingSystem);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "osv", this.logEventCommonData.OperatingSystemVersion);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "dm", this.logEventCommonData.DeviceModel);
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "l", this.logEventCommonData.Layout);
				}
				if (this.errorResult != null)
				{
					this.eventData.Add("P2", ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.GetExchangeFormattedVersion(ExWatson.ApplicationVersion));
					ProcessingResult value = this.errorResult.Value;
					this.eventData.Add("EC", value.Code.ToString());
					ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.AddIfNotNullOrEmpty(this.eventData, "EFrm", value.RawErrorFrame);
					if (value.SourceType != FrameSourceType.Unknown)
					{
						this.eventData.Add("FST", value.SourceType.ToString());
					}
					if (!string.IsNullOrEmpty(value.Package))
					{
						this.eventData.Add("Pkg", value.Package);
						this.eventData.Add("Func", value.Function);
						this.eventData.Add("Ln", value.Line);
						this.eventData.Add("Col", value.Column);
					}
				}
				return this.eventData;
			}

			public override string ToString()
			{
				if (this.toStringValue == null)
				{
					bool flag;
					string text = LogRowFormatter.FormatCollection(this.GetEventData(), true, out flag);
					this.toStringValue = string.Format("{0},{1}", this.clientTimestamp, flag ? ClientWatsonDatapointHandler.ClientWatsonReportLogEvent.GetCsvEscapedString(text) : text);
				}
				return this.toStringValue;
			}

			public string GetWatsonUrl()
			{
				if (this.errorResult != null || this.watsonParameters.Length < 7)
				{
					return string.Empty;
				}
				return string.Format("http://watson/SearchResult/SearchUnTyped?iEventType=249&V3={0}&V4={1}&V6={2}", Uri.EscapeDataString(this.watsonParameters[3] ?? string.Empty), Uri.EscapeDataString(this.watsonParameters[4] ?? string.Empty), Uri.EscapeDataString(this.watsonParameters[6] ?? string.Empty));
			}

			private static void AddIfNotNullOrEmpty(Dictionary<string, object> eventData, string key, string value)
			{
				if (!string.IsNullOrEmpty(value))
				{
					eventData.Add(key, value);
				}
			}

			private static string GetCsvEscapedString(object value)
			{
				return Encoding.UTF8.GetString(Utf8Csv.EncodeAndEscape(value.ToString(), true));
			}

			private static string GetExchangeFormattedVersion(Version version)
			{
				if (version == null)
				{
					return "unknown";
				}
				return string.Format("{0:d2}.{1:d2}.{2:d4}.{3:d3}", new object[]
				{
					version.Major,
					version.Minor,
					version.Build,
					version.Revision
				});
			}

			private const string ClientWatsonReportId = "ClientWatsonReport";

			private const string WatsonUrlFormat = "http://watson/SearchResult/SearchUnTyped?iEventType=249&V3={0}&V4={1}&V6={2}";

			private readonly string clientTimestamp;

			private readonly string ctqName;

			private readonly string[] watsonParameters;

			private readonly Guid mailboxGuid;

			private readonly Guid tenantGuid;

			private readonly bool? isScriptError;

			private readonly bool reported;

			private readonly LogEventCommonData logEventCommonData;

			private readonly ProcessingResult? errorResult;

			private Dictionary<string, object> eventData;

			private string toStringValue;
		}
	}
}

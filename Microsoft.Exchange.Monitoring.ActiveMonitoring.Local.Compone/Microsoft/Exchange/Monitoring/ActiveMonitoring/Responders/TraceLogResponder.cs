using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class TraceLogResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(TraceLogResponder.TraceAttributes traceAttributes)
		{
			string text = string.Empty;
			string value = string.Empty;
			if ((!string.IsNullOrEmpty(traceAttributes.ProviderGuid) || !string.IsNullOrEmpty(traceAttributes.ProviderGuidFile)) && !string.IsNullOrEmpty(traceAttributes.KernelLoggerFlags))
			{
				throw new ArgumentException("Cannot specify both guids and kernel logger flags at the same time.");
			}
			if (!string.IsNullOrEmpty(traceAttributes.ProviderGuidFile) && !File.Exists(traceAttributes.ProviderGuidFile))
			{
				throw new ArgumentException(string.Format("The guid file specified does not exist : {0}", traceAttributes.ProviderGuid));
			}
			if (!string.IsNullOrEmpty(traceAttributes.ProviderGuid))
			{
				value = traceAttributes.ProviderGuid.Replace(",", Environment.NewLine);
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(TraceLogResponder.DiagnosticsRegistryKey))
			{
				if (registryKey != null)
				{
					if (string.IsNullOrEmpty(traceAttributes.LogFileDirectory) && string.IsNullOrEmpty(TraceLogResponder.edsDumpDirectory))
					{
						text = (string)registryKey.GetValue("LogFolderPath");
						if (!string.IsNullOrEmpty(text))
						{
							TraceLogResponder.edsDumpDirectory = string.Format("{0}\\Dumps", text);
						}
						else
						{
							TraceLogResponder.edsDumpDirectory = Environment.ExpandEnvironmentVariables("%systemdrive%\\Dumps");
						}
					}
					text = TraceLogResponder.edsDumpDirectory;
					string text2 = (string)registryKey.GetValue("MsiInstallPath");
					if (!string.IsNullOrEmpty(text2))
					{
						TraceLogResponder.traceLogFilePathName = string.Format("{0}\\tracelog.exe", text2);
						TraceLogResponder.etwFilePathName = string.Format("{0}\\etw.exe", text2);
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("Cannot find EDS dump directory, logFileDirectory needs to be specified.");
			}
			if (traceAttributes.DurationInSeconds <= 0)
			{
				throw new ArgumentException("Trace log duration must be bigger than zero.");
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = TraceLogResponder.AssemblyPath;
			responderDefinition.TypeName = TraceLogResponder.TypeName;
			responderDefinition.Name = traceAttributes.Name;
			responderDefinition.ServiceName = traceAttributes.ServiceName;
			responderDefinition.AlertTypeId = traceAttributes.AlertTypeId;
			responderDefinition.AlertMask = traceAttributes.AlertMask;
			responderDefinition.TargetResource = traceAttributes.TargetResource;
			responderDefinition.TargetHealthState = traceAttributes.TargetHealthState;
			responderDefinition.Enabled = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.TraceLogResponder.Enabled;
			responderDefinition.Attributes["ProviderGuid"] = value;
			responderDefinition.Attributes["ProviderGuidFile"] = traceAttributes.ProviderGuidFile;
			responderDefinition.Attributes["LogFileDirectory"] = text;
			responderDefinition.Attributes["DurationInSeconds"] = traceAttributes.DurationInSeconds.ToString();
			responderDefinition.Attributes["KernelLoggerFlags"] = traceAttributes.KernelLoggerFlags;
			responderDefinition.Attributes["SampleMask"] = traceAttributes.SampleMask;
			responderDefinition.Attributes["false"] = traceAttributes.ShouldAppendAdditionalMessage.ToString();
			int num;
			if (traceAttributes.DurationInSeconds < 60)
			{
				num = 300;
			}
			else
			{
				num = traceAttributes.DurationInSeconds * 2;
			}
			responderDefinition.RecurrenceIntervalSeconds = num * 2;
			responderDefinition.WaitIntervalSeconds = num;
			responderDefinition.TimeoutSeconds = num;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = base.Broker.GetLastSuccessfulResponderResult(base.Definition);
			Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime lastExecutionTime = SqlDateTime.MinValue.Value;
				if (lastResponderResult != null)
				{
					lastExecutionTime = lastResponderResult.ExecutionStartTime;
				}
				IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(this.Definition.AlertMask, lastExecutionTime, this.Result.ExecutionStartTime);
				Task<MonitorResult> task2 = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, this.TraceContext);
				task2.Continue(delegate(MonitorResult lastMonitorResult)
				{
					this.CollectTrace(lastExecutionTime, lastMonitorResult);
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		private static string CreateGuidFile(string providerGuid, string outputFilePath, string guidFileName)
		{
			TraceLogResponder traceLogResponder = new TraceLogResponder();
			string text = Path.Combine(outputFilePath, guidFileName);
			traceLogResponder.OutputToFile(providerGuid, text);
			if (File.Exists(text))
			{
				return text;
			}
			return null;
		}

		private string GetAdEmailData(string data)
		{
			string[] array = Regex.Split(data, "\r\n");
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			for (int i = 0; i <= array.Length - 1; i++)
			{
				if (array[i].Equals("Filter statistics", StringComparison.OrdinalIgnoreCase))
				{
					if (num == 0)
					{
						stringBuilder.AppendLine("Filter Statistics");
						this.ParseEmailDataAndZipAdDriverClientLogs(array, "statistics", 10, i, stringBuilder);
					}
					num++;
				}
				else if (array[i].Equals("MergeCLIENT statistics", StringComparison.OrdinalIgnoreCase))
				{
					if (num2 == 0)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Merged Client Statistics");
						this.ParseEmailDataAndZipAdDriverClientLogs(array, "statistics", 10, i, stringBuilder);
					}
					num2++;
				}
			}
			return stringBuilder.ToString();
		}

		private void ParseEmailDataAndZipAdDriverClientLogs(string[] data, string terminatingString, int maxEntries, int currentEntry, StringBuilder output)
		{
			int num = 0;
			IPAddress ipaddress = null;
			for (int i = currentEntry + 1; i < data.Length; i++)
			{
				string text = data[i];
				if (text.Contains(terminatingString))
				{
					return;
				}
				if (text.StartsWith("COUNT", StringComparison.OrdinalIgnoreCase))
				{
					string ipString = data[i].Split(new char[]
					{
						' '
					})[1];
					if (num == 1)
					{
						string value = string.Format("Top {0} statistics", maxEntries);
						output.AppendLine(value);
					}
					if (ipaddress == null && IPAddress.TryParse(ipString, out ipaddress))
					{
						string text2 = this.ResolveIpAddressToHostname(ipaddress);
						if (!string.IsNullOrEmpty(text2))
						{
							this.FindAndZipFiles(text2);
						}
					}
					num++;
				}
				if (num == maxEntries + 2)
				{
					return;
				}
				output.AppendLine(text);
			}
		}

		private void FindAndZipFiles(string clientName)
		{
			DateTime fromTime = DateTime.UtcNow.AddHours(-1.0);
			string path = string.Format("{0}-{1}.zip", clientName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
			string archiveFileName = Path.Combine(TraceLogResponder.edsDumpDirectory, path);
			string path2 = string.Format("\\\\{0}\\Exchange\\Logging\\ADDriver", clientName);
			if (Directory.Exists(path2))
			{
				try
				{
					IEnumerable<string> enumerable = from f in Directory.GetFiles(path2, "*.LOG")
					orderby File.GetLastWriteTimeUtc(f) descending
					where File.GetLastWriteTimeUtc(f) >= fromTime && File.GetLastWriteTimeUtc(f) <= DateTime.UtcNow
					select f;
					foreach (string text in enumerable)
					{
						using (ZipArchive zipArchive = ZipFile.Open(archiveFileName, ZipArchiveMode.Update))
						{
							string fileName = Path.GetFileName(text);
							try
							{
								zipArchive.CreateEntryFromFile(text, fileName);
							}
							catch (Exception ex)
							{
								WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format(ex.Message, new object[0]), null, "FindAndZipFiles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 424);
							}
						}
					}
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format(ex2.Message, new object[0]), null, "FindAndZipFiles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 434);
				}
			}
		}

		private string ResolveIpAddressToHostname(IPAddress ipAddress)
		{
			string result = string.Empty;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
				result = hostEntry.HostName.ToString();
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format(ex.Message, new object[0]), null, "ResolveIpAddressToHostname", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 458);
			}
			return result;
		}

		private void CollectTrace(DateTime lastExecutionTime, MonitorResult monitorResult)
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			int num = attributeHelper.GetInt("DurationInSeconds", true, 0, null, null);
			string @string = attributeHelper.GetString("LogFileDirectory", true, null);
			string string2 = attributeHelper.GetString("KernelLoggerFlags", false, null);
			string string3 = attributeHelper.GetString("ProviderGuid", false, null);
			string text = attributeHelper.GetString("ProviderGuidFile", false, string.Empty);
			string name = base.Definition.Name;
			string string4 = attributeHelper.GetString("SampleMask", false, string.Empty);
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(string4, lastExecutionTime, base.Result.ExecutionStartTime);
			ProbeResult probeResult = probeResults.FirstOrDefault<ProbeResult>();
			string string5 = attributeHelper.GetString("false", false, string.Empty);
			bool shouldAppendAdditionalMessage;
			bool.TryParse(string5, out shouldAppendAdditionalMessage);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "TraceLogResponder.DoResponderWork: Started CollectTrace.", null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 493);
			string path = string.Format("{0}-{1}-{2}.etl", Environment.MachineName, name, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
			string text2 = Path.Combine(@string, path);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "TraceLogResponder.DoResponderWork: logFileNamePath is {0}.", text2, null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 501);
			if (num > 900)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("TraceLogResponder.DoResponderWork: Maximum time limit is {0} secs.", 900.ToString()), null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 510);
				num = 900;
			}
			this.RunTraceLog(string.Format("-stop \"{0}\"", name), 30000);
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			if (string.IsNullOrEmpty(text))
			{
				string guidFileName = string.Format("{0}-{1}.guid", name, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
				text = TraceLogResponder.CreateGuidFile(string3, @string, guidFileName);
			}
			string arguments = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				arguments = string.Format("-start \"{0}\" -guid \"{1}\" -f \"{2}\" -seq {3} -min {4} -max {5}", new object[]
				{
					name,
					text,
					text2,
					500.ToString(),
					2.ToString(),
					200.ToString()
				});
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("-start \"{0}\" -f \"{1}\" -seq {2} -min {3} -max {4}", new object[]
				{
					name,
					text2,
					500.ToString(),
					2.ToString(),
					200.ToString()
				}));
				if (!string.IsNullOrEmpty(string2))
				{
					stringBuilder.Append(string2);
				}
				arguments = stringBuilder.ToString();
			}
			if (!this.RunTraceLog(arguments, 30000))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("TraceLogResponder.DoResponderWork: Failed to start trace {0}, skip collecting traces.", name), null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 556);
				this.RunTraceLog(string.Format("-stop \"{0}\"", name), 30000);
				return;
			}
			Thread.Sleep(num * 1000);
			if (!this.RunTraceLog(string.Format("-stop \"{0}\"", name), 30000))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("TraceLogResponder.DoResponderWork: Cannot stop trace {0}, skip collecting traces.", name), null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 572);
				this.RunTraceLog(string.Format("-stop \"{0}\"", name), 30000);
				return;
			}
			try
			{
				string text3 = File.ReadAllText(text);
				if (text3.Contains("1C83B2FC-C04F-11D1-8AFC-00C04FC21914") && !this.AnalyzeAdTraceLog(name, text2, @string, 60000, shouldAppendAdditionalMessage, probeResult.Error))
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("TraceLogResponder.DoResponderWork: Cannot analyze trace {0}", name), null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 590);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("TraceLogResponder.DoResponderWork: Cannot analyze trace {0}", name, ex.Message), null, "CollectTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 600);
			}
		}

		private bool RunTraceLog(string arguments, int timeoutInMilliSeconds)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.FileName = TraceLogResponder.traceLogFilePathName;
			processStartInfo.Arguments = arguments;
			bool result;
			try
			{
				using (Process process = Process.Start(processStartInfo))
				{
					process.WaitForExit(timeoutInMilliSeconds);
					result = (process.ExitCode == 0);
				}
			}
			catch (FileNotFoundException)
			{
				result = false;
			}
			return result;
		}

		private bool AnalyzeAdTraceLog(string traceName, string traceFileNamePath, string outputFileLocation, int timeoutInMilliSeconds, bool shouldAppendAdditionalMessage, string probeResult)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.CreateNoWindow = false;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.FileName = TraceLogResponder.etwFilePathName;
			string path = string.Format("{0}-{1}-{2}-ADDiagParsed.txt", Environment.MachineName, traceName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
			string text = Path.Combine(outputFileLocation, path);
			processStartInfo.Arguments = traceFileNamePath;
			if (this.ShouldRunEtw(traceFileNamePath))
			{
				try
				{
					using (Process process = Process.Start(processStartInfo))
					{
						string message = string.Format("TraceLogResponder.DoResponderWork: Parsing trace file:{0}, and sending output to:{1}", traceFileNamePath, text);
						WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message, null, "AnalyzeAdTraceLog", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 675);
						string data = process.StandardOutput.ReadToEnd();
						this.OutputToFile(data, text);
						if (shouldAppendAdditionalMessage)
						{
							string adEmailData = this.GetAdEmailData(data);
							string text2 = probeResult + Environment.NewLine + Environment.NewLine + adEmailData;
							string path2 = string.Format("{0}-{1}-{2}-EmailMessage.txt", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), Environment.MachineName, traceName);
							string outputFileNamePath = Path.Combine(outputFileLocation, path2);
							this.OutputToFile(text2, outputFileNamePath);
							lock (EscalateResponderHelper.CustomMessageDictionaryLock)
							{
								EscalateResponderHelper.AdditionalMessageContainer value = new EscalateResponderHelper.AdditionalMessageContainer(DateTime.UtcNow, text2);
								EscalateResponderHelper.AlertMaskToCustomMessageMap[base.Definition.AlertMask] = value;
							}
						}
						process.WaitForExit(timeoutInMilliSeconds);
						return process.ExitCode == 0;
					}
				}
				catch (FileNotFoundException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		private void OutputToFile(string data, string outputFileNamePath)
		{
			try
			{
				File.WriteAllText(outputFileNamePath, data);
			}
			catch (Exception ex)
			{
				string message = string.Format("TraceLogResponder.DoResponderWork: Writing trace parse output failed:{0}, due to {1}", outputFileNamePath, ex.Message);
				WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message, null, "OutputToFile", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 736);
			}
		}

		private bool ShouldRunEtw(string traceFileNamePath)
		{
			for (int i = 0; i < 5; i++)
			{
				try
				{
					using (FileStream fileStream = new FileStream(traceFileNamePath, FileMode.Open, FileAccess.Read))
					{
						fileStream.ReadByte();
						return true;
					}
				}
				catch (Exception ex)
				{
					string message = string.Format("TraceLogResponder.DoResponderWork: Cannot open trace file:{0}, due to {1}, retrying...", traceFileNamePath, ex.Message);
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message, null, "ShouldRunEtw", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\TraceLogResponder.cs", 769);
				}
				Thread.Sleep(2000);
			}
			return false;
		}

		private const int MaximumFileSize = 500;

		private const int MaximumBufferCount = 200;

		private const int MinimumBufferCount = 2;

		private const int MaximumDurationInSeconds = 900;

		private const string ADDiagTraceGuid = "1C83B2FC-C04F-11D1-8AFC-00C04FC21914";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string DiagnosticsRegistryKey = string.Format("Software\\Microsoft\\ExchangeServer\\{0}\\Diagnostics", "v15");

		private static readonly string TypeName = typeof(TraceLogResponder).FullName;

		private static string edsDumpDirectory;

		private static string traceLogFilePathName;

		private static string etwFilePathName;

		public class TraceAttributes
		{
			public TraceAttributes(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string providerGuid, string providerGuidFile, string logFileDirectory, int durationInSeconds, string kernelLoggerFlags, string sampleMask, bool shouldAppendAdditionalMessage)
			{
				this.Name = name;
				this.ServiceName = serviceName;
				this.AlertTypeId = alertTypeId;
				this.AlertMask = alertMask;
				this.TargetHealthState = targetHealthState;
				this.ProviderGuid = providerGuid;
				this.ProviderGuidFile = providerGuidFile;
				this.LogFileDirectory = logFileDirectory;
				this.DurationInSeconds = durationInSeconds;
				this.KernelLoggerFlags = kernelLoggerFlags;
				this.SampleMask = sampleMask;
				this.ShouldAppendAdditionalMessage = shouldAppendAdditionalMessage;
			}

			public readonly string Name;

			public readonly string ServiceName;

			public readonly string AlertTypeId;

			public readonly string AlertMask;

			public readonly string TargetResource;

			public readonly ServiceHealthStatus TargetHealthState;

			public readonly string ProviderGuid;

			public readonly string ProviderGuidFile;

			public readonly string LogFileDirectory;

			public readonly int DurationInSeconds;

			public readonly string KernelLoggerFlags;

			public readonly string SampleMask;

			public readonly bool ShouldAppendAdditionalMessage;
		}

		internal static class AttributeNames
		{
			internal const string ProviderGuidFile = "ProviderGuidFile";

			internal const string ProviderGuid = "ProviderGuid";

			internal const string KernelLoggerFlags = "KernelLoggerFlags";

			internal const string LogFileDirectory = "LogFileDirectory";

			internal const string DurationInSeconds = "DurationInSeconds";

			internal const string SampleMask = "SampleMask";

			internal const string ShouldAppendAdditionalMessage = "false";
		}
	}
}

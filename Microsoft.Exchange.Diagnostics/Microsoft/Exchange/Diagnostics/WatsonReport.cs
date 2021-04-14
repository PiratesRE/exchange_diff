using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Internal;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class WatsonReport
	{
		public WatsonReport(string eventType, Process process)
		{
			ExWatson.IncrementWatsonCountIn();
			DiagnosticsNativeMethods.WerSetFlags(DiagnosticsNativeMethods.WER_FLAGS.WER_FAULT_REPORTING_FLAG_QUEUE_UPLOAD);
			bool flag = false;
			if (process == null)
			{
				process = Process.GetCurrentProcess();
				flag = true;
			}
			try
			{
				if (WatsonReport.bucketingParamNames == null)
				{
					Interlocked.Exchange<string[]>(ref WatsonReport.bucketingParamNames, new string[]
					{
						"flavor",
						"appVersion",
						"appName",
						"assemblyName",
						"exMethodName",
						"exceptionType",
						"callstackHash",
						"assemblyVer"
					});
				}
				this.eventType = eventType;
				this.bucketingParams = new string[8];
				this.process = process;
				int num;
				if (this.process != null)
				{
					num = this.EvaluateOrDefault<int>(() => this.process.Id, -1);
				}
				else
				{
					num = -1;
				}
				this.processId = num;
				this.targetExternalProcess = (this.processId != -1 && this.processId != DiagnosticsNativeMethods.GetCurrentProcessId());
				this.processHandle = new ProcSafeHandle();
				this.reportInfo = new DiagnosticsNativeMethods.WER_REPORT_INFORMATION();
				this.reportingAllowed = ExWatson.ErrorReportingAllowed;
				this.extraData = new StringBuilder();
				this.extraFiles = new StringBuilder();
				this.debugBuild = false;
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.Flavor, this.GetFlavor());
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppVersion, WatsonReport.ExchangeFormattedVersion(this.AppVersion));
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName, WatsonReport.GetShortParameter(this.AppName));
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName, "unknown");
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName, "unknown");
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType, string.Empty);
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash, "0");
				this.SetBucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyVer, "unknown");
				IntPtr intPtr = (IntPtr)(-1);
				if (!flag)
				{
					try
					{
						if (this.process != null)
						{
							intPtr = this.process.Handle;
						}
					}
					catch
					{
						this.process = null;
					}
				}
				this.reportInfo.size = (uint)Marshal.SizeOf(typeof(DiagnosticsNativeMethods.WER_REPORT_INFORMATION));
				this.reportInfo.process = intPtr;
				this.reportInfo.consentKey = string.Empty;
				this.reportInfo.friendlyEventName = string.Empty;
				if (this.process == null)
				{
					this.reportInfo.applicationName = "unknown";
					this.reportInfo.applicationPath = "unknown";
				}
				else
				{
					string tmpProcessName = flag ? this.ProcessName : this.process.ProcessName;
					this.reportInfo.applicationName = this.EvaluateOrDefault<string>(() => tmpProcessName, "unknown");
					this.reportInfo.applicationPath = this.EvaluateOrDefault<string>(() => this.process.MainModule.FileName, "unknown");
				}
				this.reportInfo.description = string.Empty;
				this.reportInfo.parentWindowHandle = IntPtr.Zero;
			}
			finally
			{
				if (flag)
				{
					if (this.process != null)
					{
						this.process.Dispose();
					}
					this.process = null;
				}
			}
		}

		internal Dictionary<Type, List<WatsonReportAction>> ReportActions { get; set; }

		internal string CrashDetails { get; set; }

		internal bool TargetExternalProcess
		{
			get
			{
				return this.targetExternalProcess;
			}
		}

		internal string AppName
		{
			get
			{
				if (string.IsNullOrEmpty(this.appName))
				{
					if (this.targetExternalProcess)
					{
						this.appName = this.EvaluateOrDefault<string>(delegate
						{
							string fileName = Path.GetFileName(this.process.MainModule.FileName);
							string processCommandLine = this.GetProcessCommandLine(this.processId);
							return ExWatson.GetRealAppName(fileName, processCommandLine);
						}, "unknown");
					}
					else
					{
						this.appName = ExWatson.RealAppName;
					}
				}
				return this.appName;
			}
		}

		internal Version AppVersion
		{
			get
			{
				if (this.appVersion == null)
				{
					if (this.targetExternalProcess)
					{
						if (!ExWatson.TryGetRealApplicationVersion(this.process, out this.appVersion))
						{
							this.appVersion = ExWatson.DefaultAssemblyVersion;
						}
					}
					else
					{
						this.appVersion = ExWatson.ApplicationVersion;
					}
				}
				return this.appVersion;
			}
		}

		protected static object HeapDumpReportLockObject
		{
			get
			{
				return WatsonReport.heapDumpReportLockObject;
			}
		}

		protected bool ReportingAllowed
		{
			get
			{
				return this.reportingAllowed;
			}
		}

		protected string EventType
		{
			get
			{
				return this.eventType;
			}
		}

		protected bool IsProcessValid
		{
			get
			{
				return this.process != null;
			}
		}

		protected int ProcessId
		{
			get
			{
				return this.processId;
			}
		}

		protected ProcSafeHandle ProcessHandle
		{
			get
			{
				return this.processHandle;
			}
		}

		protected ReportOptions ReportOptions
		{
			get
			{
				return this.reportOptions;
			}
			set
			{
				this.reportOptions = value;
			}
		}

		protected string HexEscapedArchiveReportName
		{
			get
			{
				StringBuilder archivedReportName = this.GetArchivedReportName();
				WatsonReport.HexEscapeInvalidCharactersInFileName(archivedReportName);
				return archivedReportName.ToString();
			}
		}

		private string ProcessName
		{
			get
			{
				if (string.IsNullOrEmpty(this.processName))
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						this.processName = currentProcess.ProcessName;
					}
				}
				return this.processName;
			}
		}

		public virtual void Send()
		{
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			this.reportHandle = new WerSafeHandle();
			using (this.processHandle = this.GetProcessHandle())
			{
				try
				{
					Directory.CreateDirectory(text, WatsonReport.CreateDirectorySecurityHelper(WatsonReport.TempDirPermissions));
					this.PrepareBucketingParameters();
					WatsonEventLog.TryLogCrash(this.Get4999EventParams());
					this.CreateReportHandle();
					string reportXmlFileName = this.WriteXmlReport(text);
					string reportTextFileName = this.WriteTextReport(text);
					this.InternalSubmit(reportXmlFileName, reportTextFileName);
				}
				catch (Exception ex)
				{
					WatsonEventLog.TryLogReportError(new object[]
					{
						string.Empty,
						ex.ToString()
					});
				}
			}
			if (!ExWatson.TestLabMachine)
			{
				for (int i = 0; i < 5; i++)
				{
					try
					{
						Directory.Delete(text, true);
						break;
					}
					catch (Exception)
					{
						Thread.Sleep(1000);
					}
				}
			}
		}

		internal static int ComputeHash(string name, int hash)
		{
			for (int i = 0; i < name.Length; i++)
			{
				hash = (hash << 5) + (hash >> 11);
				hash ^= (int)name[i];
			}
			return hash;
		}

		internal static string StripExecutableExtension(string longParameter)
		{
			if (longParameter.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || longParameter.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			{
				longParameter = longParameter.Substring(0, longParameter.Length - 4);
			}
			return longParameter;
		}

		internal static string GetShortParameter(string longParameter)
		{
			longParameter = WatsonReport.StripExecutableExtension(longParameter);
			if (string.IsNullOrEmpty(longParameter))
			{
				return string.Empty;
			}
			string result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder(longParameter.Length);
				int i = 0;
				int j = -1;
				while (i < longParameter.Length)
				{
					int num = longParameter.IndexOfAny(WatsonReport.GenericNameDelimiters, i);
					bool flag;
					bool flag2;
					if (num > 0)
					{
						if (longParameter[num] == '[')
						{
							flag = true;
							flag2 = false;
						}
						else
						{
							flag = false;
							flag2 = true;
						}
					}
					else
					{
						flag = false;
						flag2 = false;
						num = longParameter.Length;
					}
					while (i < num)
					{
						int num2 = longParameter.LastIndexOf('.', num - 1);
						if (num2 > 0)
						{
							int num3 = num2;
							num2 = longParameter.LastIndexOf('.', num2 - 1);
							if (num2 == num3 - 1)
							{
								num2 = longParameter.LastIndexOf('.', num2 - 1);
							}
						}
						while (j < num2)
						{
							char c = longParameter[j + 1];
							stringBuilder.Append(c);
							if (c != '.')
							{
								stringBuilder.Append('.');
							}
							j = longParameter.IndexOf('.', j + 1);
							if (j < 0)
							{
								break;
							}
						}
						for (i = j + 1; i < num; i++)
						{
							stringBuilder.Append(longParameter[i]);
						}
						if (flag || flag2)
						{
							stringBuilder.Append(flag ? '[' : ',');
							j = i;
							i++;
						}
					}
				}
				string text = stringBuilder.ToString();
				result = text;
			}
			catch
			{
				result = "unknown";
			}
			return result;
		}

		internal string GetProcessCommandLine(int processId)
		{
			string result = string.Empty;
			if (processId > 0)
			{
				try
				{
					string queryString = string.Format("SELECT CommandLine FROM Win32_Process WHERE ProcessID = '{0}'", processId);
					using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
					{
						foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
						{
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							using (managementObject)
							{
								PropertyData propertyData = managementObject.Properties["CommandLine"];
								result = ((propertyData == null || propertyData.Value == null) ? string.Empty : propertyData.Value.ToString());
								break;
							}
						}
					}
				}
				catch (Exception e)
				{
					this.RecordExceptionWhileCreatingReport(e);
				}
			}
			return result;
		}

		internal void LogExtraFile(string fileName)
		{
			this.extraFiles.AppendLine(fileName);
		}

		internal void LogExtraData(string data)
		{
			this.extraData.AppendLine(data);
		}

		internal void PerformWerOperation(WatsonReport.WerReportDelegate reportDelegate)
		{
			if (this.reportHandle.IsInvalid || this.reportHandle.IsClosed)
			{
				throw new InvalidOperationException("The Windows Error Report handle is invalid or closed.");
			}
			reportDelegate(this.reportHandle);
		}

		internal void RecordExceptionWhileCreatingReport(Exception e)
		{
			if (e != null)
			{
				if (this.watsonProcessingExceptions == null)
				{
					this.watsonProcessingExceptions = new List<Exception>(10);
				}
				this.watsonProcessingExceptions.Add(e);
			}
		}

		protected static string GetValidString(string s)
		{
			if (s != null)
			{
				s = s.Trim();
			}
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return "unknown";
		}

		protected static string ExchangeFormattedVersion(Version v)
		{
			if (v == null)
			{
				return "unknown";
			}
			return string.Format("{0:d2}.{1:d2}.{2:d4}.{3:d3}", new object[]
			{
				v.Major,
				v.Minor,
				v.Build,
				v.Revision
			});
		}

		protected static string EnforceTextLength(string text, int maxTextLength)
		{
			string text2 = text;
			if (!string.IsNullOrEmpty(text2) && text2.Length > maxTextLength - 1)
			{
				text2 = text2.Substring(0, maxTextLength - 1);
			}
			return text2;
		}

		protected static string SanitizeException(string exception)
		{
			string[] array = exception.Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder(exception.Length);
			foreach (string text in array)
			{
				if (!text.StartsWith("at ") && !text.StartsWith("   at "))
				{
					stringBuilder.AppendLine(text);
				}
				else if (text.Contains("Microsoft") || !text.Contains(" in "))
				{
					stringBuilder.AppendLine(text);
				}
				else
				{
					int num = text.IndexOf(" in ");
					if (num > 0)
					{
						stringBuilder.AppendLine(text.Substring(0, num));
					}
				}
			}
			return stringBuilder.ToString().Trim();
		}

		protected static bool IsAssemblyDynamic(Assembly assembly)
		{
			return assembly.IsDynamic;
		}

		protected static void HexEscapeInvalidCharactersInFileName(StringBuilder fileName)
		{
			foreach (char character in Path.GetInvalidFileNameChars())
			{
				fileName.Replace(character.ToString(), Uri.HexEscape(character));
			}
		}

		protected bool IsFlagSet(ReportOptions option)
		{
			return (this.reportOptions & option) != ReportOptions.None;
		}

		protected virtual string GetShortParameter(uint bucketParamId, string longParameter)
		{
			return WatsonReport.GetShortParameter(longParameter);
		}

		protected void WriteReportFileHeader(TextWriter reportFile, string headerComment)
		{
			if (reportFile == null)
			{
				return;
			}
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("------------------- Error Report -------------------");
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("Error report created {0}", DateTime.UtcNow.ToString());
			if (headerComment != null)
			{
				reportFile.WriteLine(headerComment);
			}
			reportFile.WriteLine();
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("--------------- Bucketing Parameters ---------------");
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("EventType={0}", this.eventType);
		}

		protected virtual ProcSafeHandle GetProcessHandle()
		{
			ProcSafeHandle procSafeHandle = new ProcSafeHandle();
			ProcSafeHandle procSafeHandle2 = this.EvaluateOrDefault<ProcSafeHandle>(() => new ProcSafeHandle((this.process == null) ? ((IntPtr)(-1)) : this.process.Handle, false), procSafeHandle);
			if (procSafeHandle2 != procSafeHandle && procSafeHandle != null)
			{
				procSafeHandle.Dispose();
			}
			return procSafeHandle2;
		}

		protected virtual string[] GetBucketingParamNames()
		{
			return WatsonReport.bucketingParamNames;
		}

		protected virtual void PrepareBucketingParameters()
		{
		}

		protected abstract WatsonIssueType GetIssueTypeCode();

		protected abstract string GetIssueDetails();

		protected abstract void WriteReportTypeSpecificSection(XmlWriter reportFile);

		protected abstract void WriteSpecializedPartOfTextReport(TextWriter reportFile);

		protected virtual void AddExtraFiles(WerSafeHandle watsonReportHandle)
		{
		}

		protected virtual void BeforeSubmit()
		{
		}

		protected virtual void AfterSubmit()
		{
		}

		protected virtual object[] Get4999EventParams()
		{
			return new object[]
			{
				this.processId,
				this.EventType,
				this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.Flavor),
				this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppVersion),
				this.GetShortParameter(2U, this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName)),
				this.GetShortParameter(3U, this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyName)),
				this.GetShortParameter(4U, this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName)),
				this.GetShortParameter(5U, this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExceptionType)),
				this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash),
				this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AssemblyVer),
				this.ReportingAllowed ? "True" : "False",
				this.IsFlagSet(ReportOptions.ReportTerminateAfterSend) ? "True" : "False",
				WatsonReport.StripExecutableExtension(this.AppName),
				this.CrashDetails ?? string.Empty
			};
		}

		protected virtual StringBuilder GetArchivedReportName()
		{
			string shortParameter = WatsonReport.GetShortParameter(this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.AppName));
			string shortParameter2 = WatsonReport.GetShortParameter(this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.ExMethodName));
			string text = this.BucketingParameter<WatsonReport.BucketParamId>(WatsonReport.BucketParamId.CallstackHash);
			StringBuilder stringBuilder = new StringBuilder(shortParameter.Length + shortParameter2.Length + text.Length + "ExWatsonReport.xml".Length + 3);
			stringBuilder.Append(shortParameter);
			stringBuilder.Append('-');
			stringBuilder.Append(shortParameter2);
			stringBuilder.Append('-');
			stringBuilder.Append(text);
			stringBuilder.Append('-');
			stringBuilder.Append("ExWatsonReport.xml");
			return stringBuilder;
		}

		protected string BucketingParameter<T>(T paramId) where T : IConvertible
		{
			return WatsonReport.GetValidString(this.bucketingParams[paramId.ToInt32(NumberFormatInfo.InvariantInfo)]);
		}

		protected void SetBucketingParameter<T>(T paramId, string value) where T : IConvertible
		{
			this.bucketingParams[paramId.ToInt32(NumberFormatInfo.InvariantInfo)] = value;
		}

		protected T EvaluateOrDefault<T>(Util.TryDelegate<T> expression, T defaultValue)
		{
			return Util.EvaluateOrDefault<T>(expression, defaultValue, delegate(Exception ex)
			{
				if (this.watsonProcessingExceptions == null)
				{
					this.watsonProcessingExceptions = new List<Exception>(10);
				}
				this.watsonProcessingExceptions.Add(ex);
			});
		}

		private static KeyValuePair<string, StreamWriter> CreateReportTxtFile(string tempDir)
		{
			string text = Path.Combine(tempDir, "report.txt");
			StreamWriter value = null;
			try
			{
				value = new StreamWriter(text, false, Encoding.Unicode);
			}
			catch
			{
			}
			return new KeyValuePair<string, StreamWriter>(text, value);
		}

		private static KeyValuePair<string, XmlWriter> CreateReportXmlFile(string tempDir)
		{
			string text = Path.Combine(tempDir, "report.xml");
			XmlWriter xmlWriter = null;
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.NewLineHandling = NewLineHandling.Entitize;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			string text2 = "type=\"text/xsl\" href=\"" + string.Format("http://exiis/watson/watsonreport.{0}.xsl", "1.00") + "\"";
			try
			{
				xmlWriter = XmlWriter.Create(text, xmlWriterSettings);
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteProcessingInstruction("xml-stylesheet", text2);
			}
			catch
			{
				try
				{
					if (xmlWriter != null)
					{
						xmlWriter.Dispose();
					}
				}
				catch
				{
				}
			}
			return new KeyValuePair<string, XmlWriter>(text, xmlWriter);
		}

		private static DirectorySecurity CreateDirectorySecurityHelper(FileSystemAccessRule[] directoryPermissions)
		{
			DirectorySecurity directorySecurity = new DirectorySecurity();
			for (int i = 0; i < directoryPermissions.Length; i++)
			{
				directorySecurity.AddAccessRule(directoryPermissions[i]);
			}
			directorySecurity.SetAccessRuleProtection(true, false);
			return directorySecurity;
		}

		private static bool ByteArraysEqual(byte[] a, byte[] b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		private static string GetBuildType()
		{
			string str = string.Empty;
			byte[] publicKeyToken = WatsonReport.GetPublicKeyToken();
			if (publicKeyToken == null)
			{
				return ExWatson.LabName;
			}
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					if (WatsonReport.IsBuddyAssembly(assembly, publicKeyToken))
					{
						str = "buddy-";
						break;
					}
				}
				catch
				{
				}
			}
			return ExWatson.LabName + str;
		}

		private static byte[] GetPublicKeyToken()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			byte[] result;
			try
			{
				result = executingAssembly.GetName().GetPublicKeyToken();
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private static bool IsBuddyAssembly(Assembly assembly, byte[] publicKeyToken)
		{
			byte[] publicKeyToken2 = assembly.GetName().GetPublicKeyToken();
			if (!WatsonReport.ByteArraysEqual(publicKeyToken2, publicKeyToken))
			{
				return false;
			}
			FileVersionInfo fileVersionInfo;
			try
			{
				fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			}
			catch
			{
				fileVersionInfo = null;
			}
			if (fileVersionInfo == null || !fileVersionInfo.IsPrivateBuild)
			{
				return false;
			}
			string text = fileVersionInfo.ProductName ?? string.Empty;
			return text.Contains("Exchange");
		}

		private static string GetAssemblyBuddyInfo(Assembly assembly, byte[] publicKeyToken)
		{
			if (publicKeyToken == null)
			{
				return "unknown";
			}
			if (!WatsonReport.IsBuddyAssembly(assembly, publicKeyToken))
			{
				return "False";
			}
			return "True";
		}

		private string WriteTextReport(string tempDir)
		{
			StreamWriter streamWriter = null;
			string text = string.Empty;
			try
			{
				KeyValuePair<string, StreamWriter> keyValuePair = WatsonReport.CreateReportTxtFile(tempDir);
				text = keyValuePair.Key;
				streamWriter = keyValuePair.Value;
				if (streamWriter != null)
				{
					this.WriteReport<StreamWriter>(streamWriter, new Action<StreamWriter>(this.WriteSpecializedPartOfTextReport));
					this.WriteReport<StreamWriter>(streamWriter, new Action<StreamWriter>(this.WriteHandlerStackTrace));
					this.WriteReport<StreamWriter>(streamWriter, new Action<StreamWriter>(this.WriteReportFileAssemblies));
					this.WriteReport<StreamWriter>(streamWriter, new Action<StreamWriter>(this.WriteReportFileExtraData));
				}
			}
			catch (Exception e)
			{
				this.RecordExceptionWhileCreatingReport(e);
			}
			finally
			{
				if (streamWriter != null)
				{
					try
					{
						this.WriteWatsonDiagnosticData(streamWriter);
					}
					catch (Exception ex)
					{
						WatsonEventLog.TryLogReportError(new object[]
						{
							text,
							ex.ToString()
						});
					}
					finally
					{
						try
						{
							streamWriter.Dispose();
						}
						catch
						{
						}
					}
				}
			}
			return text;
		}

		private void CreateReportHandle()
		{
			if (this.ReportingAllowed)
			{
				string[] array = this.GetBucketingParamNames();
				this.reportHandle = DiagnosticsNativeMethods.WerReportCreate(this.eventType, DiagnosticsNativeMethods.WER_REPORT_TYPE.WerReportNonCritical, this.reportInfo);
				uint num = 0U;
				while ((ulong)num < (ulong)((long)this.bucketingParams.Length))
				{
					string text = this.bucketingParams[(int)((UIntPtr)num)];
					if (num == 2U || num == 3U || num == 4U || num == 5U)
					{
						text = this.GetShortParameter(num, text);
					}
					if (!string.IsNullOrEmpty(text))
					{
						DiagnosticsNativeMethods.WerReportSetParameter(this.reportHandle, num, array[(int)((UIntPtr)num)], WatsonReport.EnforceTextLength(text, 255));
					}
					num += 1U;
				}
				this.AddExtraFiles(this.reportHandle);
			}
		}

		private string WriteXmlReport(string tempDir)
		{
			XmlWriter xmlWriter = null;
			string text = string.Empty;
			SafeXmlTag xmlTopLevel = null;
			try
			{
				KeyValuePair<string, XmlWriter> keyValuePair = WatsonReport.CreateReportXmlFile(tempDir);
				xmlWriter = keyValuePair.Value;
				text = keyValuePair.Key;
				if (xmlWriter != null)
				{
					this.WriteReport<XmlWriter>(xmlWriter, delegate(XmlWriter reportFile)
					{
						xmlTopLevel = new SafeXmlTag(reportFile, "watson-report").WithAttribute("event-type", this.eventType).WithAttribute("issue-type", ((int)this.GetIssueTypeCode()).ToString()).WithAttribute("timestamp", DateTime.UtcNow.ToString("u")).WithAttribute("reporting-enabled", this.ReportingAllowed ? "1" : "0").WithAttribute("terminate-after-send", this.IsFlagSet(ReportOptions.ReportTerminateAfterSend) ? "1" : "0").WithAttribute("xml", "space", "preserve");
					});
					this.WriteReport<XmlWriter>(xmlWriter, new Action<XmlWriter>(this.WriteReportXmlFile));
					this.WriteReport<XmlWriter>(xmlWriter, new Action<XmlWriter>(this.WriteReportFileAssemblies));
					this.WriteReport<XmlWriter>(xmlWriter, new Action<XmlWriter>(this.WriteReportTypeSpecificSection));
					using (new SafeXmlTag(xmlWriter, "report-actions"))
					{
						ExWatson.EvaluateReportActions(xmlWriter, this);
					}
				}
			}
			catch (Exception e)
			{
				this.RecordExceptionWhileCreatingReport(e);
			}
			finally
			{
				if (xmlWriter != null)
				{
					try
					{
						this.WriteWatsonDiagnosticData(xmlWriter);
						if (xmlTopLevel != null)
						{
							xmlTopLevel.Dispose();
							xmlTopLevel = null;
						}
						xmlWriter.WriteEndDocument();
					}
					catch (Exception ex)
					{
						WatsonEventLog.TryLogReportError(new object[]
						{
							text,
							ex.ToString()
						});
					}
					finally
					{
						try
						{
							xmlWriter.Dispose();
						}
						catch
						{
						}
					}
				}
			}
			return text;
		}

		private void WriteReportXmlFile(XmlWriter reportFile)
		{
			using (SafeXmlTag safeXmlTag = new SafeXmlTag(reportFile, "details"))
			{
				safeXmlTag.SetContent(this.GetIssueDetails());
			}
			string text = this.GetFlavor();
			int num = text.LastIndexOf('-');
			if (num > 0 && num < text.Length - 1)
			{
				text.Substring(num + 1);
				text = text.Substring(0, num);
			}
			num = text.LastIndexOf('-');
			if (num > 0 && num < text.Length - 1)
			{
				text.Substring(num + 1);
				text = text.Substring(0, num);
			}
			string value = text;
			using (new SafeXmlTag(reportFile, "machine-data").WithAttribute("name", ExWatson.MsInternal ? ExWatson.ComputerName : string.Empty).WithAttribute("type", value).WithAttribute("arch", ExWatson.ProcessorArchitecture).WithAttribute("flavor", this.debugBuild ? "DBG" : "RTL").WithAttribute("osver", Environment.OSVersion.VersionString))
			{
			}
			using (new SafeXmlTag(reportFile, "exchange-data").WithAttribute("version", WatsonReport.ExchangeFormattedVersion(ExWatson.ExchangeVersion)).WithAttribute("install-path", ExWatson.ExchangeInstallPath).WithAttribute("install-source", ExWatson.MsInternal ? ExWatson.ExchangeInstallSource : string.Empty))
			{
			}
			using (new SafeXmlTag(reportFile, "bucketing-params"))
			{
				string[] array = this.GetBucketingParamNames();
				int num2 = array.Length;
				for (int i = 0; i < num2; i++)
				{
					using (SafeXmlTag safeXmlTag5 = new SafeXmlTag(reportFile, "param").WithAttribute("id", "P" + (i + 1)).WithAttribute("name", array[i]))
					{
						safeXmlTag5.SetContent(this.bucketingParams[i]);
					}
				}
			}
			using (SafeXmlTag safeXmlTag6 = new SafeXmlTag(reportFile, "handler-stacktrace"))
			{
				safeXmlTag6.SetContent(WatsonReport.SanitizeException(new StackTrace(true).ToString()));
			}
		}

		private void InternalSubmit(string reportXmlFileName, string reportTextFileName)
		{
			DiagnosticsNativeMethods.WER_SUBMIT_RESULT wer_SUBMIT_RESULT = DiagnosticsNativeMethods.WER_SUBMIT_RESULT.WerDisabled;
			try
			{
				if (this.ReportingAllowed)
				{
					if (this.reportHandle.IsInvalid)
					{
						wer_SUBMIT_RESULT = DiagnosticsNativeMethods.WER_SUBMIT_RESULT.WerReportFailed;
					}
					else
					{
						DiagnosticsNativeMethods.WER_FILE_FLAGS fileFlags = ExWatson.TestLabMachine ? ((DiagnosticsNativeMethods.WER_FILE_FLAGS)0U) : DiagnosticsNativeMethods.WER_FILE_FLAGS.WER_FILE_DELETE_WHEN_DONE;
						DiagnosticsNativeMethods.WerReportAddFile(this.reportHandle, reportTextFileName, DiagnosticsNativeMethods.WER_FILE_TYPE.WerFileTypeOther, fileFlags);
						DiagnosticsNativeMethods.WerReportAddFile(this.reportHandle, reportXmlFileName, DiagnosticsNativeMethods.WER_FILE_TYPE.WerFileTypeOther, fileFlags);
						if (ExWatson.ShouldSubmitReport(this, reportXmlFileName, reportTextFileName, ref wer_SUBMIT_RESULT))
						{
							try
							{
								DiagnosticsNativeMethods.WER_SUBMIT_FLAGS wer_SUBMIT_FLAGS = DiagnosticsNativeMethods.WER_SUBMIT_FLAGS.WER_SUBMIT_OUTOFPROCESS | DiagnosticsNativeMethods.WER_SUBMIT_FLAGS.WER_SUBMIT_NO_CLOSE_UI | DiagnosticsNativeMethods.WER_SUBMIT_FLAGS.WER_SUBMIT_NO_QUEUE | DiagnosticsNativeMethods.WER_SUBMIT_FLAGS.WER_SUBMIT_START_MINIMIZED;
								if (ExWatson.TestLabMachine || ExWatson.WerSubmitBypassDataThrottling)
								{
									wer_SUBMIT_FLAGS |= DiagnosticsNativeMethods.WER_SUBMIT_FLAGS.WER_SUBMIT_BYPASS_DATA_THROTTLING;
								}
								this.BeforeSubmit();
								bool flag = true;
								try
								{
									DiagnosticsNativeMethods.SubmitExWatsonReport(this.reportHandle, DiagnosticsNativeMethods.WER_CONSENT.WerConsentApproved, wer_SUBMIT_FLAGS, IntPtr.Zero, false, this.IsFlagSet(ReportOptions.ReportTerminateAfterSend));
								}
								catch (DllNotFoundException)
								{
									flag = false;
								}
								if (!flag)
								{
									DiagnosticsNativeMethods.WerReportSubmit(this.reportHandle, DiagnosticsNativeMethods.WER_CONSENT.WerConsentApproved, wer_SUBMIT_FLAGS, IntPtr.Zero);
								}
							}
							finally
							{
								this.AfterSubmit();
							}
						}
					}
				}
			}
			catch
			{
				wer_SUBMIT_RESULT = DiagnosticsNativeMethods.WER_SUBMIT_RESULT.WerReportFailed;
			}
			finally
			{
				this.reportHandle.Dispose();
				ExWatson.IncrementWatsonCountOut();
			}
		}

		private bool TryGetCustomAttribute<TAttribute>(Assembly assembly, out TAttribute result) where TAttribute : Attribute
		{
			bool result2 = false;
			result = default(TAttribute);
			try
			{
				IEnumerable<TAttribute> customAttributes = assembly.GetCustomAttributes<TAttribute>();
				if (customAttributes != null)
				{
					using (IEnumerator<TAttribute> enumerator = customAttributes.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							TAttribute tattribute = enumerator.Current;
							if (!enumerator.MoveNext())
							{
								result = tattribute;
								result2 = true;
							}
						}
					}
				}
			}
			catch
			{
			}
			return result2;
		}

		private void WriteHandlerStackTrace(TextWriter reportFile)
		{
			StackTrace stackTrace = new StackTrace(true);
			reportFile.WriteLine(reportFile.NewLine);
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("------------- StackTrace from handler --------------");
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("This is the call stack from where the exception was caught, not where it was thrown.");
			reportFile.WriteLine(WatsonReport.SanitizeException(stackTrace.ToString()));
		}

		private void WriteReportFileAssemblies(TextWriter reportFile)
		{
			reportFile.WriteLine(reportFile.NewLine);
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("-------------------- Assemblies --------------------");
			reportFile.WriteLine("----------------------------------------------------");
			Assembly[] array = this.EvaluateOrDefault<Assembly[]>(() => AppDomain.CurrentDomain.GetAssemblies(), new Assembly[0]);
			if (array.Length == 0)
			{
				reportFile.WriteLine("<Error getting list of current appdomain assemblies>");
			}
			byte[] publicKeyToken = WatsonReport.GetPublicKeyToken();
			Assembly[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Assembly assembly = array2[i];
				string text = this.EvaluateOrDefault<string>(() => assembly.FullName, "<Unknown Assembly>");
				string text2;
				if (!WatsonReport.IsAssemblyDynamic(assembly))
				{
					text2 = this.EvaluateOrDefault<string>(() => assembly.Location, "<Unknown Location>");
				}
				else
				{
					text2 = "<Dynamic Assembly>";
				}
				string arg = text2;
				reportFile.WriteLine(text);
				reportFile.WriteLine("    Location={0}", arg);
				Module[] array3 = this.EvaluateOrDefault<Module[]>(() => assembly.GetLoadedModules(), new Module[0]);
				if (array3.Length == 0)
				{
					reportFile.WriteLine("<Error getting list of loaded modules for assembly \"{0}\">", text);
				}
				Module[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					Module module = array4[j];
					string text3;
					if (!WatsonReport.IsAssemblyDynamic(assembly))
					{
						text3 = this.EvaluateOrDefault<string>(() => module.FullyQualifiedName, "<Unknown Module>");
					}
					else
					{
						text3 = "<Dynamic Module>";
					}
					string arg2 = text3;
					reportFile.WriteLine("    Module={0}", arg2);
				}
				AssemblyFileVersionAttribute assemblyFileVersionAttribute = null;
				if (this.TryGetCustomAttribute<AssemblyFileVersionAttribute>(assembly, out assemblyFileVersionAttribute))
				{
					reportFile.WriteLine("    Version={0}", assemblyFileVersionAttribute.Version);
				}
				DebuggableAttribute debuggableAttribute = null;
				if (this.TryGetCustomAttribute<DebuggableAttribute>(assembly, out debuggableAttribute))
				{
					reportFile.WriteLine("    BuildType={0}", debuggableAttribute.IsJITTrackingEnabled ? "debug" : "retail");
				}
				AssemblyProductAttribute assemblyProductAttribute = null;
				if (this.TryGetCustomAttribute<AssemblyProductAttribute>(assembly, out assemblyProductAttribute))
				{
					reportFile.WriteLine("    Product={0}", assemblyProductAttribute.Product);
				}
				reportFile.WriteLine("    Buddy={0}", WatsonReport.GetAssemblyBuddyInfo(assembly, publicKeyToken));
				reportFile.WriteLine();
			}
		}

		private void WriteReportFileAssemblies(XmlWriter reportFile)
		{
			using (new SafeXmlTag(reportFile, "loaded-assemblies"))
			{
				Assembly[] array = this.EvaluateOrDefault<Assembly[]>(() => AppDomain.CurrentDomain.GetAssemblies(), new Assembly[0]);
				if (array.Length != 0)
				{
					byte[] publicKeyToken = WatsonReport.GetPublicKeyToken();
					Assembly[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						Assembly assembly = array2[i];
						string value = this.EvaluateOrDefault<string>(delegate
						{
							DebuggableAttribute debuggableAttribute = null;
							if (!this.TryGetCustomAttribute<DebuggableAttribute>(assembly, out debuggableAttribute))
							{
								return "unknown";
							}
							if (!debuggableAttribute.IsJITTrackingEnabled)
							{
								return "retail";
							}
							return "debug";
						}, "unknown");
						string value2 = this.EvaluateOrDefault<string>(delegate
						{
							AssemblyProductAttribute assemblyProductAttribute = null;
							if (this.TryGetCustomAttribute<AssemblyProductAttribute>(assembly, out assemblyProductAttribute))
							{
								return assemblyProductAttribute.Product;
							}
							return "unknown";
						}, "unknown");
						string value3 = this.EvaluateOrDefault<string>(delegate
						{
							AssemblyFileVersionAttribute assemblyFileVersionAttribute = null;
							if (this.TryGetCustomAttribute<AssemblyFileVersionAttribute>(assembly, out assemblyFileVersionAttribute))
							{
								return assemblyFileVersionAttribute.Version;
							}
							return "unknown";
						}, "unknown");
						using (new SafeXmlTag(reportFile, "assembly").WithAttribute("type", value).WithAttribute("product", value2).WithAttribute("version", value3).WithAttribute("buddy", WatsonReport.GetAssemblyBuddyInfo(assembly, publicKeyToken)))
						{
							using (SafeXmlTag safeXmlTag3 = new SafeXmlTag(reportFile, "full-name"))
							{
								string content = this.EvaluateOrDefault<string>(() => assembly.FullName, "<Unknown Assembly>");
								safeXmlTag3.SetContent(content);
							}
							using (SafeXmlTag safeXmlTag4 = new SafeXmlTag(reportFile, "location"))
							{
								string text;
								if (!WatsonReport.IsAssemblyDynamic(assembly))
								{
									text = this.EvaluateOrDefault<string>(() => assembly.Location, "<Unknown Location>");
								}
								else
								{
									text = "<Dynamic Assembly>";
								}
								string content2 = text;
								safeXmlTag4.SetContent(content2);
							}
							Module[] array3 = this.EvaluateOrDefault<Module[]>(() => assembly.GetLoadedModules(), new Module[0]);
							if (array3.Length != 0)
							{
								using (new SafeXmlTag(reportFile, "modules"))
								{
									Module[] array4 = array3;
									for (int j = 0; j < array4.Length; j++)
									{
										Module module = array4[j];
										string text2;
										if (!WatsonReport.IsAssemblyDynamic(assembly))
										{
											text2 = this.EvaluateOrDefault<string>(() => module.FullyQualifiedName, "<Unknown Module>");
										}
										else
										{
											text2 = "<Dynamic Module>";
										}
										string content3 = text2;
										using (SafeXmlTag safeXmlTag6 = new SafeXmlTag(reportFile, "module"))
										{
											safeXmlTag6.SetContent(content3);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void WriteReportFileExtraData(TextWriter reportFile)
		{
			reportFile.WriteLine(reportFile.NewLine);
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("--------- Extra Data for Watson Report -------------");
			reportFile.WriteLine("----------------------------------------------------");
			reportFile.WriteLine("Error Reporting Enabled={0}", this.ReportingAllowed);
			reportFile.WriteLine("Process is terminating={0}", this.IsFlagSet(ReportOptions.ReportTerminateAfterSend));
			if (ExWatson.MsInternal)
			{
				reportFile.WriteLine();
				reportFile.WriteLine("*** MS Internal ***");
				if (!string.IsNullOrEmpty(ExWatson.ComputerName))
				{
					reportFile.WriteLine("Computer Name={0}", ExWatson.ComputerName);
				}
				if (!string.IsNullOrEmpty(ExWatson.ExchangeInstallSource))
				{
					reportFile.WriteLine("Exchange Install Source={0}", ExWatson.ExchangeInstallSource);
				}
			}
			if (this.extraData.Length > 0)
			{
				reportFile.WriteLine();
				reportFile.WriteLine("*** Extra Data ***");
				reportFile.WriteLine(this.extraData.ToString());
			}
			if (this.extraFiles.Length > 0)
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("--------- Extra Files for Watson Report ------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(this.extraFiles.ToString());
			}
		}

		private void WriteWatsonDiagnosticData(TextWriter reportFile)
		{
			if (reportFile != null && this.watsonProcessingExceptions != null)
			{
				reportFile.WriteLine(reportFile.NewLine);
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine("--------- ExWatson-Specific Diagnostics ------------");
				reportFile.WriteLine("----------------------------------------------------");
				reportFile.WriteLine(Environment.NewLine + "WARNING: Exceptions thrown while preparing report.  Dumps may be missing and/or some report data may be tagged as Unknown." + Environment.NewLine);
				int count = this.watsonProcessingExceptions.Count;
				for (int i = 0; i < count; i++)
				{
					reportFile.WriteLine("{0}", this.watsonProcessingExceptions[i]);
					reportFile.WriteLine();
				}
			}
		}

		private void WriteWatsonDiagnosticData(XmlWriter reportFile)
		{
			if (reportFile != null && this.watsonProcessingExceptions != null)
			{
				using (new SafeXmlTag(reportFile, "exwatson-internal-errors"))
				{
					int count = this.watsonProcessingExceptions.Count;
					for (int i = 0; i < count; i++)
					{
						using (SafeXmlTag safeXmlTag2 = new SafeXmlTag(reportFile, "error"))
						{
							safeXmlTag2.SetContent(this.watsonProcessingExceptions[i].ToString());
						}
					}
				}
			}
		}

		private void WriteReport<T>(T reportFile, Action<T> writeReport)
		{
			try
			{
				writeReport(reportFile);
			}
			catch (Exception e)
			{
				this.RecordExceptionWhileCreatingReport(e);
			}
		}

		private string GetFlavor()
		{
			string str = this.EvaluateOrDefault<string>(() => WatsonReport.GetBuildType(), "unknown");
			bool isDebug = this.debugBuild;
			if (this.process != null)
			{
				try
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(this.process.MainModule.FileName);
					isDebug = versionInfo.IsDebug;
				}
				catch
				{
				}
			}
			return str + (isDebug ? "DBG-" : "RTL-") + ExWatson.ProcessorArchitecture;
		}

		protected const string Unknown = "unknown";

		protected const string Reserved = "reserved";

		protected const string ArchiveReportNameSuffix = "ExWatsonReport.xml";

		protected const char ArchiveReportNameDelimeter = '-';

		private const string StylesheetUriTemplate = "http://exiis/watson/watsonreport.{0}.xsl";

		private const string StylesheetVersion = "1.00";

		private const int MaxParamSize = 255;

		private static readonly char[] GenericNameDelimiters = new char[]
		{
			'[',
			','
		};

		private static readonly FileSystemAccessRule[] TempDirPermissions = new FileSystemAccessRule[]
		{
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow)
		};

		private static readonly object heapDumpReportLockObject = new object();

		private static string[] bucketingParamNames;

		private readonly bool targetExternalProcess;

		private bool debugBuild;

		private bool reportingAllowed;

		private string eventType;

		private StringBuilder extraData;

		private StringBuilder extraFiles;

		private string[] bucketingParams;

		private Process process;

		private int processId;

		private ProcSafeHandle processHandle;

		private WerSafeHandle reportHandle;

		private DiagnosticsNativeMethods.WER_REPORT_INFORMATION reportInfo;

		private List<Exception> watsonProcessingExceptions;

		private ReportOptions reportOptions;

		private string processName = string.Empty;

		private string appName = string.Empty;

		private Version appVersion;

		internal delegate void WerReportDelegate(WerSafeHandle reportHandle);

		internal enum BucketParamId
		{
			Flavor,
			AppVersion,
			AppName,
			AssemblyName,
			ExMethodName,
			ExceptionType,
			CallstackHash,
			AssemblyVer,
			_Count
		}
	}
}

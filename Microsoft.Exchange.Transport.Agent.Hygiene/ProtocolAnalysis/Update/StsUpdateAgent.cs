using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StsUpdate;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Configuration;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Update
{
	internal class StsUpdateAgent
	{
		public static StsUpdateAgent.VersionComparison CompareAndSetCurrentVersion(string major, string minor)
		{
			string a = major + minor;
			if (string.IsNullOrEmpty(StsUpdateAgent.version) || a != StsUpdateAgent.version)
			{
				StsUpdateAgent.version = a;
				return StsUpdateAgent.VersionComparison.NotEq;
			}
			return StsUpdateAgent.VersionComparison.Eq;
		}

		public static bool EndProcessing
		{
			get
			{
				return StsUpdateAgent.shuttingDown;
			}
		}

		public void Shutdown()
		{
			this.OnShutdownHandler();
		}

		public void Startup()
		{
			this.OnStartupHandler();
		}

		public StsUpdateAgent()
		{
			StsUpdateAgent.updateAgentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			StsUpdateAgent.updateAgentDatFullFileName = Path.Combine(StsUpdateAgent.updateAgentPath, "Microsoft.SmartScreen.DomainRep.dat");
			StsUpdateAgent.updateAgentSparamFullFileName = Path.Combine(StsUpdateAgent.updateAgentPath, "Microsoft.SmartScreen.Sparam.dat");
		}

		private void FileWatcherStart()
		{
			this.dataFileWatcher = new FileSystemWatcher(StsUpdateAgent.updateAgentPath, "Microsoft.SmartScreen.DomainRep.dat");
			this.dataFileWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite);
			this.dataFileWatcher.Changed += this.OnDataFileChanged;
			this.dataFileWatcher.Created += this.OnDataFileChanged;
			this.dataFileWatcher.Renamed += new RenamedEventHandler(this.OnDataFileChanged);
			this.dataFileWatcher.EnableRaisingEvents = true;
		}

		private void OnStartupHandler()
		{
			this.FileWatcherStart();
			ExTraceGlobals.AgentTracer.TraceDebug((long)this.GetHashCode(), "OnStartupHandler");
		}

		private void OnShutdownHandler()
		{
			StsUpdateAgent.shuttingDown = true;
			if (this.dataFileWatcher != null)
			{
				this.dataFileWatcher = null;
			}
			Database.Detach();
		}

		private void ProcessNewData()
		{
			FileStream fileStream = null;
			byte[] data = null;
			BinaryReader binaryReader = null;
			try
			{
				if (!File.Exists(StsUpdateAgent.updateAgentDatFullFileName))
				{
					ExTraceGlobals.AgentTracer.TraceError((long)this.GetHashCode(), "ProcessNewData: data file does not exist.");
					return;
				}
				fileStream = new FileStream(StsUpdateAgent.updateAgentDatFullFileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
				FileInfo fileInfo = new FileInfo(StsUpdateAgent.updateAgentDatFullFileName);
				binaryReader = new BinaryReader(fileStream);
				if (fileInfo.Length > 2147483647L)
				{
					ExTraceGlobals.AgentTracer.TraceError((long)this.GetHashCode(), "ProcessNewData: failed to read the file. File size is larger than 2 GB");
					StsUpdateAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_UpdateAgentFileNotLoaded, null, null);
					return;
				}
				data = binaryReader.ReadBytes((int)fileInfo.Length);
			}
			catch (IOException ex)
			{
				ExTraceGlobals.AgentTracer.TraceError<string>((long)this.GetHashCode(), "ProcessNewData: failed to read the file. {0}", ex.Message);
				StsUpdateAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_UpdateAgentFileNotLoaded, null, null);
				return;
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.AgentTracer.TraceError<string>((long)this.GetHashCode(), "ProcessNewData: failed to read the file. {0}", ex2.Message);
				StsUpdateAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_UpdateAgentFileNotLoaded, null, null);
				return;
			}
			finally
			{
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			VerifyContent verifyContent = new VerifyContent();
			if (verifyContent.Parse(data))
			{
				if (StsUpdateAgent.shuttingDown)
				{
					return;
				}
				ProtocolAnalysisSrlSettings protocolAnalysisSrlSettings = new ProtocolAnalysisSrlSettings();
				FileStream fileStream2 = null;
				try
				{
					if (!File.Exists(StsUpdateAgent.updateAgentSparamFullFileName))
					{
						ExTraceGlobals.AgentTracer.TraceError((long)this.GetHashCode(), "ProcessNewData: SPARAM file does not exist.");
						return;
					}
					fileStream2 = new FileStream(StsUpdateAgent.updateAgentSparamFullFileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
					protocolAnalysisSrlSettings.StoreReputationServiceParams(fileStream2, ExTraceGlobals.OnDownloadTracer);
					ConfigurationAccess.NotifySrlConfigChanged(protocolAnalysisSrlSettings.Fields);
					StsUpdateAgent.PerformanceCounters.TotalSrlUpdate();
				}
				catch (IOException ex3)
				{
					ExTraceGlobals.AgentTracer.TraceError<string>((long)this.GetHashCode(), "ProcessNewData: failed to read the SPARAM file. {0}", ex3.Message);
					return;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ExTraceGlobals.AgentTracer.TraceError<string>((long)this.GetHashCode(), "ProcessNewData: failed to read the SPARAM file. {0}", ex4.Message);
					return;
				}
				finally
				{
					if (fileStream2 != null)
					{
						fileStream2.Dispose();
					}
				}
				return;
			}
		}

		private void OnDataFileChanged(object source, FileSystemEventArgs ev)
		{
			lock (this.syncObject)
			{
				if (!StsUpdateAgent.shuttingDown)
				{
					this.ProcessNewData();
				}
			}
		}

		private const string UpdateFileName = "Microsoft.SmartScreen.DomainRep.dat";

		private const string UpdateSparamFileName = "Microsoft.SmartScreen.Sparam.dat";

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.FactoryTracer.Category, "MSExchange Antispam");

		private static string updateAgentPath;

		private static string updateAgentDatFullFileName;

		private static string updateAgentSparamFullFileName;

		private static bool shuttingDown;

		private static string version;

		private object syncObject = new object();

		private FileSystemWatcher dataFileWatcher;

		internal sealed class PerformanceCounters
		{
			public static void TotalUpdate()
			{
				StsUpdatePerfCounters.TotalUpdate.Increment();
			}

			public static void TotalSrlUpdate()
			{
				StsUpdatePerfCounters.TotalSrlUpdate.Increment();
			}

			public static void RemoveCounters()
			{
				StsUpdatePerfCounters.TotalSrlUpdate.RawValue = 0L;
				StsUpdatePerfCounters.TotalUpdate.RawValue = 0L;
			}
		}

		internal enum VersionComparison
		{
			Eq,
			NotEq
		}
	}
}

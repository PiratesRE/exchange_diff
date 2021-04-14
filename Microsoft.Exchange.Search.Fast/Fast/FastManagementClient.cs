using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Ceres.Common.Utils.Net;
using Microsoft.Ceres.Common.WcfUtils;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Win32;

namespace Microsoft.Exchange.Search.Fast
{
	internal abstract class FastManagementClient : IDisposeTrackable, IDisposable
	{
		static FastManagementClient()
		{
			if (string.IsNullOrEmpty(FastManagementClient.fsisInstallPath) && !ExEnvironment.IsTestProcess)
			{
				throw new InvalidOperationException("Failure to detect Fast installation.");
			}
			Environment.SetEnvironmentVariable("CERES_REGISTRY_PRODUCT_NAME", "Search Foundation for Exchange", EnvironmentVariableTarget.Process);
			AppDomain.CurrentDomain.AssemblyResolve += FastManagementClient.OnAssemblyResolveEvent;
		}

		protected FastManagementClient()
		{
			this.diagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("FastManagementClient", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexManagementTracer, (long)this.GetHashCode());
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected static SearchConfig Config
		{
			get
			{
				if (FastManagementClient.config == null)
				{
					lock (FastManagementClient.lockObject)
					{
						if (FastManagementClient.config == null)
						{
							FlightingSearchConfig flightingSearchConfig = new FlightingSearchConfig();
							Thread.MemoryBarrier();
							FastManagementClient.config = flightingSearchConfig;
						}
					}
				}
				return FastManagementClient.config;
			}
		}

		protected static int FsisInstallBasePort
		{
			get
			{
				return FastManagementClient.fsisInstallBasePort;
			}
		}

		protected IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		protected abstract int ManagementPortOffset { get; }

		public abstract DisposeTracker GetDisposeTracker();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected abstract void InternalConnectManagementAgents(WcfManagementClient client);

		protected virtual void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.client != null)
				{
					this.client.Dispose();
					this.client = null;
				}
			}
		}

		protected void PerformFastOperation(Action function, string eventLogKey)
		{
			this.PerformFastOperation<object>(delegate()
			{
				function();
				return null;
			}, eventLogKey);
		}

		protected virtual T PerformFastOperation<T>(Func<T> function, string eventLogKey)
		{
			int num = 0;
			Exception ex;
			for (;;)
			{
				ex = null;
				try
				{
					if (this.client == null)
					{
						this.ConnectManagementAgents();
					}
					return function();
				}
				catch (Exception ex2)
				{
					if (Util.ShouldRethrowException(ex2))
					{
						throw;
					}
					ex = ex2;
				}
				num++;
				if (num >= 3)
				{
					break;
				}
				Thread.Sleep(FastManagementClient.OperationRetryWait);
				this.LogExceptionAndReconnectManagementAgents(ex, eventLogKey);
			}
			throw new PerformingFastOperationException(ex);
		}

		protected void ConnectManagementAgents()
		{
			this.ConnectManagementAgents("localhost");
		}

		protected void ConnectManagementAgents(string serverName)
		{
			int num = 0;
			for (;;)
			{
				try
				{
					this.CreateWcfClient(serverName);
					this.InternalConnectManagementAgents(this.client);
					this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Successfully connected the Management Agents.", new object[0]);
					break;
				}
				catch (Exception ex)
				{
					if (Util.ShouldRethrowException(ex))
					{
						throw;
					}
					num++;
					if (num >= 3)
					{
						throw new PerformingFastOperationException(ex);
					}
					this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Retry connecting management agents due to error:  {0}", new object[]
					{
						ex
					});
				}
				Thread.Sleep(FastManagementClient.ConnectionRetryWait);
			}
		}

		protected SystemClient ConnectSystem()
		{
			Uri uri = new Uri(string.Format("net.tcp://{0}:{1}/Management", "localhost", FastManagementClient.FsisInstallBasePort + this.ManagementPortOffset));
			Binding binding = ClientFactory.CreateBinding(uri, true, true);
			SystemClient systemClient = new SystemClient(binding, EndpointIdentity.CreateUpnIdentity("*"), null)
			{
				SystemManagerLocations = new List<Uri>
				{
					uri
				}
			};
			try
			{
				if (!systemClient.Connect())
				{
					throw systemClient.ConnectionException;
				}
			}
			catch
			{
				systemClient.Dispose();
				throw;
			}
			return systemClient;
		}

		protected TManagementAgent GetManagementAgent<TManagementAgent>(string agentName)
		{
			return this.client.GetManagementAgent<TManagementAgent>(agentName);
		}

		private static string GetFastInstallPath()
		{
			string value = RegistryReader.Instance.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Search Foundation for Exchange", "InstallationPath", string.Empty);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			return new DirectoryInfo(value).FullName;
		}

		private static Assembly OnAssemblyResolveEvent(object sender, ResolveEventArgs args)
		{
			string text = args.Name.Split(new char[]
			{
				','
			})[0];
			string text2 = Path.Combine(FastManagementClient.fsisInstallPath, "Installer\\Bin");
			string text3 = Path.Combine(FastManagementClient.fsisInstallPath, "HostController");
			foreach (string text4 in new string[]
			{
				text2,
				text3
			})
			{
				string text5 = string.Concat(new object[]
				{
					text4,
					Path.DirectorySeparatorChar,
					text,
					".dll"
				});
				if (File.Exists(text5))
				{
					return Assembly.LoadFrom(text5);
				}
			}
			return null;
		}

		private void CreateWcfClient(string serverName)
		{
			if (this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
			string uriString = string.Format("net.tcp://{0}:{1}/Management", serverName, FastManagementClient.fsisInstallBasePort + this.ManagementPortOffset);
			ClientConnectionSettings clientConnectionSettings = ClientUtils.CreateConnectionSettings(new Uri(uriString), true, "*", 1);
			this.client = new WcfManagementClient(clientConnectionSettings);
		}

		private void LogExceptionAndReconnectManagementAgents(Exception ex, string eventLogKey)
		{
			this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Retry due to error: {0}", new object[]
			{
				ex
			});
			PerformingFastOperationException ex2 = new PerformingFastOperationException(ex);
			this.diagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_FASTConnectionIssue, eventLogKey, new object[]
			{
				ex2
			});
			this.ConnectManagementAgents();
		}

		private const int MaxRetries = 3;

		private const string ProductNameEnvironmentVariable = "CERES_REGISTRY_PRODUCT_NAME";

		private const string ProductName = "Search Foundation for Exchange";

		private const string FsisProductKeyPath = "SOFTWARE\\Microsoft\\Search Foundation for Exchange";

		private const string FsisProductInstallPathParameterName = "InstallationPath";

		private const string FsisProductInstallBasePortParameterName = "BasePort";

		private static readonly TimeSpan OperationRetryWait = TimeSpan.FromSeconds(1.0);

		private static readonly object lockObject = new object();

		private static readonly TimeSpan ConnectionRetryWait = TimeSpan.FromSeconds(10.0);

		private static readonly string fsisInstallPath = FastManagementClient.GetFastInstallPath();

		private static readonly int fsisInstallBasePort = FastManagementClient.Config.BasePort;

		private static SearchConfig config;

		private readonly IDiagnosticsSession diagnosticsSession;

		private WcfManagementClient client;

		private DisposeTracker disposeTracker;
	}
}

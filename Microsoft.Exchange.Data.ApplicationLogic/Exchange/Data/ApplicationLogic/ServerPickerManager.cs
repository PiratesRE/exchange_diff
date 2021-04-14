using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class ServerPickerManager : ADConfigurationLoader<Server, PickerServerList>
	{
		public ServerPickerManager(string serviceName, ServerRole serverRole, Trace tracer) : this(serviceName, serverRole, tracer, ServerPickerClient.Default)
		{
		}

		public ServerPickerManager(string serviceName, ServerRole serverRole, Trace tracer, ServerPickerClient serverPickerClient)
		{
			this.serverRole = serverRole;
			this.serviceName = serviceName;
			this.tracer = tracer;
			this.serverPickerClient = serverPickerClient;
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 98, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\ServerPicker\\ServerPickerManager.cs");
			base.ReadConfiguration();
		}

		public event EventHandler OnServersUpdated;

		public ServerRole ServerRole
		{
			get
			{
				base.CheckDisposed();
				return this.serverRole;
			}
		}

		public ServerComponentEnum Component
		{
			get
			{
				base.CheckDisposed();
				if (this.serverRole == ServerRole.HubTransport)
				{
					return ServerComponentEnum.HubTransport;
				}
				return ServerComponentEnum.None;
			}
		}

		public Trace Tracer
		{
			get
			{
				base.CheckDisposed();
				return this.tracer;
			}
		}

		public ServerPickerClient ServerPickerClient
		{
			get
			{
				base.CheckDisposed();
				return this.serverPickerClient;
			}
		}

		public ITopologyConfigurationSession ConfigurationSession
		{
			get
			{
				base.CheckDisposed();
				return this.configurationSession;
			}
		}

		public bool HasValidConfiguration
		{
			get
			{
				bool result;
				lock (this.serversLock)
				{
					result = (this.servers != null && this.servers.IsValid);
				}
				return result;
			}
		}

		public PickerServerList GetPickerServerList()
		{
			base.CheckDisposed();
			PickerServerList result;
			lock (this.serversLock)
			{
				this.servers.AddRef();
				result = this.servers;
			}
			return result;
		}

		internal void UpdateServersInRetryPerfmon(PickerServerList pickerServerList)
		{
			base.CheckDisposed();
			lock (this.serversLock)
			{
				if (this.servers == pickerServerList)
				{
					this.serverPickerClient.UpdateServersInRetryPerfmon(pickerServerList.RetryServerCount);
					if (pickerServerList.RetryServerCount == pickerServerList.Count)
					{
						this.serverPickerClient.LogNoActiveServerEvent(this.serverRole.ToString());
						this.serverPickerClient.UpdateServersPercentageActivePerfmon(0);
					}
					else
					{
						int percentage = (pickerServerList.Count == 0) ? 0 : ((pickerServerList.Count - pickerServerList.RetryServerCount) * 100 / pickerServerList.Count);
						this.serverPickerClient.UpdateServersPercentageActivePerfmon(percentage);
					}
				}
			}
		}

		internal void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			base.CheckDisposed();
			object[] array = new object[messageArgs.Length + 1];
			array[0] = this.serviceName;
			Array.Copy(messageArgs, 0, array, 1, messageArgs.Length);
			ServerPickerManager.EventLog.LogEvent(tuple, periodicKey, array);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.InternalDispose(disposing);
				lock (this.serversLock)
				{
					if (this.servers != null)
					{
						this.servers.Release();
						this.servers = null;
					}
				}
			}
		}

		protected override void LogFailure(ADConfigurationLoader<Server, PickerServerList>.FailureLocation failureLocation, Exception exception)
		{
			this.tracer.TraceError<ADConfigurationLoader<Server, PickerServerList>.FailureLocation, Exception>(0L, "Failed to perform {0} due to exception {1}", failureLocation, exception);
			if (exception == null)
			{
				this.LogEvent(ApplicationLogicEventLogConstants.Tuple_TopologyException, "Null Exception", new object[]
				{
					"null"
				});
				return;
			}
			if (exception is LocalServerNotFoundException)
			{
				this.tracer.TraceError(0L, "No local server");
				this.LogEvent(this.HasValidConfiguration ? ApplicationLogicEventLogConstants.Tuple_NoLocalServerWarning : ApplicationLogicEventLogConstants.Tuple_NoLocalServer, exception.GetType().FullName, new object[]
				{
					exception
				});
				return;
			}
			this.LogEvent(ApplicationLogicEventLogConstants.Tuple_TopologyException, exception.GetType().FullName, new object[]
			{
				exception
			});
		}

		protected override void PreAdOperation(ref PickerServerList newServers)
		{
			newServers = new PickerServerList(this);
		}

		protected override void AdOperation(ref PickerServerList newServers)
		{
			newServers.LoadFromAD(this.servers);
		}

		protected override void PostAdOperation(PickerServerList newServers, bool wasSuccessful)
		{
			this.lastLoadTime = DateTime.UtcNow;
			if (newServers.IsValid)
			{
				this.SetServers(newServers);
				this.wasLastLoadSuccessful = true;
				return;
			}
			lock (this.serversLock)
			{
				if (this.servers == null)
				{
					this.SetServers(newServers);
				}
				else
				{
					newServers.Release();
				}
			}
			this.wasLastLoadSuccessful = false;
		}

		protected override void OnServerChangeCallback(ADNotificationEventArgs args)
		{
			if (args != null)
			{
				this.tracer.TraceDebug<string, string>(0L, "OnServerChangeCallback notification change type {0}, object ID {1}", args.ChangeType.ToString(), (args.Id == null) ? "(null)" : args.Id.ToString());
			}
			if (args.ChangeType == ADNotificationChangeType.ModifyOrAdd && args.Id != null)
			{
				Server server;
				ADOperationResult adoperationResult;
				if (ADNotificationAdapter.TryReadConfiguration<Server>(() => this.configurationSession.Read<Server>(args.Id), out server, out adoperationResult))
				{
					lock (this.serversLock)
					{
						if (this.servers != null && this.servers.IsChangeIgnorable(server))
						{
							return;
						}
						goto IL_107;
					}
				}
				this.tracer.TraceError<ADObjectId, Exception>(0L, "Failed to read server object with Id {0} due to {1}", args.Id, adoperationResult.Exception);
			}
			IL_107:
			base.ReadConfiguration();
		}

		private void SetServers(PickerServerList newServers)
		{
			lock (this.serversLock)
			{
				if (this.servers != newServers)
				{
					if (this.servers != null)
					{
						this.servers.Release();
					}
					this.servers = newServers;
				}
				this.serverPickerClient.UpdateServersTotalPerfmon(this.servers.Count);
				this.UpdateServersInRetryPerfmon(this.servers);
				if (this.servers.Count == 0)
				{
					this.LogEvent(ApplicationLogicEventLogConstants.Tuple_NoServerInSite, this.serverRole.ToString(), new object[]
					{
						this.serverRole.ToString()
					});
				}
			}
			EventHandler eventHandler = this.OnServersUpdated;
			if (eventHandler != null)
			{
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					eventHandler(this, null);
				});
			}
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			PickerServerList pickerServerList = this.GetPickerServerList();
			XElement result;
			try
			{
				result = new XElement("ServerPickerManager", new object[]
				{
					new XElement("serverRole", this.serverRole),
					new XElement("hasValidConfiguration", this.HasValidConfiguration),
					new XElement("lastLoadTime", this.lastLoadTime),
					new XElement("wasLastLoadSuccessful", this.wasLastLoadSuccessful),
					pickerServerList.GetDiagnosticInfo(argument)
				});
			}
			finally
			{
				pickerServerList.Release();
			}
			return result;
		}

		private readonly ServerRole serverRole;

		private readonly ITopologyConfigurationSession configurationSession;

		private readonly string serviceName;

		private readonly Trace tracer;

		private readonly ServerPickerClient serverPickerClient;

		private static Guid eventLogComponentGuid = new Guid("{FF503692-3FF7-48c9-9BF9-8586487B4E36}");

		public static readonly ExEventLog EventLog = new ExEventLog(ServerPickerManager.eventLogComponentGuid, "MSExchangeApplicationLogic");

		private PickerServerList servers;

		private object serversLock = new object();

		private DateTime lastLoadTime;

		private bool wasLastLoadSuccessful;
	}
}

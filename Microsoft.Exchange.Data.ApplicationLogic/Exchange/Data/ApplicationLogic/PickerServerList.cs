using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class PickerServerList : IDisposable
	{
		internal PickerServerList(ServerPickerManager context)
		{
			this.context = context;
		}

		public PickerServer LocalServer
		{
			get
			{
				return this.localServer;
			}
		}

		public string LocalServerLegacyDN
		{
			get
			{
				if (this.localServer == null)
				{
					return string.Empty;
				}
				return this.localServer.LegacyDN;
			}
		}

		public string LocalServerFQDN
		{
			get
			{
				if (this.localServer == null)
				{
					return string.Empty;
				}
				return this.localServer.FQDN;
			}
		}

		public string LocalSystemAttendantDN
		{
			get
			{
				if (this.localServer == null)
				{
					return string.Empty;
				}
				return this.localServer.SystemAttendantDN;
			}
		}

		public string LocalServerName
		{
			get
			{
				if (this.localServer == null)
				{
					return string.Empty;
				}
				return this.localServer.MachineName;
			}
		}

		public bool SubjectLoggingEnabled
		{
			get
			{
				return this.localServer != null && this.localServer.MessageTrackingLogSubjectLoggingEnabled;
			}
		}

		public IPAddress LocalIP
		{
			get
			{
				if (this.localIP == null)
				{
					return IPAddress.None;
				}
				return this.localIP;
			}
		}

		public byte[] LocalIPBytes
		{
			get
			{
				if (this.localIPBytes == null)
				{
					return IPAddress.None.GetAddressBytes();
				}
				return this.localIPBytes;
			}
		}

		public Guid LocalServerGuid
		{
			get
			{
				if (this.localServer == null)
				{
					return Guid.Empty;
				}
				return this.localServer.ServerGuid;
			}
		}

		internal int RetryServerCount
		{
			get
			{
				return this.retryServerCount;
			}
		}

		internal int Count
		{
			get
			{
				return this.serverList.Count;
			}
		}

		internal Trace Tracer
		{
			get
			{
				return this.context.Tracer;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		internal PickerServer this[int index]
		{
			get
			{
				PickerServer result;
				lock (this.stateLock)
				{
					if (index >= 0 && index < this.serverList.Count)
					{
						result = this.serverList[index];
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
		}

		public PickerServer PickNextUsingRoundRobinPreferringLocal()
		{
			this.context.Tracer.TraceDebug(0L, "Picking next server using round robin preferring local");
			lock (this.stateLock)
			{
				if (this.localServer != null && (this.localServer.ServerRole & this.context.ServerRole) != ServerRole.None && this.IsEligibleForUse(this.localServer))
				{
					return this.localServer;
				}
			}
			return this.PickNextUsingRoundRobin();
		}

		public PickerServer PickNextUsingRoundRobinPreferringNonLocal()
		{
			return this.PickNextUsingRoundRobin(true);
		}

		public PickerServer PickNextUsingRoundRobin()
		{
			return this.PickNextUsingRoundRobin(false);
		}

		public void PromoteServer(PickerServer server)
		{
			lock (this.stateLock)
			{
				int num = this.serverList.IndexOf(server);
				if (num != -1)
				{
					this.currentServerIndex = num;
				}
			}
		}

		private PickerServer PickNextUsingRoundRobin(bool preferNonLocal)
		{
			this.context.Tracer.TraceDebug<string>(0L, "Picking next server using round robin {0}", preferNonLocal ? "preferring non local" : string.Empty);
			if (this.serverList.Count == 0)
			{
				this.context.Tracer.TraceDebug<string>(0L, "No {0} servers found", this.context.ServerRole.ToString());
				return null;
			}
			lock (this.stateLock)
			{
				bool flag2 = false;
				for (int i = 0; i < this.serverList.Count; i++)
				{
					if (this.currentServerIndex >= this.serverList.Count)
					{
						this.currentServerIndex = 0;
					}
					PickerServer pickerServer = this.serverList[this.currentServerIndex++];
					if (preferNonLocal && pickerServer == this.localServer)
					{
						this.context.Tracer.TraceDebug(0L, "Skipping local server in hopes of finding another.");
						flag2 = true;
					}
					else if (pickerServer.IsEligibleForUse())
					{
						this.context.Tracer.TraceDebug<string, string>(0L, "Found {0} as the next {1} server", pickerServer.LegacyDN, this.context.ServerRole.ToString());
						return pickerServer;
					}
				}
				if (flag2 && this.localServer.IsEligibleForUse())
				{
					this.context.Tracer.TraceDebug(0L, "Returning local server.");
					return this.localServer;
				}
			}
			this.context.Tracer.TraceDebug<string>(0L, "No active {0} server available based on round robin", this.context.ServerRole.ToString());
			return null;
		}

		public PickerServer PickServerByFqdn(string fqdn)
		{
			PickerServer result;
			if (this.fqdnServers.TryGetValue(fqdn, out result))
			{
				return result;
			}
			return null;
		}

		public void UpdateServerHealth(PickerServer server, bool? isHealthy)
		{
			lock (this.stateLock)
			{
				int num = this.serverList.IndexOf(server);
				if (num != -1)
				{
					server = this.serverList[num];
					server.InternalUpdateServerHealth(isHealthy);
				}
			}
		}

		public void ForceAllToActive()
		{
			this.TryForceAllToActive();
		}

		public bool TryForceAllToActive()
		{
			bool result;
			lock (this.stateLock)
			{
				if (DateTime.UtcNow - this.lastForceRetryToActiveTime > PickerServerList.ForceRetryToActiveInterval)
				{
					this.context.Tracer.TraceDebug<string>(0L, "Force all {0} servers to become active", this.context.ServerRole.ToString());
					foreach (PickerServer pickerServer in this.serverList)
					{
						pickerServer.InternalUpdateServerHealth(null);
					}
					this.lastForceRetryToActiveTime = DateTime.UtcNow;
					result = true;
				}
				else
				{
					this.context.Tracer.TraceDebug<DateTime>(0L, "Last force retry to active happened at {0}", this.lastForceRetryToActiveTime);
					result = false;
				}
			}
			return result;
		}

		public void AddRef()
		{
			if (this.refCount <= 0)
			{
				throw new InvalidOperationException("Invalid reference management detected.");
			}
			Interlocked.Increment(ref this.refCount);
		}

		public void Release()
		{
			if (this.refCount <= 0)
			{
				throw new InvalidOperationException("Invalid reference management detected.");
			}
			if (Interlocked.Decrement(ref this.refCount) == 0)
			{
				this.Dispose();
			}
		}

		public void Dispose()
		{
			if (this.localServer != null)
			{
				this.localServer.Dispose();
				this.localServer = null;
			}
			foreach (PickerServer pickerServer in this.serverList)
			{
				pickerServer.Dispose();
			}
			this.serverList.Clear();
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement("Servers");
			new XElement("PickerServerList", new object[]
			{
				new XElement("refCount", this.refCount),
				new XElement("serverListCount", this.serverList.Count),
				new XElement("lastForceRetryToActiveTime", this.lastForceRetryToActiveTime),
				xelement
			});
			lock (this.stateLock)
			{
				foreach (PickerServer pickerServer in this.serverList)
				{
					xelement.Add(pickerServer.GetDiagnosticInfo(argument));
				}
			}
			return xelement;
		}

		internal bool IsEligibleForUse(PickerServer server)
		{
			int num = this.serverList.IndexOf(server);
			return num != -1 && this.serverList[num].IsEligibleForUse();
		}

		internal bool IsChangeIgnorable(Server server)
		{
			PickerServer pickerServer = this.FindMatchingServer(server);
			if (pickerServer == null)
			{
				if ((server.CurrentServerRole & this.context.ServerRole) == ServerRole.None)
				{
					return true;
				}
			}
			else if ((server.CurrentServerRole & this.context.ServerRole) != ServerRole.None && ADObjectId.Equals(server.ServerSite, this.localSite) && server.VersionNumber >= Server.CurrentProductMinimumVersion && pickerServer.ArePropertiesEqual(server, this.context.ServerRole))
			{
				return true;
			}
			return false;
		}

		internal void DecrementServersInRetryCount()
		{
			Interlocked.Decrement(ref this.retryServerCount);
			this.context.UpdateServersInRetryPerfmon(this);
		}

		internal void IncrementServersInRetryCount()
		{
			Interlocked.Increment(ref this.retryServerCount);
			this.context.UpdateServersInRetryPerfmon(this);
		}

		internal void LoadFromAD(PickerServerList oldServers)
		{
			this.context.Tracer.TracePfd<int, string>(0L, "PFD EMS {0} Loading {1} servers From AD...", 30619, this.context.ServerRole.ToString());
			Server server = this.context.ConfigurationSession.FindLocalServer();
			if (server != null)
			{
				this.CheckForOverride(server);
				if (server.ServerSite == null)
				{
					this.context.Tracer.TraceError(0L, "Local server doesn't belong to a site");
					ExEventLog.EventTuple tuple = this.context.HasValidConfiguration ? ApplicationLogicEventLogConstants.Tuple_LocalServerNotInSiteWarning : ApplicationLogicEventLogConstants.Tuple_LocalServerNotInSite;
					this.context.LogEvent(tuple, null, new object[0]);
					return;
				}
				this.context.ServerPickerClient.LocalServerDiscovered(server);
				this.localSite = server.ServerSite;
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, (ulong)((long)this.context.ServerRole)),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, server.ServerSite),
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.CurrentProductMinimumVersion),
					new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.NextProductMinimumVersion)
				});
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				ADPagedReader<Server> adpagedReader = this.context.ConfigurationSession.FindPaged<Server>(null, QueryScope.SubTree, filter, null, 0);
				foreach (Server server2 in adpagedReader)
				{
					ServiceState serviceState = ServerComponentStates.ReadEffectiveComponentState(server2.Fqdn, server2.ComponentStates, ServerComponentStateSources.AD, ServerComponentStates.GetComponentId(this.context.Component), ServiceState.Active);
					if (this.context.Component != ServerComponentEnum.None && serviceState != ServiceState.Active)
					{
						this.context.Tracer.TraceDebug<string, string, ServiceState>(0L, "Component {0} is not active on server {1}. Current component state is {2}.", this.context.Component.ToString(), server2.DistinguishedName, serviceState);
					}
					else
					{
						bool flag = true;
						if (this.overrideList != null && this.overrideList.Count > 0)
						{
							if (this.overrideList.Contains(server2.DistinguishedName))
							{
								this.context.Tracer.TraceDebug<string, string>(0L, "Adding {0} Server from override list: {1} to active list", this.context.ServerRole.ToString(), server2.DistinguishedName);
							}
							else
							{
								flag = false;
							}
						}
						else
						{
							this.context.Tracer.TraceDebug<string, string, int>(0L, "Adding {0} Server: {1} Version: {2} to active list", this.context.ServerRole.ToString(), server2.DistinguishedName, server2.VersionNumber);
						}
						if (flag)
						{
							if (this.fqdnServers.ContainsKey(server2.Fqdn) || hashSet.Contains(server2.Name) || hashSet2.Contains(server2.ExchangeLegacyDN))
							{
								this.context.Tracer.TraceError<string, string, string>(0L, "Found more than one server with same FQDN {0} or LegacyDN {1} or MachineName {2}.", server2.Fqdn, server2.ExchangeLegacyDN, server2.Name);
								this.context.LogEvent(ApplicationLogicEventLogConstants.Tuple_MisconfiguredServer, server2.Fqdn, new object[]
								{
									server2.ExchangeLegacyDN,
									server2.Name
								});
							}
							else
							{
								PickerServer pickerServer = this.context.ServerPickerClient.CreatePickerServer(server2, this);
								if (oldServers != null)
								{
									PickerServer pickerServer2 = oldServers.PickServerByFqdn(pickerServer.FQDN);
									if (pickerServer2 != null)
									{
										pickerServer2.CopyStatusTo(pickerServer);
									}
								}
								this.serverList.Add(pickerServer);
								this.fqdnServers.Add(pickerServer.FQDN, pickerServer);
								hashSet.Add(pickerServer.MachineName);
								hashSet2.Add(pickerServer.LegacyDN);
							}
						}
					}
				}
				this.context.ServerPickerClient.ServerListUpdated(this.serverList);
				if ((server.CurrentServerRole & this.context.ServerRole) == ServerRole.None)
				{
					this.localServer = this.context.ServerPickerClient.CreatePickerServer(server, this);
				}
				else
				{
					PickerServer pickerServer3 = this.FindMatchingServer(server);
					if (pickerServer3 != null)
					{
						this.localServer = pickerServer3;
					}
					else
					{
						this.context.Tracer.TraceDebug<string>(0L, "Local server ({0}) meets criteria, but wasn't found in AD using list query; won't be preferred", server.Fqdn);
						this.localServer = this.context.ServerPickerClient.CreatePickerServer(server, this);
					}
				}
				try
				{
					this.localIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
				}
				catch (SocketException ex)
				{
					this.context.Tracer.TraceError<string>(0L, "Can't get local IP address due to: {0}", ex.ToString());
					ExEventLog.EventTuple tuple2 = this.context.HasValidConfiguration ? ApplicationLogicEventLogConstants.Tuple_CantGetLocalIPWarning : ApplicationLogicEventLogConstants.Tuple_CantGetLocalIP;
					this.context.LogEvent(tuple2, ex.GetType().FullName, new object[]
					{
						ex
					});
					return;
				}
				this.localIPBytes = this.localIP.GetAddressBytes();
				this.context.Tracer.TracePfd<int, string, int>(0L, "PFD EMS {0} Finished finding {1} servers: {2} found.", 19355, this.context.ServerRole.ToString(), this.serverList.Count);
				this.isValid = true;
			}
		}

		private void CheckForOverride(Server server)
		{
			if (this.context.ServerPickerClient.OverrideListPropertyDefinition == null)
			{
				return;
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = server[this.context.ServerPickerClient.OverrideListPropertyDefinition] as MultiValuedProperty<ADObjectId>;
			if (multiValuedProperty != null)
			{
				this.overrideList = new HashSet<string>();
				foreach (ADObjectId adobjectId in multiValuedProperty)
				{
					this.overrideList.Add(adobjectId.DistinguishedName);
				}
			}
		}

		private PickerServer FindMatchingServer(Server server)
		{
			foreach (PickerServer pickerServer in this.serverList)
			{
				if (string.Equals(pickerServer.LegacyDN, server.ExchangeLegacyDN, StringComparison.OrdinalIgnoreCase))
				{
					return pickerServer;
				}
			}
			return null;
		}

		private static readonly TimeSpan ForceRetryToActiveInterval = TimeSpan.FromSeconds(30.0);

		private object stateLock = new object();

		private List<PickerServer> serverList = new List<PickerServer>();

		private int currentServerIndex;

		private DateTime lastForceRetryToActiveTime = DateTime.UtcNow;

		private int retryServerCount;

		private ServerPickerManager context;

		private ADObjectId localSite;

		private HashSet<string> overrideList;

		private PickerServer localServer;

		private IPAddress localIP;

		private byte[] localIPBytes;

		private bool isValid;

		private int refCount = 1;

		private Dictionary<string, PickerServer> fqdnServers = new Dictionary<string, PickerServer>(StringComparer.OrdinalIgnoreCase);
	}
}

using System;
using System.Net;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class PickerServer : IDisposable
	{
		internal PickerServer(Server server, PickerServerList pickerServerList)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (pickerServerList == null)
			{
				throw new ArgumentNullException("pickerServerList");
			}
			this.stateLock = new object();
			this.machineName = server.Name;
			this.serverGuid = server.Guid;
			this.exchangeLegacyDN = server.ExchangeLegacyDN;
			this.fqdn = server.Fqdn;
			this.versionNumber = server.VersionNumber;
			this.serverRole = server.CurrentServerRole;
			this.messageTrackingLogSubjectLoggingEnabled = server.MessageTrackingLogSubjectLoggingEnabled;
			this.cachedHashCode = (StringComparer.OrdinalIgnoreCase.GetHashCode(this.LegacyDN ?? string.Empty) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.FQDN ?? string.Empty) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.MachineName ?? string.Empty));
			this.pickerServerList = pickerServerList;
			this.active = true;
		}

		public string LegacyDN
		{
			get
			{
				return this.exchangeLegacyDN;
			}
		}

		public string FQDN
		{
			get
			{
				return this.fqdn;
			}
		}

		public int VersionNumber
		{
			get
			{
				return this.versionNumber;
			}
		}

		public string SystemAttendantDN
		{
			get
			{
				return this.LegacyDN + "/cn=Microsoft System Attendant";
			}
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public ServerRole ServerRole
		{
			get
			{
				return this.serverRole;
			}
		}

		public Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public bool MessageTrackingLogSubjectLoggingEnabled
		{
			get
			{
				return this.messageTrackingLogSubjectLoggingEnabled;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.active;
			}
		}

		protected virtual void ServerDeactivated()
		{
		}

		public virtual void Dispose()
		{
		}

		internal bool IsEligibleForUse()
		{
			bool result;
			lock (this.stateLock)
			{
				if (this.active)
				{
					result = true;
				}
				else if (DateTime.UtcNow >= this.nextRetryTime)
				{
					this.pickerServerList.Tracer.TraceDebug<string>(0L, "Retry timeout expires. Server {0} will be retried again.", this.LegacyDN);
					this.nextRetryTime = DateTime.UtcNow.Add(PickerServer.RetryInterval);
					this.retryAttempts++;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public override bool Equals(object serverObj)
		{
			PickerServer pickerServer = serverObj as PickerServer;
			return pickerServer != null && (string.Equals(this.LegacyDN, pickerServer.LegacyDN, StringComparison.OrdinalIgnoreCase) && string.Equals(this.FQDN, pickerServer.FQDN, StringComparison.OrdinalIgnoreCase)) && string.Equals(this.MachineName, pickerServer.MachineName, StringComparison.OrdinalIgnoreCase);
		}

		public virtual bool ArePropertiesEqual(Server server, ServerRole serverRole)
		{
			return string.Equals(this.machineName, server.Name) && string.Equals(this.exchangeLegacyDN, server.ExchangeLegacyDN) && string.Equals(this.fqdn, server.Fqdn) && this.versionNumber == server.VersionNumber && (this.serverRole & serverRole) == (server.CurrentServerRole & serverRole) && this.messageTrackingLogSubjectLoggingEnabled == server.MessageTrackingLogSubjectLoggingEnabled;
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			return new XElement("Server", new object[]
			{
				new XElement("active", this.active),
				new XElement("machineName", this.machineName),
				new XElement("version", this.versionNumber),
				new XElement("serverRole", this.serverRole),
				new XElement("retryAttempts", this.retryAttempts),
				new XElement("nextRetryTime", this.nextRetryTime)
			});
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Active: {1}, RetryAttempts: {2}, NextRetry: {3}", new object[]
			{
				this.MachineName,
				this.active,
				this.retryAttempts,
				this.nextRetryTime
			});
		}

		internal void InternalUpdateServerHealth(bool? isHealthy)
		{
			lock (this.stateLock)
			{
				if (isHealthy == null)
				{
					this.nextRetryTime = DateTime.MinValue;
				}
				else if (this.active)
				{
					if (!isHealthy.Value)
					{
						this.active = false;
						this.ServerDeactivated();
						this.pickerServerList.IncrementServersInRetryCount();
						this.nextRetryTime = DateTime.UtcNow.Add(PickerServer.RetryInterval);
						this.pickerServerList.Tracer.TraceDebug<string, DateTime>(0L, "Server {0} is no longer healthy, will try again at {1}.", this.LegacyDN, this.nextRetryTime);
					}
				}
				else if (isHealthy.Value)
				{
					this.active = true;
					this.retryAttempts = 0;
					this.pickerServerList.DecrementServersInRetryCount();
					this.pickerServerList.Tracer.TraceDebug<string>(0L, "Server {0} is healthy again.", this.LegacyDN);
				}
				else
				{
					this.nextRetryTime = DateTime.UtcNow.Add(PickerServer.RetryInterval);
					this.pickerServerList.Tracer.TraceDebug<string, DateTime>(0L, "Server {0} is still unhealthy, will try again at {1}.", this.LegacyDN, this.nextRetryTime);
				}
			}
		}

		internal void CopyStatusTo(PickerServer newServer)
		{
			if (!this.active)
			{
				lock (this.stateLock)
				{
					newServer.active = this.active;
					newServer.nextRetryTime = this.nextRetryTime;
					newServer.retryAttempts = this.retryAttempts;
				}
			}
		}

		protected static readonly TimeSpan RetryInterval = new TimeSpan(0, 5, 0);

		protected static NetworkCredential localSystemCredential = new NetworkCredential(Environment.MachineName + "$", string.Empty, string.Empty);

		protected DateTime nextRetryTime;

		protected bool active;

		protected PickerServerList pickerServerList;

		private readonly string machineName;

		private readonly string exchangeLegacyDN;

		private readonly string fqdn;

		private readonly int versionNumber;

		private readonly ServerRole serverRole;

		private readonly bool messageTrackingLogSubjectLoggingEnabled;

		private readonly int cachedHashCode;

		private readonly Guid serverGuid;

		private int retryAttempts;

		private object stateLock;
	}
}

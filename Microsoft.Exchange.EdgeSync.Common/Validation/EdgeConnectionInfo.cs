using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Common.Internal;
using Microsoft.Exchange.EdgeSync.Logging;
using Microsoft.Exchange.MessageSecurity;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class EdgeConnectionInfo
	{
		public EdgeConnectionInfo(ReplicationTopology topology, Server edgeServer)
		{
			this.edgeServer = edgeServer;
			this.leaseType = LeaseTokenType.None;
			this.lastSynchronizedDate = DateTime.MinValue;
			this.Connect(topology);
		}

		public Server EdgeServer
		{
			get
			{
				return this.edgeServer;
			}
		}

		public string LeaseHolder
		{
			get
			{
				return this.leaseHolder;
			}
		}

		public DateTime LeaseExpiry
		{
			get
			{
				return this.leaseExpiry;
			}
		}

		public LdapTargetConnection EdgeConnection
		{
			get
			{
				return this.edgeConnection;
			}
		}

		public string FailureDetail
		{
			get
			{
				return this.failureDetail;
			}
		}

		public DateTime LastSynchronizedDate
		{
			get
			{
				return this.lastSynchronizedDate;
			}
		}

		public LeaseTokenType LeaseType
		{
			get
			{
				return this.leaseType;
			}
		}

		public Dictionary<string, Cookie> Cookies
		{
			get
			{
				return this.cookies;
			}
		}

		private void Connect(ReplicationTopology topology)
		{
			EdgeSyncLog edgeSyncLog = new EdgeSyncLog(string.Empty, new Version(), string.Empty, string.Empty, string.Empty);
			EdgeSyncLogSession logSession = edgeSyncLog.OpenSession(string.Empty, string.Empty, 0, string.Empty, EdgeSyncLoggingLevel.None);
			try
			{
				DirectTrust.Load();
				NetworkCredential networkCredential = Util.ExtractNetworkCredential(topology.LocalHub, this.edgeServer.Fqdn, logSession);
				if (networkCredential == null)
				{
					this.failureDetail = Strings.NoCredentialsFound(this.EdgeServer.Fqdn).ToString();
				}
				else
				{
					this.edgeConnection = (LdapTargetConnection)TestEdgeConnectionFactory.Create(topology.LocalHub, new TargetServerConfig(this.EdgeServer.Name, this.EdgeServer.Fqdn, this.EdgeServer.EdgeSyncAdamSslPort), networkCredential, SyncTreeType.General, logSession);
					this.failureDetail = string.Empty;
					if (this.edgeConnection != null)
					{
						this.ExtractLeaseInfo();
						this.ExtractCookieRecords();
					}
				}
			}
			catch (ExDirectoryException ex)
			{
				this.failureDetail = ex.Message;
				this.edgeConnection = null;
			}
			finally
			{
				DirectTrust.Unload();
			}
		}

		private void ExtractCookieRecords()
		{
			this.edgeConnection.TryReadCookie(out this.cookies);
		}

		private void ExtractLeaseInfo()
		{
			string stringForm = this.edgeConnection.GetLease().StringForm;
			if (!string.IsNullOrEmpty(stringForm))
			{
				LeaseToken leaseToken = LeaseToken.Parse(stringForm);
				this.leaseType = leaseToken.Type;
				this.leaseExpiry = leaseToken.Expiry;
				this.leaseHolder = leaseToken.Path;
				this.lastSynchronizedDate = leaseToken.LastSync;
			}
		}

		private Server edgeServer;

		private LdapTargetConnection edgeConnection;

		private string failureDetail;

		private DateTime lastSynchronizedDate;

		private DateTime leaseExpiry;

		private string leaseHolder;

		private LeaseTokenType leaseType;

		private Dictionary<string, Cookie> cookies;
	}
}

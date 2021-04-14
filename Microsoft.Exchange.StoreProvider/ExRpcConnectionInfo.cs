using System;
using System.Net.NetworkInformation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.XropService;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExRpcConnectionInfo
	{
		public ExRpcConnectionCreateFlag CreateFlags { get; private set; }

		public ConnectFlag ConnectFlags { get; private set; }

		public string ServerDn { get; private set; }

		public Guid MdbGuid { get; private set; }

		public string UserDn { get; private set; }

		public string UserName { get; private set; }

		public string Domain { get; private set; }

		public string Password { get; private set; }

		public string HttpProxyServerName { get; private set; }

		public int ConnectionModulation { get; private set; }

		public int LocaleIdForReturnedString { get; private set; }

		public int LocaleIdForSort { get; private set; }

		public int CodePageId { get; private set; }

		public int ReconnectIntervalInMins { get; private set; }

		public int RpcBufferSize { get; private set; }

		public int AuxBufferSize { get; private set; }

		public byte[] ClientSessionInfo { get; private set; }

		public MapiApplicationId ApplicationId { get; private set; }

		public TimeSpan ConnectionTimeout { get; private set; }

		public TimeSpan CallTimeout { get; private set; }

		public Client XropClient { get; private set; }

		public bool IsCrossServer
		{
			get
			{
				if (this.isCrossServer == null)
				{
					this.isCrossServer = new bool?(this.IsCrossServerCall());
				}
				return this.isCrossServer.Value;
			}
		}

		public ExRpcConnectionInfo(ExRpcConnectionCreateFlag createFlags, ConnectFlag connectFlags, string serverDn, Guid mdbGuid, string userDn, string userName, string domain, string password, string httpProxyServerName, int connectionModulation, int lcidString, int lcidSort, int cpid, int cReconnectIntervalInMins, int cbRpcBufferSize, Client xropClient, byte[] clientSessionInfo, string applicationId, TimeSpan connectionTimeout, TimeSpan callTimeout)
		{
			this.CreateFlags = createFlags;
			this.ConnectFlags = connectFlags;
			this.ServerDn = serverDn;
			this.MdbGuid = mdbGuid;
			this.UserDn = userDn;
			this.UserName = userName;
			this.Domain = domain;
			this.Password = password;
			this.HttpProxyServerName = httpProxyServerName;
			this.ConnectionModulation = connectionModulation;
			this.LocaleIdForReturnedString = lcidString;
			this.LocaleIdForSort = lcidSort;
			this.CodePageId = cpid;
			this.ReconnectIntervalInMins = cReconnectIntervalInMins;
			this.RpcBufferSize = cbRpcBufferSize;
			this.AuxBufferSize = cbRpcBufferSize;
			this.XropClient = xropClient;
			this.ClientSessionInfo = clientSessionInfo;
			this.ApplicationId = MapiApplicationId.FromClientInfoString(applicationId);
			this.ConnectionTimeout = connectionTimeout;
			this.CallTimeout = callTimeout;
		}

		public override string ToString()
		{
			return string.Format("serverDn={0}, domain={1}, userDn={2}, userName={3}, mdbGuid={4}, applicationId={5}", new object[]
			{
				this.ServerDn,
				this.Domain,
				this.UserDn,
				this.UserName,
				this.MdbGuid,
				this.ApplicationId.ClientInfo
			});
		}

		internal string GetDestinationServerName()
		{
			if (!string.IsNullOrEmpty(this.destinationServerName))
			{
				return this.destinationServerName;
			}
			if (string.IsNullOrEmpty(this.ServerDn))
			{
				throw new InvalidOperationException("serverDn cannot be null/empty");
			}
			int num = this.ServerDn.IndexOf("/cn=Configuration/cn=Servers/cn=", StringComparison.OrdinalIgnoreCase);
			if (num >= 0)
			{
				this.destinationServerName = this.ServerDn.Substring(num + "/cn=Configuration/cn=Servers/cn=".Length);
			}
			else
			{
				this.destinationServerName = this.ServerDn;
			}
			return this.destinationServerName;
		}

		internal static string GetLocalServerFQDN()
		{
			if (string.IsNullOrEmpty(ExRpcConnectionInfo.localServerFQDN))
			{
				string text = MapiStore.GetLocalServerFqdn();
				if (string.IsNullOrEmpty(text))
				{
					text = Environment.MachineName;
					string localServerDomainName = ExRpcConnectionInfo.LocalServerDomainName;
					if (!text.Contains(localServerDomainName))
					{
						text = text + "." + localServerDomainName;
					}
				}
				ExRpcConnectionInfo.localServerFQDN = text;
			}
			return ExRpcConnectionInfo.localServerFQDN;
		}

		private bool IsCrossServerCall()
		{
			if ((this.ConnectFlags & ConnectFlag.LocalRpcOnly) == ConnectFlag.LocalRpcOnly || this.XropClient != null)
			{
				return false;
			}
			string text = this.GetDestinationServerName();
			string value = ExRpcConnectionInfo.GetLocalServerFQDN();
			string machineName = Environment.MachineName;
			return !text.Equals("localhost", StringComparison.OrdinalIgnoreCase) && !text.Equals(value, StringComparison.OrdinalIgnoreCase) && !text.Equals(machineName, StringComparison.OrdinalIgnoreCase);
		}

		private bool? isCrossServer = null;

		private string destinationServerName;

		private static string localServerFQDN;

		private static readonly string LocalServerDomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
	}
}

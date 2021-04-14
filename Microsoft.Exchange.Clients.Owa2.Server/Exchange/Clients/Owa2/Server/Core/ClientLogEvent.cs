using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ClientLogEvent : ILogEvent
	{
		internal ClientLogEvent(Datapoint datapoint, string userContext) : this(datapoint, userContext, string.Empty, string.Empty, string.Empty, string.Empty, false, null)
		{
		}

		internal ClientLogEvent(Datapoint datapoint, string userContext, string ipAddress, string userName, string serverVersion, bool isMowa, string clientIdCookieValue = null) : this(datapoint, userContext, ipAddress, userName, string.Empty, serverVersion, isMowa, clientIdCookieValue)
		{
		}

		internal ClientLogEvent(Datapoint datapoint, string userContext, string ipAddress, string userName, string clientVersion, string serverVersion, bool isMowa, string clientIdCookieValue = null)
		{
			this.datapoint = datapoint;
			this.userContext = userContext;
			this.ipAddress = ExtensibleLogger.FormatPIIValue(ipAddress);
			this.userName = ExtensibleLogger.FormatPIIValue(userName);
			this.clientVersion = clientVersion;
			this.serverVersion = serverVersion;
			this.isMowa = isMowa;
			this.clientIdCookieValue = clientIdCookieValue;
			this.BuildDictionary();
		}

		public string EventId
		{
			get
			{
				return this.datapoint.Id;
			}
		}

		public string Time
		{
			get
			{
				return this.datapoint.Time;
			}
		}

		internal Datapoint InnerDatapoint
		{
			get
			{
				return this.datapoint;
			}
		}

		internal Dictionary<string, object> DatapointProperties
		{
			get
			{
				return this.datapointProperties;
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.datapointProperties;
		}

		public bool IsForConsumer(DatapointConsumer consumer)
		{
			return this.datapoint.IsForConsumer(consumer);
		}

		public void UpdatePerfTraceDatapoint(UserContext userContext)
		{
			if (userContext != null)
			{
				LogEventCommonData logEventCommonData = userContext.LogEventCommonData;
				this.UpdateDeviceInfo(logEventCommonData);
				this.UpdateClientInfo(logEventCommonData);
				this.UpdateUserAgent(userContext.UserAgent);
			}
		}

		public void UpdateActionRecordDataPoint(UserContext userContext)
		{
			LogEventCommonData logEventCommonData = userContext.LogEventCommonData;
			this.UpdateDeviceInfo(logEventCommonData);
			if (!string.IsNullOrEmpty(logEventCommonData.OfflineEnabled))
			{
				this.CheckAndAddKeyValue("oe", logEventCommonData.OfflineEnabled);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.Layout))
			{
				this.CheckAndAddKeyValue("l", logEventCommonData.Layout);
			}
			this.UpdateTenantGuid(userContext.ExchangePrincipal);
			this.UpdateUserAgent(userContext.UserAgent);
		}

		public void UpdatePerformanceNavigationDatapoint(UserContext userContext)
		{
			LogEventCommonData logEventCommonData = userContext.LogEventCommonData;
			this.UpdateDeviceInfo(logEventCommonData);
			this.UpdateFlightInfo(logEventCommonData);
			this.UpdateTenantGuid(userContext.ExchangePrincipal);
			this.UpdateMailboxGuid(userContext.ExchangePrincipal);
			this.UpdateUserAgent(userContext.UserAgent);
		}

		public void UpdateTenantGuid(ExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal != null && exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.OrganizationId != null)
			{
				string text = exchangePrincipal.MailboxInfo.OrganizationId.ToExternalDirectoryOrganizationId();
				if (text != string.Empty)
				{
					this.CheckAndAddKeyValue("tg", text);
				}
			}
		}

		public void UpdateMailboxGuid(ExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal != null && exchangePrincipal.MailboxInfo != null)
			{
				this.CheckAndAddKeyValue("mg", exchangePrincipal.MailboxInfo.MailboxGuid);
			}
		}

		public void UpdateClientBuildVersion(LogEventCommonData logEventCommonData)
		{
			if (!string.IsNullOrEmpty(logEventCommonData.ClientBuild))
			{
				this.CheckAndAddKeyValue("cbld", logEventCommonData.ClientBuild);
			}
		}

		public void UpdateDeviceInfo(LogEventCommonData logEventCommonData)
		{
			if (!string.IsNullOrEmpty(logEventCommonData.Platform))
			{
				this.CheckAndAddKeyValue("pl", logEventCommonData.Platform);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.Browser))
			{
				this.CheckAndAddKeyValue("brn", logEventCommonData.Browser);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.BrowserVersion))
			{
				this.CheckAndAddKeyValue("brv", logEventCommonData.BrowserVersion);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.OperatingSystem))
			{
				this.CheckAndAddKeyValue("osn", logEventCommonData.OperatingSystem);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.OperatingSystemVersion))
			{
				this.CheckAndAddKeyValue("osv", logEventCommonData.OperatingSystemVersion);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.DeviceModel))
			{
				this.CheckAndAddKeyValue("dm", logEventCommonData.DeviceModel);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.PalBuild))
			{
				this.CheckAndAddKeyValue("pbld", logEventCommonData.PalBuild);
			}
		}

		public void UpdateUserAgent(string userAgentString)
		{
			this.CheckAndAddKeyValue("UA", userAgentString);
		}

		public void UpdateFlightInfo(LogEventCommonData logEventCommonData)
		{
			if (!string.IsNullOrEmpty(logEventCommonData.Flights))
			{
				this.CheckAndAddKeyValue("flt", logEventCommonData.Flights);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.Features))
			{
				this.CheckAndAddKeyValue("ftr", logEventCommonData.Features);
			}
		}

		public void UpdateClientInfo(LogEventCommonData logEventCommonData)
		{
			this.UpdateClientLocaleInfo(logEventCommonData);
			if (!string.IsNullOrEmpty(logEventCommonData.Layout))
			{
				this.CheckAndAddKeyValue("l", logEventCommonData.Layout);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.OfflineEnabled))
			{
				this.CheckAndAddKeyValue("oe", logEventCommonData.OfflineEnabled);
			}
		}

		public void UpdateClientLocaleInfo(LogEventCommonData logEventCommonData)
		{
			if (!string.IsNullOrEmpty(logEventCommonData.Culture))
			{
				this.CheckAndAddKeyValue("clg", logEventCommonData.Culture);
			}
			if (!string.IsNullOrEmpty(logEventCommonData.TimeZone))
			{
				this.CheckAndAddKeyValue("tz", logEventCommonData.TimeZone);
			}
		}

		public void UpdateTenantInfo(UserContext userContext)
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.LogTenantInfo.Enabled)
			{
				if (userContext.LogEventCommonData != null && !string.IsNullOrEmpty(userContext.LogEventCommonData.TenantDomain))
				{
					this.CheckAndAddKeyValue("dom", userContext.LogEventCommonData.TenantDomain);
				}
				if (!string.IsNullOrEmpty(userContext.BposSkuCapability))
				{
					this.CheckAndAddKeyValue("sku", userContext.BposSkuCapability);
				}
			}
		}

		public void UpdateNetid(UserContext userContext)
		{
			if (userContext.MailboxIdentity != null && userContext.MailboxIdentity.GetOWAMiniRecipient().NetID != null)
			{
				this.CheckAndAddKeyValue("NetId", userContext.MailboxIdentity.GetOWAMiniRecipient().NetID.ToString());
			}
		}

		public void UpdateDatabaseInfo(UserContext userContext)
		{
			if (userContext.LogEventCommonData.DatabaseGuid != Guid.Empty)
			{
				this.CheckAndAddKeyValue("db", userContext.LogEventCommonData.DatabaseGuid);
			}
		}

		public void UpdatePassThroughProxyInfo(bool isPassThroughProxy)
		{
			this.CheckAndAddKeyValue("PTP", isPassThroughProxy ? "1" : "0");
		}

		public string GetValue(string keyName)
		{
			return (string)this.datapointProperties[keyName];
		}

		public bool TryGetValue<T>(string keyName, out T result) where T : class
		{
			object obj;
			if (!this.datapointProperties.TryGetValue(keyName, out obj))
			{
				result = default(T);
				return false;
			}
			if (obj is T)
			{
				result = (T)((object)obj);
				return true;
			}
			result = default(T);
			return false;
		}

		public override string ToString()
		{
			return this.datapoint.Consumers.ToString() + ": " + this.EventId;
		}

		private int GetCustomKeyCount()
		{
			if (this.datapoint.Keys != null && this.datapoint.Values != null && this.datapoint.Keys.Length == this.datapoint.Values.Length)
			{
				return this.datapoint.Keys.Length;
			}
			return 0;
		}

		private void BuildDictionary()
		{
			this.datapointProperties = new Dictionary<string, object>
			{
				{
					"ts",
					this.datapoint.Time
				},
				{
					ClientLogEvent.UserContextKey,
					this.userContext
				},
				{
					"ds",
					this.datapoint.Size
				},
				{
					"DC",
					(int)this.datapoint.Consumers
				},
				{
					"Mowa",
					this.isMowa ? "1" : "0"
				}
			};
			if (!string.IsNullOrEmpty(this.ipAddress))
			{
				this.datapointProperties.Add("ip", this.ipAddress);
			}
			if (!string.IsNullOrEmpty(this.userName))
			{
				this.datapointProperties.Add("user", this.userName);
			}
			if (!string.IsNullOrEmpty(this.clientVersion))
			{
				this.datapointProperties.Add("cbld", this.clientVersion);
			}
			if (!string.IsNullOrEmpty(this.serverVersion))
			{
				this.datapointProperties.Add("Bld", this.serverVersion);
			}
			if (!string.IsNullOrEmpty(this.clientIdCookieValue))
			{
				this.datapointProperties.Add("ClientId", this.clientIdCookieValue);
			}
			for (int i = 0; i < this.GetCustomKeyCount(); i++)
			{
				this.CheckAndAddKeyValue(this.datapoint.Keys[i], this.datapoint.Values[i]);
			}
		}

		private void CheckAndAddKeyValue(string key, object value)
		{
			if (!this.datapointProperties.ContainsKey(key))
			{
				this.datapointProperties.Add(key, value);
				return;
			}
			if (this.datapointProperties.ContainsKey("dk"))
			{
				Dictionary<string, object> dictionary;
				(dictionary = this.datapointProperties)["dk"] = dictionary["dk"] + "," + key;
				return;
			}
			this.datapointProperties.Add("dk", key);
		}

		public const string TimeStampKey = "ts";

		public const string IpAddressKey = "ip";

		public const string UserNameKey = "user";

		public const string DatapointSizeKey = "ds";

		public const string ClientVersionKey = "cbld";

		public const string ServerVersionKey = "Bld";

		public const string ConsumersKey = "DC";

		public const string IsMowaKey = "Mowa";

		public const string TenantGuidKey = "tg";

		public const string MailboxGuidKey = "mg";

		public const string NetIdKey = "NetId";

		public const string IsPassThroughProxyKey = "PTP";

		public const string DuplicatedKeysKey = "dk";

		public const string UserAgentKey = "UA";

		public const string ClientIdCookieKey = "ClientId";

		public static readonly string UserContextKey = UserContextCookie.UserContextCookiePrefix;

		private readonly Datapoint datapoint;

		private readonly string userContext;

		private readonly string ipAddress;

		private readonly string userName;

		private readonly string clientVersion;

		private readonly string serverVersion;

		private readonly bool isMowa;

		public readonly string clientIdCookieValue;

		private Dictionary<string, object> datapointProperties;
	}
}

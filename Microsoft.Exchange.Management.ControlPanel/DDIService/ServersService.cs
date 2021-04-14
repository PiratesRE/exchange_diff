using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class ServersService
	{
		public static void ConvertIpBinding(string fieldName, MultiValuedProperty<IPBinding> ipBinding, DataRow row)
		{
			MultiValuedProperty<Identity> multiValuedProperty = new MultiValuedProperty<Identity>();
			foreach (IPBinding ipbinding in ipBinding)
			{
				string text = ipbinding.Address.ToString() + ":" + ipbinding.Port.ToString();
				multiValuedProperty.Add(new Identity(text, text));
			}
			row[fieldName] = multiValuedProperty.ToArray();
		}

		public static void OnPrePop3Setting(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ServersService.OnPrePopImapSetting("Pop3", inputRow, dataTable, store);
		}

		public static void OnPreImap4Setting(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ServersService.OnPrePopImapSetting("Imap4", inputRow, dataTable, store);
		}

		public static void OnPrePopImapSetting(string prefix, DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow[prefix + "PreAuthenticatedConnectionTimeout"]))
			{
				inputRow[prefix + "PreAuthenticatedConnectionTimeout"] = EnhancedTimeSpan.FromSeconds(Convert.ToDouble(dataRow[prefix + "PreAuthenticatedConnectionTimeout"]));
			}
			if (!DBNull.Value.Equals(dataRow[prefix + "AuthenticatedConnectionTimeout"]))
			{
				inputRow[prefix + "AuthenticatedConnectionTimeout"] = EnhancedTimeSpan.FromSeconds(Convert.ToDouble(dataRow[prefix + "AuthenticatedConnectionTimeout"]));
			}
		}

		public static void OnPostPop3Setting(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ServersService.OnPostPopImapSetting("Pop3", inputRow, dataTable, store);
		}

		public static void OnPostImap4Setting(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ServersService.OnPostPopImapSetting("Imap4", inputRow, dataTable, store);
		}

		public static void OnPostPopImapSetting(string prefix, DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			object dataObject = store.GetDataObject(prefix + "AdConfig");
			if (dataObject != null && dataObject is PopImapAdConfiguration)
			{
				PopImapAdConfiguration popImapAdConfiguration = (PopImapAdConfiguration)dataObject;
				if (popImapAdConfiguration.UnencryptedOrTLSBindings != null)
				{
					ServersService.ConvertIpBinding(prefix + "UnencryptedOrTLSBindings", popImapAdConfiguration.UnencryptedOrTLSBindings, row);
				}
				if (popImapAdConfiguration.SSLBindings != null)
				{
					ServersService.ConvertIpBinding(prefix + "SSLBindings", popImapAdConfiguration.SSLBindings, row);
				}
			}
		}

		public static bool IsCurrentExchangeMajorVersion(ServerVersion version)
		{
			return version.Major == Server.CurrentExchangeMajorVersion;
		}

		public static int CountMountedCopy(object databaseCopies)
		{
			int num = 0;
			IEnumerable<object> enumerable = databaseCopies as IEnumerable<object>;
			if (enumerable != null)
			{
				foreach (object obj in enumerable)
				{
					DatabaseCopyStatusEntry databaseCopyStatusEntry = obj as DatabaseCopyStatusEntry;
					if (databaseCopyStatusEntry != null && databaseCopyStatusEntry.Status == CopyStatus.Mounted)
					{
						num++;
					}
				}
			}
			return num;
		}

		public static string GenerateLocalPathString(LocalLongFullPath localLongFullPath)
		{
			if (localLongFullPath != null)
			{
				return localLongFullPath.PathName;
			}
			return string.Empty;
		}

		private static void ProcessNetworkInfo(MultiValuedProperty<Identity> adapterGuids, ArrayList adapterDNSServers, NetworkConnectionInfo info)
		{
			if (info != null)
			{
				ArrayList arrayList = new ArrayList();
				adapterGuids.Add(new Identity(info.AdapterGuid.ToString(), info.Name));
				foreach (IPAddress ipaddress in info.DnsServers)
				{
					arrayList.Add(ipaddress.ToString());
				}
				adapterDNSServers.Add(arrayList.ToArray());
			}
		}

		public static void OnPostNetworkInfo(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			object dataObject = store.GetDataObject("NetworkInfo");
			ArrayList arrayList = new ArrayList();
			MultiValuedProperty<Identity> multiValuedProperty = new MultiValuedProperty<Identity>();
			multiValuedProperty.Add(new Identity(Guid.Empty.ToString(), Strings.DNSTypeAllIPV4));
			arrayList.Add(new string[0]);
			if (dataObject != null && dataObject is IEnumerable)
			{
				foreach (object obj in (dataObject as IEnumerable))
				{
					ServersService.ProcessNetworkInfo(multiValuedProperty, arrayList, obj as NetworkConnectionInfo);
				}
			}
			multiValuedProperty.Add(new Identity("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", Strings.DNSTypeCustom));
			arrayList.Add(new string[0]);
			dataRow["AdapterDNSServers"] = arrayList.ToArray();
			dataRow["AdapterGuids"] = multiValuedProperty.ToArray();
		}

		public static void OnPostDNS(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			object obj = dataRow["ExternalDNSAdapterEnabled"];
			if (!DBNull.Value.Equals(obj) && !(bool)obj)
			{
				dataRow["ExternalDNSAdapterGuid"] = "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF";
			}
			obj = dataRow["InternalDNSAdapterEnabled"];
			if (!DBNull.Value.Equals(obj) && !(bool)obj)
			{
				dataRow["InternalDNSAdapterGuid"] = "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF";
			}
		}

		public static void OnPostTransportLimits(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["MaxPerDomainOutboundConnections"]))
			{
				dataRow["MaxPerDomainOutboundConnections"] = DDIUtil.ConvertUnlimitedToString<int>(dataRow["MaxPerDomainOutboundConnections"], (int t) => t.ToString());
			}
			if (!DBNull.Value.Equals(dataRow["MaxOutboundConnections"]))
			{
				dataRow["MaxOutboundConnections"] = DDIUtil.ConvertUnlimitedToString<int>(dataRow["MaxOutboundConnections"], (int t) => t.ToString());
			}
			if (!DBNull.Value.Equals(dataRow["OutboundConnectionFailureRetryInterval"]))
			{
				dataRow["OutboundConnectionFailureRetryInterval"] = ((EnhancedTimeSpan)dataRow["OutboundConnectionFailureRetryInterval"]).ToString(TimeUnit.Second, 0);
			}
			if (!DBNull.Value.Equals(dataRow["TransientFailureRetryInterval"]))
			{
				dataRow["TransientFailureRetryInterval"] = ((EnhancedTimeSpan)dataRow["TransientFailureRetryInterval"]).ToString(TimeUnit.Minute, 2);
			}
			if (!DBNull.Value.Equals(dataRow["MessageExpirationTimeout"]))
			{
				dataRow["MessageExpirationTimeout"] = ((EnhancedTimeSpan)dataRow["MessageExpirationTimeout"]).ToString(TimeUnit.Day, 9);
			}
			if (!DBNull.Value.Equals(dataRow["DelayNotificationTimeout"]))
			{
				dataRow["DelayNotificationTimeout"] = ((EnhancedTimeSpan)dataRow["DelayNotificationTimeout"]).ToString(TimeUnit.Hour, 5);
			}
		}

		public static void OnPreTransportServerSetting(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["OutboundConnectionFailureRetryInterval"]))
			{
				inputRow["OutboundConnectionFailureRetryInterval"] = ((string)dataRow["OutboundConnectionFailureRetryInterval"]).FromTimeSpan(TimeUnit.Second);
			}
			if (!DBNull.Value.Equals(dataRow["TransientFailureRetryInterval"]))
			{
				inputRow["TransientFailureRetryInterval"] = ((string)dataRow["TransientFailureRetryInterval"]).FromTimeSpan(TimeUnit.Minute);
			}
			if (!DBNull.Value.Equals(dataRow["MessageExpirationTimeout"]))
			{
				inputRow["MessageExpirationTimeout"] = ((string)dataRow["MessageExpirationTimeout"]).FromTimeSpan(TimeUnit.Day);
			}
			if (!DBNull.Value.Equals(dataRow["DelayNotificationTimeout"]))
			{
				inputRow["DelayNotificationTimeout"] = ((string)dataRow["DelayNotificationTimeout"]).FromTimeSpan(TimeUnit.Hour);
			}
		}

		private const string DummyGuid = "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF";

		private const string MessageExpirationTimeout = "MessageExpirationTimeout";

		private const string DelayNotificationTimeout = "DelayNotificationTimeout";

		private const string TransientFailureRetryInterval = "TransientFailureRetryInterval";

		private const string OutboundConnectionFailureRetryInterval = "OutboundConnectionFailureRetryInterval";

		private const string MaxPerDomainOutboundConnections = "MaxPerDomainOutboundConnections";

		private const string MaxOutboundConnections = "MaxOutboundConnections";
	}
}

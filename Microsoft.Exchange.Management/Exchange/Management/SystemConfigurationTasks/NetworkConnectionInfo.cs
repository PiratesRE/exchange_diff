using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class NetworkConnectionInfo : IConfigurable, IEquatable<NetworkConnectionInfo>
	{
		public NetworkConnectionInfo(string name, Guid adapterGuid, IPAddress[] ipAddresses, IPAddress[] dnsServers, string macAddress)
		{
			this.adapterGuid = adapterGuid;
			this.ipAddresses = ipAddresses;
			this.name = name;
			this.dnsServers = dnsServers;
			this.macAddress = macAddress;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IPAddress[] DnsServers
		{
			get
			{
				return this.dnsServers;
			}
		}

		public IPAddress[] IPAddresses
		{
			get
			{
				return this.ipAddresses;
			}
		}

		public Guid AdapterGuid
		{
			get
			{
				return this.adapterGuid;
			}
		}

		public string MacAddress
		{
			get
			{
				return this.macAddress;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return new ConfigObjectId(this.adapterGuid.ToString());
			}
		}

		internal static IList<NetworkConnectionInfo> GetConnectionInfo(ManagementScope scope)
		{
			NetworkConnectionInfo[] array = null;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("select * from Win32_NetworkAdapterConfiguration where IPEnabled = true")))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					array = new NetworkConnectionInfo[managementObjectCollection.Count];
					int num = 0;
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						string text = (string)managementObject["Description"];
						Guid guid = new Guid((string)managementObject["SettingID"]);
						string[] array2 = (string[])managementObject["IPAddress"];
						IPAddress[] array3;
						if (array2 != null)
						{
							array3 = new IPAddress[array2.Length];
							for (int i = 0; i < array2.Length; i++)
							{
								array3[i] = IPAddress.Parse(array2[i]);
							}
						}
						else
						{
							array3 = new IPAddress[0];
						}
						string[] array4 = (string[])managementObject["DNSServerSearchOrder"];
						IPAddress[] array5;
						if (array4 != null)
						{
							array5 = new IPAddress[array4.Length];
							for (int j = 0; j < array4.Length; j++)
							{
								array5[j] = IPAddress.Parse(array4[j]);
							}
						}
						else
						{
							array5 = new IPAddress[0];
						}
						string text2 = (string)managementObject["MACAddress"];
						array[num++] = new NetworkConnectionInfo(text, guid, array3, array5, text2);
					}
				}
			}
			return array;
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		public bool Equals(NetworkConnectionInfo networkConnectionInfo)
		{
			return networkConnectionInfo != null && this.AdapterGuid.Equals(networkConnectionInfo.AdapterGuid);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as NetworkConnectionInfo);
		}

		public override int GetHashCode()
		{
			return this.AdapterGuid.GetHashCode();
		}

		internal const int HresultRpcUnavailable = -2147023174;

		private const string Query = "select * from Win32_NetworkAdapterConfiguration where IPEnabled = true";

		private readonly Guid adapterGuid;

		private readonly string name;

		private IPAddress[] dnsServers;

		private readonly string macAddress;

		private IPAddress[] ipAddresses;
	}
}

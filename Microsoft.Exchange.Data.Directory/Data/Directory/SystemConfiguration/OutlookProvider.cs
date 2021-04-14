using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class OutlookProvider : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return OutlookProvider.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return OutlookProvider.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public string CertPrincipalName
		{
			get
			{
				return (string)this[OutlookProviderSchema.CertPrincipalName];
			}
			set
			{
				this[OutlookProviderSchema.CertPrincipalName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Server
		{
			get
			{
				return (string)this[OutlookProviderSchema.Server];
			}
			set
			{
				this[OutlookProviderSchema.Server] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TTL
		{
			get
			{
				return (int)(this[OutlookProviderSchema.TTL] ?? 1);
			}
			set
			{
				this[OutlookProviderSchema.TTL] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OutlookProviderFlags OutlookProviderFlags
		{
			get
			{
				return (OutlookProviderFlags)(this[OutlookProviderSchema.Flags] ?? OutlookProviderFlags.None);
			}
			set
			{
				this[OutlookProviderSchema.Flags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] RequiredClientVersions
		{
			get
			{
				ClientVersionCollection clientVersionCollection = (ClientVersionCollection)this[OutlookProviderSchema.RequiredClientVersions];
				if (clientVersionCollection != null)
				{
					string[] array = new string[clientVersionCollection.Count];
					int num = 0;
					foreach (ClientVersion clientVersion in clientVersionCollection)
					{
						array[num] = clientVersion.ToString();
					}
					return array;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					ClientVersionCollection clientVersionCollection = new ClientVersionCollection();
					for (int i = 0; i < value.Length; i++)
					{
						string clientVersionString = value[i];
						ClientVersion item = ClientVersion.Parse(clientVersionString);
						clientVersionCollection.Add(item);
					}
					this[OutlookProviderSchema.RequiredClientVersions] = clientVersionCollection;
					return;
				}
				this[OutlookProviderSchema.RequiredClientVersions] = null;
			}
		}

		public ClientVersionCollection GetRequiredClientVersionCollection()
		{
			return (ClientVersionCollection)this[OutlookProviderSchema.RequiredClientVersions];
		}

		internal static ADObjectId GetParentContainer(ITopologyConfigurationSession adSession)
		{
			ADObjectId clientAccessContainerId = adSession.GetClientAccessContainerId();
			ADObjectId relativePath = new ADObjectId("CN=AutoDiscover");
			ADObjectId relativePath2 = new ADObjectId("CN=Outlook");
			return clientAccessContainerId.GetDescendantId(relativePath).GetDescendantId(relativePath2);
		}

		internal void InitializeDefaults()
		{
			this.TTL = 1;
		}

		private static OutlookProviderSchema schema = ObjectSchema.GetInstance<OutlookProviderSchema>();

		private static string mostDerivedClass = "msExchAutoDiscoverConfig";
	}
}

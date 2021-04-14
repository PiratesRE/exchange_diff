using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal class ActiveSyncDevices : ADContainer
	{
		public ActiveSyncDevices()
		{
			base.Name = "ExchangeActiveSyncDevices";
		}

		public DateTime? DeletionPeriod
		{
			get
			{
				return (DateTime?)this[ActiveSyncDevicesSchema.DeletionPeriod];
			}
			internal set
			{
				this[ActiveSyncDevicesSchema.DeletionPeriod] = value;
			}
		}

		public int ObjectsDeletedThisPeriod
		{
			get
			{
				return (int)this[ActiveSyncDevicesSchema.ObjectsDeletedThisPeriod];
			}
			internal set
			{
				this[ActiveSyncDevicesSchema.ObjectsDeletedThisPeriod] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncDevices.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchActiveSyncDevices";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal const string ContainerName = "ExchangeActiveSyncDevices";

		internal const string MostDerivedClassName = "msExchActiveSyncDevices";

		private static ActiveSyncDevicesSchema schema = ObjectSchema.GetInstance<ActiveSyncDevicesSchema>();
	}
}

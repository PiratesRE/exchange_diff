using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ActiveSyncDeviceClasses : ADContainer
	{
		public ActiveSyncDeviceClasses()
		{
			this.Name = "ExchangeDeviceClasses";
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncDeviceClasses.schema;
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

		internal override ADObjectId ParentPath
		{
			get
			{
				return ActiveSyncDeviceClasses.parentPath;
			}
		}

		internal const string ContainerName = "ExchangeDeviceClasses";

		private const string MostDerivedClassName = "msExchActiveSyncDevices";

		private static ADObjectId parentPath = new ADObjectId("CN=Mobile Mailbox Settings");

		private static ActiveSyncDeviceClassesSchema schema = ObjectSchema.GetInstance<ActiveSyncDeviceClassesSchema>();
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ServersContainer : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ServersContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ServersContainer.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.AdminDisplayName))
			{
				base.AdminDisplayName = ServersContainer.DefaultName;
			}
			if (!base.IsModified(ServersContainerSchema.ContainerInfo))
			{
				this[ServersContainerSchema.ContainerInfo] = ContainerInfo.Servers;
			}
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		public static readonly string DefaultName = "Servers";

		private static ServersContainerSchema schema = ObjectSchema.GetInstance<ServersContainerSchema>();

		private static string mostDerivedClass = "msExchServersContainer";
	}
}

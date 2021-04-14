using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ConnectorsContainer : ADContainer
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ConnectorsContainer.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		public const string DefaultName = "Connections";

		private static string mostDerivedClass = "msExchConnectors";
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class AdvancedSecurityContainer : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AdvancedSecurityContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AdvancedSecurityContainer.mostDerivedClass;
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

		public const string DefaultName = "Advanced Security";

		private static AdvancedSecurityContainerSchema schema = ObjectSchema.GetInstance<AdvancedSecurityContainerSchema>();

		private static string mostDerivedClass = "msExchAdvancedSecurityContainer";
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class RoutingGroupsContainer : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RoutingGroupsContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RoutingGroupsContainer.mostDerivedClass;
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

		private static RoutingGroupsContainerSchema schema = ObjectSchema.GetInstance<RoutingGroupsContainerSchema>();

		private static string mostDerivedClass = "msExchRoutingGroupContainer";

		public static readonly string DefaultName = "Routing Groups";
	}
}

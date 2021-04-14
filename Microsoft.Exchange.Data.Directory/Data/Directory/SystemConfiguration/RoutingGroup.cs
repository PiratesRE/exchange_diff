using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class RoutingGroup : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RoutingGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RoutingGroup.mostDerivedClass;
			}
		}

		public ADObjectId RoutingMasterDN
		{
			get
			{
				return (ADObjectId)this[RoutingGroupSchema.RoutingMasterDN];
			}
			internal set
			{
				this[RoutingGroupSchema.RoutingMasterDN] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RoutingMembersDN
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RoutingGroupSchema.RoutingGroupMembersDN];
			}
			internal set
			{
				this[RoutingGroupSchema.RoutingGroupMembersDN] = value;
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

		private static RoutingGroupSchema schema = ObjectSchema.GetInstance<RoutingGroupSchema>();

		private static string mostDerivedClass = "msExchRoutingGroup";

		public static readonly string DefaultName = "Exchange Routing Group (DWBGZMFD01QNBJR)";
	}
}

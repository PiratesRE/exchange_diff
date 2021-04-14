using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class GroupCapacityDisplay : ConfigurableObject
	{
		public GroupCapacityDisplay() : base(new SimplePropertyBag(GroupCapacityDisplay.GroupCapacityDisplaySchema.GroupName, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public GroupCapacityDisplay(string groupName, CapacityBlock capacity) : this()
		{
			this.GroupName = groupName;
			this.StartDate = new DateTime?(capacity.StartDate);
			this.UpgradeUnits = new int?(capacity.UpgradeUnits);
		}

		public string GroupName
		{
			get
			{
				return (string)this[GroupCapacityDisplay.GroupCapacityDisplaySchema.GroupName];
			}
			internal set
			{
				this[GroupCapacityDisplay.GroupCapacityDisplaySchema.GroupName] = value;
			}
		}

		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)this[GroupCapacityDisplay.GroupCapacityDisplaySchema.StartDate];
			}
			internal set
			{
				this[GroupCapacityDisplay.GroupCapacityDisplaySchema.StartDate] = value;
			}
		}

		public int? UpgradeUnits
		{
			get
			{
				return (int?)this[GroupCapacityDisplay.GroupCapacityDisplaySchema.UpgradeUnits];
			}
			internal set
			{
				this[GroupCapacityDisplay.GroupCapacityDisplaySchema.UpgradeUnits] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return GroupCapacityDisplay.schema;
			}
		}

		public override bool Equals(object obj)
		{
			GroupCapacityDisplay groupCapacityDisplay = obj as GroupCapacityDisplay;
			return groupCapacityDisplay != null && (string.Equals(this.GroupName, groupCapacityDisplay.GroupName, StringComparison.OrdinalIgnoreCase) && object.Equals(this.StartDate, groupCapacityDisplay.StartDate)) && this.UpgradeUnits == groupCapacityDisplay.UpgradeUnits;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static GroupCapacityDisplay.GroupCapacityDisplaySchema schema = ObjectSchema.GetInstance<GroupCapacityDisplay.GroupCapacityDisplaySchema>();

		internal class GroupCapacityDisplaySchema : SimpleProviderObjectSchema
		{
			public static readonly ProviderPropertyDefinition GroupName = new SimpleProviderPropertyDefinition("GroupName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StartDate = new SimpleProviderPropertyDefinition("StartDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition UpgradeUnits = new SimpleProviderPropertyDefinition("UpgradeUnits", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}

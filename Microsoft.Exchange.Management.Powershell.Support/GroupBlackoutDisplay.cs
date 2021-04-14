using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class GroupBlackoutDisplay : ConfigurableObject
	{
		public GroupBlackoutDisplay() : base(new SimplePropertyBag(GroupBlackoutDisplay.GroupBlackoutDisplaySchema.GroupName, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public GroupBlackoutDisplay(string groupName, BlackoutInterval blackout) : this()
		{
			this.GroupName = groupName;
			this.StartDate = new DateTime?(blackout.StartDate);
			this.EndDate = new DateTime?(blackout.EndDate);
			this.Reason = blackout.Reason;
		}

		public string GroupName
		{
			get
			{
				return (string)this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.GroupName];
			}
			internal set
			{
				this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.GroupName] = value;
			}
		}

		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.StartDate];
			}
			internal set
			{
				this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.StartDate] = value;
			}
		}

		public DateTime? EndDate
		{
			get
			{
				return (DateTime?)this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.EndDate];
			}
			internal set
			{
				this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.EndDate] = value;
			}
		}

		public string Reason
		{
			get
			{
				return (string)this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.Reason];
			}
			internal set
			{
				this[GroupBlackoutDisplay.GroupBlackoutDisplaySchema.Reason] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return GroupBlackoutDisplay.schema;
			}
		}

		public override bool Equals(object obj)
		{
			GroupBlackoutDisplay groupBlackoutDisplay = obj as GroupBlackoutDisplay;
			return groupBlackoutDisplay != null && (string.Equals(this.GroupName, groupBlackoutDisplay.GroupName, StringComparison.OrdinalIgnoreCase) && object.Equals(this.StartDate, groupBlackoutDisplay.StartDate)) && object.Equals(this.EndDate, groupBlackoutDisplay.EndDate);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static GroupBlackoutDisplay.GroupBlackoutDisplaySchema schema = ObjectSchema.GetInstance<GroupBlackoutDisplay.GroupBlackoutDisplaySchema>();

		internal class GroupBlackoutDisplaySchema : SimpleProviderObjectSchema
		{
			public static readonly ProviderPropertyDefinition GroupName = new SimpleProviderPropertyDefinition("GroupName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StartDate = new SimpleProviderPropertyDefinition("StartDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition EndDate = new SimpleProviderPropertyDefinition("EndDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Reason = new SimpleProviderPropertyDefinition("Reason", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}

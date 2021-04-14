using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("DistributionGroup")]
	[Serializable]
	public class DistributionGroup : DistributionGroupBase
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return DistributionGroup.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public DistributionGroup()
		{
		}

		public DistributionGroup(ADGroup dataObject) : base(dataObject)
		{
		}

		internal static DistributionGroup FromDataObject(ADGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new DistributionGroup(dataObject);
		}

		public GroupTypeFlags GroupType
		{
			get
			{
				return (GroupTypeFlags)this[DistributionGroupSchema.GroupType];
			}
		}

		[Parameter(Mandatory = false)]
		public string SamAccountName
		{
			get
			{
				return (string)this[DistributionGroupSchema.SamAccountName];
			}
			set
			{
				this[DistributionGroupSchema.SamAccountName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BypassNestedModerationEnabled
		{
			get
			{
				return (bool)this[DistributionGroupSchema.BypassNestedModerationEnabled];
			}
			set
			{
				this[DistributionGroupSchema.BypassNestedModerationEnabled] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[DistributionGroupSchema.ManagedBy];
			}
			set
			{
				this[DistributionGroupSchema.ManagedBy] = value;
			}
		}

		public MemberUpdateType MemberJoinRestriction
		{
			get
			{
				return (MemberUpdateType)this[DistributionGroupSchema.MemberJoinRestriction];
			}
			set
			{
				this[DistributionGroupSchema.MemberJoinRestriction] = value;
			}
		}

		public MemberUpdateType MemberDepartRestriction
		{
			get
			{
				return (MemberUpdateType)this[DistributionGroupSchema.MemberDepartRestriction];
			}
			set
			{
				this[DistributionGroupSchema.MemberDepartRestriction] = value;
			}
		}

		private static DistributionGroupSchema schema = ObjectSchema.GetInstance<DistributionGroupSchema>();
	}
}

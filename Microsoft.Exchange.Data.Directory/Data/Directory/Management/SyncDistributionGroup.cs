using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncDistributionGroup")]
	[Serializable]
	public class SyncDistributionGroup : DistributionGroup
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncDistributionGroup.schema;
			}
		}

		public SyncDistributionGroup()
		{
			base.SetObjectClass("group");
		}

		public SyncDistributionGroup(ADGroup dataObject) : base(dataObject)
		{
		}

		internal new static SyncDistributionGroup FromDataObject(ADGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncDistributionGroup(dataObject);
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[SyncDistributionGroupSchema.BlockedSendersHash];
			}
			set
			{
				this[SyncDistributionGroupSchema.BlockedSendersHash] = value;
			}
		}

		public new MultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.BypassModerationFrom];
			}
			set
			{
				this[MailEnabledRecipientSchema.BypassModerationFrom] = value;
			}
		}

		public new MultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailEnabledRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				this[MailEnabledRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Members
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SyncDistributionGroupSchema.Members];
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[SyncDistributionGroupSchema.Notes];
			}
			set
			{
				this[SyncDistributionGroupSchema.Notes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[SyncDistributionGroupSchema.RecipientDisplayType];
			}
			set
			{
				this[SyncDistributionGroupSchema.RecipientDisplayType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[SyncDistributionGroupSchema.SafeRecipientsHash];
			}
			set
			{
				this[SyncDistributionGroupSchema.SafeRecipientsHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[SyncDistributionGroupSchema.SafeSendersHash];
			}
			set
			{
				this[SyncDistributionGroupSchema.SafeSendersHash] = value;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncDistributionGroupSchema.EndOfList];
			}
			internal set
			{
				this[SyncDistributionGroupSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncDistributionGroupSchema.Cookie];
			}
			internal set
			{
				this[SyncDistributionGroupSchema.Cookie] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return (string)this[SyncDistributionGroupSchema.DirSyncId];
			}
			set
			{
				this[SyncDistributionGroupSchema.DirSyncId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[SyncDistributionGroupSchema.SeniorityIndex];
			}
			set
			{
				this[SyncDistributionGroupSchema.SeniorityIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[SyncDistributionGroupSchema.PhoneticDisplayName];
			}
			set
			{
				this[SyncDistributionGroupSchema.PhoneticDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHierarchicalGroup
		{
			get
			{
				return (bool)this[SyncDistributionGroupSchema.IsHierarchicalGroup];
			}
			set
			{
				this[SyncDistributionGroupSchema.IsHierarchicalGroup] = value;
			}
		}

		public ADObjectId RawManagedBy
		{
			get
			{
				return (ADObjectId)this[SyncDistributionGroupSchema.RawManagedBy];
			}
			internal set
			{
				this[SyncDistributionGroupSchema.RawManagedBy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> CoManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SyncDistributionGroupSchema.CoManagedBy];
			}
			internal set
			{
				this[SyncDistributionGroupSchema.CoManagedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncDistributionGroupSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncDistributionGroupSchema.OnPremisesObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncDistributionGroupSchema.IsDirSynced];
			}
			set
			{
				this[SyncDistributionGroupSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncDistributionGroupSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncDistributionGroupSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[SyncDistributionGroupSchema.ExcludedFromBackSync];
			}
			set
			{
				this[SyncDistributionGroupSchema.ExcludedFromBackSync] = value;
			}
		}

		private static SyncDistributionGroupSchema schema = ObjectSchema.GetInstance<SyncDistributionGroupSchema>();
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("DynamicDistributionGroup")]
	[Serializable]
	public class SyncDynamicDistributionGroup : DynamicDistributionGroup
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncDynamicDistributionGroup.schema;
			}
		}

		public SyncDynamicDistributionGroup()
		{
			base.SetObjectClass("group");
		}

		public SyncDynamicDistributionGroup(ADDynamicGroup dataObject) : base(dataObject)
		{
		}

		internal new static SyncDynamicDistributionGroup FromDataObject(ADDynamicGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncDynamicDistributionGroup(dataObject);
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[SyncDynamicDistributionGroupSchema.BlockedSendersHash];
			}
			set
			{
				this[SyncDynamicDistributionGroupSchema.BlockedSendersHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[SyncDynamicDistributionGroupSchema.RecipientDisplayType];
			}
			set
			{
				this[SyncDynamicDistributionGroupSchema.RecipientDisplayType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[SyncDynamicDistributionGroupSchema.SafeRecipientsHash];
			}
			set
			{
				this[SyncDynamicDistributionGroupSchema.SafeRecipientsHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[SyncDynamicDistributionGroupSchema.SafeSendersHash];
			}
			set
			{
				this[SyncDynamicDistributionGroupSchema.SafeSendersHash] = value;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncDynamicDistributionGroupSchema.EndOfList];
			}
			internal set
			{
				this[SyncDynamicDistributionGroupSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncDynamicDistributionGroupSchema.Cookie];
			}
			internal set
			{
				this[SyncDynamicDistributionGroupSchema.Cookie] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return (string)this[SyncDynamicDistributionGroupSchema.DirSyncId];
			}
			set
			{
				this[SyncDynamicDistributionGroupSchema.DirSyncId] = value;
			}
		}

		private static SyncDynamicDistributionGroupSchema schema = ObjectSchema.GetInstance<SyncDynamicDistributionGroupSchema>();
	}
}

using System;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ADABGroup : ABGroup
	{
		public ADABGroup(ADABSession ownerSession, ADGroup activeDirectoryGroup) : base(ownerSession)
		{
			if (activeDirectoryGroup == null)
			{
				throw new ArgumentNullException("activeDirectoryGroup");
			}
			if (activeDirectoryGroup.Id == null)
			{
				throw new ArgumentException("activeDirectoryGroup.Id can't be null.", "activeDirectoryGroup.Id");
			}
			this.recipient = activeDirectoryGroup;
			this.activeDirectoryGroup = activeDirectoryGroup;
		}

		public ADABGroup(ADABSession ownerSession, ADDynamicGroup dynamicGroup) : base(ownerSession)
		{
			if (dynamicGroup == null)
			{
				throw new ArgumentNullException("dynamicGroup");
			}
			if (dynamicGroup.Id == null)
			{
				throw new ArgumentException("dynamicGroup.Id can't be null.", "dynamicGroup.Id");
			}
			this.recipient = dynamicGroup;
			this.dynamicGroup = dynamicGroup;
		}

		protected override string GetAlias()
		{
			return this.recipient.Alias;
		}

		protected override bool GetCanEmail()
		{
			return ADABUtils.CanEmailRecipientType(this.recipient.RecipientType);
		}

		protected override string GetDisplayName()
		{
			return this.recipient.DisplayName;
		}

		protected override string GetLegacyExchangeDN()
		{
			return this.recipient.LegacyExchangeDN;
		}

		protected override string GetEmailAddress()
		{
			return this.recipient.PrimarySmtpAddress.ToString();
		}

		protected override ABObjectId GetId()
		{
			if (this.id == null)
			{
				this.id = new ADABObjectId(this.recipient.Id);
			}
			return this.id;
		}

		protected override ABObjectId GetOwnerId()
		{
			if (this.ownerId != null)
			{
				return this.ownerId;
			}
			if (ADABUtils.GetOwnerId(this.recipient, out this.ownerId))
			{
				return this.ownerId;
			}
			return null;
		}

		protected override bool? GetHiddenMembership()
		{
			if (this.activeDirectoryGroup != null)
			{
				return new bool?(this.activeDirectoryGroup.HiddenGroupMembershipEnabled);
			}
			if (this.dynamicGroup != null)
			{
				return new bool?(true);
			}
			return null;
		}

		private ADRecipient recipient;

		private ADGroup activeDirectoryGroup;

		private ADDynamicGroup dynamicGroup;

		private ADABObjectId id;

		private ADABObjectId ownerId;
	}
}

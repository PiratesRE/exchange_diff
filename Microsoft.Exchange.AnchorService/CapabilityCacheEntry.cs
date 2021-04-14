using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CapabilityCacheEntry : CacheEntryBase
	{
		public CapabilityCacheEntry(AnchorContext context, ADUser user) : base(context, user)
		{
			this.MailboxUser = user;
		}

		public OrganizationCapability AnchorCapability
		{
			get
			{
				return base.Context.AnchorCapability;
			}
		}

		public virtual OrganizationCapability ActiveCapability
		{
			get
			{
				return base.Context.AnchorCapability;
			}
		}

		public ADUser MailboxUser { get; protected set; }

		public override bool IsLocal
		{
			get
			{
				AnchorUtil.AssertOrThrow(this.MailboxUser != null, "expect to have a valid user", new object[0]);
				AnchorUtil.AssertOrThrow(base.ADProvider != null, "expect to have a valid recipient session", new object[0]);
				try
				{
					base.ADProvider.EnsureLocalMailbox(this.MailboxUser, false);
				}
				catch (AnchorMailboxNotFoundOnServerException)
				{
					return false;
				}
				return true;
			}
		}

		public override bool IsActive
		{
			get
			{
				AnchorUtil.AssertOrThrow(this.MailboxUser != null, "expect to have a valid user", new object[0]);
				return this.MailboxUser.PersistedCapabilities.Contains((Capability)this.AnchorCapability);
			}
		}

		public override int UniqueEntryCount
		{
			get
			{
				return base.ADProvider.GetOrganizationMailboxesByCapability(this.AnchorCapability).Count<ADUser>();
			}
		}

		public override bool Sync()
		{
			ADUser aduser = base.ADProvider.GetADRecipientByObjectId(base.ObjectId) as ADUser;
			if (aduser == null && this.MailboxUser != null)
			{
				base.ADProvider = new AnchorADProvider(base.Context, base.OrganizationId, this.MailboxUser.OriginatingServer);
				aduser = (base.ADProvider.GetADRecipientByObjectId(base.ObjectId) as ADUser);
			}
			if (aduser == null)
			{
				return false;
			}
			this.MailboxUser = aduser;
			return base.Sync();
		}

		public override void Activate()
		{
		}

		public override void Deactivate()
		{
		}
	}
}

using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class AddFavorite
	{
		internal AddFavorite(IXSOFactory xsoFactory, IMailboxSession mailboxSession, InstantMessageBuddy imBuddy)
		{
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (imBuddy == null)
			{
				throw new ArgumentNullException("imBuddy");
			}
			if (imBuddy.EmailAddress == null && string.IsNullOrEmpty(imBuddy.SipUri))
			{
				throw new ArgumentException("Either EmailAddress or SipUri is mandatory for imBuddy");
			}
			this.xso = xsoFactory;
			this.session = mailboxSession;
			this.utilities = new UnifiedContactStoreUtilities(this.session, this.xso);
			this.buddy = imBuddy;
		}

		internal bool Execute()
		{
			EmailAddress emailAddress = null;
			if (this.buddy.EmailAddress != null)
			{
				emailAddress = new EmailAddress
				{
					Address = this.buddy.EmailAddress.EmailAddress,
					Name = this.buddy.EmailAddress.Name,
					OriginalDisplayName = this.buddy.EmailAddress.OriginalDisplayName,
					RoutingType = this.buddy.EmailAddress.RoutingType
				};
			}
			StoreObjectId storeObjectId;
			PersonId personId;
			this.utilities.RetrieveOrCreateContact(this.buddy.SipUri, emailAddress, this.buddy.DisplayName, this.buddy.FirstName, this.buddy.LastName, out storeObjectId, out personId);
			if (storeObjectId != null)
			{
				StoreObjectId systemPdlId = this.utilities.GetSystemPdlId(UnifiedContactStoreUtilities.FavoritesPdlDisplayName, "IPM.DistList.MOC.Favorites");
				if (systemPdlId != null)
				{
					this.utilities.AddContactToGroup(storeObjectId, this.buddy.DisplayName, systemPdlId);
					this.SetIsFavoriteFlag(storeObjectId);
					return true;
				}
			}
			return false;
		}

		private void SetIsFavoriteFlag(StoreObjectId contactId)
		{
			using (IContact contact = this.xso.BindToContact(this.session, contactId, null))
			{
				contact.OpenAsReadWrite();
				contact.IsFavorite = true;
				contact.Save(SaveMode.ResolveConflicts);
			}
		}

		private readonly UnifiedContactStoreUtilities utilities;

		private readonly IXSOFactory xso;

		private readonly IMailboxSession session;

		private readonly InstantMessageBuddy buddy;
	}
}

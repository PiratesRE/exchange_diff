using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.Providers
{
	internal class MailMessageConfigDataProvider : IConfigDataProvider
	{
		public MailMessageConfigDataProvider(IRecipientSession adSession, ADUser mailbox)
		{
			this.adSession = adSession;
			this.mailbox = mailbox;
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotImplementedException();
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			if (this.mailMessage != null && this.mailMessage.Identity == identity)
			{
				return this.mailMessage;
			}
			return null;
		}

		public void Save(IConfigurable instance)
		{
			this.mailMessage = (MailMessage)instance;
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(ExchangePrincipal.FromDirectoryObjectId(this.adSession, this.mailbox.Id, RemotingOptions.LocalConnectionsOnly), currentCulture, "Client=Management;Action=New-MailMessage"))
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				using (MessageItem messageItem = MessageItem.Create(mailboxSession, defaultFolderId))
				{
					if (!string.IsNullOrEmpty(this.mailMessage.Subject))
					{
						messageItem.Subject = this.mailMessage.Subject;
					}
					if (!string.IsNullOrEmpty(this.mailMessage.Body))
					{
						using (TextWriter textWriter = messageItem.Body.OpenTextWriter((BodyFormat)this.mailMessage.BodyFormat))
						{
							textWriter.WriteLine(this.mailMessage.Body);
						}
					}
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load();
					this.mailMessage.SetIdentity(messageItem.Id.ObjectId);
				}
			}
		}

		public string Source
		{
			get
			{
				if (this.adSession != null)
				{
					return this.adSession.Source;
				}
				return null;
			}
		}

		private IRecipientSession adSession;

		private ADUser mailbox;

		private MailMessage mailMessage;
	}
}

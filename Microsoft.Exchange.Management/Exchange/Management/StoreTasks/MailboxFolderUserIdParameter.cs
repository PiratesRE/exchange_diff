using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public class MailboxFolderUserIdParameter : IIdentityParameter
	{
		internal MailboxFolderUserId ResolveMailboxFolderUserId(MailboxSession mailboxSession)
		{
			if (this.mailboxFolderUserId == null)
			{
				this.mailboxFolderUserId = this.CreateMailboxFolderUserId(mailboxSession);
			}
			if (this.mailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.External)
			{
				this.mailboxFolderUserId.EnsureExternalUser(mailboxSession);
			}
			return this.mailboxFolderUserId;
		}

		private MailboxFolderUserId CreateMailboxFolderUserId(MailboxSession mailboxSession)
		{
			bool flag = !string.IsNullOrEmpty(this.rawIdentity) && SmtpAddress.IsValidSmtpAddress(this.rawIdentity);
			if (flag)
			{
				MailboxFolderUserId mailboxFolderUserId = MailboxFolderUserId.TryCreateFromSmtpAddress(this.rawIdentity, mailboxSession);
				if (mailboxFolderUserId != null)
				{
					return mailboxFolderUserId;
				}
			}
			IRecipientSession adrecipientSession = mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			IEnumerable<ADRecipient> objects = this.RecipientIdParameter.GetObjects<ADRecipient>(null, adrecipientSession);
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ADRecipient recipient = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(this.RecipientIdParameter.ToString()));
					}
					bool allowInvalidSecurityPrincipal = !flag;
					MailboxFolderUserId mailboxFolderUserId = MailboxFolderUserId.TryCreateFromADRecipient(recipient, allowInvalidSecurityPrincipal);
					if (mailboxFolderUserId != null)
					{
						return mailboxFolderUserId;
					}
					if (!flag)
					{
						throw new InvalidInternalUserIdException(this.RecipientIdParameter.ToString());
					}
				}
			}
			if (flag)
			{
				throw new InvalidExternalUserIdException(this.rawIdentity);
			}
			return MailboxFolderUserId.CreateFromUnknownUser(this.rawIdentity);
		}

		private RecipientIdParameter RecipientIdParameter
		{
			get
			{
				if (this.recipientIdParameter == null)
				{
					this.recipientIdParameter = RecipientIdParameter.Parse(this.rawIdentity);
				}
				return this.recipientIdParameter;
			}
		}

		public MailboxFolderUserIdParameter(string identity)
		{
			if (string.Equals(MailboxFolderUserId.AnonymousUserId.ToString(), identity, StringComparison.InvariantCultureIgnoreCase) || string.Equals("Anonymous", identity, StringComparison.InvariantCultureIgnoreCase))
			{
				this.mailboxFolderUserId = MailboxFolderUserId.AnonymousUserId;
				return;
			}
			if (string.Equals(MailboxFolderUserId.DefaultUserId.ToString(), identity, StringComparison.InvariantCultureIgnoreCase) || string.Equals("Default", identity, StringComparison.InvariantCultureIgnoreCase))
			{
				this.mailboxFolderUserId = MailboxFolderUserId.DefaultUserId;
				return;
			}
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentException("identity");
			}
			this.rawIdentity = identity;
		}

		public MailboxFolderUserIdParameter()
		{
		}

		public MailboxFolderUserIdParameter(ADObjectId adObjectId)
		{
			this.recipientIdParameter = new RecipientIdParameter(adObjectId);
		}

		public MailboxFolderUserIdParameter(Mailbox recipient)
		{
			this.recipientIdParameter = new RecipientIdParameter(recipient.Id);
		}

		public MailboxFolderUserIdParameter(DistributionGroup recipient)
		{
			this.recipientIdParameter = new RecipientIdParameter(recipient.Id);
		}

		public MailboxFolderUserIdParameter(MailUser recipient)
		{
			this.recipientIdParameter = new RecipientIdParameter(recipient.Id);
		}

		public MailboxFolderUserIdParameter(MailboxFolderUserId mailboxFolderUserId)
		{
			if (mailboxFolderUserId == null)
			{
				throw new ArgumentNullException("mailboxFolderUserId");
			}
			this.mailboxFolderUserId = mailboxFolderUserId;
		}

		public static MailboxFolderUserIdParameter Parse(string identity)
		{
			return new MailboxFolderUserIdParameter(identity);
		}

		public override string ToString()
		{
			if (this.mailboxFolderUserId != null)
			{
				return this.mailboxFolderUserId.ToString();
			}
			if (this.recipientIdParameter != null)
			{
				return this.recipientIdParameter.ToString();
			}
			return this.rawIdentity;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			MailboxFolderUserId mailboxFolderUserId = objectId as MailboxFolderUserId;
			if (mailboxFolderUserId == null)
			{
				ADObjectId adObjectId = objectId as ADObjectId;
				this.recipientIdParameter = new RecipientIdParameter(adObjectId);
				return;
			}
			this.mailboxFolderUserId = mailboxFolderUserId;
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			throw new NotImplementedException();
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			throw new NotImplementedException();
		}

		private MailboxFolderUserId mailboxFolderUserId;

		private RecipientIdParameter recipientIdParameter;

		private readonly string rawIdentity;
	}
}

using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal sealed class JunkEmailSettings : ItemPropertiesBase
	{
		public override void Apply(MrsPSHandler psHandler, MailboxSession mailboxSession)
		{
			JunkEmailRule junkEmailRule = mailboxSession.JunkEmailRule;
			if (this.TrustedSenderDomain != null)
			{
				this.AddEntriesToCollection(junkEmailRule.TrustedSenderDomainCollection, "TrustedSenderDomainCollection", mailboxSession.MailboxGuid, this.TrustedSenderDomain);
			}
			if (this.TrustedSenderEmail != null)
			{
				this.AddEntriesToCollection(junkEmailRule.TrustedSenderEmailCollection, "TrustedSenderEmailCollection", mailboxSession.MailboxGuid, this.TrustedSenderEmail);
			}
			if (this.BlockedSenderDomain != null)
			{
				this.AddEntriesToCollection(junkEmailRule.BlockedSenderDomainCollection, "BlockedSenderDomainCollection", mailboxSession.MailboxGuid, this.BlockedSenderDomain);
			}
			if (this.BlockedSenderEmail != null)
			{
				this.AddEntriesToCollection(junkEmailRule.BlockedSenderEmailCollection, "BlockedSenderEmailCollection", mailboxSession.MailboxGuid, this.BlockedSenderEmail);
			}
			if (this.TrustedRecipientDomain != null)
			{
				this.AddEntriesToCollection(junkEmailRule.TrustedRecipientDomainCollection, "TrustedRecipientDomainCollection", mailboxSession.MailboxGuid, this.TrustedRecipientDomain);
			}
			if (this.TrustedRecipientEmail != null)
			{
				this.AddEntriesToCollection(junkEmailRule.TrustedRecipientEmailCollection, "TrustedRecipientEmailCollection", mailboxSession.MailboxGuid, this.TrustedRecipientEmail);
			}
			if (this.TrustedContactsEmail != null)
			{
				IRecipientSession adrecipientSession = mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				if (adrecipientSession == null)
				{
					string itemList = this.TrustedContactsEmail.Aggregate((string result, string email) => result + ", " + email);
					throw new MailboxSettingsJunkMailErrorPermanentException("TrustedContactsEmail", itemList, "error getting RecipientSession");
				}
				junkEmailRule.SynchronizeContactsCache();
				foreach (string email2 in this.TrustedContactsEmail)
				{
					junkEmailRule.AddTrustedContact(email2, adrecipientSession);
				}
			}
			if (this.SendAsEmail != null)
			{
				this.AddEntriesToCollection(junkEmailRule.TrustedSenderEmailCollection, "TrustedSenderEmailCollection", mailboxSession.MailboxGuid, this.SendAsEmail);
			}
			junkEmailRule.Save();
		}

		private void AddEntriesToCollection(JunkEmailCollection collection, string collectionName, Guid mailboxGuid, string[] entriesToAdd)
		{
			foreach (string text in entriesToAdd)
			{
				JunkEmailCollection.ValidationProblem validationProblem = collection.TryAdd(text);
				if (validationProblem != JunkEmailCollection.ValidationProblem.NoError && validationProblem != JunkEmailCollection.ValidationProblem.Duplicate)
				{
					throw new MailboxSettingsJunkMailErrorPermanentException(collectionName, text, validationProblem.ToString());
				}
			}
		}

		[DataMember]
		public string[] TrustedSenderEmail { get; set; }

		[DataMember]
		public string[] TrustedSenderDomain { get; set; }

		[DataMember]
		public string[] TrustedRecipientEmail { get; set; }

		[DataMember]
		public string[] TrustedRecipientDomain { get; set; }

		[DataMember]
		public string[] TrustedContactsEmail { get; set; }

		[DataMember]
		public string[] BlockedSenderEmail { get; set; }

		[DataMember]
		public string[] BlockedSenderDomain { get; set; }

		[DataMember]
		public string[] SendAsEmail { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal abstract class ContactInfo
	{
		internal virtual IADOrgPerson ADOrgPerson
		{
			get
			{
				return null;
			}
		}

		internal virtual UMDialPlan DialPlan
		{
			get
			{
				return null;
			}
		}

		internal abstract string DisplayName { get; }

		internal abstract string FirstName { get; }

		internal abstract string LastName { get; }

		internal abstract string Title { get; }

		internal abstract string Company { get; }

		internal abstract string HomePhone { get; set; }

		internal abstract string MobilePhone { get; set; }

		internal abstract string FaxNumber { get; }

		internal abstract string BusinessPhone { get; set; }

		internal virtual string Extension
		{
			get
			{
				return null;
			}
		}

		internal virtual string SipLine
		{
			get
			{
				return null;
			}
		}

		internal abstract string EMailAddress { get; }

		internal abstract string IMAddress { get; }

		internal abstract FoundByType FoundBy { get; }

		internal abstract string Id { get; }

		internal abstract string EwsId { get; }

		internal abstract string EwsType { get; }

		internal abstract string City { get; }

		internal abstract string Country { get; }

		internal virtual bool ResolvesToMultipleContacts
		{
			get
			{
				return false;
			}
		}

		internal abstract ICollection<string> SanitizedPhoneNumbers { get; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[]
			{
				this.DisplayName,
				base.GetType().Name
			});
		}

		internal static ContactInfo FindByParticipant(UMSubscriber currentUser, Participant participant)
		{
			ContactInfo result = null;
			try
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(currentUser);
				ADRecipient adrecipient = iadrecipientLookup.LookupByParticipant(participant);
				IADOrgPerson iadorgPerson = null;
				if (adrecipient != null)
				{
					iadorgPerson = (adrecipient as IADOrgPerson);
				}
				if (iadorgPerson != null)
				{
					result = new ADContactInfo(iadorgPerson);
				}
				else
				{
					result = ContactInfo.Find(currentUser, participant);
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.UtilTracer, 0, "FindByParticipant: {0}", new object[]
				{
					ex
				});
				throw;
			}
			return result;
		}

		internal static PersonalContactInfo Find(UMSubscriber currentUser, Participant participant)
		{
			PersonalContactInfo result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = currentUser.CreateSessionLock())
			{
				using (ContactsFolder contactsFolder = ContactsFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Contacts)))
				{
					using (FindInfo<Contact> findInfo = contactsFolder.FindByEmailAddress(participant.EmailAddress, PersonalContactInfo.ContactPropertyDefinitions))
					{
						if (findInfo.FindStatus != FindStatus.NotFound && findInfo.Result != null)
						{
							return new PersonalContactInfo(mailboxSessionLock.Session.MailboxOwner.MailboxInfo.MailboxGuid, findInfo.Result);
						}
					}
					result = null;
				}
			}
			return result;
		}

		internal static ContactInfo FindContactByCallerId(UMSubscriber calledUser, PhoneNumber callerId)
		{
			ContactInfo contactInfo = null;
			try
			{
				if (calledUser != null && !PhoneNumber.IsNullOrEmpty(callerId))
				{
					contactInfo = ADContactInfo.FindCallerByCallerId(calledUser, callerId);
					if (contactInfo == null)
					{
						contactInfo = PersonalContactInfo.FindContactByCallerId(calledUser, callerId);
					}
				}
			}
			catch (LocalizedException ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToResolveVoicemailCaller, null, new object[]
				{
					callerId,
					(calledUser != null && calledUser.MailAddress != null) ? calledUser.MailAddress : string.Empty,
					ex.Message
				});
				CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "FindContactByCallerId: {0}", new object[]
				{
					ex
				});
				throw;
			}
			return contactInfo;
		}

		internal virtual Participant CreateParticipant(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			string text = this.DisplayName ?? string.Empty;
			string text2 = this.EMailAddress ?? string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				text = text2;
				if (string.IsNullOrEmpty(text))
				{
					text = MessageContentBuilder.FormatCallerId(callerId, cultureInfo);
				}
			}
			return new Participant(text, text2, "SMTP");
		}

		internal virtual LocalizedString GetVoicemailBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return callerId.IsEmpty ? Strings.VoiceMailBodyCallerResolvedNoCallerId(AntiXssEncoder.HtmlEncode(this.DisplayName, false)) : this.GetVoicemailBodyCallerResolvedLabel(callerId, cultureInfo);
		}

		internal virtual LocalizedString GetMissedCallBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(this);
			string senderName = AntiXssEncoder.HtmlEncode(this.DisplayName, false);
			string senderPhone = MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo);
			return phoneLabel.IsEmpty ? Strings.MissedCallBodyCallerResolvedNoPhoneLabel(senderName, senderPhone) : Strings.MissedCallBodyCallerResolved(senderName, senderPhone, phoneLabel);
		}

		internal virtual LocalizedString GetFaxBodyDisplayLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(this);
			string senderName = AntiXssEncoder.HtmlEncode(this.DisplayName, false);
			string senderPhone = MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo);
			return phoneLabel.IsEmpty ? Strings.FaxBodyCallerResolvedNoPhoneLabel(senderName, senderPhone) : Strings.FaxBodyCallerResolved(senderName, senderPhone, phoneLabel);
		}

		private LocalizedString GetVoicemailBodyCallerResolvedLabel(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(this);
			string senderName = AntiXssEncoder.HtmlEncode(this.DisplayName, false);
			string senderPhone = MessageContentBuilder.FormatCallerIdWithAnchor(callerId, cultureInfo);
			return phoneLabel.IsEmpty ? Strings.VoiceMailBodyCallerResolvedNoPhoneLabel(senderName, senderPhone) : Strings.VoiceMailBodyCallerResolved(senderName, senderPhone, phoneLabel);
		}
	}
}

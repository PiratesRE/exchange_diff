using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UserObject : IEquatable<UserObject>
	{
		internal UserObject(ADRecipient adRecipient)
		{
			UserObject.ThrowOnNullArgument(adRecipient, "adRecipient");
			this.exchangePrincipal = UserObject.GetExchangePrincipalFromAdRecipient(adRecipient);
			this.Participant = new Participant(adRecipient);
			this.Alias = adRecipient.Alias;
			this.recipientType = adRecipient.RecipientType;
			this.IsGroupMailbox = (adRecipient.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
			this.objectId = adRecipient.Id;
			if (this.exchangePrincipal != null)
			{
				this.EmailAddress = this.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
				this.CalendarRepairDisabled = UserObject.IsCalendarRepairDisabled(adRecipient, this.exchangePrincipal);
			}
		}

		internal UserObject(Participant participant, IRecipientSession session)
		{
			UserObject.ThrowOnNullArgument(participant, "participant");
			this.Participant = participant;
			this.EmailAddress = participant.EmailAddress;
			ADRecipient adrecipient;
			if (!participant.TryGetADRecipient(session, out adrecipient))
			{
				Globals.ValidatorTracer.TraceDebug<Participant>(0L, "Unable to get the AD recipient with the specified EmailAddress ({0}).", participant);
			}
			if (adrecipient != null)
			{
				this.exchangePrincipal = UserObject.GetExchangePrincipalFromAdRecipient(adrecipient);
				this.objectId = adrecipient.Id;
				this.recipientType = adrecipient.RecipientType;
				this.IsGroupMailbox = (adrecipient.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
				this.EmailAddress = adrecipient.PrimarySmtpAddress.ToString();
				this.Alias = adrecipient.Alias;
			}
			this.CalendarRepairDisabled = UserObject.IsCalendarRepairDisabled(adrecipient, this.exchangePrincipal);
		}

		internal UserObject(ADRawEntry rawEntry, IRecipientSession session)
		{
			UserObject.ThrowOnNullArgument(rawEntry, "rawEntry");
			this.Participant = new Participant(rawEntry);
			this.recipientType = (RecipientType)rawEntry[ADRecipientSchema.RecipientType];
			this.IsGroupMailbox = ((RecipientTypeDetails)rawEntry[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.GroupMailbox);
			this.EmailAddress = ((SmtpAddress)rawEntry[ADRecipientSchema.PrimarySmtpAddress]).ToString();
			this.Alias = (rawEntry[ADRecipientSchema.Alias] as string);
			this.objectId = rawEntry.Id;
			this.CalendarRepairDisabled = true;
			if (this.recipientType == RecipientType.UserMailbox)
			{
				ADObjectId adobjectId = rawEntry[ADMailboxRecipientSchema.Database] as ADObjectId;
				if (adobjectId != null)
				{
					this.exchangePrincipal = ExchangePrincipal.FromAnyVersionMailboxData((string)rawEntry[ADRecipientSchema.DisplayName], (Guid)rawEntry[ADMailboxRecipientSchema.ExchangeGuid], adobjectId.ObjectGuid, this.EmailAddress, (string)rawEntry[ADRecipientSchema.LegacyExchangeDN], this.objectId, this.recipientType, (SecurityIdentifier)rawEntry[ADRecipientSchema.MasterAccountSid], (OrganizationId)rawEntry[ADObjectSchema.OrganizationId], RemotingOptions.AllowCrossSite, false);
					if (this.exchangePrincipal != null)
					{
						this.CalendarRepairDisabled = (bool)rawEntry[ADUserSchema.CalendarRepairDisabled];
					}
				}
			}
		}

		internal UserObject(Attendee attendee, ADRecipient attendeeRecipient, IRecipientSession session)
		{
			UserObject.ThrowOnNullArgument(attendee, "attendee");
			this.Attendee = attendee;
			if (attendeeRecipient != null)
			{
				this.objectId = attendeeRecipient.Id;
				this.recipientType = attendeeRecipient.RecipientType;
				this.IsGroupMailbox = (attendeeRecipient.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox);
				this.exchangePrincipal = UserObject.GetExchangePrincipalFromAdRecipient(attendeeRecipient);
				this.EmailAddress = attendeeRecipient.PrimarySmtpAddress.ToString();
				this.Alias = attendeeRecipient.Alias;
			}
			else
			{
				this.recipientType = RecipientType.Invalid;
				this.EmailAddress = attendee.Participant.EmailAddress;
				this.Alias = this.EmailAddress;
			}
			this.CalendarRepairDisabled = UserObject.IsCalendarRepairDisabled(attendeeRecipient, this.exchangePrincipal);
		}

		private static ExchangePrincipal GetExchangePrincipalFromAdRecipient(ADRecipient recipient)
		{
			ExchangePrincipal result = null;
			if (recipient is ADUser && recipient.RecipientType == RecipientType.UserMailbox)
			{
				try
				{
					result = ExchangePrincipal.FromADUser(recipient.OrganizationId.ToADSessionSettings(), (ADUser)recipient, RemotingOptions.AllowCrossSite);
				}
				catch (UnableToFindServerForDatabaseException)
				{
					Globals.ValidatorTracer.TraceDebug<ADRecipient>(0L, "Unable to get the ExchangePrincipal for the specified ADRecipient: {0}. Server was not found for Database.", recipient);
				}
			}
			return result;
		}

		private static void ThrowOnNullArgument(object parameter, string parameterName)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		private static bool IsCalendarRepairDisabled(ADRecipient adRecipient, ExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal == null)
			{
				return true;
			}
			ADUser aduser = adRecipient as ADUser;
			return aduser == null || aduser.CalendarRepairDisabled;
		}

		internal void SetResponse(ResponseType response, ExDateTime time)
		{
			this.ResponseType = response;
			this.ReplyTime = time;
		}

		public override string ToString()
		{
			return this.Alias ?? this.EmailAddress;
		}

		internal ADObjectId ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		internal string EmailAddress { get; private set; }

		internal ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.exchangePrincipal;
			}
		}

		internal string Alias { get; private set; }

		internal Participant Participant { get; private set; }

		internal Attendee Attendee
		{
			get
			{
				return this.attendee;
			}
			set
			{
				this.attendee = value;
				this.SetResponse(value.ResponseType, value.ReplyTime);
				this.Participant = value.Participant;
			}
		}

		internal ResponseType ResponseType { get; private set; }

		internal ExDateTime ReplyTime { get; private set; }

		internal RecipientType RecipientType
		{
			get
			{
				return this.recipientType;
			}
		}

		internal bool CalendarRepairDisabled { get; private set; }

		internal bool IsGroupMailbox { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			UserObject userObject = obj as UserObject;
			return userObject != null && this.Equals(userObject);
		}

		public bool Equals(UserObject other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.ObjectId != null)
			{
				return this.ObjectId.Equals(other.ObjectId);
			}
			if (other.ObjectId != null)
			{
				return false;
			}
			if (this.Alias != null)
			{
				return string.Equals(this.Alias, other.Alias, StringComparison.OrdinalIgnoreCase);
			}
			return other.Alias == null && string.Equals(this.EmailAddress, other.EmailAddress, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			if (this.ObjectId != null)
			{
				return this.ObjectId.GetHashCode();
			}
			if (this.Alias != null)
			{
				return this.Alias.GetHashCode();
			}
			return this.EmailAddress.GetHashCode();
		}

		private Attendee attendee;

		private readonly ADObjectId objectId;

		private readonly ExchangePrincipal exchangePrincipal;

		private readonly RecipientType recipientType;
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SmsRecipientInfoCache : IDisposable
	{
		private RecipientInfoCache RecipientInfoCache { get; set; }

		private List<RecipientInfoCacheEntry> CacheEntries { get; set; }

		private MailboxSession MailboxSession { get; set; }

		private Trace Tracer { get; set; }

		private bool IsDirty { get; set; }

		public static SmsRecipientInfoCache Create(MailboxSession mailboxSession, Trace tracer)
		{
			return new SmsRecipientInfoCache(mailboxSession, tracer);
		}

		public SmsRecipientInfoCache(MailboxSession mailboxSession, Trace tracer)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			this.MailboxSession = mailboxSession;
			this.Tracer = tracer;
			this.RecipientInfoCache = RecipientInfoCache.Create(this.MailboxSession, "SMS.RecipientInfoCache");
			try
			{
				this.CacheEntries = this.RecipientInfoCache.Load("AutoCompleteCache");
			}
			catch (CorruptDataException)
			{
				this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The SMS recipient cache is corrupt in {0}'s mailbox", mailboxSession.MailboxOwner.MailboxInfo.DisplayName);
			}
		}

		public RecipientInfoCacheEntry LookUp(string number)
		{
			if (this.CacheEntries == null || this.CacheEntries.Count == 0)
			{
				return null;
			}
			E164Number number2;
			if (!E164Number.TryParse(number, out number2))
			{
				return null;
			}
			foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in this.CacheEntries)
			{
				E164Number number3;
				if (!E164Number.TryParse(recipientInfoCacheEntry.RoutingAddress, out number3))
				{
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "There's an invalid phone number in the SMS recipient cache of {0}'s mailbox", this.MailboxSession.MailboxOwner.MailboxInfo.DisplayName);
				}
				else if (SmsRecipientInfoCache.NumbersMatch(number2, number3))
				{
					return recipientInfoCacheEntry;
				}
			}
			return null;
		}

		public void AddRecipient(Participant recipient)
		{
			if (this.CacheEntries == null)
			{
				this.CacheEntries = new List<RecipientInfoCacheEntry>(150);
			}
			this.IsDirty |= this.AddParticipant(recipient);
		}

		private bool AddParticipant(Participant participant)
		{
			E164Number e164Number;
			if (!E164Number.TryParse(participant.EmailAddress, out e164Number))
			{
				return false;
			}
			for (int i = 0; i < this.CacheEntries.Count; i++)
			{
				RecipientInfoCacheEntry recipientInfoCacheEntry = this.CacheEntries[i];
				E164Number number;
				if (!E164Number.TryParse(recipientInfoCacheEntry.RoutingAddress, out number))
				{
					this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "There's an invalid phone number in the SMS recipient cache of {0}'s mailbox", this.MailboxSession.MailboxOwner.MailboxInfo.DisplayName);
				}
				else if (SmsRecipientInfoCache.NumbersMatch(e164Number, number))
				{
					this.CacheEntries[i] = SmsRecipientInfoCache.CreateCacheEntry(participant, e164Number.Number);
					return true;
				}
			}
			if (this.CacheEntries.Count < 150)
			{
				this.CacheEntries.Add(SmsRecipientInfoCache.CreateCacheEntry(participant, e164Number.Number));
				return true;
			}
			int index = 0;
			for (int j = 1; j < this.CacheEntries.Count; j++)
			{
				if (this.CacheEntries[j].DateTimeTicks < this.CacheEntries[index].DateTimeTicks)
				{
					index = j;
				}
			}
			this.CacheEntries[index] = SmsRecipientInfoCache.CreateCacheEntry(participant, e164Number.Number);
			return true;
		}

		private static RecipientInfoCacheEntry CreateCacheEntry(Participant participant, string number)
		{
			ParticipantOrigin origin = participant.Origin;
			return new RecipientInfoCacheEntry(participant.DisplayName, null, number, null, participant.RoutingType, AddressOrigin.OneOff, 0, null, EmailAddressIndex.None, null, number);
		}

		private static bool NumbersMatch(E164Number number1, E164Number number2)
		{
			return number1 == number2 || ((string.IsNullOrEmpty(number1.CountryCode) || string.IsNullOrEmpty(number2.CountryCode)) && string.Equals(number1.SignificantNumber, number2.SignificantNumber, StringComparison.OrdinalIgnoreCase));
		}

		private void Dispose(bool disposing)
		{
			if (this.RecipientInfoCache != null)
			{
				this.RecipientInfoCache.Dispose();
				this.RecipientInfoCache = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Commit()
		{
			if (this.IsDirty)
			{
				this.RecipientInfoCache.Save(this.CacheEntries, "AutoCompleteCache", 150);
			}
		}

		private const string ConfigurationName = "SMS.RecipientInfoCache";

		private const short CacheSize = 150;
	}
}

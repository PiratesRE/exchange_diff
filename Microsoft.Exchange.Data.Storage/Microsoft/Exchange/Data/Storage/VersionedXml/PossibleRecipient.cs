using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class PossibleRecipient
	{
		internal static PossibleRecipient GetMathed(IEnumerable<PossibleRecipient> candidates, E164Number number, bool snOnly)
		{
			if (candidates == null)
			{
				throw new ArgumentNullException("candidates");
			}
			if (null == number)
			{
				throw new ArgumentNullException("number");
			}
			foreach (PossibleRecipient possibleRecipient in candidates)
			{
				if (possibleRecipient.Ready && E164Number.Equals(possibleRecipient.PhoneNumber, number, snOnly))
				{
					return possibleRecipient;
				}
			}
			return null;
		}

		internal static IList<PossibleRecipient> GetCandidates(IList<PossibleRecipient> recipients, bool effective)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			List<PossibleRecipient> list = new List<PossibleRecipient>(recipients.Count);
			foreach (PossibleRecipient possibleRecipient in recipients)
			{
				if (possibleRecipient.Ready && effective == possibleRecipient.Effective)
				{
					list.Add(possibleRecipient);
				}
			}
			return new ReadOnlyCollection<PossibleRecipient>(list);
		}

		internal static void MarkEffective(IEnumerable<PossibleRecipient> recipients, bool effective)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			foreach (PossibleRecipient possibleRecipient in recipients)
			{
				possibleRecipient.MarkEffective(effective);
			}
		}

		internal static int CountTimesSince(List<DateTime> history, DateTime time, bool clearBefore)
		{
			history.Sort();
			int num = history.BinarySearch(time);
			if (0 > num)
			{
				num = -num - 1;
			}
			int result = history.Count - num;
			if (clearBefore)
			{
				history.RemoveRange(0, num);
			}
			return result;
		}

		internal static void PurgeNonEffectiveBefore(List<PossibleRecipient> recipients, DateTime time, int keptAtMost)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			recipients.Sort(delegate(PossibleRecipient a, PossibleRecipient b)
			{
				if (a.Effective == b.Effective)
				{
					return -DateTime.Compare(a.EffectiveLastModificationTime, b.EffectiveLastModificationTime);
				}
				if (!a.Effective)
				{
					return 1;
				}
				return -1;
			});
			int num = 0;
			int num2 = recipients.Count - 1;
			while (0 <= num2 && !recipients[num2].Effective)
			{
				if (recipients[num2].EffectiveLastModificationTime < time)
				{
					recipients.RemoveAt(num2);
				}
				else
				{
					num++;
				}
				num2--;
			}
			if (num > keptAtMost)
			{
				int num3 = num - keptAtMost;
				recipients.RemoveRange(recipients.Count - 1 - num3, num3);
			}
		}

		internal void MarkEffective(bool effective)
		{
			if (this.Effective != effective)
			{
				this.Effective = effective;
				this.EffectiveLastModificationTime = DateTime.UtcNow;
			}
		}

		public PossibleRecipient()
		{
		}

		public PossibleRecipient(bool effective, DateTime effectiveLastModificationTime, string region, string carrier, E164Number phonenumber, bool acknowledged, string passcode, List<DateTime> passcodeSentTimeHistory, List<DateTime> passcodeVerificationFailedTimeHistory)
		{
			this.Effective = effective;
			this.EffectiveLastModificationTime = effectiveLastModificationTime;
			this.Region = region;
			this.Carrier = carrier;
			this.PhoneNumber = phonenumber;
			this.Acknowledged = acknowledged;
			this.Passcode = passcode;
			this.PasscodeSentTimeHistory = passcodeSentTimeHistory;
			this.PasscodeVerificationFailedTimeHistory = passcodeVerificationFailedTimeHistory;
		}

		[XmlElement("Effective")]
		public bool Effective { get; set; }

		[XmlElement("EffectiveLastModificationTime")]
		public DateTime EffectiveLastModificationTime { get; set; }

		[XmlElement("Region")]
		public string Region { get; set; }

		[XmlElement("Carrier")]
		public string Carrier { get; set; }

		[XmlElement("PhoneNumber")]
		public E164Number PhoneNumber { get; set; }

		[XmlElement("PhoneNumberSetTime")]
		public DateTime PhoneNumberSetTime { get; set; }

		[XmlElement("Acknowledged")]
		public bool Acknowledged { get; set; }

		[XmlElement("Passcode")]
		public string Passcode { get; set; }

		[XmlArray("PasscodeSentTimeHistory")]
		[XmlArrayItem("SentTime")]
		public List<DateTime> PasscodeSentTimeHistory
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<DateTime>(ref this.passcodeSentTimeHistory);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<DateTime>(ref this.passcodeSentTimeHistory, value);
			}
		}

		[XmlArrayItem("FailedTime")]
		[XmlArray("PasscodeVerificationFailedTimeHistory")]
		public List<DateTime> PasscodeVerificationFailedTimeHistory
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<DateTime>(ref this.passcodeVerificationFailedTimeHistory);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<DateTime>(ref this.passcodeVerificationFailedTimeHistory, value);
			}
		}

		[XmlIgnore]
		public bool Ready
		{
			get
			{
				return !string.IsNullOrEmpty(this.Region) && !string.IsNullOrEmpty(this.Carrier) && null != this.PhoneNumber && !string.IsNullOrEmpty(this.PhoneNumber.Number);
			}
		}

		internal void SetPhonenumber(E164Number number)
		{
			if (this.PhoneNumber != number)
			{
				this.Acknowledged = false;
				this.SetPasscode(null);
			}
			this.PhoneNumber = number;
		}

		internal void SetPasscode(string passcode)
		{
			this.Passcode = passcode;
			this.PasscodeVerificationFailedTimeHistory.Clear();
		}

		internal void SetAcknowledged(bool acknowledged)
		{
			this.Acknowledged = acknowledged;
			this.PasscodeVerificationFailedTimeHistory.Clear();
		}

		private List<DateTime> passcodeSentTimeHistory;

		private List<DateTime> passcodeVerificationFailedTimeHistory;
	}
}

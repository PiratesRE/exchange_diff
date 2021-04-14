using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SharingRuleEntry
	{
		public SharingRuleEntry(SharingPolicyDomain policyDomain)
		{
			if (policyDomain == null)
			{
				throw new ArgumentNullException("policyDomain");
			}
			this.IsAnyDomain = ("*" == policyDomain.Domain);
			if (!this.IsAnyDomain)
			{
				this.Domain = policyDomain.Domain;
			}
			this.IsCalendarSharing = ((SharingPolicyAction)0 != (~SharingPolicyAction.ContactsSharing & policyDomain.Actions));
			this.CalendarSharing = (this.IsCalendarSharing ? (~SharingPolicyAction.ContactsSharing & policyDomain.Actions).ToString() : SharingPolicyAction.CalendarSharingFreeBusySimple.ToString());
			this.IsContactsSharing = ((SharingPolicyAction)0 != (policyDomain.Actions & SharingPolicyAction.ContactsSharing));
		}

		[DataMember]
		public bool IsAnyDomain { get; private set; }

		[DataMember]
		public string Domain { get; private set; }

		[DataMember]
		public string FormattedDomain
		{
			get
			{
				if (!this.IsAnyDomain)
				{
					return this.Domain;
				}
				return Strings.SharingDomainOptionAll;
			}
			private set
			{
			}
		}

		[DataMember]
		public bool IsCalendarSharing { get; private set; }

		[DataMember]
		public string CalendarSharing { get; private set; }

		[DataMember]
		public bool IsContactsSharing { get; private set; }

		public static explicit operator SharingPolicyDomain(SharingRuleEntry ruleEntry)
		{
			if (ruleEntry != null)
			{
				string domain = ruleEntry.IsAnyDomain ? "*" : ruleEntry.Domain;
				SharingPolicyAction sharingPolicyAction = (SharingPolicyAction)0;
				if (ruleEntry.IsCalendarSharing)
				{
					sharingPolicyAction = (SharingPolicyAction)Enum.Parse(typeof(SharingPolicyAction), ruleEntry.CalendarSharing);
				}
				if (ruleEntry.IsContactsSharing)
				{
					sharingPolicyAction |= SharingPolicyAction.ContactsSharing;
				}
				return new SharingPolicyDomain(domain, sharingPolicyAction);
			}
			return null;
		}

		public static explicit operator SharingRuleEntry(SharingPolicyDomain policyDomain)
		{
			if (policyDomain != null)
			{
				return new SharingRuleEntry(policyDomain);
			}
			return null;
		}
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class SharingPolicy : ADConfigurationObject
	{
		public MultiValuedProperty<SharingPolicyDomain> Domains
		{
			get
			{
				return (MultiValuedProperty<SharingPolicyDomain>)this[SharingPolicySchema.Domains];
			}
			set
			{
				this[SharingPolicySchema.Domains] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[SharingPolicySchema.Enabled];
			}
			set
			{
				this[SharingPolicySchema.Enabled] = value;
			}
		}

		public bool Default
		{
			get
			{
				return (bool)this[SharingPolicySchema.Default];
			}
			set
			{
				this[SharingPolicySchema.Default] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SharingPolicy.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchSharingPolicy";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return FederatedOrganizationId.Container;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		internal bool IsAllowedForAnySharing(string domain, SharingPolicyAction actions)
		{
			if (this.Enabled)
			{
				foreach (SharingPolicyDomain sharingPolicyDomain in this.Domains)
				{
					if ((sharingPolicyDomain.Actions & actions) != (SharingPolicyAction)0 && SharingPolicy.IsDomainMatch(sharingPolicyDomain.Domain, domain))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal SharingPolicyAction GetAllowed(string domain)
		{
			SharingPolicyAction sharingPolicyAction = (SharingPolicyAction)0;
			if (this.Enabled)
			{
				foreach (SharingPolicyDomain sharingPolicyDomain in this.Domains)
				{
					if (SharingPolicy.IsDomainMatch(sharingPolicyDomain.Domain, domain))
					{
						sharingPolicyAction |= sharingPolicyDomain.Actions;
					}
				}
			}
			return sharingPolicyAction;
		}

		internal bool IsAllowedForAnonymousCalendarSharing()
		{
			return this.GetAllowedForAnonymousCalendarSharing() != (SharingPolicyAction)0;
		}

		internal bool IsAllowedForAnonymousMeetingSharing()
		{
			return this.GetAllowedForAnonymousMeetingSharing() != (SharingPolicyAction)0;
		}

		internal bool IsAllowedForAnyAnonymousFeature()
		{
			return this.IsAllowedForAnonymousMeetingSharing() || this.IsAllowedForAnonymousCalendarSharing();
		}

		internal SharingPolicyAction GetAllowedForAnonymousCalendarSharing()
		{
			SharingPolicyAction sharingPolicyAction = (SharingPolicyAction)0;
			if (this.Enabled)
			{
				foreach (SharingPolicyDomain sharingPolicyDomain in this.Domains)
				{
					if (sharingPolicyDomain.Domain == "Anonymous")
					{
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.CalendarSharingFreeBusyDetail) == SharingPolicyAction.CalendarSharingFreeBusyDetail)
						{
							sharingPolicyAction |= SharingPolicyAction.CalendarSharingFreeBusyDetail;
						}
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.CalendarSharingFreeBusySimple) == SharingPolicyAction.CalendarSharingFreeBusySimple)
						{
							sharingPolicyAction |= SharingPolicyAction.CalendarSharingFreeBusySimple;
						}
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.CalendarSharingFreeBusyReviewer) == SharingPolicyAction.CalendarSharingFreeBusyReviewer)
						{
							sharingPolicyAction |= SharingPolicyAction.CalendarSharingFreeBusyReviewer;
							break;
						}
						break;
					}
				}
			}
			return sharingPolicyAction;
		}

		internal SharingPolicyAction GetAllowedForAnonymousMeetingSharing()
		{
			SharingPolicyAction sharingPolicyAction = (SharingPolicyAction)0;
			if (this.Enabled)
			{
				foreach (SharingPolicyDomain sharingPolicyDomain in this.Domains)
				{
					if (sharingPolicyDomain.Domain == "Anonymous")
					{
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.MeetingFullDetails) == SharingPolicyAction.MeetingFullDetails)
						{
							sharingPolicyAction |= SharingPolicyAction.MeetingFullDetails;
						}
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.MeetingFullDetailsWithAttendees) == SharingPolicyAction.MeetingFullDetailsWithAttendees)
						{
							sharingPolicyAction |= SharingPolicyAction.MeetingFullDetailsWithAttendees;
						}
						if ((sharingPolicyDomain.Actions & SharingPolicyAction.MeetingLimitedDetails) == SharingPolicyAction.MeetingLimitedDetails)
						{
							sharingPolicyAction |= SharingPolicyAction.MeetingLimitedDetails;
							break;
						}
						break;
					}
				}
			}
			return sharingPolicyAction;
		}

		private static bool IsDomainMatch(string rule, string domain)
		{
			return rule == "*" || StringComparer.OrdinalIgnoreCase.Equals(rule, domain);
		}

		internal const string TaskNoun = "SharingPolicy";

		internal const string LdapName = "msExchSharingPolicy";

		private static readonly SharingPolicySchema SchemaObject = ObjectSchema.GetInstance<SharingPolicySchema>();
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal class ADContactInfo : ContactInfo
	{
		internal ADContactInfo(IADOrgPerson orgPerson)
		{
			this.orgPerson = orgPerson;
			this.mobilePhone = Utils.TrimSpaces(orgPerson.MobilePhone);
			this.homePhone = Utils.TrimSpaces(orgPerson.HomePhone);
			this.title = Utils.TrimSpaces(orgPerson.Title);
			this.companyName = Utils.TrimSpaces(orgPerson.Company);
			this.displayName = (Utils.TrimSpaces(orgPerson.DisplayName) ?? (Utils.TrimSpaces(orgPerson.FirstName) + " " + Utils.TrimSpaces(orgPerson.LastName)));
			ExAssert.RetailAssert(orgPerson is ADRecipient, "Class {0} implements IADOrgPerson doesn't inherit from ADRecipient", new object[]
			{
				orgPerson.GetType().FullName
			});
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(orgPerson as ADRecipient))
			{
				if (umsubscriber != null)
				{
					this.dialPlan = umsubscriber.DialPlan;
					this.extension = umsubscriber.Extension;
					if (string.IsNullOrEmpty(this.businessPhone))
					{
						this.businessPhone = umsubscriber.Extension;
					}
				}
			}
			this.businessPhone = (Utils.TrimSpaces(this.orgPerson.Phone) ?? this.extension);
			foreach (string text in orgPerson.EmailAddresses.ToStringArray())
			{
				if (text.StartsWith("SIP:", true, CultureInfo.InvariantCulture))
				{
					this.imaddress = text;
					break;
				}
			}
			this.exchangeLegacyDN = orgPerson.LegacyExchangeDN;
		}

		internal ADContactInfo(IADOrgPerson orgPerson, FoundByType foundBy) : this(orgPerson)
		{
			this.foundBy = foundBy;
		}

		internal ADContactInfo(IADOrgPerson orgPerson, UMDialPlan dialPlan, PhoneNumber callerId) : this(orgPerson)
		{
			this.SetFoundbyType(dialPlan, callerId);
		}

		internal override IADOrgPerson ADOrgPerson
		{
			get
			{
				return this.orgPerson;
			}
		}

		internal override UMDialPlan DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		internal override string Title
		{
			get
			{
				return this.title;
			}
		}

		internal override string Company
		{
			get
			{
				return this.companyName;
			}
		}

		internal override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal override string FirstName
		{
			get
			{
				return this.orgPerson.FirstName;
			}
		}

		internal override string LastName
		{
			get
			{
				return this.orgPerson.LastName;
			}
		}

		internal override string BusinessPhone
		{
			get
			{
				return this.businessPhone;
			}
			set
			{
				this.businessPhone = value;
			}
		}

		internal override string MobilePhone
		{
			get
			{
				return this.mobilePhone;
			}
			set
			{
				this.mobilePhone = value;
			}
		}

		internal override string FaxNumber
		{
			get
			{
				return this.orgPerson.Fax;
			}
		}

		internal override string HomePhone
		{
			get
			{
				return this.homePhone;
			}
			set
			{
				this.homePhone = value;
			}
		}

		internal override string Extension
		{
			get
			{
				return this.extension;
			}
		}

		internal override string SipLine
		{
			get
			{
				return this.ADOrgPerson.RtcSipLine;
			}
		}

		internal override string IMAddress
		{
			get
			{
				return this.imaddress;
			}
		}

		internal override string EMailAddress
		{
			get
			{
				return this.orgPerson.PrimarySmtpAddress.ToString();
			}
		}

		internal override FoundByType FoundBy
		{
			get
			{
				return this.foundBy;
			}
		}

		internal string LegacyExchangeDN
		{
			get
			{
				return this.exchangeLegacyDN;
			}
		}

		internal override string Id
		{
			get
			{
				return this.orgPerson.Guid.ToString();
			}
		}

		internal override string EwsId
		{
			get
			{
				return this.orgPerson.PrimarySmtpAddress.ToString();
			}
		}

		internal override string EwsType
		{
			get
			{
				return "Mailbox";
			}
		}

		internal override string City
		{
			get
			{
				return this.orgPerson.City;
			}
		}

		internal override string Country
		{
			get
			{
				if (!(this.orgPerson.CountryOrRegion != null))
				{
					return null;
				}
				return this.orgPerson.CountryOrRegion.Name;
			}
		}

		internal override ICollection<string> SanitizedPhoneNumbers
		{
			get
			{
				return this.orgPerson.SanitizedPhoneNumbers;
			}
		}

		internal static bool TryFindUmSubscriberByCallerId(UMDialPlan dialPlan, PhoneNumber callerId, out ADContactInfo contactInfo)
		{
			contactInfo = null;
			if (dialPlan == null)
			{
				return true;
			}
			try
			{
				contactInfo = ADContactInfo.FindUmSubscriberByCallerId(dialPlan, callerId);
			}
			catch (LocalizedException)
			{
				return false;
			}
			return true;
		}

		internal static ADContactInfo FindUmSubscriberByCallerId(UMDialPlan dialPlan, PhoneNumber callerId)
		{
			if (dialPlan == null)
			{
				return null;
			}
			IADRecipientLookup lookup = ADRecipientLookupFactory.CreateUmProxyAddressLookup(dialPlan);
			return ADContactInfo.FindContactByCallerId(dialPlan, callerId, lookup);
		}

		internal static bool TryFindCallerByCallerId(UMSubscriber umuser, PhoneNumber callerId, out ADContactInfo contactInfo)
		{
			contactInfo = null;
			try
			{
				contactInfo = ADContactInfo.FindCallerByCallerId(umuser, callerId);
				return true;
			}
			catch (LocalizedException)
			{
			}
			return false;
		}

		internal static ADContactInfo FindCallerByCallerId(UMSubscriber umuser, PhoneNumber callerId)
		{
			if (umuser == null || umuser.DialPlan == null)
			{
				return null;
			}
			IADRecipientLookup lookup = ADRecipientLookupFactory.CreateUmProxyAddressLookup(umuser.DialPlan);
			return ADContactInfo.FindContactByCallerId(umuser.DialPlan, callerId, lookup);
		}

		internal override Participant CreateParticipant(PhoneNumber callerId, CultureInfo cultureInfo)
		{
			return new Participant(this.ADOrgPerson as ADRecipient);
		}

		private static ADContactInfo FindContactByCallerId(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			if (callerId == null || PhoneNumber.IsNullOrEmpty(callerId) || dialPlan == null)
			{
				return null;
			}
			ADContactInfo result = null;
			try
			{
				if ((result = ADContactInfo.FindContactByDialPlan(dialPlan, callerId, lookup)) != null)
				{
					return result;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "ADContactInfo::FindContactByCallerId attempting match against additional AD attributes", new object[0]);
				if ((result = ADContactInfo.FindContactBySipProxy(dialPlan, callerId, lookup)) != null)
				{
					return result;
				}
				callerId = callerId.Extend(dialPlan);
				if ((result = ADContactInfo.FindContactByUMCallingLineId(dialPlan, callerId, lookup)) != null)
				{
					return result;
				}
				if ((result = ADContactInfo.FindContactByRtcSipLine(dialPlan, callerId, lookup)) != null)
				{
					return result;
				}
				if ((result = ADContactInfo.FindContactByADHeuristics(dialPlan, callerId, lookup)) != null)
				{
					return result;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "ADContactInfo::FindByUMCallingLineId did not find any matches", new object[0]);
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Ignoring exception while resolving caller e={0}", new object[]
				{
					ex
				});
				result = null;
				throw;
			}
			return result;
		}

		private static ADContactInfo FindContactByDialPlan(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			ADContactInfo adcontactInfo = null;
			IADOrgPerson iadorgPerson = lookup.LookupByExtensionAndEquivalentDialPlan(callerId.ToDial, dialPlan) as IADOrgPerson;
			if (iadorgPerson != null)
			{
				adcontactInfo = new ADContactInfo(iadorgPerson, FoundByType.BusinessPhone);
			}
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, (adcontactInfo == null) ? "<null>" : adcontactInfo.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "ADContactInfo::FindContactInDialPlan returning _UserDisplayName", new object[0]);
			return adcontactInfo;
		}

		private static ADContactInfo FindContactBySipProxy(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			ADContactInfo adcontactInfo = null;
			if (callerId.UriType == UMUriType.SipName)
			{
				IADOrgPerson iadorgPerson = lookup.LookupBySipExtension(callerId.Number) as IADOrgPerson;
				if (iadorgPerson != null)
				{
					adcontactInfo = new ADContactInfo(iadorgPerson, FoundByType.BusinessPhone);
				}
			}
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, (adcontactInfo == null) ? "<null>" : adcontactInfo.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "ADContactInfo::FindContactBySipProxy returning contact = _UserDisplayName", new object[0]);
			return adcontactInfo;
		}

		private static ADContactInfo FindContactByUMCallingLineId(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			ADContactInfo result = null;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.UMCallingLineIds, callerId.ToDial);
			IADOrgPerson iadorgPerson = ADContactInfo.SearchForUniquePerson(filter, lookup);
			if (iadorgPerson != null)
			{
				result = new ADContactInfo(iadorgPerson, FoundByType.BusinessPhone);
			}
			return result;
		}

		private static ADContactInfo FindContactByRtcSipLine(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			ADContactInfo result = null;
			if (callerId.UriType != UMUriType.E164 && callerId.UriType != UMUriType.TelExtn)
			{
				return null;
			}
			QueryFilter filter = ADContactInfo.BuildRtcSipLineFilter(dialPlan, callerId);
			IADOrgPerson iadorgPerson = ADContactInfo.SearchForUniquePerson(filter, lookup);
			if (iadorgPerson != null)
			{
				result = new ADContactInfo(iadorgPerson, FoundByType.BusinessPhone);
			}
			return result;
		}

		private static ADContactInfo FindContactByADHeuristics(UMDialPlan dialPlan, PhoneNumber callerId, IADRecipientLookup lookup)
		{
			ADContactInfo result = null;
			if (!dialPlan.AllowHeuristicADCallingLineIdResolution)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Not considering clid for heuristic match because its not allowed on the dialplan", new object[0]);
				return null;
			}
			if (callerId.UriType != UMUriType.E164 && callerId.UriType != UMUriType.TelExtn)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Not considering clid for heuristic match because its not an E164 or tel extension", new object[0]);
				return null;
			}
			string text = callerId.ToDial;
			int num = text.Length - (text.StartsWith("+", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			if (num < 3)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Not considering clid for heuristic match because it has {0} significant digits.", new object[]
				{
					num
				});
				return null;
			}
			List<string> optionalPrefixes = callerId.GetOptionalPrefixes(dialPlan);
			foreach (string text2 in optionalPrefixes)
			{
				if (text.StartsWith(text2, StringComparison.OrdinalIgnoreCase) && text.Length > text2.Length)
				{
					text = text.Substring(text2.Length);
					break;
				}
			}
			if (UMUriType.TelExtn == dialPlan.URIType && text.Length < dialPlan.NumberOfDigitsInExtension)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Not performing heuristic search because the searchstring length '{0}' is less than extension length '{1}'", new object[]
				{
					text.Length,
					dialPlan.NumberOfDigitsInExtension
				});
				return null;
			}
			QueryFilter filter = new TextFilter(ADRecipientSchema.IndexedPhoneNumbers, text, MatchOptions.Suffix, MatchFlags.Default);
			ADRecipient[] matches = lookup.LookupByQueryFilter(filter);
			IADOrgPerson iadorgPerson = null;
			if (ADContactInfo.TryFindBestHeuristicPhoneMatch(matches, optionalPrefixes, text, dialPlan, out iadorgPerson))
			{
				result = new ADContactInfo(iadorgPerson, dialPlan, callerId);
			}
			return result;
		}

		private static bool TryFindBestHeuristicPhoneMatch(ADRecipient[] matches, List<string> optionalPrefixes, string searchString, UMDialPlan dialPlan, out IADOrgPerson bestPerson)
		{
			bestPerson = null;
			string value = null;
			int num = int.MaxValue;
			if (matches == null || matches.Length == 0)
			{
				return false;
			}
			foreach (ADRecipient adrecipient in matches)
			{
				IADOrgPerson iadorgPerson = (IADOrgPerson)adrecipient;
				foreach (string text in iadorgPerson.SanitizedPhoneNumbers)
				{
					if (text.EndsWith(searchString, StringComparison.OrdinalIgnoreCase))
					{
						string b = string.Empty;
						if (text.Length > searchString.Length)
						{
							b = text.Substring(0, text.Length - searchString.Length);
						}
						int j = 0;
						while (j < optionalPrefixes.Count)
						{
							if (string.Equals(optionalPrefixes[j], b, StringComparison.OrdinalIgnoreCase))
							{
								if (j < num)
								{
									bestPerson = iadorgPerson;
									value = text;
									num = j;
									break;
								}
								if (j == num && bestPerson != null && bestPerson.Guid != iadorgPerson.Guid)
								{
									CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "ADContactInfo found more than one match.  Discarding matches.", new object[0]);
									bestPerson = null;
									value = string.Empty;
									break;
								}
								break;
							}
							else
							{
								j++;
							}
						}
					}
				}
			}
			if (bestPerson == null)
			{
				return false;
			}
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PII, bestPerson.ToString()),
				PIIMessage.Create(PIIType._PII, value)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "ADContactInfo choosing person=PII1, phone=PII2", new object[0]);
			return true;
		}

		private static IADOrgPerson SearchForUniquePerson(QueryFilter filter, IADRecipientLookup lookup)
		{
			IADOrgPerson iadorgPerson = null;
			ADRecipient[] array = lookup.LookupByQueryFilter(filter);
			if (array == null || array.Length == 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "No recipient found for filter'{0}'", new object[]
				{
					filter
				});
			}
			else if (array.Length == 1)
			{
				iadorgPerson = (array[0] as IADOrgPerson);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Non-unique recipient for filter '{0}'", new object[]
				{
					filter
				});
			}
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, (iadorgPerson == null) ? "<null>" : iadorgPerson.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "ADContactInfo::SearchForUniquePerson returning person = _UserDisplayName", new object[0]);
			return iadorgPerson;
		}

		private static QueryFilter BuildRtcSipLineFilter(UMDialPlan dialPlan, PhoneNumber callerId)
		{
			string text = "tel:";
			if (UMUriType.E164 == callerId.UriType || callerId.ToDial.StartsWith("+"))
			{
				text += callerId.ToDial;
			}
			else if (!string.IsNullOrEmpty(dialPlan.CountryOrRegionCode) && !callerId.ToDial.StartsWith(dialPlan.CountryOrRegionCode, StringComparison.OrdinalIgnoreCase))
			{
				text = text + "+" + dialPlan.CountryOrRegionCode + callerId.Number;
			}
			else
			{
				text = text + "+" + callerId.Number;
			}
			QueryFilter queryFilter = new TextFilter(ADOrgPersonSchema.RtcSipLine, text + ";", MatchOptions.Prefix, MatchFlags.IgnoreCase);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.RtcSipLine, text);
			return new OrFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
		}

		private void SetFoundbyType(UMDialPlan dialPlan, PhoneNumber callerId)
		{
			if (callerId.IsMatch(this.BusinessPhone ?? string.Empty, dialPlan))
			{
				this.foundBy = FoundByType.BusinessPhone;
				return;
			}
			if (callerId.IsMatch(this.HomePhone ?? string.Empty, dialPlan))
			{
				this.foundBy = FoundByType.HomePhone;
				return;
			}
			if (callerId.IsMatch(this.MobilePhone ?? string.Empty, dialPlan))
			{
				this.foundBy = FoundByType.MobilePhone;
			}
		}

		private const int MinimumSignificantDigitsForAdSearch = 3;

		private IADOrgPerson orgPerson;

		private UMDialPlan dialPlan;

		private string businessPhone;

		private string mobilePhone;

		private string homePhone;

		private string extension;

		private string imaddress;

		private string title;

		private string displayName;

		private string companyName;

		private string exchangeLegacyDN;

		private FoundByType foundBy;
	}
}

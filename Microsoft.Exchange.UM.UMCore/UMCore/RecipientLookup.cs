using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecipientLookup
	{
		private RecipientLookup(BaseUMCallSession vo)
		{
			this.partialMatches = new List<ContactSearchItem>();
			this.exactMatches = new List<ContactSearchItem>();
			this.rawMatches = new List<ADRecipient>();
			DialPermissionWrapper dialPermissionWrapper = DialPermissionWrapperFactory.Create(vo);
			if (dialPermissionWrapper.ContactScope == DialScopeEnum.AddressList && dialPermissionWrapper.SearchRoot != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::ctor(): SearchRoot: {0}.", new object[]
				{
					dialPermissionWrapper.SearchRoot
				});
				this.lookup = ADRecipientLookupFactory.CreateFromOrganizationId(dialPermissionWrapper.OrganizationId, dialPermissionWrapper.SearchRoot);
				return;
			}
			this.lookup = ADRecipientLookupFactory.CreateFromOrganizationId(dialPermissionWrapper.OrganizationId, null);
		}

		internal int QueryResults
		{
			get
			{
				return this.queryResults;
			}
		}

		internal int TotalMatches
		{
			get
			{
				return this.totalMatches;
			}
		}

		internal List<ContactSearchItem> PartialMatches
		{
			get
			{
				return this.partialMatches;
			}
		}

		internal List<ContactSearchItem> ExactMatches
		{
			get
			{
				return this.exactMatches;
			}
		}

		internal static RecipientLookup Create(BaseUMCallSession vo)
		{
			return new RecipientLookup(vo);
		}

		internal void SetSearchInExistingResults()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::SetSearchInExistingResults().", new object[0]);
			this.searchInExistingResults = true;
			this.searchDomain = new List<ContactSearchItem>();
			this.searchDomain.AddRange(this.exactMatches);
			this.searchDomain.AddRange(this.partialMatches);
		}

		internal int Lookup(string dtmf, SearchMethod mode, bool wildcardsearch, bool filterOptOutUsers, bool anonymousCaller, UMDialPlan targetDialPlan)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::Lookup({0},{1},{2},{3},{4},{5}).", new object[]
			{
				dtmf,
				mode,
				wildcardsearch,
				filterOptOutUsers,
				anonymousCaller,
				(targetDialPlan != null) ? targetDialPlan.DistinguishedName : "<null>"
			});
			this.exactMatches.Clear();
			this.partialMatches.Clear();
			this.rawMatches.Clear();
			this.totalMatches = 0;
			if (!this.searchInExistingResults)
			{
				return this.SearchActiveDirectory(dtmf, mode, wildcardsearch, filterOptOutUsers, anonymousCaller, targetDialPlan);
			}
			return this.SearchInExistingResults(dtmf, wildcardsearch, filterOptOutUsers);
		}

		internal int SearchInExistingResults(string dtmf, bool wildcardsearch, bool filterOptOutUsers)
		{
			string value = "emailAddress:";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			stringBuilder.Append(dtmf);
			string value2 = stringBuilder.ToString();
			foreach (ContactSearchItem contactSearchItem in this.searchDomain)
			{
				if (contactSearchItem.DtmfEmailAlias.StartsWith(value2, StringComparison.OrdinalIgnoreCase))
				{
					if (this.IsEqualDtmf(new MultiValuedProperty<string>(contactSearchItem.DtmfEmailAlias), SearchMethod.EmailAlias, dtmf))
					{
						this.exactMatches.Add(contactSearchItem);
					}
					else
					{
						this.partialMatches.Add(contactSearchItem);
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::SearchInExistingResults() - returning Exact={0} Partial={1}.", new object[]
			{
				this.exactMatches.Count,
				this.partialMatches.Count
			});
			return this.exactMatches.Count + this.partialMatches.Count;
		}

		internal int SearchActiveDirectory(string dtmf, SearchMethod mode, bool wildcardsearch, bool filterOptOutUsers, bool anonymousCaller, UMDialPlan targetDialPlan)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::SearchActiveDirectory({0},{1},{2},{3},{4},{5}).", new object[]
			{
				dtmf,
				mode,
				wildcardsearch,
				filterOptOutUsers,
				anonymousCaller,
				(targetDialPlan != null) ? targetDialPlan.DistinguishedName : "<null>"
			});
			if (wildcardsearch)
			{
				dtmf += "*";
			}
			string mode2 = string.Empty;
			switch (mode)
			{
			case SearchMethod.FirstNameLastName:
				mode2 = "firstNameLastName:";
				break;
			case SearchMethod.LastNameFirstName:
				mode2 = "lastNameFirstName:";
				break;
			case SearchMethod.EmailAlias:
				mode2 = "emailAddress:";
				break;
			default:
				ExAssert.RetailAssert(false, "Unsupported search mode {0}", new object[]
				{
					mode
				});
				break;
			}
			ADRecipient[] array = this.lookup.LookupByDtmfMap(mode2, dtmf, filterOptOutUsers, anonymousCaller, targetDialPlan, Constants.DirectorySearch.MaxResultsToPreprocess + 1);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::SearchActiveDirectory({0}) rawMatches = {1}.", new object[]
			{
				dtmf,
				array.Length
			});
			if (array != null)
			{
				this.rawMatches.AddRange(array);
			}
			this.queryResults = this.rawMatches.Count;
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::SearchActiveDirectory returning #results = {0}.", new object[]
			{
				this.queryResults
			});
			return this.rawMatches.Count;
		}

		internal ADRecipient LookupByExtension(string extension, BaseUMCallSession vo, DirectorySearchPurpose purpose, DialScopeEnum scope)
		{
			extension = extension.TrimEnd(RecipientLookup.nonNumerics);
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, extension);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "RecipientLookup::LookupByExtension(_PhoneNumber,{0},{1}).", new object[]
			{
				purpose,
				scope
			});
			ADRecipient adrecipient = this.lookup.LookupByExtensionAndDialPlan(extension, vo.CurrentCallContext.DialPlan);
			PIIMessage data2 = PIIMessage.Create(PIIType._SmtpAddress, (adrecipient != null) ? adrecipient.PrimarySmtpAddress.ToString() : "<null>");
			if (adrecipient != null && this.SatisfiesPurpose(adrecipient, purpose) && this.IsInScope(adrecipient, vo.CurrentCallContext.DialPlan, scope))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data2, "LookupByExtension: Found user=_SmtpAddress in scope={0} for purpose={1}.", new object[]
				{
					scope,
					purpose
				});
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data2, "Did not find user=_SmtpAddress in scope={0} for purpose={1}.", new object[]
				{
					scope,
					purpose
				});
				adrecipient = null;
			}
			PIIMessage data3 = PIIMessage.Create(PIIType._User, adrecipient);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data3, "LookupByExtension returning recipient=_User.", new object[0]);
			return adrecipient;
		}

		internal void PostProcess(string dtmf, SearchMethod mode, DirectorySearchPurpose purpose)
		{
			if (this.searchInExistingResults)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "RecipientLookup::PostProcess - SearchInExistingResults - returning without doing anything.", new object[0]);
				return;
			}
			List<ContactSearchItem> list = this.FilterByPurpose(this.rawMatches, purpose);
			this.totalMatches = list.Count;
			foreach (ContactSearchItem contactSearchItem in list)
			{
				ADRecipient recipient = contactSearchItem.Recipient;
				PIIMessage data = PIIMessage.Create(PIIType._User, recipient.DisplayName);
				if (this.IsEqualDtmf(recipient.UMDtmfMap, mode, dtmf))
				{
					this.exactMatches.Add(contactSearchItem);
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "ExactMatch: [_User] For Dtmf {0} SearchMode: {1}.", new object[]
					{
						dtmf,
						mode
					});
				}
				else
				{
					this.partialMatches.Add(contactSearchItem);
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "PartialMatch: [_User] For Dtmf {0} SearchMode: {1}.", new object[]
					{
						dtmf,
						mode
					});
				}
			}
			Dictionary<string, ContactSearchItem> dictionary = new Dictionary<string, ContactSearchItem>();
			foreach (ContactSearchItem contactSearchItem2 in list)
			{
				IADOrgPerson iadorgPerson = contactSearchItem2.Recipient as IADOrgPerson;
				if (iadorgPerson != null)
				{
					if (dictionary.ContainsKey(iadorgPerson.DisplayName))
					{
						ContactSearchItem contactSearchItem3 = dictionary[iadorgPerson.DisplayName];
						contactSearchItem3.NeedDisambiguation = true;
						contactSearchItem2.NeedDisambiguation = true;
					}
					else
					{
						dictionary[iadorgPerson.DisplayName] = contactSearchItem2;
					}
				}
			}
		}

		private static bool MapDtmfAttribute(string s, ref SearchMethod mode)
		{
			bool result = false;
			if (string.Compare(s, "firstNameLastName:", true, CultureInfo.InvariantCulture) == 0)
			{
				mode = SearchMethod.FirstNameLastName;
				result = true;
			}
			else if (string.Compare(s, "lastNameFirstName:", true, CultureInfo.InvariantCulture) == 0)
			{
				mode = SearchMethod.LastNameFirstName;
				result = true;
			}
			else if (string.Compare(s, "emailAddress:", true, CultureInfo.InvariantCulture) == 0)
			{
				mode = SearchMethod.EmailAlias;
				result = true;
			}
			return result;
		}

		private static bool AllowedForAnonymousAccess(ADRecipient r)
		{
			bool flag = false;
			switch (r.RecipientType)
			{
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			case RecipientType.Contact:
			case RecipientType.MailContact:
				flag = true;
				break;
			}
			PIIMessage data = PIIMessage.Create(PIIType._User, r.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, data, "AllowedForAnonymousAccess(_User[{0}] returning {1}.", new object[]
			{
				r.RecipientType,
				flag
			});
			return flag;
		}

		private List<ContactSearchItem> FilterByPurpose(List<ADRecipient> results, DirectorySearchPurpose purpose)
		{
			List<ContactSearchItem> list = new List<ContactSearchItem>();
			foreach (ADRecipient r in results)
			{
				if (this.SatisfiesPurpose(r, purpose))
				{
					list.Add(ContactSearchItem.CreateFromRecipient(r));
				}
			}
			return list;
		}

		private bool SatisfiesPurpose(ADRecipient r, DirectorySearchPurpose purpose)
		{
			bool flag = false;
			switch (purpose)
			{
			case DirectorySearchPurpose.Call:
			{
				IADOrgPerson iadorgPerson = r as IADOrgPerson;
				if (iadorgPerson != null)
				{
					if (!string.IsNullOrEmpty(r.UMExtension) || !string.IsNullOrEmpty(iadorgPerson.Phone) || !string.IsNullOrEmpty(iadorgPerson.HomePhone) || !string.IsNullOrEmpty(iadorgPerson.MobilePhone))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SatisfiesPurpose::Either HomePhone,BusinessPhone or MobilePhone is present on ADOrgPerson.", new object[0]);
						flag = true;
					}
				}
				else
				{
					PIIMessage data = PIIMessage.Create(PIIType._User, r.DisplayName);
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "SatisfiesPurpose::Recipient _User is not of type ADOrgPerson.", new object[0]);
				}
				break;
			}
			case DirectorySearchPurpose.SendMessage:
				flag = (SmtpAddress.Empty != r.PrimarySmtpAddress);
				break;
			case DirectorySearchPurpose.Both:
				flag = true;
				break;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SatisfiesPurpose returning: {0}", new object[]
			{
				flag
			});
			return flag;
		}

		private bool IsInScope(ADRecipient r, UMDialPlan targetDP, DialScopeEnum scope)
		{
			bool flag;
			if (scope == DialScopeEnum.DialPlan)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(r);
				UMDialPlan dialPlanFromRecipient = iadsystemConfigurationLookup.GetDialPlanFromRecipient(r);
				PIIMessage data = PIIMessage.Create(PIIType._SmtpAddress, r.PrimarySmtpAddress);
				if (dialPlanFromRecipient == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "IsInScope: User _SmtpAddress has no dialplan info, and Scope is Dialplan, returning false.", new object[0]);
					flag = false;
				}
				else
				{
					flag = Utils.IsIdenticalDialPlan(dialPlanFromRecipient, targetDP);
					CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "IsInScope: Dialplan of User _SmtpAddress {0} originating dialplan", new object[]
					{
						flag ? "matches" : "does not match"
					});
				}
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "IsInScope: DialScope is GlobalAddressList, returning TRUE.", new object[0]);
				flag = true;
			}
			return flag;
		}

		private bool IsEqualDtmf(MultiValuedProperty<string> dtmfMap, SearchMethod mode, string dtmf)
		{
			foreach (string text in dtmfMap)
			{
				int num = text.IndexOf(':');
				if (num != -1)
				{
					string s = text.Substring(0, num + 1);
					string strA = text.Substring(num + 1);
					SearchMethod searchMethod = SearchMethod.None;
					if (RecipientLookup.MapDtmfAttribute(s, ref searchMethod) && searchMethod == mode)
					{
						return string.Compare(strA, dtmf, true, CultureInfo.InvariantCulture) == 0;
					}
				}
			}
			return false;
		}

		private static char[] nonNumerics = "*#".ToCharArray();

		private IADRecipientLookup lookup;

		private List<ContactSearchItem> partialMatches;

		private List<ContactSearchItem> exactMatches;

		private List<ADRecipient> rawMatches;

		private bool searchInExistingResults;

		private List<ContactSearchItem> searchDomain;

		private int queryResults;

		private int totalMatches;
	}
}

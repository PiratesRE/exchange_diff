using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchRecipient
	{
		public SearchRecipient(ADRawEntry entry, ADRawEntry parent = null)
		{
			this.ADEntry = entry;
			this.Parent = parent;
		}

		public static PropertyDefinition[] SearchProperties
		{
			get
			{
				return SearchRecipient.searchProperties;
			}
		}

		public static PropertyDefinition[] DisplayProperties
		{
			get
			{
				return SearchRecipient.displayProperties;
			}
		}

		public static RecipientType[] RecipientTypes
		{
			get
			{
				return SearchRecipient.recipientTypes;
			}
		}

		public static RecipientTypeDetails[] RecipientTypeDetail
		{
			get
			{
				return SearchRecipient.recipientTypesDetails;
			}
		}

		public static RecipientTypeDetails[] GroupRecipientTypeDetail
		{
			get
			{
				return SearchRecipient.groupRecipientTypesDetails;
			}
		}

		public ADRawEntry Parent { get; set; }

		public ADRawEntry ADEntry { get; set; }

		public static bool IsMembershipGroup(ADRawEntry recipient)
		{
			RecipientTypeDetails typeDetails = (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails];
			return SearchRecipient.IsMembershipGroupTypeDetail(typeDetails);
		}

		public static bool IsMembershipGroupTypeDetail(RecipientTypeDetails typeDetails)
		{
			return SearchRecipient.GroupRecipientTypeDetail.Any((RecipientTypeDetails t) => t == typeDetails);
		}

		public static bool IsPublicFolder(ADRawEntry recipient)
		{
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails];
			return recipientTypeDetails == RecipientTypeDetails.PublicFolder;
		}

		public static IEnumerable<QueryFilter> GetRecipientTypeFilter(bool includeGroups = false)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (RecipientTypeDetails recipientTypeDetails in SearchRecipient.RecipientTypeDetail)
			{
				if (recipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, recipientTypeDetails));
				}
			}
			if (includeGroups)
			{
				foreach (RecipientTypeDetails recipientTypeDetails2 in SearchRecipient.GroupRecipientTypeDetail)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, recipientTypeDetails2));
				}
			}
			return list;
		}

		public static QueryFilter GetRecipientTypeSearchFilter(string searchFilter, bool includeGroups = false)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if (!string.IsNullOrEmpty(searchFilter) && (!searchFilter.StartsWith("*") || !searchFilter.EndsWith("*") || searchFilter.Length > 2))
			{
				Guid empty = Guid.Empty;
				if (Guid.TryParse(searchFilter, out empty))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, empty));
				}
				else
				{
					SmtpAddress smtpAddress = new SmtpAddress(searchFilter);
					if (smtpAddress.IsValidAddress)
					{
						list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "SMTP:" + smtpAddress.ToString()));
						list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ExternalEmailAddress, "SMTP:" + smtpAddress.ToString()));
					}
					else
					{
						list.Add(SearchRecipient.CreateComparisonFilter(ADUserSchema.UserPrincipalName, searchFilter));
						list.Add(SearchRecipient.CreateComparisonFilter(ADRecipientSchema.Alias, searchFilter));
						list.Add(SearchRecipient.CreateComparisonFilter(ADUserSchema.FirstName, searchFilter));
						list.Add(SearchRecipient.CreateComparisonFilter(ADUserSchema.LastName, searchFilter));
						list.Add(SearchRecipient.CreateComparisonFilter(ADRecipientSchema.DisplayName, searchFilter));
					}
				}
			}
			QueryFilter queryFilter = SearchRecipient.CombineFilters(SearchRecipient.GetRecipientTypeFilter(includeGroups));
			if (list.Count > 0)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					SearchRecipient.CombineFilters(list)
				});
			}
			return queryFilter;
		}

		public static QueryFilter CombineFilters(IEnumerable<QueryFilter> orFilters)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				new OrFilter(orFilters.ToArray<QueryFilter>())
			});
		}

		public static QueryFilter GetSourceFilter(SearchSource source)
		{
			switch (source.SourceType)
			{
			case SourceType.LegacyExchangeDN:
				if (source.CanBeCrossPremise)
				{
					return new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, source.ReferenceId),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "x500:" + source.ReferenceId)
					});
				}
				return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, source.ReferenceId);
			case SourceType.Recipient:
				return SearchRecipient.SearchRecipientIdParameter.GetFilter(source.ReferenceId);
			case SourceType.MailboxGuid:
			{
				Guid guid;
				if (Guid.TryParse(source.ReferenceId, out guid))
				{
					return new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ExchangeGuid, guid);
				}
				break;
			}
			}
			return null;
		}

		public static bool IsWildcard(string s)
		{
			return !string.IsNullOrEmpty(s) && (s.StartsWith("*") || s.EndsWith("*"));
		}

		public static bool IsSuffixSearchWildcard(string s)
		{
			return !string.IsNullOrEmpty(s) && s.StartsWith("*") && s.Length > 1;
		}

		private static QueryFilter CreateComparisonFilter(ADPropertyDefinition schemaProperty, string searchFilter)
		{
			if (SearchRecipient.IsWildcard(searchFilter))
			{
				string text = searchFilter.Substring(0, searchFilter.Length - 1);
				return new TextFilter(schemaProperty, text, MatchOptions.Prefix, MatchFlags.IgnoreCase);
			}
			return new ComparisonFilter(ComparisonOperator.Equal, schemaProperty, searchFilter);
		}

		private static PropertyDefinition[] searchProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion,
			ADObjectSchema.Id,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientTypeDetails,
			ADUserSchema.ArchiveGuid,
			ADUserSchema.ArchiveDomain,
			ADUserSchema.ArchiveDatabaseRaw,
			ADUserSchema.ArchiveStatus,
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADRecipientSchema.MasterAccountSid,
			ADRecipientSchema.RecipientTypeDetailsValue,
			IADSecurityPrincipalSchema.SamAccountName,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.RawCapabilities,
			ADPublicFolderSchema.EntryId,
			ADRecipientSchema.DefaultPublicFolderMailbox
		};

		private static PropertyDefinition[] displayProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientTypeDetails
		};

		private static RecipientType[] recipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.PublicDatabase
		};

		private static RecipientTypeDetails[] recipientTypesDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailUser,
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.SharedMailbox,
			RecipientTypeDetails.TeamMailbox,
			RecipientTypeDetails.RoomMailbox,
			RecipientTypeDetails.EquipmentMailbox,
			RecipientTypeDetails.PublicFolderMailbox,
			(RecipientTypeDetails)((ulong)int.MinValue),
			RecipientTypeDetails.RemoteRoomMailbox,
			RecipientTypeDetails.RemoteTeamMailbox,
			RecipientTypeDetails.RemoteSharedMailbox,
			RecipientTypeDetails.RemoteEquipmentMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.LinkedRoomMailbox,
			RecipientTypeDetails.LegacyMailbox
		};

		private static RecipientTypeDetails[] groupRecipientTypesDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailUniversalDistributionGroup,
			RecipientTypeDetails.MailUniversalSecurityGroup,
			RecipientTypeDetails.MailNonUniversalGroup,
			RecipientTypeDetails.DynamicDistributionGroup,
			RecipientTypeDetails.UniversalDistributionGroup,
			RecipientTypeDetails.UniversalSecurityGroup,
			RecipientTypeDetails.NonUniversalGroup
		};

		[Serializable]
		private class SearchRecipientIdParameter : RecipientIdParameter
		{
			public static QueryFilter GetFilter(string identity)
			{
				ADObjectId propertyValue;
				if (ADIdParameter.TryResolveCanonicalName(identity, out propertyValue))
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, propertyValue);
				}
				return null;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SearchPeopleInDirectory : FindPeopleImplementation
	{
		public SearchPeopleInDirectory(FindPeopleParameters parameters, OrganizationId organizationId, ADObjectId addressListId, MailboxSession mailboxSession, Dictionary<Guid, IStorePropertyBag> adResults) : base(parameters, null, false)
		{
			ServiceCommandBase.ThrowIfNull(organizationId, "organizationId", "SearchPeopleInDirectory::SearchPeopleInDirectory");
			ServiceCommandBase.ThrowIfNull(addressListId, "addressListId", "SearchPeopleInDirectory::SearchPeopleInDirectory");
			ServiceCommandBase.ThrowIfNull(mailboxSession, "mailboxSession", "SearchPeopleInDirectory::SearchPeopleInDirectory");
			this.organizationId = organizationId;
			this.addressListId = addressListId;
			this.mailboxSession = mailboxSession;
			this.adResults = adResults;
		}

		public override FindPeopleResult Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Persona[] array = this.ExecuteInternal();
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.GalSearchTime, stopwatch.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.GalCount, array.Length);
			return FindPeopleResult.CreateSearchResult(array);
		}

		private Persona[] ExecuteInternal()
		{
			QueryFilter andValidateRestrictionFilter = base.GetAndValidateRestrictionFilter();
			QueryFilter aggregationRestrictionFilter = base.GetAggregationRestrictionFilter();
			QueryFilter queryFilter = this.BuildFilter(andValidateRestrictionFilter, aggregationRestrictionFilter);
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(base.PersonaShape);
			ADPersonToContactConverterSet converterSet = this.GetConverterSet(propertyListForPersonaResponseShape.GetPropertyDefinitions(), andValidateRestrictionFilter, aggregationRestrictionFilter);
			bool flag = false;
			ADRawEntry[] array = this.ExecuteADQuery(queryFilter, converterSet, out flag);
			if (queryFilter != null)
			{
				array = this.FilterData(array, queryFilter, base.MaxRows);
				if (array.Length < base.MaxRows && flag)
				{
					ExTraceGlobals.FindPeopleCallTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "AD query with filter: '{0}' and MaxResults {1} returned {2} raw results.", queryFilter.ToString(), array.Length, base.MaxRows);
				}
			}
			if (array.Length == 0)
			{
				return Array<Persona>.Empty;
			}
			List<Persona> list = new List<Persona>(array.Length);
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (ADRawEntry adObject in array)
			{
				IStorePropertyBag storePropertyBag = converterSet.Convert(adObject);
				if (this.adResults != null)
				{
					this.adResults.Add((Guid)storePropertyBag[ContactSchema.GALLinkID], storePropertyBag);
				}
				Persona persona = Persona.LoadFromADContact(storePropertyBag, this.mailboxSession, converterSet, propertyListForPersonaResponseShape);
				if (persona != null)
				{
					list.Add(persona);
				}
			}
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.GalDataConversion, stopwatch.ElapsedMilliseconds);
			return list.ToArray();
		}

		private string GetAnrQueryString()
		{
			Participant participant;
			if (!Participant.TryParse(base.QueryString, out participant) || participant.ValidationStatus != ParticipantValidationStatus.NoError || participant.RoutingType != "SMTP")
			{
				return base.QueryString;
			}
			return participant.RoutingType + ":" + participant.EmailAddress;
		}

		private ADRawEntry[] FilterData(ADRawEntry[] results, QueryFilter filter, int maxRows)
		{
			List<ADRawEntry> list = new List<ADRawEntry>(Math.Min(results.Length, maxRows));
			foreach (ADRawEntry adrawEntry in results)
			{
				if (EvaluatableFilter.Evaluate(filter, adrawEntry))
				{
					list.Add(adrawEntry);
					if (list.Count == maxRows)
					{
						break;
					}
				}
			}
			return list.ToArray();
		}

		private QueryFilter BuildFilter(QueryFilter restrictionFilter, QueryFilter aggregationRestrictionFilter)
		{
			QueryFilter queryFilter = null;
			if (restrictionFilter != null)
			{
				queryFilter = restrictionFilter.CloneWithPropertyReplacement(SearchPeopleInDirectory.ContactPropertyToADPropertyMap);
			}
			QueryFilter queryFilter2 = null;
			if (aggregationRestrictionFilter != null)
			{
				queryFilter2 = aggregationRestrictionFilter.CloneWithPropertyReplacement(SearchPeopleInDirectory.PersonPropertyToADPropertyMap);
			}
			if (queryFilter == null && queryFilter2 == null)
			{
				return null;
			}
			QueryFilter filter;
			if (queryFilter != null && queryFilter2 != null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}
			else if (queryFilter != null)
			{
				filter = queryFilter;
			}
			else
			{
				filter = queryFilter2;
			}
			return QueryFilter.SimplifyFilter(filter);
		}

		private ADPersonToContactConverterSet GetConverterSet(IEnumerable<PropertyDefinition> properties, QueryFilter restrictionFilter, QueryFilter aggregationRestrictionFilter)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(properties);
			if (aggregationRestrictionFilter != null)
			{
				foreach (PropertyDefinition item in aggregationRestrictionFilter.FilterProperties())
				{
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			IEnumerable<PropertyDefinition> additionalContactProperties = null;
			if (restrictionFilter != null)
			{
				additionalContactProperties = restrictionFilter.FilterProperties();
			}
			return ADPersonToContactConverterSet.FromPersonProperties(list.ToArray(), additionalContactProperties);
		}

		private ADRawEntry[] ExecuteADQuery(QueryFilter filter, ADPersonToContactConverterSet converterSet, out bool mayBeMore)
		{
			string anrQueryString = this.GetAnrQueryString();
			int num = base.MaxRows;
			Persona.GetPropertyListForPersonaResponseShape(base.PersonaShape);
			if (filter != null)
			{
				num = base.MaxRows * 5;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, base.CultureInfo.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(this.organizationId, this.addressListId), 44, "ExecuteADQuery", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\Server\\ServiceCommands\\SearchPeopleInDirectory.cs");
			ADRawEntry[] array = tenantOrRootOrgRecipientSession.FindByANR(anrQueryString, num, SearchPeopleInDirectory.AdSearchSortBy, converterSet.ADProperties);
			mayBeMore = (array.Length == num);
			return array;
		}

		private const int FetchMultiplier = 5;

		private static readonly SortBy AdSearchSortBy = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);

		private static readonly Dictionary<PropertyDefinition, PropertyDefinition> PersonPropertyToADPropertyMap = new Dictionary<PropertyDefinition, PropertyDefinition>
		{
			{
				PersonSchema.EmailAddress,
				ADRecipientSchema.PrimarySmtpAddress
			},
			{
				PersonSchema.PostalAddress,
				ADOrgPersonSchema.StreetAddress
			},
			{
				PersonSchema.IMAddress,
				ADUserSchema.RTCSIPPrimaryUserAddress
			},
			{
				PersonSchema.PersonType,
				ADRecipientSchema.RecipientPersonType
			}
		};

		private static readonly Dictionary<PropertyDefinition, PropertyDefinition> ContactPropertyToADPropertyMap = new Dictionary<PropertyDefinition, PropertyDefinition>
		{
			{
				StoreObjectSchema.DisplayName,
				ADRecipientSchema.DisplayName
			},
			{
				ContactSchema.Surname,
				ADOrgPersonSchema.LastName
			},
			{
				ContactSchema.GivenName,
				ADOrgPersonSchema.FirstName
			},
			{
				ContactSchema.MiddleName,
				ADOrgPersonSchema.FirstName
			},
			{
				ContactSchema.Nickname,
				ADOrgPersonSchema.FirstName
			},
			{
				ContactBaseSchema.FileAs,
				ADRecipientSchema.SimpleDisplayName
			},
			{
				ContactSchema.CompanyName,
				ADOrgPersonSchema.Company
			},
			{
				ContactSchema.Title,
				ADOrgPersonSchema.Title
			},
			{
				ContactSchema.Department,
				ADOrgPersonSchema.Department
			},
			{
				ContactSchema.OfficeLocation,
				ADOrgPersonSchema.Office
			},
			{
				ContactSchema.AssistantName,
				ADRecipientSchema.AssistantName
			},
			{
				ContactSchema.YomiCompany,
				ADRecipientSchema.PhoneticCompany
			},
			{
				ContactSchema.YomiFirstName,
				ADRecipientSchema.PhoneticFirstName
			},
			{
				ContactSchema.YomiLastName,
				ADRecipientSchema.PhoneticLastName
			},
			{
				ContactSchema.OrganizationMainPhone,
				ADOrgPersonSchema.Phone
			},
			{
				ContactSchema.PrimaryTelephoneNumber,
				ADOrgPersonSchema.Phone
			},
			{
				ContactSchema.BusinessPhoneNumber,
				ADOrgPersonSchema.Phone
			},
			{
				ContactSchema.BusinessPhoneNumber2,
				ADOrgPersonSchema.OtherTelephone
			},
			{
				ContactSchema.CarPhone,
				ADOrgPersonSchema.MobilePhone
			},
			{
				ContactSchema.MobilePhone,
				ADOrgPersonSchema.MobilePhone
			},
			{
				ContactSchema.MobilePhone2,
				ADOrgPersonSchema.MobilePhone
			},
			{
				ContactSchema.HomePhone,
				ADOrgPersonSchema.HomePhone
			},
			{
				ContactSchema.HomePhone2,
				ADOrgPersonSchema.OtherHomePhone
			},
			{
				ContactSchema.WorkFax,
				ADOrgPersonSchema.Fax
			},
			{
				ContactSchema.HomeFax,
				ADOrgPersonSchema.Fax
			},
			{
				ContactSchema.OtherFax,
				ADOrgPersonSchema.Fax
			},
			{
				ContactSchema.Email1EmailAddress,
				ADRecipientSchema.PrimarySmtpAddress
			},
			{
				ContactSchema.Email2EmailAddress,
				ADRecipientSchema.PrimarySmtpAddress
			},
			{
				ContactSchema.Email3EmailAddress,
				ADRecipientSchema.PrimarySmtpAddress
			},
			{
				ContactSchema.IMAddress,
				ADUserSchema.RTCSIPPrimaryUserAddress
			},
			{
				ContactSchema.IMAddress2,
				ADUserSchema.RTCSIPPrimaryUserAddress
			},
			{
				ContactSchema.IMAddress3,
				ADUserSchema.RTCSIPPrimaryUserAddress
			},
			{
				ContactSchema.HomePostOfficeBox,
				ADOrgPersonSchema.PostOfficeBox
			},
			{
				ContactSchema.WorkPostOfficeBox,
				ADOrgPersonSchema.PostOfficeBox
			},
			{
				ContactSchema.OtherPostOfficeBox,
				ADOrgPersonSchema.PostOfficeBox
			},
			{
				ContactSchema.HomeStreet,
				ADOrgPersonSchema.StreetAddress
			},
			{
				ContactSchema.WorkAddressStreet,
				ADOrgPersonSchema.StreetAddress
			},
			{
				ContactSchema.OtherStreet,
				ADOrgPersonSchema.StreetAddress
			},
			{
				ContactSchema.HomeCountry,
				ADOrgPersonSchema.CountryOrRegion
			},
			{
				ContactSchema.WorkAddressCountry,
				ADOrgPersonSchema.CountryOrRegion
			},
			{
				ContactSchema.OtherCountry,
				ADOrgPersonSchema.CountryOrRegion
			},
			{
				ContactSchema.HomePostalCode,
				ADOrgPersonSchema.PostalCode
			},
			{
				ContactSchema.WorkAddressPostalCode,
				ADOrgPersonSchema.PostalCode
			},
			{
				ContactSchema.OtherPostalCode,
				ADOrgPersonSchema.PostalCode
			},
			{
				ContactSchema.HomeState,
				ADOrgPersonSchema.StateOrProvince
			},
			{
				ContactSchema.WorkAddressState,
				ADOrgPersonSchema.StateOrProvince
			},
			{
				ContactSchema.OtherState,
				ADOrgPersonSchema.StateOrProvince
			},
			{
				ContactSchema.HomeCity,
				ADOrgPersonSchema.City
			},
			{
				ContactSchema.WorkAddressCity,
				ADOrgPersonSchema.City
			},
			{
				ContactSchema.OtherCity,
				ADOrgPersonSchema.City
			}
		};

		private readonly OrganizationId organizationId;

		private readonly ADObjectId addressListId;

		private readonly MailboxSession mailboxSession;

		private readonly Dictionary<Guid, IStorePropertyBag> adResults;
	}
}

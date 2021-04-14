using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class FindPeopleImplementation
	{
		protected FindPeopleImplementation(FindPeopleParameters parameters, HashSet<PropertyPath> additionalSupportedProperties, bool pagingSupported)
		{
			ServiceCommandBase.ThrowIfNull(parameters.Logger, "logger", "FindPeopleImplementation::FindPeopleImplementation");
			this.parameters = parameters;
			this.additionalSupportedProperties = (additionalSupportedProperties ?? FindPeopleImplementation.EmptyPropertyHashSet);
			if (this.parameters.PersonaShape == null)
			{
				this.parameters.PersonaShape = Persona.DefaultPersonaShape;
			}
			if (this.parameters.QueryString != null && this.parameters.QueryString.Length > FindPeopleConfiguration.MaxQueryStringLength)
			{
				this.parameters.QueryString = this.parameters.QueryString.Substring(0, FindPeopleConfiguration.MaxQueryStringLength);
			}
			this.pagingSupported = pagingSupported;
			this.Log(FindPeopleMetadata.QueryString, this.parameters.QueryString);
		}

		public string QueryString
		{
			get
			{
				return this.parameters.QueryString;
			}
		}

		public SortResults[] SortResults
		{
			get
			{
				return this.parameters.SortResults;
			}
		}

		public BasePagingType Paging
		{
			get
			{
				return this.parameters.Paging;
			}
		}

		public RestrictionType Restriction
		{
			get
			{
				return this.parameters.Restriction;
			}
		}

		public RestrictionType AggregationRestriction
		{
			get
			{
				return this.parameters.AggregationRestriction;
			}
		}

		public PersonaResponseShape PersonaShape
		{
			get
			{
				return this.parameters.PersonaShape;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return this.parameters.CultureInfo;
			}
		}

		public RequestDetailsLogger Logger
		{
			get
			{
				return this.parameters.Logger;
			}
		}

		protected int MaxRows
		{
			get
			{
				int result = FindPeopleConfiguration.MaxRowsDefault;
				if (this.parameters.Paging != null && this.parameters.Paging.MaxRowsSpecified)
				{
					result = this.parameters.Paging.MaxRows;
				}
				return result;
			}
		}

		public abstract FindPeopleResult Execute();

		public virtual void Validate()
		{
			this.ValidatePaging();
			this.ValidatePersonaShape();
		}

		protected QueryFilter GetAndValidateRestrictionFilter()
		{
			QueryFilter queryFilter = this.GetRestrictionFilter();
			if (queryFilter != null)
			{
				this.ValidateSupportedProperties(queryFilter.FilterProperties(), FindPeopleProperties.SupportedRestrictionProperties, "FindPeople Restriction");
				queryFilter = BasePagingType.ApplyQueryAppend(queryFilter, this.Paging);
			}
			return queryFilter;
		}

		protected QueryFilter GetRestrictionFilter()
		{
			QueryFilter result = null;
			if (this.Restriction != null && this.Restriction.Item != null)
			{
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				result = serviceObjectToFilterConverter.Convert(this.Restriction.Item);
			}
			return result;
		}

		protected QueryFilter GetAggregationRestrictionFilter()
		{
			QueryFilter queryFilter = null;
			if (this.AggregationRestriction != null && this.AggregationRestriction.Item != null)
			{
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				queryFilter = serviceObjectToFilterConverter.Convert(this.AggregationRestriction.Item);
				foreach (PropertyDefinition propertyDefinition in queryFilter.FilterProperties())
				{
					if (!PersonSchema.Instance.AllProperties.Contains(propertyDefinition))
					{
						throw new UnsupportedPathForQueryException(propertyDefinition, new NotSupportedException(string.Format("Unsupported aggregation property {0}", propertyDefinition.ToString())));
					}
				}
			}
			return queryFilter;
		}

		protected void Log(FindPeopleMetadata metadata, object value)
		{
			this.Logger.Set(metadata, value);
		}

		protected void ValidateSupportedProperties(IEnumerable<PropertyDefinition> properties, ICollection<PropertyDefinition> supportedProperties, string usage)
		{
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				if (!supportedProperties.Contains(propertyDefinition))
				{
					throw new UnsupportedPathForQueryException(propertyDefinition, new NotSupportedException(string.Format("Unsupported property {0} in {1}", propertyDefinition.ToString(), usage)));
				}
			}
		}

		protected virtual void ValidatePaging()
		{
			BasePagingType.Validate(this.Paging);
			if (this.pagingSupported)
			{
				if (!this.Paging.MaxRowsSpecified)
				{
					throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidIndexedPagingParameters);
				}
			}
			else if (((IndexedPageView)this.Paging).Offset != 0)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidIndexedPagingParameters);
			}
		}

		protected void ValidatePersonaShape()
		{
			if (this.PersonaShape.BaseShape == ShapeEnum.AllProperties)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidShape);
			}
			if (this.PersonaShape.AdditionalProperties != null)
			{
				foreach (PropertyPath item in this.PersonaShape.AdditionalProperties)
				{
					if (!FindPeopleProperties.SupportedRequestProperties.Contains(item) && !this.additionalSupportedProperties.Contains(item))
					{
						throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidShape);
					}
				}
			}
		}

		public static Persona GetPersona(StoreSession storeSession, IStorePropertyBag propertyBag, int unreadCount = 0, PersonType personType = PersonType.Person)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, null);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(StoreObjectSchema.DisplayName, null);
			PersonId valueOrDefault3 = propertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
			EmailAddressWrapper emailAddressWrapper = null;
			EmailAddressWrapper[] emailAddresses = null;
			if (personType == PersonType.DistributionList)
			{
				emailAddressWrapper = new EmailAddressWrapper
				{
					Name = valueOrDefault2,
					RoutingType = "MAPIPDL"
				};
				emailAddresses = new EmailAddressWrapper[]
				{
					emailAddressWrapper
				};
			}
			else if (valueOrDefault != null)
			{
				emailAddressWrapper = new EmailAddressWrapper
				{
					Name = valueOrDefault2,
					EmailAddress = valueOrDefault,
					RoutingType = propertyBag.GetValueOrDefault<string>(ContactSchema.Email1AddrType, null)
				};
				emailAddresses = new EmailAddressWrapper[]
				{
					emailAddressWrapper
				};
			}
			return new Persona
			{
				PersonaId = IdConverter.PersonaIdFromPersonId(storeSession.MailboxGuid, valueOrDefault3),
				DisplayName = valueOrDefault2,
				EmailAddress = emailAddressWrapper,
				EmailAddresses = emailAddresses,
				ImAddress = propertyBag.GetValueOrDefault<string>(ContactSchema.IMAddress, null),
				UnreadCount = unreadCount,
				RelevanceScore = propertyBag.GetValueOrDefault<int>(ContactSchema.RelevanceScore, int.MaxValue),
				PersonaType = personType.ToString("g")
			};
		}

		public static FindPeopleResult QueryContactsInPublicFolder(PublicFolderSession session, Folder folder, SortBy[] sortBy, IndexedPageView paging, QueryFilter queryFilter = null)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("folder", folder);
			Persona[] array = null;
			int estimatedRowCount;
			using (IQueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, sortBy, FindPeopleImplementation.PublicFolderListContactProperties))
			{
				estimatedRowCount = queryResult.EstimatedRowCount;
				queryResult.SeekToOffset(SeekReference.OriginBeginning, paging.Offset);
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(paging.MaxRows);
				array = new Persona[propertyBags.Length];
				for (int i = 0; i < propertyBags.Length; i++)
				{
					PersonType valueOrDefault = propertyBags[i].GetValueOrDefault<PersonType>(ContactSchema.PersonType, PersonType.Person);
					array[i] = FindPeopleImplementation.GetPersona(session, propertyBags[i], 0, valueOrDefault);
					array[i].DisplayNameFirstLast = propertyBags[i].GetValueOrDefault<string>(ContactBaseSchema.DisplayNameFirstLast, null);
					array[i].DisplayNameLastFirst = propertyBags[i].GetValueOrDefault<string>(ContactBaseSchema.DisplayNameLastFirst, null);
				}
			}
			return FindPeopleResult.CreateMailboxBrowseResult(array, estimatedRowCount);
		}

		private static readonly HashSet<PropertyPath> EmptyPropertyHashSet = new HashSet<PropertyPath>();

		private readonly FindPeopleParameters parameters;

		private readonly bool pagingSupported;

		private readonly HashSet<PropertyPath> additionalSupportedProperties;

		private static readonly string PersonaTypePerson = PersonaTypeConverter.ToString(PersonType.Person);

		private static readonly PropertyDefinition[] PublicFolderListContactProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.DisplayName,
			ContactBaseSchema.DisplayNameFirstLast,
			ContactBaseSchema.DisplayNameLastFirst,
			ContactSchema.Email1AddrType,
			ContactSchema.Email1EmailAddress,
			ContactSchema.IMAddress,
			ContactSchema.PersonId,
			ContactSchema.PersonType,
			ContactSchema.RelevanceScore
		};
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class AddressBookBase : ADLegacyVersionableObject, ISupportRecipientFilter, IProvisioningCacheInvalidation
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return "addressBookContainer";
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AddressBookBase.schema;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		internal static object Base64GuidGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(AddressBookBaseSchema.Base64Guid.Name, DirectoryStrings.IdIsNotSet), AddressBookBaseSchema.Base64Guid, propertyBag[ADObjectSchema.Id]));
			}
			return Convert.ToBase64String(adobjectId.ObjectGuid.ToByteArray());
		}

		internal static object ContainerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(AddressBookBaseSchema.Container.Name, DirectoryStrings.IdIsNotSet), AddressBookBaseSchema.Container, propertyBag[ADObjectSchema.Id]));
				}
				ADObjectId adobjectId2 = (ADObjectId)propertyBag[ADObjectSchema.ConfigurationUnit];
				ADObjectId adobjectId3;
				if (adobjectId2 == null)
				{
					adobjectId3 = adobjectId.DescendantDN(6);
					if (adobjectId3.DistinguishedName.Length == adobjectId.DistinguishedName.Length)
					{
						return string.Empty;
					}
				}
				else
				{
					adobjectId3 = adobjectId.DescendantDN(3);
					if (adobjectId3.DistinguishedName.Length != adobjectId2.DistinguishedName.Length)
					{
						adobjectId3 = adobjectId.DescendantDN(6);
					}
				}
				StringBuilder stringBuilder = new StringBuilder("\\");
				for (ADObjectId parent = adobjectId.Parent; parent != adobjectId3; parent = parent.Parent)
				{
					stringBuilder.Insert(0, "\\");
					stringBuilder.Insert(1, parent.Rdn.UnescapedName.Replace("\\", "\\\\"));
				}
				if (stringBuilder.Length > 1)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
				result = stringBuilder.ToString();
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Container", ex.Message), AddressBookBaseSchema.Container, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object PathGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(AddressBookBaseSchema.Path.Name, DirectoryStrings.IdIsNotSet), AddressBookBaseSchema.Path, propertyBag[ADObjectSchema.Id]));
			}
			string text = (string)AddressBookBase.ContainerGetter(propertyBag);
			if (string.IsNullOrEmpty(text))
			{
				return "\\";
			}
			string text2 = adobjectId.Rdn.UnescapedName.Replace("\\", "\\\\");
			if (text.EndsWith("\\"))
			{
				return text + text2;
			}
			return text + "\\" + text2;
		}

		internal static object IsTopContainerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(AddressBookBaseSchema.IsTopContainer.Name, DirectoryStrings.IdIsNotSet), AddressBookBaseSchema.IsTopContainer, propertyBag[ADObjectSchema.Id]));
				}
				ADObjectId adobjectId2 = (ADObjectId)propertyBag[ADObjectSchema.ConfigurationUnit];
				if (adobjectId2 == null)
				{
					result = (adobjectId.DistinguishedName.Length == adobjectId.DescendantDN(6).DistinguishedName.Length);
				}
				else
				{
					result = (adobjectId.DistinguishedName.Equals(adobjectId2.GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization).DistinguishedName, StringComparison.OrdinalIgnoreCase) || adobjectId.DistinguishedName.Equals(adobjectId2.GetDescendantId(AddressList.RdnAlContainerToOrganization).DistinguishedName, StringComparison.OrdinalIgnoreCase) || adobjectId.DistinguishedName.Equals(adobjectId2.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).DistinguishedName, StringComparison.OrdinalIgnoreCase));
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("IsTopContainer", ex.Message), AddressBookBaseSchema.IsTopContainer, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		private static bool IsStartWithRDN(IPropertyBag propertyBag, ADObjectId rDN, ADPropertyDefinition propertyDefinition)
		{
			bool result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(propertyDefinition.Name, DirectoryStrings.IdIsNotSet), propertyDefinition, propertyBag[ADObjectSchema.Id]));
				}
				ADObjectId adobjectId2 = (ADObjectId)propertyBag[ADObjectSchema.ConfigurationUnit];
				if (adobjectId2 == null)
				{
					result = adobjectId.DescendantDN(6).DistinguishedName.StartsWith(rDN.DistinguishedName, StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					result = adobjectId.DistinguishedName.Substring(0, adobjectId.DistinguishedName.Length - adobjectId2.DistinguishedName.Length - 1).EndsWith(rDN.DistinguishedName, StringComparison.OrdinalIgnoreCase);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(propertyDefinition.Name, ex.Message), propertyDefinition, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object IsGlobalAddressListGetter(IPropertyBag propertyBag)
		{
			return AddressBookBase.IsStartWithRDN(propertyBag, GlobalAddressList.RdnGalContainerToOrganization, AddressBookBaseSchema.IsGlobalAddressList);
		}

		internal static object IsInSystemAddressListContainerGetter(IPropertyBag propertyBag)
		{
			return AddressBookBase.IsStartWithRDN(propertyBag, SystemAddressList.RdnSystemAddressListContainerToOrganization, AddressBookBaseSchema.IsInSystemAddressListContainer);
		}

		internal static QueryFilter IsDefaultGlobalAddressListFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2007),
					new BitMaskAndFilter(AddressBookBaseSchema.RecipientFilterFlags, 2UL)
				}),
				new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2003),
					new ExistsFilter(AddressBookBaseSchema.LdapRecipientFilter),
					new NotFilter(new ExistsFilter(AddressBookBaseSchema.PurportedSearchUI))
				})
			});
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static object IsDefaultGlobalAddressListGetter(IPropertyBag propertyBag)
		{
			if (!(bool)AddressBookBase.IsGlobalAddressListGetter(propertyBag))
			{
				return false;
			}
			ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)propertyBag[ADObjectSchema.ExchangeVersion];
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[AddressBookBaseSchema.PurportedSearchUI];
			if (exchangeObjectVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				string value = (string)propertyBag[AddressBookBaseSchema.LdapRecipientFilter];
				return !string.IsNullOrEmpty(value) && 0 == multiValuedProperty.Count;
			}
			return RecipientFilterableObjectFlags.IsDefault == (RecipientFilterableObjectFlags.IsDefault & (RecipientFilterableObjectFlags)propertyBag[AddressBookBaseSchema.RecipientFilterFlags]);
		}

		private static QueryFilter CreateFindFilter(string searchString, AddressBookBase addressBookBase, AddressBookBase.RecipientCategory recipientCategory)
		{
			QueryFilter queryFilter = new AmbiguousNameResolutionFilter(searchString);
			QueryFilter queryFilter2 = null;
			switch (recipientCategory)
			{
			case AddressBookBase.RecipientCategory.People:
				queryFilter2 = AddressBookBase.CategoryFilters.PersonFilter;
				break;
			case AddressBookBase.RecipientCategory.Groups:
				queryFilter2 = AddressBookBase.CategoryFilters.GroupFilter;
				break;
			}
			QueryFilter queryFilter3;
			if (addressBookBase != null)
			{
				queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, addressBookBase.Id);
			}
			else
			{
				queryFilter3 = new ExistsFilter(ADRecipientSchema.AddressListMembership);
			}
			if (queryFilter2 != null && queryFilter3 != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter3,
					queryFilter2,
					queryFilter
				});
			}
			else if (queryFilter2 != null && queryFilter3 == null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					queryFilter
				});
			}
			else if (queryFilter2 == null && queryFilter3 != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter3,
					queryFilter
				});
			}
			return queryFilter;
		}

		private static IRecipientSession GetScopedRecipientSession(ADObjectId rootId, int lcid, string preferredServerName, OrganizationId organizationId)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, rootId, lcid, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false), 1181, "GetScopedRecipientSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\AddressBookBase.cs");
			if (!string.IsNullOrEmpty(preferredServerName))
			{
				tenantOrRootOrgRecipientSession.DomainController = preferredServerName;
			}
			return tenantOrRootOrgRecipientSession;
		}

		private static bool ListObjectModeEnabled()
		{
			bool result;
			lock (AddressBookBase.heuristicsLock)
			{
				if (AddressBookBase.heuristicsNextUpdateTime < DateTime.UtcNow)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1214, "ListObjectModeEnabled", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\AddressBookBase.cs");
					tenantOrTopologyConfigurationSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(10.0));
					ADObjectId descendantId = tenantOrTopologyConfigurationSession.ConfigurationNamingContext.GetDescendantId(NtdsService.ContainerId);
					NtdsService ntdsService = tenantOrTopologyConfigurationSession.Read<NtdsService>(descendantId);
					if (ntdsService != null)
					{
						AddressBookBase.listObjectMode = ntdsService.DoListObject;
					}
					else
					{
						AddressBookBase.listObjectMode = false;
					}
					AddressBookBase.heuristicsNextUpdateTime = DateTime.UtcNow + TimeSpan.FromMinutes(15.0);
				}
				result = AddressBookBase.listObjectMode;
			}
			return result;
		}

		internal IEnumerable<ADRecipient> FindUpdatingRecipientsPaged(IRecipientSession recipientSession, ADObjectId rootId)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			OrganizationId organizationId = base.OrganizationId;
			ADObjectId propertyValue = (base.Guid == Guid.Empty) ? base.Id : new ADObjectId(base.Guid);
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, propertyValue);
			QueryFilter queryFilter2 = null;
			if (!string.IsNullOrEmpty(this.LdapRecipientFilter))
			{
				queryFilter2 = new CustomLdapFilter(this.LdapRecipientFilter);
			}
			if (queryFilter2 == null)
			{
				return recipientSession.FindPaged(rootId, QueryScope.SubTree, queryFilter, null, 0);
			}
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new NotFilter(queryFilter2)
				}),
				new AndFilter(new QueryFilter[]
				{
					new NotFilter(queryFilter),
					queryFilter2
				})
			});
			if (this.RecipientContainer == null || (organizationId.OrganizationalUnit != null && organizationId.OrganizationalUnit.DistinguishedName.Equals(this.RecipientContainer.DistinguishedName, StringComparison.OrdinalIgnoreCase)))
			{
				return recipientSession.FindPaged(rootId, QueryScope.SubTree, filter, null, 0);
			}
			if (rootId == null)
			{
				return this.FindUpdatingRecipientsPaged(recipientSession);
			}
			if (!rootId.DistinguishedName.EndsWith(this.RecipientContainer.DistinguishedName, StringComparison.OrdinalIgnoreCase))
			{
				return recipientSession.FindPaged(rootId, QueryScope.SubTree, queryFilter, null, 0);
			}
			return recipientSession.FindPaged(rootId, QueryScope.SubTree, filter, null, 0);
		}

		private IEnumerable<ADRecipient> FindUpdatingRecipientsPaged(IRecipientSession recipientSession)
		{
			OrganizationId organizationId = base.OrganizationId;
			ADObjectId searchId = (base.Guid == Guid.Empty) ? base.Id : new ADObjectId(base.Guid);
			QueryFilter includedFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, searchId);
			QueryFilter toBeIncludedFilter = new CustomLdapFilter(this.LdapRecipientFilter);
			ADPagedReader<ADRecipient> included = recipientSession.FindPaged(null, QueryScope.SubTree, includedFilter, new SortBy(ADObjectSchema.Guid, SortOrder.Ascending), 0);
			ADPagedReader<ADRecipient> toBeIncluded = recipientSession.FindPaged(this.RecipientContainer, QueryScope.SubTree, toBeIncludedFilter, new SortBy(ADObjectSchema.Guid, SortOrder.Ascending), 0);
			IEnumerator<ADRecipient> includedEnumerator = included.GetEnumerator();
			IEnumerator<ADRecipient> toBeIncludedEnumerator = toBeIncluded.GetEnumerator();
			bool includedAvailable = includedEnumerator.MoveNext();
			bool toBeIncludedAvailable = toBeIncludedEnumerator.MoveNext();
			while (includedAvailable)
			{
				if (!toBeIncludedAvailable)
				{
					break;
				}
				int temp = includedEnumerator.Current.Guid.CompareTo(toBeIncludedEnumerator.Current.Guid);
				if (temp < 0)
				{
					yield return includedEnumerator.Current;
					includedAvailable = includedEnumerator.MoveNext();
				}
				else if (temp > 0)
				{
					yield return toBeIncludedEnumerator.Current;
					toBeIncludedAvailable = toBeIncludedEnumerator.MoveNext();
				}
				else
				{
					includedAvailable = includedEnumerator.MoveNext();
					toBeIncludedAvailable = toBeIncludedEnumerator.MoveNext();
				}
			}
			while (includedAvailable)
			{
				yield return includedEnumerator.Current;
				includedAvailable = includedEnumerator.MoveNext();
			}
			while (toBeIncludedAvailable)
			{
				yield return toBeIncludedEnumerator.Current;
				toBeIncludedAvailable = toBeIncludedEnumerator.MoveNext();
			}
			yield break;
		}

		internal bool CheckForAssociatedAddressBookPolicies()
		{
			QueryFilter filter;
			if (this.IsGlobalAddressList)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
					new ExistsFilter(AddressBookBaseSchema.AssociatedAddressBookPoliciesForGAL)
				});
			}
			else
			{
				filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
					new OrFilter(new QueryFilter[]
					{
						new ExistsFilter(AddressBookBaseSchema.AssociatedAddressBookPoliciesForAddressLists),
						new ExistsFilter(AddressBookBaseSchema.AssociatedAddressBookPoliciesForAllRoomList)
					})
				});
			}
			if (base.Session != null)
			{
				AddressBookBase[] array = base.Session.Find<AddressBookBase>(null, QueryScope.SubTree, filter, null, 1);
				return array != null && array.Length > 0;
			}
			return true;
		}

		[Parameter(Mandatory = false)]
		public string SimpleDisplayName
		{
			get
			{
				return (string)this[AddressBookBaseSchema.SimpleDisplayName];
			}
			set
			{
				this[AddressBookBaseSchema.SimpleDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[AddressBookBaseSchema.DisplayName];
			}
			set
			{
				this[AddressBookBaseSchema.DisplayName] = value;
			}
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[AddressBookBaseSchema.RecipientFilter];
			}
		}

		public string LastUpdatedRecipientFilter
		{
			get
			{
				return (string)this[AddressBookBaseSchema.LastUpdatedRecipientFilter];
			}
		}

		public bool RecipientFilterApplied
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.RecipientFilterApplied];
			}
		}

		public string LdapRecipientFilter
		{
			get
			{
				return (string)this[AddressBookBaseSchema.LdapRecipientFilter];
			}
		}

		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return (WellKnownRecipientType?)this[AddressBookBaseSchema.IncludedRecipients];
			}
			internal set
			{
				this[AddressBookBaseSchema.IncludedRecipients] = value;
			}
		}

		WellKnownRecipientType? ISupportRecipientFilter.IncludedRecipients
		{
			get
			{
				return this.IncludedRecipients;
			}
			set
			{
				this.IncludedRecipients = value;
			}
		}

		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalDepartment];
			}
			internal set
			{
				this[AddressBookBaseSchema.ConditionalDepartment] = value;
			}
		}

		MultiValuedProperty<string> ISupportRecipientFilter.ConditionalDepartment
		{
			get
			{
				return this.ConditionalDepartment;
			}
			set
			{
				this.ConditionalDepartment = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCompany];
			}
			internal set
			{
				this[AddressBookBaseSchema.ConditionalCompany] = value;
			}
		}

		MultiValuedProperty<string> ISupportRecipientFilter.ConditionalCompany
		{
			get
			{
				return this.ConditionalCompany;
			}
			set
			{
				this.ConditionalCompany = value;
			}
		}

		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalStateOrProvince];
			}
			internal set
			{
				this[AddressBookBaseSchema.ConditionalStateOrProvince] = value;
			}
		}

		MultiValuedProperty<string> ISupportRecipientFilter.ConditionalStateOrProvince
		{
			get
			{
				return this.ConditionalStateOrProvince;
			}
			set
			{
				this.ConditionalStateOrProvince = value;
			}
		}

		public WellKnownRecipientFilterType RecipientFilterType
		{
			get
			{
				return (WellKnownRecipientFilterType)this[AddressBookBaseSchema.RecipientFilterType];
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute1];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute1] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute2];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute2] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute3];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute3] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute4];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute4] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute5];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute5] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute6];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute6] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute7];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute7] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute8];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute8] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute9];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute9] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute10];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute10] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute11];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute11] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute12];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute12] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute13];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute13] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute14];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute14] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressBookBaseSchema.ConditionalCustomAttribute15];
			}
			set
			{
				this[AddressBookBaseSchema.ConditionalCustomAttribute15] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return (string)this[AddressBookBaseSchema.Name];
			}
			set
			{
				this[AddressBookBaseSchema.Name] = value;
			}
		}

		public ADObjectId RecipientContainer
		{
			get
			{
				return (ADObjectId)this[AddressBookBaseSchema.RecipientContainer];
			}
			set
			{
				this[AddressBookBaseSchema.RecipientContainer] = value;
			}
		}

		internal void SetRecipientFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				this[AddressBookBaseSchema.RecipientFilter] = string.Empty;
				this[AddressBookBaseSchema.LdapRecipientFilter] = string.Empty;
			}
			else
			{
				this[AddressBookBaseSchema.RecipientFilter] = filter.GenerateInfixString(FilterLanguage.Monad);
				this[AddressBookBaseSchema.LdapRecipientFilter] = LdapFilterBuilder.LdapFilterFromQueryFilter(filter);
			}
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Custom, this.propertyBag, AddressBookBaseSchema.RecipientFilterMetadata);
		}

		public string Base64Guid
		{
			get
			{
				return (string)this[AddressBookBaseSchema.Base64Guid];
			}
		}

		public string Container
		{
			get
			{
				return (string)this[AddressBookBaseSchema.Container];
			}
		}

		public string Path
		{
			get
			{
				return (string)this[AddressBookBaseSchema.Path];
			}
		}

		public bool IsTopContainer
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsTopContainer];
			}
		}

		public int Depth
		{
			get
			{
				return this.depth;
			}
		}

		public bool IsGlobalAddressList
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsGlobalAddressList];
			}
		}

		public bool IsSystemAddressList
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsSystemAddressList];
			}
			internal set
			{
				this[AddressBookBaseSchema.IsSystemAddressList] = value;
			}
		}

		public bool IsModernGroupsAddressList
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsModernGroupsAddressList];
			}
			internal set
			{
				this[AddressBookBaseSchema.IsModernGroupsAddressList] = value;
			}
		}

		public bool IsInSystemAddressListContainer
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsInSystemAddressListContainer];
			}
		}

		public bool IsDefaultGlobalAddressList
		{
			get
			{
				return (bool)this[AddressBookBaseSchema.IsDefaultGlobalAddressList];
			}
		}

		internal override SystemFlagsEnum SystemFlags
		{
			get
			{
				return (SystemFlagsEnum)this[AddressBookBaseSchema.SystemFlags];
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.RecipientFilterSchema
		{
			get
			{
				return AddressBookBaseSchema.RecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.LdapRecipientFilterSchema
		{
			get
			{
				return AddressBookBaseSchema.LdapRecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.IncludedRecipientsSchema
		{
			get
			{
				return AddressBookBaseSchema.IncludedRecipients;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalDepartmentSchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalDepartment;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCompanySchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCompany;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalStateOrProvinceSchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalStateOrProvince;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute1Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute1;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute2Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute2;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute3Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute3;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute4Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute4;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute5Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute5;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute6Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute6;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute7Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute7;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute8Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute8;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute9Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute9;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute10Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute10;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute11Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute11;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute12Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute12;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute13Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute13;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute14Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute14;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute15Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute15;
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			bool flag = false;
			if (base.OrganizationId == null)
			{
				return flag;
			}
			if (base.ObjectState == ObjectState.New || base.ObjectState == ObjectState.Deleted)
			{
				flag = true;
			}
			if (!flag && base.ObjectState == ObjectState.Changed && (base.IsChanged(ADObjectSchema.ExchangeVersion) || base.IsChanged(AddressBookBaseSchema.LdapRecipientFilter) || base.IsChanged(AddressBookBaseSchema.RecipientContainer) || base.IsChanged(AddressBookBaseSchema.IsSystemAddressList)))
			{
				flag = true;
			}
			if (flag)
			{
				orgId = base.OrganizationId;
				keys = new Guid[1];
				keys[0] = CannedProvisioningCacheKeys.AddressBookPolicies;
			}
			return flag;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (!this.IsTopContainer && this.IsSystemAddressList && !this.IsInSystemAddressListContainer)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSystemAddressListInWrongContainer, this.Identity, AddressBookBaseSchema.IsSystemAddressList.Name));
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			ValidationError validationError = RecipientFilterHelper.ValidatePrecannedRecipientFilter(this.propertyBag, AddressBookBaseSchema.RecipientFilterMetadata, AddressBookBaseSchema.RecipientFilter, AddressBookBaseSchema.IncludedRecipients, this.Identity);
			if (validationError != null)
			{
				errors.Add(validationError);
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(AddressBookBaseSchema.RecipientFilterMetadata))
			{
				this.IncludedRecipients = new WellKnownRecipientType?(WellKnownRecipientType.None);
			}
			base.StampPersistableDefaultValues();
		}

		internal object[][] BrowseTo(ref string cookie, int startRange, int itemCount, out int currentRow, params PropertyDefinition[] properties)
		{
			int defaultLcid = LcidMapper.DefaultLcid;
			string empty = string.Empty;
			return this.BrowseTo(ref cookie, null, ref defaultLcid, ref empty, null, startRange, itemCount, out currentRow, false, false, properties);
		}

		internal object[][] BrowseTo(ref string cookie, string seekTo, int itemCount, out int currentRow, params PropertyDefinition[] properties)
		{
			int defaultLcid = LcidMapper.DefaultLcid;
			string empty = string.Empty;
			return this.BrowseTo(ref cookie, null, ref defaultLcid, ref empty, seekTo, 0, itemCount, out currentRow, true, false, properties);
		}

		internal object[][] BrowseTo(ref string cookie, ADObjectId rootId, ref int lcid, ref string preferredServerName, int startRange, int itemCount, out int currentRow, bool isVirtualListView, params PropertyDefinition[] properties)
		{
			return this.BrowseTo(ref cookie, rootId, ref lcid, ref preferredServerName, null, startRange, itemCount, out currentRow, false, isVirtualListView, properties);
		}

		internal object[][] BrowseTo(ref string cookie, ADObjectId rootId, ref int lcid, ref string preferredServerName, string seekTo, int itemCount, out int currentRow, bool isVirtualListView, params PropertyDefinition[] properties)
		{
			return this.BrowseTo(ref cookie, rootId, ref lcid, ref preferredServerName, seekTo, 0, itemCount, out currentRow, true, isVirtualListView, properties);
		}

		private object[][] BrowseTo(ref string cookie, ADObjectId rootId, ref int lcid, ref string preferredServerName, string seekToString, int seekToOffset, int itemCount, out int currentRow, bool seekToCondition, bool isVirtualListView, PropertyDefinition[] properties)
		{
			currentRow = 0;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, rootId, lcid, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.OrganizationId), 2293, "BrowseTo", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\AddressBookBase.cs");
			tenantOrRootOrgRecipientSession.DomainController = preferredServerName;
			ADVirtualListView advirtualListView = (ADVirtualListView)tenantOrRootOrgRecipientSession.Browse(isVirtualListView ? null : base.Id, itemCount, properties);
			if (!string.IsNullOrEmpty(cookie))
			{
				try
				{
					advirtualListView.Cookie = Convert.FromBase64String(cookie);
					cookie = string.Empty;
				}
				catch (FormatException)
				{
				}
			}
			if (seekToCondition)
			{
				TextFilter seekFilter = new TextFilter(ADRecipientSchema.DisplayName, seekToString, MatchOptions.Prefix, MatchFlags.Default);
				advirtualListView.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
			}
			else
			{
				advirtualListView.SeekToOffset(SeekReference.OriginBeginning, seekToOffset);
			}
			object[][] rows = advirtualListView.GetRows(itemCount);
			currentRow = advirtualListView.CurrentRow;
			if (advirtualListView.Cookie != null)
			{
				cookie = Convert.ToBase64String(advirtualListView.Cookie);
			}
			lcid = advirtualListView.Lcid;
			preferredServerName = advirtualListView.PreferredServerName;
			return rows;
		}

		internal static bool IsGlobalAddressListId(ADObjectId alObjectId)
		{
			return alObjectId.DescendantDN(6).DistinguishedName.StartsWith(GlobalAddressList.RdnGalContainerToOrganization.DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		internal static object[][] PagedSearch(ADObjectId rootId, AddressBookBase addressBookBase, OrganizationId organizationId, AddressBookBase.RecipientCategory recipientCategory, string searchString, ref string cookie, int pagesToSkip, int pageSize, out int itemsTouched, ref int lcid, ref string preferredServerName, PropertyDefinition[] properties)
		{
			if (pagesToSkip < 0)
			{
				throw new ArgumentException("pagesToSkip must be greater than 0");
			}
			int itemsToSkip = pagesToSkip * pageSize;
			return AddressBookBase.PagedSearch(rootId, addressBookBase, organizationId, recipientCategory, searchString, itemsToSkip, ref cookie, pageSize, out itemsTouched, ref lcid, ref preferredServerName, properties);
		}

		internal static object[][] PagedSearch(ADObjectId rootId, AddressBookBase addressBookBase, OrganizationId organizationId, AddressBookBase.RecipientCategory recipientCategory, string searchString, int itemsToSkip, ref string cookie, int pageSize, out int itemsTouched, ref int lcid, ref string preferredServerName, PropertyDefinition[] properties)
		{
			itemsTouched = 0;
			if (string.IsNullOrEmpty(searchString))
			{
				throw new ArgumentException("searchString should not be null or empty");
			}
			if (itemsToSkip < 0)
			{
				throw new ArgumentOutOfRangeException("itemsToSkip must be >= 0");
			}
			if (pageSize <= 0 || pageSize > 10000)
			{
				throw new ArgumentOutOfRangeException("pageSize must be greater than 0 and less than " + 10000);
			}
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentException("properties should not be null or empty");
			}
			byte[] cookie2 = new byte[0];
			SortBy sortBy = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);
			ADPagedReader<ADRawEntry> adpagedReader = null;
			List<object[]> list = new List<object[]>();
			QueryFilter filter = AddressBookBase.CreateFindFilter(searchString, addressBookBase, recipientCategory);
			if (!string.IsNullOrEmpty(cookie))
			{
				try
				{
					cookie2 = Convert.FromBase64String(cookie);
					cookie = string.Empty;
				}
				catch (FormatException)
				{
				}
			}
			IRecipientSession scopedRecipientSession = AddressBookBase.GetScopedRecipientSession(rootId, lcid, preferredServerName, organizationId);
			int num = ADGenericPagedReader<ADRawEntry>.DefaultPageSize;
			int i = itemsToSkip + pageSize;
			if (itemsToSkip > 0)
			{
				if (itemsToSkip < num)
				{
					num = itemsToSkip;
				}
				adpagedReader = scopedRecipientSession.FindPagedADRawEntry(rootId, QueryScope.SubTree, filter, sortBy, num, properties);
				adpagedReader.Cookie = cookie2;
				using (IEnumerator<ADRawEntry> enumerator = adpagedReader.GetEnumerator())
				{
					while (i > ADGenericPagedReader<ADRawEntry>.DefaultPageSize)
					{
						if (itemsToSkip < ADGenericPagedReader<ADRawEntry>.DefaultPageSize)
						{
							num = itemsToSkip;
						}
						for (int j = 0; j < num; j++)
						{
							if (!enumerator.MoveNext())
							{
								return new object[0][];
							}
							itemsToSkip--;
							i--;
							itemsTouched++;
						}
						if (itemsToSkip == 0)
						{
							while (enumerator.MoveNext())
							{
								itemsTouched++;
								list.Add(enumerator.Current.GetProperties(properties));
							}
							return list.ToArray();
						}
					}
				}
				cookie2 = adpagedReader.Cookie;
				if (!string.IsNullOrEmpty(adpagedReader.PreferredServerName))
				{
					scopedRecipientSession.DomainController = adpagedReader.PreferredServerName;
				}
			}
			adpagedReader = scopedRecipientSession.FindPagedADRawEntry(rootId, QueryScope.SubTree, filter, sortBy, pageSize + itemsToSkip, properties);
			adpagedReader.Cookie = cookie2;
			IEnumerator<ADRawEntry> enumerator3;
			IEnumerator<ADRawEntry> enumerator2 = enumerator3 = adpagedReader.GetEnumerator();
			try
			{
				for (int k = 0; k < itemsToSkip; k++)
				{
					if (!enumerator2.MoveNext())
					{
						return new object[0][];
					}
					itemsTouched++;
				}
				int num2 = 0;
				while (num2 < pageSize && enumerator2.MoveNext())
				{
					itemsTouched++;
					list.Add(enumerator2.Current.GetProperties(properties));
					num2++;
				}
			}
			finally
			{
				if (enumerator3 != null)
				{
					enumerator3.Dispose();
				}
			}
			if (adpagedReader.Cookie == null)
			{
				cookie = string.Empty;
			}
			else
			{
				cookie = Convert.ToBase64String(adpagedReader.Cookie);
			}
			lcid = adpagedReader.Lcid;
			preferredServerName = adpagedReader.PreferredServerName;
			return list.ToArray();
		}

		internal bool CanOpenAddressList(ClientSecurityContext clientSecurityContext)
		{
			bool flag = clientSecurityContext.HasExtendedRightOnObject(this.GetSecurityDescriptor(), WellKnownGuid.OpenAddressBookRight);
			ExTraceGlobals.AddressListTracer.TraceDebug<string, bool>(0L, "CanOpenAddressList: {0} {1}", base.DistinguishedName, flag);
			return flag;
		}

		private bool CanEnumerateChildren(ClientSecurityContext clientSecurityContext)
		{
			AccessMask grantedAccess = (AccessMask)clientSecurityContext.GetGrantedAccess(this.GetSecurityDescriptor(), AccessMask.List);
			ExTraceGlobals.AddressListTracer.TraceDebug<string, AccessMask>(0L, "CanEnumerateChildren: {0} perms {1}", base.DistinguishedName, grantedAccess);
			return (grantedAccess & AccessMask.List) != AccessMask.Open;
		}

		private bool CanListObject(ClientSecurityContext clientSecurityContext)
		{
			if (!AddressBookBase.ListObjectModeEnabled())
			{
				return false;
			}
			AccessMask grantedAccess = (AccessMask)clientSecurityContext.GetGrantedAccess(this.GetSecurityDescriptor(), AccessMask.ListObject);
			ExTraceGlobals.AddressListTracer.TraceDebug<string, AccessMask>(0L, "CanListObject: {0} perms {1}", base.DistinguishedName, grantedAccess);
			return (grantedAccess & AccessMask.ListObject) != AccessMask.Open;
		}

		internal static void ResetHeuristics()
		{
			lock (AddressBookBase.heuristicsLock)
			{
				AddressBookBase.listObjectMode = false;
				AddressBookBase.heuristicsNextUpdateTime = DateTime.MinValue;
			}
		}

		private SecurityDescriptor GetSecurityDescriptor()
		{
			SecurityDescriptor securityDescriptor;
			if (this.weakSecurityDescriptor == null)
			{
				securityDescriptor = base.ReadSecurityDescriptorBlob();
				this.weakSecurityDescriptor = new WeakReference(securityDescriptor);
			}
			else
			{
				securityDescriptor = (this.weakSecurityDescriptor.Target as SecurityDescriptor);
				if (securityDescriptor == null)
				{
					securityDescriptor = base.ReadSecurityDescriptorBlob();
					this.weakSecurityDescriptor.Target = securityDescriptor;
				}
			}
			if (securityDescriptor == null)
			{
				throw new ADOperationException(DirectoryStrings.AddressBookNoSecurityDescriptor(base.DistinguishedName));
			}
			return securityDescriptor;
		}

		private bool Contains(ADUser user)
		{
			MultiValuedProperty<ADObjectId> addressListMembership = user.AddressListMembership;
			if (addressListMembership == null || addressListMembership.Count <= 0)
			{
				return false;
			}
			foreach (ADObjectId id in addressListMembership)
			{
				if (base.Id.Equals(id))
				{
					return true;
				}
			}
			return false;
		}

		internal static AddressBookBase GetGlobalAddressList(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, IRecipientSession recipientSession)
		{
			return AddressBookBase.GetGlobalAddressList(clientSecurityContext, configurationSession, recipientSession, null);
		}

		internal static AddressBookBase GetGlobalAddressList(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, IRecipientSession recipientSession, bool preferCurrentUserGAL)
		{
			ADObjectId adobjectId = null;
			if (preferCurrentUserGAL && clientSecurityContext != null)
			{
				ADUser aduser = recipientSession.FindBySid(clientSecurityContext.UserSid) as ADUser;
				if (aduser == null)
				{
					ExTraceGlobals.AddressListTracer.TraceDebug<SecurityIdentifier>(0L, "Couldn't get a user object for sid '{0}'", clientSecurityContext.UserSid);
				}
				else
				{
					adobjectId = aduser.GlobalAddressListFromAddressBookPolicy;
					if (adobjectId == null)
					{
						ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "Couldn't get a GAL for user object of '{0}'", aduser.DistinguishedName);
					}
				}
			}
			return AddressBookBase.GetGlobalAddressList(clientSecurityContext, configurationSession, recipientSession, adobjectId);
		}

		internal static AddressBookBase GetGlobalAddressList(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, IRecipientSession recipientSession, ADObjectId globalAddressListFromAddressBookPolicy)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			if (clientSecurityContext.UserSid == null)
			{
				throw new ArgumentException("clientSecurityContext has null user sid");
			}
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (recipientSession.SessionSettings == null || recipientSession.ConfigScope == ConfigScopes.Global)
			{
				throw new ArgumentException("recipientSession is not properly scoped");
			}
			ExTraceGlobals.AddressListTracer.TraceDebug<SecurityIdentifier>(0L, "AddressBookBase.GetGlobalAddressList called for UserSid = '{0}'", clientSecurityContext.UserSid);
			if (globalAddressListFromAddressBookPolicy != null)
			{
				return configurationSession.Read<AddressBookBase>(globalAddressListFromAddressBookPolicy);
			}
			AddressBookBase addressBookBase = null;
			ADUser aduser = null;
			bool? flag = null;
			int? num = null;
			int num2 = 0;
			ADObjectId descendantId = configurationSession.GetOrgContainerId().GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization);
			foreach (AddressBookBase addressBookBase2 in AddressBookBase.GetAllAddressLists(descendantId, clientSecurityContext, configurationSession, null))
			{
				ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "Evaluating GAL '{0}'", addressBookBase2.Id.DistinguishedName);
				num2++;
				bool flag2 = false;
				bool? flag3 = null;
				int? num3 = null;
				if (addressBookBase2.Id.Equals(descendantId))
				{
					ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "GAL '{0}' is container, skipping", addressBookBase2.Id.DistinguishedName);
				}
				else
				{
					if (addressBookBase == null)
					{
						ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "Picking GAL '{0}' for now", addressBookBase2.Id.DistinguishedName);
						flag2 = true;
					}
					else
					{
						if (aduser == null)
						{
							ExTraceGlobals.AddressListTracer.TraceDebug<SecurityIdentifier>(0L, "Looking up user from sid '{0}'", clientSecurityContext.UserSid);
							aduser = (recipientSession.FindBySid(clientSecurityContext.UserSid) as ADUser);
							if (aduser == null)
							{
								ExTraceGlobals.AddressListTracer.TraceError<SecurityIdentifier>(0L, "Couldn't get a user object for sid '{0}'", clientSecurityContext.UserSid);
								return null;
							}
						}
						if (flag == null)
						{
							flag = new bool?(addressBookBase.Contains(aduser));
							ExTraceGlobals.AddressListTracer.TraceDebug<bool, string>(0L, "Current pick contains user: {0} ('{1}')", flag.Value, addressBookBase.Id.DistinguishedName);
						}
						flag3 = new bool?(addressBookBase2.Contains(aduser));
						ExTraceGlobals.AddressListTracer.TraceDebug<bool, string>(0L, "Candidate contains user: {0} ('{1}')", flag3.Value, addressBookBase2.Id.DistinguishedName);
						if (flag.Value == flag3)
						{
							if (num == null)
							{
								num = new int?(AddressBookBase.GetAddressListSize(configurationSession, addressBookBase.Id.ObjectGuid));
								ExTraceGlobals.AddressListTracer.TraceDebug<int?, string>(0L, "Current pick size: {0} ('{1}')", num, addressBookBase.Id.DistinguishedName);
							}
							num3 = new int?(AddressBookBase.GetAddressListSize(configurationSession, addressBookBase2.Id.ObjectGuid));
							ExTraceGlobals.AddressListTracer.TraceDebug<int?, string>(0L, "Candidate size: {0} ('{1}')", num3, addressBookBase2.Id.DistinguishedName);
							if (num3 > num)
							{
								ExTraceGlobals.AddressListTracer.TraceDebug<string, int?, int?>(0L, "Picking candidate since it is size ({1}) is larger than the previous pick ({2}) ('{0}')", addressBookBase2.Id.DistinguishedName, num3, num);
								flag2 = true;
							}
						}
						else if (flag3.Value)
						{
							ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "Picking candidate since it contains user and the previous pick doesn't ('{0}')", addressBookBase2.Id.DistinguishedName);
							flag2 = true;
						}
					}
					if (flag2)
					{
						addressBookBase = addressBookBase2;
						num = num3;
						flag = flag3;
					}
				}
			}
			if (addressBookBase == null)
			{
				string text = clientSecurityContext.UserSid.ToString();
				string text2 = (aduser == null) ? "(null)" : aduser.DistinguishedName;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_UnableToFindGALForUser, text, new object[]
				{
					text2,
					text
				});
			}
			ExTraceGlobals.AddressListTracer.TraceDebug<string, int>(0L, "Picked GAL = '{0}'. Total evaluated = {1}", (addressBookBase != null) ? addressBookBase.Id.DistinguishedName : "(null)", num2);
			return addressBookBase;
		}

		internal static IEnumerable<AddressBookBase> GetAllAddressLists(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId, bool includeModernGroupsAddressList)
		{
			return AddressBookBase.GetAllAddressLists(configurationSession.GetOrgContainerId().GetDescendantId(AddressList.RdnAlContainerToOrganization), clientSecurityContext, configurationSession, addressBookPolicyId, includeModernGroupsAddressList);
		}

		internal static IEnumerable<AddressBookBase> GetAllAddressLists(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId)
		{
			return AddressBookBase.GetAllAddressLists(configurationSession.GetOrgContainerId().GetDescendantId(AddressList.RdnAlContainerToOrganization), clientSecurityContext, configurationSession, addressBookPolicyId);
		}

		internal static IEnumerable<AddressBookBase> GetAllAddressLists(ADObjectId containerId, ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId)
		{
			return AddressBookBase.GetAllAddressLists(containerId, clientSecurityContext, configurationSession, addressBookPolicyId, false);
		}

		internal static IEnumerable<AddressBookBase> GetAllAddressLists(ADObjectId containerId, ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId, bool includeModernGroupsAddressList)
		{
			if (clientSecurityContext != null && clientSecurityContext.UserSid == null)
			{
				throw new ArgumentException("clientSecurityContext has null user sid");
			}
			if (addressBookPolicyId != null && clientSecurityContext == null)
			{
				throw new ArgumentException("addressBookPolicy exists without clientSecurityContext");
			}
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			ExTraceGlobals.AddressListTracer.TraceDebug(0L, "AddressBookBase.GetAllAddressLists called for UserSid = '{0}'", new object[]
			{
				(clientSecurityContext != null) ? clientSecurityContext.UserSid : "(null)"
			});
			IEnumerable<AddressBookBase> addressBooks;
			if (addressBookPolicyId != null)
			{
				AddressBookBase.AddressBookHierarchyHelper addressBookHierarchyHelper = new AddressBookBase.AddressBookHierarchyHelper(clientSecurityContext, configurationSession, null);
				addressBooks = addressBookHierarchyHelper.BuildHierarchyOfAddressListsFromABP(addressBookPolicyId);
			}
			else if (clientSecurityContext != null)
			{
				AddressBookBase.AddressBookHierarchyHelper addressBookHierarchyHelper2 = new AddressBookBase.AddressBookHierarchyHelper(clientSecurityContext, configurationSession, null);
				addressBooks = addressBookHierarchyHelper2.BuildHierarchy(containerId);
			}
			else
			{
				addressBooks = configurationSession.FindPaged<AddressBookBase>(containerId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.IsSystemAddressList, false), null, 0);
			}
			foreach (AddressBookBase addressList in addressBooks)
			{
				if (!addressList.IsInSystemAddressListContainer)
				{
					if (clientSecurityContext != null && !addressList.CanOpenAddressList(clientSecurityContext))
					{
						ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "AddressList '{0}' is not accessible, skipping", addressList.Id.DistinguishedName);
					}
					else if (!includeModernGroupsAddressList && addressList.IsModernGroupsAddressList)
					{
						ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "AddressList '{0}' is ModernGroups address list but the Modern Groups flight is not enabled, so skipping it", addressList.Id.DistinguishedName);
					}
					else
					{
						yield return addressList;
					}
				}
			}
			yield break;
		}

		internal static IList<AddressBookBase> GetAllAddressListsByHierarchy(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId, CultureInfo cultureInfo)
		{
			ADObjectId descendantId = configurationSession.GetOrgContainerId().GetDescendantId(AddressList.RdnAlContainerToOrganization);
			AddressBookBase.AddressBookHierarchyHelper addressBookHierarchyHelper = new AddressBookBase.AddressBookHierarchyHelper(clientSecurityContext, configurationSession, cultureInfo);
			if (addressBookPolicyId != null)
			{
				return addressBookHierarchyHelper.BuildHierarchyOfAddressListsFromABP(addressBookPolicyId);
			}
			return addressBookHierarchyHelper.BuildHierarchy(descendantId);
		}

		internal static bool IsModernGroupsAddressListPresent(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId)
		{
			IEnumerable<AddressBookBase> allAddressLists = AddressBookBase.GetAllAddressLists(clientSecurityContext, configurationSession, addressBookPolicyId, true);
			ExTraceGlobals.AddressListTracer.TraceDebug(0L, "AddressBookBase.GetModernGroupsAddressList called for UserSid = '{0}'", new object[]
			{
				(clientSecurityContext != null) ? clientSecurityContext.UserSid : "(null)"
			});
			foreach (AddressBookBase addressBookBase in allAddressLists)
			{
				if (addressBookBase.IsModernGroupsAddressList)
				{
					return true;
				}
			}
			return false;
		}

		internal static AddressBookBase GetAllRoomsAddressList(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, ADObjectId addressBookPolicyId)
		{
			AddressBookBase addressBookBase = null;
			if (clientSecurityContext != null && clientSecurityContext.UserSid == null)
			{
				throw new ArgumentException("clientSecurityContext has null user sid");
			}
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			ExTraceGlobals.AddressListTracer.TraceDebug(0L, "AddressBookBase.GetAllRoomsAddressList called for UserSid = '{0}'", new object[]
			{
				(clientSecurityContext != null) ? clientSecurityContext.UserSid : "(null)"
			});
			if (addressBookPolicyId == null)
			{
				Organization orgContainer = configurationSession.GetOrgContainer();
				MultiValuedProperty<ADObjectId> resourceAddressLists = orgContainer.ResourceAddressLists;
				if (resourceAddressLists.Count <= 0)
				{
					addressBookBase = null;
				}
				else
				{
					ADObjectId adobjectId = resourceAddressLists[0];
					AddressBookBase addressBookBase2 = configurationSession.Read<AddressBookBase>(adobjectId);
					if (addressBookBase2 == null)
					{
						ExTraceGlobals.AddressListTracer.TraceError(0L, "Could not find 'all rooms' entry by " + adobjectId.ToDNString() + " via " + configurationSession.ConfigurationNamingContext.ToDNString());
					}
					else if (clientSecurityContext == null || addressBookBase2.CanOpenAddressList(clientSecurityContext))
					{
						addressBookBase = addressBookBase2;
					}
				}
				ExTraceGlobals.AddressListTracer.TraceDebug<string>(0L, "All Rooms Address Book = {0}", (addressBookBase != null) ? addressBookBase.Id.DistinguishedName : "(null)");
				return addressBookBase;
			}
			AddressBookMailboxPolicy addressBookMailboxPolicy = configurationSession.Read<AddressBookMailboxPolicy>(addressBookPolicyId);
			if (addressBookMailboxPolicy != null)
			{
				return configurationSession.Read<AddressBookBase>(addressBookMailboxPolicy.RoomList);
			}
			return null;
		}

		internal static int GetAddressListSize(IConfigurationSession session, Guid addressListObjectGuid)
		{
			return NspiVirtualListView.GetEstimatedRowCount(session, addressListObjectGuid);
		}

		private const string MostDerivedClass = "addressBookContainer";

		private static AddressBookBaseSchema schema = ObjectSchema.GetInstance<AddressBookBaseSchema>();

		private static bool listObjectMode;

		private static DateTime heuristicsNextUpdateTime;

		private static object heuristicsLock = new object();

		private WeakReference weakSecurityDescriptor;

		private int depth;

		public enum RecipientCategory
		{
			People,
			Groups,
			Rooms,
			All
		}

		private static class CategoryFilters
		{
			internal static ComparisonFilter ObjectCategoryFilter(string category)
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, category);
			}

			private const string Person = "person";

			private const string Group = "group";

			private const string DynamicDistributionList = "msExchDynamicDistributionList";

			internal static QueryFilter PersonFilter = AddressBookBase.CategoryFilters.ObjectCategoryFilter("person");

			internal static QueryFilter GroupFilter = new OrFilter(new QueryFilter[]
			{
				AddressBookBase.CategoryFilters.ObjectCategoryFilter("group"),
				AddressBookBase.CategoryFilters.ObjectCategoryFilter("msExchDynamicDistributionList")
			});
		}

		private sealed class AddressBookHierarchyHelper
		{
			internal AddressBookHierarchyHelper(ClientSecurityContext clientSecurityContext, IConfigurationSession configurationSession, CultureInfo cultureInfo)
			{
				if (clientSecurityContext != null && clientSecurityContext.UserSid == null)
				{
					throw new ArgumentException("clientSecurityContext has null user sid");
				}
				if (configurationSession == null)
				{
					throw new ArgumentNullException("configurationSession");
				}
				ExTraceGlobals.AddressListTracer.TraceDebug(0L, "AddressBookHierarchyHelper called for UserSid = '{0}'", new object[]
				{
					(clientSecurityContext != null) ? clientSecurityContext.UserSid : "(null)"
				});
				this.clientSecurityContext = clientSecurityContext;
				this.configurationSession = configurationSession;
				if (cultureInfo != null)
				{
					this.nameComparer = new AddressBookBase.AddressBookHierarchyHelper.AddressListNameComparer(cultureInfo);
				}
			}

			internal List<AddressBookBase> BuildHierarchy(ADObjectId containerId)
			{
				this.LoadAddressListDictionary(containerId);
				this.BuildDictionaryOfChildren();
				this.results = new List<AddressBookBase>(this.addressLists.Count);
				this.ProcessChildren(null, this.roots, 0, true);
				return this.results;
			}

			internal List<AddressBookBase> BuildHierarchyOfAddressListsFromABP(ADObjectId addressBookPolicyId)
			{
				if (addressBookPolicyId == null)
				{
					throw new ArgumentNullException("addressBookPolicyId");
				}
				AddressBookMailboxPolicy addressBookMailboxPolicy = this.configurationSession.Read<AddressBookMailboxPolicy>(addressBookPolicyId);
				if (addressBookMailboxPolicy != null)
				{
					List<ADObjectId> list = new List<ADObjectId>(addressBookMailboxPolicy.AddressLists.ToArray());
					if (!list.Contains(addressBookMailboxPolicy.RoomList))
					{
						list.Add(addressBookMailboxPolicy.RoomList);
					}
					this.LoadAddressListDictionaryFromABP(list);
					this.BuildDictionaryOfChildren();
					this.results = new List<AddressBookBase>(this.addressLists.Count);
					this.ProcessChildren(null, this.roots, 0, true);
					return this.results;
				}
				return new List<AddressBookBase>();
			}

			private void LoadAddressListDictionary(ADObjectId containerId)
			{
				ADPagedReader<AddressBookBase> adpagedReader = this.configurationSession.FindPaged<AddressBookBase>(containerId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.IsSystemAddressList, false), null, 0);
				foreach (AddressBookBase addressBookBase in adpagedReader)
				{
					if (!addressBookBase.IsInSystemAddressListContainer)
					{
						this.addressLists.Add(addressBookBase.DistinguishedName, addressBookBase);
					}
				}
			}

			private void LoadAddressListDictionaryFromABP(List<ADObjectId> addressLists)
			{
				Result<AddressBookBase>[] array = this.configurationSession.FindByADObjectIds<AddressBookBase>(addressLists.ToArray());
				foreach (Result<AddressBookBase> result in array)
				{
					AddressBookBase data = result.Data;
					if (data != null && !data.IsInSystemAddressListContainer)
					{
						this.addressLists.Add(data.DistinguishedName, data);
					}
				}
			}

			private void BuildDictionaryOfChildren()
			{
				foreach (AddressBookBase addressBookBase in this.addressLists.Values)
				{
					bool flag = false;
					string distinguishedName = addressBookBase.Id.Parent.DistinguishedName;
					List<AddressBookBase> list;
					if (this.addressLists.ContainsKey(distinguishedName))
					{
						if (!this.parents.TryGetValue(distinguishedName, out list))
						{
							list = new List<AddressBookBase>();
							this.parents.Add(distinguishedName, list);
						}
					}
					else
					{
						flag = true;
						list = this.roots;
					}
					ExTraceGlobals.AddressListTracer.TraceDebug<string, string, string>(0L, "AddressList '{0}', parent '{1}'{2}", addressBookBase.Id.DistinguishedName, distinguishedName, flag ? "(root)" : string.Empty);
					list.Add(addressBookBase);
				}
			}

			private void ProcessChildren(AddressBookBase parentAddressList, List<AddressBookBase> lists, int depth, bool parentIsVisible)
			{
				if (this.nameComparer != null && lists.Count > 1)
				{
					lists.Sort(this.nameComparer);
				}
				foreach (AddressBookBase addressBookBase in lists)
				{
					addressBookBase.depth = depth;
					if (this.ObjectIsVisible(addressBookBase, parentAddressList, parentIsVisible))
					{
						this.results.Add(addressBookBase);
						List<AddressBookBase> lists2;
						if (this.parents.TryGetValue(addressBookBase.DistinguishedName, out lists2))
						{
							this.ProcessChildren(addressBookBase, lists2, depth + 1, addressBookBase.CanEnumerateChildren(this.clientSecurityContext));
						}
					}
				}
			}

			private bool ObjectIsVisible(AddressBookBase addressList, AddressBookBase parentAddressList, bool parentIsVisible)
			{
				if (parentIsVisible)
				{
					return true;
				}
				if (parentAddressList == null)
				{
					return true;
				}
				if (this.hasListObjectCache == null)
				{
					this.hasListObjectCache = new Dictionary<AddressBookBase, bool>();
				}
				bool flag;
				if (!this.hasListObjectCache.TryGetValue(parentAddressList, out flag))
				{
					flag = parentAddressList.CanListObject(this.clientSecurityContext);
					this.hasListObjectCache.Add(parentAddressList, flag);
				}
				if (!flag)
				{
					return false;
				}
				bool flag2;
				if (!this.hasListObjectCache.TryGetValue(addressList, out flag2))
				{
					flag2 = addressList.CanListObject(this.clientSecurityContext);
					this.hasListObjectCache.Add(addressList, flag2);
				}
				return flag2;
			}

			private readonly ClientSecurityContext clientSecurityContext;

			private readonly IConfigurationSession configurationSession;

			private readonly Dictionary<string, AddressBookBase> addressLists = new Dictionary<string, AddressBookBase>();

			private readonly Dictionary<string, List<AddressBookBase>> parents = new Dictionary<string, List<AddressBookBase>>();

			private readonly List<AddressBookBase> roots = new List<AddressBookBase>();

			private readonly IComparer<AddressBookBase> nameComparer;

			private List<AddressBookBase> results;

			private Dictionary<AddressBookBase, bool> hasListObjectCache;

			private class AddressListNameComparer : IComparer<AddressBookBase>
			{
				internal AddressListNameComparer(CultureInfo cultureInfo)
				{
					this.cultureInfo = cultureInfo;
				}

				public int Compare(AddressBookBase x, AddressBookBase y)
				{
					return string.Compare(x.DisplayName, y.DisplayName, true, this.cultureInfo);
				}

				private readonly CultureInfo cultureInfo;
			}
		}
	}
}

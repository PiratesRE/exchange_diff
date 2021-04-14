using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ExtendedOrganizationalUnit : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExtendedOrganizationalUnit.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExtendedOrganizationalUnit.mostDerivedClass;
			}
		}

		public OrganizationalUnitType Type
		{
			get
			{
				return this.organizationalUnitType;
			}
		}

		public string CanonicalName
		{
			get
			{
				return (string)this[ADObjectSchema.CanonicalName];
			}
		}

		public bool IsWellKnownContainer
		{
			get
			{
				return this.isWellKnowContainer;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncStatusAck
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExtendedOrganizationalUnitSchema.DirSyncStatusAck];
			}
			set
			{
				this[ExtendedOrganizationalUnitSchema.DirSyncStatusAck] = value;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (((MultiValuedProperty<string>)this[ADObjectSchema.ObjectClass]).Contains(ADDomain.MostDerivedClass))
			{
				this.organizationalUnitType = OrganizationalUnitType.Domain;
				this.isWellKnowContainer = false;
				return;
			}
			if (((MultiValuedProperty<string>)this[ADObjectSchema.ObjectClass]).Contains(ADContainer.MostDerivedClass))
			{
				this.organizationalUnitType = OrganizationalUnitType.Container;
				this.isWellKnowContainer = true;
				return;
			}
			if (((MultiValuedProperty<string>)this[ADObjectSchema.ObjectClass]).Contains(ADOrganizationalUnit.MostDerivedClass))
			{
				this.organizationalUnitType = OrganizationalUnitType.OrganizationalUnit;
				this.isWellKnowContainer = false;
				return;
			}
			errors.Add(new PropertyValidationError(DataStrings.BadEnumValue(typeof(OrganizationalUnitType)), ADObjectSchema.ObjectClass, null));
		}

		internal MultiValuedProperty<string> UPNSuffixes
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExtendedOrganizationalUnitSchema.UPNSuffixes];
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOrganizationalUnit.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADDomain.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADContainer.MostDerivedClass)
				});
			}
		}

		internal static bool IsTenant(IDirectorySession session)
		{
			return !session.SessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId);
		}

		internal static IEnumerable<ExtendedOrganizationalUnit> FindFirstLevelChildOrganizationalUnit(bool includeContainers, IConfigurationSession session, ADObjectId rootId, QueryFilter preFilter, SortBy sortBy, int pageSize)
		{
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOrganizationalUnit.MostDerivedClass),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADDomain.MostDerivedClass)
			});
			if (!ExtendedOrganizationalUnit.IsTenant(session))
			{
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = session.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.DomainControllersWkGuid, rootId);
				ExchangeOrganizationalUnit exchangeOrganizationalUnit2 = session.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.UsersWkGuid, rootId);
				if (exchangeOrganizationalUnit != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, exchangeOrganizationalUnit.Id)
					});
				}
				if (includeContainers && exchangeOrganizationalUnit2 != null)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, exchangeOrganizationalUnit2.Id)
					});
				}
			}
			if (preFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					preFilter,
					queryFilter
				});
			}
			return session.FindPaged<ExtendedOrganizationalUnit>(rootId, QueryScope.OneLevel, queryFilter, sortBy, pageSize);
		}

		internal static IEnumerable<ExtendedOrganizationalUnit> FindSubTreeChildOrganizationalUnit(bool includeContainers, IConfigurationSession session, ADObjectId rootId, QueryFilter preFilter)
		{
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOrganizationalUnit.MostDerivedClass),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADDomain.MostDerivedClass)
			});
			if (!ExtendedOrganizationalUnit.IsTenant(session))
			{
				IList<QueryFilter> list = new List<QueryFilter>();
				IList<QueryFilter> list2 = new List<QueryFilter>();
				IEnumerable<ADDomain> enumerable = ADForest.GetLocalForest(session.DomainController).FindDomains();
				foreach (ADDomain addomain in enumerable)
				{
					ExchangeOrganizationalUnit exchangeOrganizationalUnit = session.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.DomainControllersWkGuid, addomain.Id);
					if (exchangeOrganizationalUnit != null)
					{
						list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, exchangeOrganizationalUnit.Id));
					}
					ExchangeOrganizationalUnit exchangeOrganizationalUnit2 = session.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.UsersWkGuid, addomain.Id);
					if (exchangeOrganizationalUnit2 != null)
					{
						list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, exchangeOrganizationalUnit2.Id));
					}
				}
				foreach (QueryFilter queryFilter2 in list)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
				}
				if (includeContainers)
				{
					foreach (QueryFilter queryFilter3 in list2)
					{
						queryFilter = new OrFilter(new QueryFilter[]
						{
							queryFilter,
							queryFilter3
						});
					}
				}
			}
			if (preFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					preFilter,
					queryFilter
				});
			}
			return session.FindPaged<ExtendedOrganizationalUnit>(rootId, QueryScope.SubTree, queryFilter, null, 0);
		}

		private static ExtendedOrganizationalUnitSchema schema = ObjectSchema.GetInstance<ExtendedOrganizationalUnitSchema>();

		private static string mostDerivedClass = "organizationalUnit";

		private OrganizationalUnitType organizationalUnitType;

		private bool isWellKnowContainer;
	}
}

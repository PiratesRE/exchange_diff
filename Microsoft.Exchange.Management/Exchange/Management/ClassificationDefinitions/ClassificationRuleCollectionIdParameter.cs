using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public class ClassificationRuleCollectionIdParameter : ADIdParameter
	{
		public ClassificationRuleCollectionIdParameter()
		{
			this.Initialize();
		}

		protected ClassificationRuleCollectionIdParameter(string identity) : base(identity)
		{
			ExAssert.RetailAssert(!string.IsNullOrEmpty(base.RawIdentity), "The identity argument should have been validated by the base class");
			this.Initialize();
		}

		public static ClassificationRuleCollectionIdParameter Parse(string identity)
		{
			return new ClassificationRuleCollectionIdParameter(identity);
		}

		public static implicit operator ClassificationRuleCollectionIdParameter(ADObjectId identity)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			return new ClassificationRuleCollectionIdParameter(identity.ToString());
		}

		internal override void Initialize(ObjectId objectId)
		{
			ADObjectId adobjectId = objectId as ADObjectId;
			if (adobjectId != null && base.InternalADObjectId == null && string.IsNullOrEmpty(adobjectId.DistinguishedName))
			{
				return;
			}
			base.Initialize(objectId);
		}

		private void Initialize()
		{
			this.ShouldIncludeOutOfBoxCollections = false;
			this.IsHierarchyValid = false;
			this.OrganizationName = null;
			string rawIdentity = base.RawIdentity;
			this.FriendlyName = rawIdentity;
			if (string.IsNullOrEmpty(rawIdentity))
			{
				this.IsHierarchical = false;
				return;
			}
			if (base.InternalADObjectId != null)
			{
				this.IsHierarchical = false;
				this.FriendlyName = null;
				return;
			}
			int num = rawIdentity.IndexOf('\\');
			this.IsHierarchical = (num >= 1 && num + 1 < rawIdentity.Length && base.IsMultitenancyEnabled());
			if (this.IsHierarchical)
			{
				this.OrganizationName = rawIdentity.Substring(0, num);
				if (this.IsWildcardDefined(this.OrganizationName))
				{
					this.IsHierarchyValid = false;
				}
				else
				{
					ExchangeConfigurationUnit configurationUnit = base.GetConfigurationUnit(this.OrganizationName);
					this.IsHierarchyValid = (configurationUnit != null);
				}
				this.FriendlyName = rawIdentity.Substring(num + 1);
			}
		}

		private static QueryFilter CreateExcludeFilter<T>(IList<T> dataObjectToBeExcludedFromQuery) where T : IConfigurable, new()
		{
			if (dataObjectToBeExcludedFromQuery.Count == 0)
			{
				return null;
			}
			IEnumerable<QueryFilter> source = from oobRuleCollection in dataObjectToBeExcludedFromQuery
			select new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.DistinguishedName, ((ADObjectId)oobRuleCollection.Identity).DistinguishedName);
			return QueryFilter.AndTogether(source.ToArray<QueryFilter>());
		}

		private ClassificationIdentityMatcher<Tuple<TransportRule, XDocument>> CreateNameMatchFilter(string identityToMatch)
		{
			TextFilter queryFilter = this.IsWildcardDefined(this.FriendlyName) ? ((TextFilter)base.CreateWildcardFilter(ADObjectSchema.Name, identityToMatch)) : new TextFilter(ADObjectSchema.Name, identityToMatch, MatchOptions.FullString, MatchFlags.Default);
			return ClassificationRuleCollectionNameMatcher.CreateFrom(queryFilter, identityToMatch);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					ADConfigurationObjectSchema.AdminDisplayName
				};
			}
		}

		internal bool IsHierarchical { get; private set; }

		internal bool IsHierarchyValid { get; private set; }

		internal string FriendlyName { get; private set; }

		internal bool ShouldIncludeOutOfBoxCollections { get; set; }

		private protected string OrganizationName { protected get; private set; }

		internal override IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData)
		{
			IEnumerable<T> objectsInOrganization = base.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData);
			if (typeof(T) != typeof(TransportRule))
			{
				return objectsInOrganization;
			}
			ClassificationIdentityMatcher<Tuple<TransportRule, XDocument>> nameMatcher = this.CreateNameMatchFilter(identityString);
			if (nameMatcher == null)
			{
				return objectsInOrganization;
			}
			List<T> list = objectsInOrganization.ToList<T>();
			QueryFilter queryFilter = ClassificationRuleCollectionIdParameter.CreateExcludeFilter<T>(list);
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				queryFilter = ((queryFilter == null) ? optionalData.AdditionalFilter : QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalData.AdditionalFilter
				}));
			}
			IEnumerable<Tuple<TransportRule, XDocument>> source = DlpUtils.AggregateOobAndCustomClassificationDefinitions(session.SessionSettings.CurrentOrganizationId, session as IConfigurationSession, null, queryFilter, new ClassificationDefinitionsDataReader(false), null);
			return list.Concat(from rulePackDataObject in source
			where nameMatcher.Match(rulePackDataObject)
			select (T)((object)rulePackDataObject.Item1));
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			IEnumerable<T> enumerable = Enumerable.Empty<T>();
			if (this.ShouldIncludeOutOfBoxCollections && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && base.InternalADObjectId == null)
			{
				bool flag = false;
				bool flag2 = OrganizationId.ForestWideOrgId.Equals(session.SessionSettings.CurrentOrganizationId);
				if (flag2 && this.IsHierarchical && this.IsHierarchyValid)
				{
					ClassificationRuleCollectionIdParameter classificationRuleCollectionIdParameter = new ClassificationRuleCollectionIdParameter(this.FriendlyName);
					enumerable = classificationRuleCollectionIdParameter.GetObjects<T>(rootId, (IConfigDataProvider)session, optionalData, out notFoundReason);
					flag = true;
				}
				else if (!flag2 && !this.IsHierarchical)
				{
					ITopologyConfigurationSession session2 = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 431, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ClassificationDefinitions\\ClassificationRuleCollectionIdParameter.cs");
					enumerable = base.GetObjects<T>(ClassificationDefinitionUtils.GetClassificationRuleCollectionContainerId(session2), session2, null, out notFoundReason);
					flag = true;
				}
				if (flag)
				{
					if (optionalData == null)
					{
						optionalData = new OptionalIdentityData();
					}
					List<T> list = enumerable.ToList<T>();
					QueryFilter queryFilter = ClassificationRuleCollectionIdParameter.CreateExcludeFilter<T>(list);
					if (queryFilter != null)
					{
						optionalData.AdditionalFilter = ((optionalData.AdditionalFilter == null) ? queryFilter : QueryFilter.AndTogether(new QueryFilter[]
						{
							optionalData.AdditionalFilter,
							queryFilter
						}));
					}
					enumerable = list;
				}
			}
			return enumerable.Concat(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
		}
	}
}

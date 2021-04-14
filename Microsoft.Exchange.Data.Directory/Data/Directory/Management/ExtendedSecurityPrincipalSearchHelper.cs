using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal static class ExtendedSecurityPrincipalSearchHelper
	{
		internal static IEnumerable<ExtendedSecurityPrincipal> PerformSearch(ExtendedSecurityPrincipalSearcher searcher, IConfigDataProvider session, ADObjectId rootId, ADObjectId includeDomailLocalFrom, MultiValuedProperty<SecurityPrincipalType> types)
		{
			if (types.Contains(SecurityPrincipalType.WellknownSecurityPrincipal))
			{
				IRecipientSession dataSession = (IRecipientSession)session;
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				IConfigurationSession configSession;
				if (dataSession.ConfigScope == ConfigScopes.TenantSubTree)
				{
					configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(dataSession.DomainController, dataSession.ReadOnly, dataSession.ConsistencyMode, dataSession.NetworkCredential, sessionSettings, dataSession.ConfigScope, 60, "PerformSearch", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Management\\ExtendedSecurityPrincipalSearchHelper.cs");
				}
				else
				{
					configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(dataSession.DomainController, dataSession.ReadOnly, dataSession.ConsistencyMode, dataSession.NetworkCredential, sessionSettings, 70, "PerformSearch", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Management\\ExtendedSecurityPrincipalSearchHelper.cs");
				}
				foreach (ExtendedSecurityPrincipal wellknown in searcher(configSession, null, ExtendedSecurityPrincipalSearchHelper.GenerateTargetFilterForWellknownSecurityPrincipal()))
				{
					yield return wellknown;
				}
			}
			IRecipientSession recipientSession = (IRecipientSession)session;
			if (types.Contains(SecurityPrincipalType.GlobalSecurityGroup) || types.Contains(SecurityPrincipalType.UniversalSecurityGroup) || types.Contains(SecurityPrincipalType.User) || types.Contains(SecurityPrincipalType.Group))
			{
				recipientSession.UseGlobalCatalog = (rootId == null);
				foreach (ExtendedSecurityPrincipal recipient in searcher(recipientSession, rootId, ExtendedSecurityPrincipalSearchHelper.GenerateTargetFilterForUserAndNonDomainLocalGroup(types)))
				{
					yield return recipient;
				}
			}
			if (includeDomailLocalFrom != null)
			{
				recipientSession.UseGlobalCatalog = false;
				ADObjectId searchRoot = null;
				if (rootId == null || includeDomailLocalFrom.IsDescendantOf(rootId))
				{
					searchRoot = includeDomailLocalFrom;
				}
				else if (rootId.DomainId.Equals(includeDomailLocalFrom))
				{
					searchRoot = rootId;
				}
				if (searchRoot != null)
				{
					foreach (ExtendedSecurityPrincipal group in searcher(recipientSession, searchRoot, ExtendedSecurityPrincipalSearchHelper.GenerateTargetFilterForSecurityGroup(GroupTypeFlags.DomainLocal)))
					{
						yield return group;
					}
				}
			}
			yield break;
		}

		private static QueryFilter GenerateTargetFilterForWellknownSecurityPrincipal()
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ExtendedSecurityPrincipal.WellknownSecurityPrincipalClassName);
		}

		private static QueryFilter GenerateTargetFilterForUserAndNonDomainLocalGroup(MultiValuedProperty<SecurityPrincipalType> types)
		{
			List<CompositeFilter> list = new List<CompositeFilter>();
			foreach (SecurityPrincipalType securityPrincipalType in types)
			{
				CompositeFilter compositeFilter = null;
				switch (securityPrincipalType)
				{
				case SecurityPrincipalType.User:
					compositeFilter = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADUser.ObjectCategoryNameInternal),
						ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true)
					});
					break;
				case SecurityPrincipalType.Group:
					compositeFilter = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADGroup.MostDerivedClass),
						new BitMaskOrFilter(ADGroupSchema.GroupType, (ulong)int.MinValue),
						new NotFilter(new BitMaskAndFilter(ADGroupSchema.GroupType, 4UL))
					});
					break;
				case SecurityPrincipalType.UniversalSecurityGroup:
					if (!types.Contains(SecurityPrincipalType.Group))
					{
						compositeFilter = ExtendedSecurityPrincipalSearchHelper.GenerateTargetFilterForSecurityGroup(GroupTypeFlags.Universal);
					}
					break;
				case SecurityPrincipalType.GlobalSecurityGroup:
					if (!types.Contains(SecurityPrincipalType.Group))
					{
						compositeFilter = ExtendedSecurityPrincipalSearchHelper.GenerateTargetFilterForSecurityGroup(GroupTypeFlags.Global);
					}
					break;
				}
				if (compositeFilter != null)
				{
					if (Datacenter.IsMicrosoftHostedOnly(true) && (securityPrincipalType == SecurityPrincipalType.Group || securityPrincipalType == SecurityPrincipalType.UniversalSecurityGroup))
					{
						compositeFilter = new AndFilter(new QueryFilter[]
						{
							compositeFilter,
							new OrFilter(new QueryFilter[]
							{
								new NotFilter(new BitMaskAndFilter(ADGroupSchema.GroupType, 8UL)),
								new AndFilter(new QueryFilter[]
								{
									new BitMaskAndFilter(ADGroupSchema.GroupType, 8UL),
									new OrFilter(new QueryFilter[]
									{
										new ExistsFilter(ADRecipientSchema.Alias),
										Filters.GetRecipientTypeDetailsFilterOptimization(RecipientTypeDetails.RoleGroup)
									})
								})
							})
						});
					}
					list.Add(compositeFilter);
				}
			}
			return new OrFilter(list.ToArray());
		}

		private static CompositeFilter GenerateTargetFilterForSecurityGroup(GroupTypeFlags flag)
		{
			return new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADGroup.MostDerivedClass),
				new BitMaskOrFilter(ADGroupSchema.GroupType, (ulong)int.MinValue),
				new BitMaskAndFilter(ADGroupSchema.GroupType, (ulong)flag)
			});
		}
	}
}

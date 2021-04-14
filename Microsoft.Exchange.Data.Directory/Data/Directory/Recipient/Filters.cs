using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class Filters
	{
		private static QueryFilter CreateUserEnabledFilter(bool enabled)
		{
			QueryFilter queryFilter = new BitMaskAndFilter(ADUserSchema.UserAccountControl, 2UL);
			if (!enabled)
			{
				return queryFilter;
			}
			return new NotFilter(queryFilter);
		}

		private static QueryFilter CreateMailEnabledFilter(bool exists)
		{
			QueryFilter queryFilter = new ExistsFilter(ADRecipientSchema.Alias);
			if (!exists)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		private static QueryFilter CreateUniversalGroupFilter(bool isUniversal)
		{
			QueryFilter queryFilter = new BitMaskOrFilter(ADGroupSchema.GroupType, 8UL);
			if (!isUniversal)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		private static QueryFilter CreateSecurityGroupFilter(bool securityEnabled)
		{
			QueryFilter queryFilter = new BitMaskOrFilter(ADGroupSchema.GroupType, (ulong)int.MinValue);
			if (!securityEnabled)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		private static QueryFilter CreateMailboxEnabledFilter(bool exists)
		{
			QueryFilter queryFilter = new ExistsFilter(IADMailStorageSchema.ServerLegacyDN);
			if (!exists)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		private static QueryFilter CreateRecipientTypeFilter(RecipientType recipientType)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, recipientType);
		}

		private static QueryFilter CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails recipientTypeDetails)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, recipientTypeDetails);
		}

		private static QueryFilter[] InitializeStaticRecipientTypeFilters()
		{
			QueryFilter[] array = new QueryFilter[Filters.RecipientTypeCount];
			array[1] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true),
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				Filters.CreateMailEnabledFilter(false)
			});
			array[2] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true),
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				Filters.CreateMailEnabledFilter(true),
				Filters.CreateMailboxEnabledFilter(true)
			});
			array[3] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter(ADUser.MostDerivedClass, true),
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				Filters.CreateMailEnabledFilter(true),
				Filters.CreateMailboxEnabledFilter(false)
			});
			array[4] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter(ADContact.MostDerivedClass),
				Filters.CreateMailEnabledFilter(false)
			});
			array[5] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectClassFilter(ADContact.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[6] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
				Filters.CreateMailEnabledFilter(false)
			});
			array[9] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
				Filters.NonUniversalGroupFilter,
				Filters.CreateMailEnabledFilter(true)
			});
			array[7] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
				Filters.UniversalDistributionGroupFilter,
				Filters.CreateMailEnabledFilter(true)
			});
			array[8] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
				Filters.UniversalSecurityGroupFilter,
				Filters.CreateMailEnabledFilter(true)
			});
			array[10] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADDynamicGroup.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[11] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADPublicFolder.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[12] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADPublicDatabase.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[13] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADSystemAttendantMailbox.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[15] = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADMicrosoftExchangeRecipient.MostDerivedClass),
				Filters.CreateMailEnabledFilter(true)
			});
			array[14] = new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				Filters.CreateMailboxEnabledFilter(true),
				new OrFilter(new QueryFilter[]
				{
					ADObject.ObjectCategoryFilter(ADSystemMailbox.MostDerivedClass),
					new AndFilter(new QueryFilter[]
					{
						ADUser.ImplicitFilterInternal,
						new TextFilter(ADObjectSchema.Name, "SystemMailbox{", MatchOptions.Prefix, MatchFlags.Default)
					})
				})
			});
			array[16] = ADComputerRecipient.ImplicitFilterInternal;
			return array;
		}

		internal static bool HasWellKnownRecipientTypeFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				return false;
			}
			if (Filters.IsEqualityFilterOnPropertyDefinition(filter, new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.RecipientTypeDetails
			}))
			{
				return true;
			}
			if (Filters.IsOrFilterOnPropertyDefinitionComparisons(filter, new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.RecipientTypeDetails
			}))
			{
				return true;
			}
			AndFilter andFilter = filter as AndFilter;
			return andFilter != null && andFilter.FilterCount > 0 && (Filters.IsEqualityFilterOnPropertyDefinition(andFilter.Filters[0], new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.RecipientTypeDetails
			}) || Filters.IsOrFilterOnPropertyDefinitionComparisons(andFilter.Filters[0], new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.RecipientTypeDetails
			}));
		}

		internal static bool IsOrFilterOnPropertyDefinitionComparisons(QueryFilter filterToCheck, params PropertyDefinition[] propertyDefinitions)
		{
			OrFilter orFilter = filterToCheck as OrFilter;
			if (orFilter == null || orFilter.FilterCount <= 0)
			{
				return false;
			}
			foreach (QueryFilter filterToCheck2 in orFilter.Filters)
			{
				if (!Filters.IsEqualityFilterOnPropertyDefinition(filterToCheck2, propertyDefinitions))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsEqualityFilterOnPropertyDefinition(QueryFilter filterToCheck, params PropertyDefinition[] propertyDefinitions)
		{
			ComparisonFilter comparisonFilter = filterToCheck as ComparisonFilter;
			if (comparisonFilter != null && comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
			{
				foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
				{
					if (comparisonFilter.Property == propertyDefinition)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static QueryFilter OptimizeRecipientTypeFilter(OrFilter orFilter)
		{
			Filters.RecipientTypeBitVector32 recipientTypeBitVector = default(Filters.RecipientTypeBitVector32);
			List<QueryFilter> list = null;
			foreach (QueryFilter queryFilter in orFilter.Filters)
			{
				ComparisonFilter comparisonFilter = queryFilter as ComparisonFilter;
				if (comparisonFilter != null && comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && comparisonFilter.Property == ADRecipientSchema.RecipientType)
				{
					RecipientType index = (RecipientType)ADObject.PropertyValueFromEqualityFilter(comparisonFilter);
					recipientTypeBitVector[index] = true;
				}
				else
				{
					if (list == null)
					{
						list = new List<QueryFilter>(orFilter.FilterCount);
					}
					list.Add(queryFilter);
				}
			}
			QueryFilter queryFilter2 = null;
			if (!Filters.RecipientTypeFilterOptimizations.TryGetValue(recipientTypeBitVector.Data, out queryFilter2))
			{
				return orFilter;
			}
			if (list == null)
			{
				return queryFilter2;
			}
			list.Add(queryFilter2);
			return new OrFilter(list.ToArray());
		}

		internal static QueryFilter GetRecipientTypeDetailsFilterOptimization(RecipientTypeDetails recipientTypeDetails)
		{
			QueryFilter result = null;
			Filters.RecipientTypeDetailsFilterOptimizations.TryGetValue(recipientTypeDetails, out result);
			return result;
		}

		private static Dictionary<int, QueryFilter> InitializeStaticRecipientTypeFilterOptimizations()
		{
			Dictionary<int, QueryFilter> dictionary = new Dictionary<int, QueryFilter>(32);
			Filters.RecipientTypeBitVector32 recipientTypeBitVector = default(Filters.RecipientTypeBitVector32);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.DynamicDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.UserMailbox] = true;
			recipientTypeBitVector[RecipientType.MailContact] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			recipientTypeBitVector[RecipientType.MailUser] = true;
			QueryFilter value = Filters.AllMailableUsersContactsDDLsUniversalGroupsFilter;
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.Contact] = true;
			recipientTypeBitVector[RecipientType.MailContact] = true;
			value = ADObject.ObjectClassFilter(ADContact.MostDerivedClass);
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.Group] = true;
			recipientTypeBitVector[RecipientType.MailNonUniversalGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			value = ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass);
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.MailNonUniversalGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			value = new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass)
			});
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.User] = true;
			recipientTypeBitVector[RecipientType.MailUser] = true;
			recipientTypeBitVector[RecipientType.UserMailbox] = true;
			value = ADUser.ImplicitFilterInternal;
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.DynamicDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.UserMailbox] = true;
			recipientTypeBitVector[RecipientType.MailContact] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			recipientTypeBitVector[RecipientType.MailUser] = true;
			recipientTypeBitVector[RecipientType.MailNonUniversalGroup] = true;
			recipientTypeBitVector[RecipientType.PublicFolder] = true;
			value = new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				new OrFilter(new QueryFilter[]
				{
					ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
					ADObject.ObjectCategoryFilter(ADDynamicGroup.ObjectCategoryNameInternal),
					ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
					ADObject.ObjectCategoryFilter(ADPublicFolder.MostDerivedClass)
				})
			});
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.DynamicDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.UserMailbox] = true;
			recipientTypeBitVector[RecipientType.MailContact] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			recipientTypeBitVector[RecipientType.MailUser] = true;
			recipientTypeBitVector[RecipientType.MailNonUniversalGroup] = true;
			recipientTypeBitVector[RecipientType.PublicFolder] = true;
			recipientTypeBitVector[RecipientType.SystemAttendantMailbox] = true;
			recipientTypeBitVector[RecipientType.SystemMailbox] = true;
			value = new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				new OrFilter(new QueryFilter[]
				{
					ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
					ADObject.ObjectCategoryFilter(ADDynamicGroup.ObjectCategoryNameInternal),
					ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
					ADObject.ObjectCategoryFilter(ADPublicFolder.MostDerivedClass),
					ADObject.ObjectCategoryFilter(ADSystemAttendantMailbox.MostDerivedClass),
					ADObject.ObjectCategoryFilter(ADSystemMailbox.MostDerivedClass)
				})
			});
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.DynamicDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalDistributionGroup] = true;
			recipientTypeBitVector[RecipientType.MailUniversalSecurityGroup] = true;
			value = new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				new OrFilter(new QueryFilter[]
				{
					ADObject.ObjectCategoryFilter(ADDynamicGroup.ObjectCategoryNameInternal),
					new AndFilter(new QueryFilter[]
					{
						ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
						Filters.CreateUniversalGroupFilter(true)
					})
				})
			});
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			recipientTypeBitVector[RecipientType.MailContact] = true;
			recipientTypeBitVector[RecipientType.MailUser] = true;
			value = new AndFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				Filters.CreateMailEnabledFilter(true),
				new OrFilter(new QueryFilter[]
				{
					ADObject.ObjectClassFilter(ADContact.MostDerivedClass),
					Filters.CreateMailboxEnabledFilter(false)
				})
			});
			dictionary.Add(recipientTypeBitVector.Data, value);
			recipientTypeBitVector.Reset();
			return dictionary;
		}

		private static Dictionary<RecipientTypeDetails, QueryFilter> InitializeStaticRecipientTypeDetailsFilterOptimizations()
		{
			Dictionary<RecipientTypeDetails, QueryFilter> dictionary = new Dictionary<RecipientTypeDetails, QueryFilter>(32);
			RecipientTypeDetails key = RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.MailContact | RecipientTypeDetails.MailUser | RecipientTypeDetails.MailUniversalDistributionGroup | RecipientTypeDetails.MailUniversalSecurityGroup | RecipientTypeDetails.DynamicDistributionGroup | RecipientTypeDetails.MailForestContact | RecipientTypeDetails.RoomList | RecipientTypeDetails.DiscoveryMailbox | RecipientTypeDetails.RemoteUserMailbox | RecipientTypeDetails.RemoteRoomMailbox | RecipientTypeDetails.RemoteEquipmentMailbox | RecipientTypeDetails.RemoteSharedMailbox | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.RemoteTeamMailbox | RecipientTypeDetails.GroupMailbox | RecipientTypeDetails.LinkedRoomMailbox | RecipientTypeDetails.RemoteGroupMailbox;
			dictionary.Add(key, new AndFilter(new QueryFilter[]
			{
				Filters.AllMailableUsersContactsDDLsUniversalGroupsFilter,
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan))
			}));
			dictionary.Add(RecipientTypeDetails.User | RecipientTypeDetails.DisabledUser, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.User),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LinkedUser))
			}));
			dictionary.Add(RecipientTypeDetails.UniversalDistributionGroup | RecipientTypeDetails.UniversalSecurityGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.Group),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoleGroup)),
				Filters.CreateUniversalGroupFilter(true)
			}));
			dictionary.Add(RecipientTypeDetails.MailContact | RecipientTypeDetails.MailForestContact, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailContact)
			}));
			key = (RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.MailUser | RecipientTypeDetails.User | RecipientTypeDetails.DisabledUser | RecipientTypeDetails.RemoteUserMailbox | RecipientTypeDetails.RemoteRoomMailbox | RecipientTypeDetails.RemoteEquipmentMailbox | RecipientTypeDetails.RemoteSharedMailbox | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.RemoteTeamMailbox);
			dictionary.Add(key, new OrFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					ADUser.ImplicitFilterInternal,
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue))
				}),
				new AndFilter(new QueryFilter[]
				{
					ADUser.ImplicitFilterInternal,
					new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.DiscoveryMailbox)),
					new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LinkedUser))
				})
			}));
			key = (RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.MailContact | RecipientTypeDetails.MailUser | RecipientTypeDetails.MailUniversalDistributionGroup | RecipientTypeDetails.MailNonUniversalGroup | RecipientTypeDetails.MailUniversalSecurityGroup | RecipientTypeDetails.DynamicDistributionGroup | RecipientTypeDetails.PublicFolder | RecipientTypeDetails.MailForestContact | RecipientTypeDetails.RoomList | RecipientTypeDetails.DiscoveryMailbox | RecipientTypeDetails.RemoteUserMailbox | RecipientTypeDetails.RemoteRoomMailbox | RecipientTypeDetails.RemoteEquipmentMailbox | RecipientTypeDetails.RemoteSharedMailbox | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.RemoteTeamMailbox | RecipientTypeDetails.LinkedRoomMailbox);
			dictionary.Add(key, Filters.AllRecipientsForGetRecipientTask);
			key = (RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.DiscoveryMailbox | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.GroupMailbox | RecipientTypeDetails.LinkedRoomMailbox);
			dictionary.Add(key, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan))
			}));
			key = (RecipientTypeDetails.MailUniversalDistributionGroup | RecipientTypeDetails.MailNonUniversalGroup | RecipientTypeDetails.MailUniversalSecurityGroup | RecipientTypeDetails.RoomList);
			dictionary.Add(key, new OrFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailNonUniversalGroup),
				Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalDistributionGroup),
				Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalSecurityGroup)
			}));
			key = (RecipientTypeDetails.MailUniversalDistributionGroup | RecipientTypeDetails.MailNonUniversalGroup | RecipientTypeDetails.MailUniversalSecurityGroup);
			dictionary.Add(key, new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					Filters.CreateRecipientTypeFilter(RecipientType.MailNonUniversalGroup),
					Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalDistributionGroup),
					Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalSecurityGroup)
				}),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoomList))
			}));
			dictionary.Add(RecipientTypeDetails.RoomMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoomMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.LinkedRoomMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LinkedRoomMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.EquipmentMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.EquipmentMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.LinkedMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LinkedMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.UserMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.UserMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.MailForestContact, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailContact),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailForestContact)
			}));
			dictionary.Add(RecipientTypeDetails.SharedMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.SharedMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.TeamMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.TeamMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.RemoteGroupMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalDistributionGroup),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteGroupMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.GroupMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.GroupMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.ArbitrationMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.MailboxPlan, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan)
			}));
			dictionary.Add(RecipientTypeDetails.LinkedUser, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.User),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LinkedUser)
			}));
			dictionary.Add(RecipientTypeDetails.RoomList, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalDistributionGroup),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoomList)
			}));
			dictionary.Add(RecipientTypeDetails.DiscoveryMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.DiscoveryMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.AuditLogMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.LegacyMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.UserMailbox),
				new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue)),
					Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.LegacyMailbox)
				})
			}));
			dictionary.Add(RecipientTypeDetails.MailContact, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailContact),
				new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue)),
					Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailContact)
				})
			}));
			dictionary.Add(RecipientTypeDetails.User, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.User),
				Filters.UserEnabledFilter
			}));
			dictionary.Add(RecipientTypeDetails.Contact, Filters.CreateRecipientTypeFilter(RecipientType.Contact));
			dictionary.Add(RecipientTypeDetails.UniversalDistributionGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.Group),
				Filters.UniversalDistributionGroupFilter
			}));
			dictionary.Add(RecipientTypeDetails.UniversalSecurityGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.Group),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoleGroup)),
				Filters.UniversalSecurityGroupFilter
			}));
			dictionary.Add(RecipientTypeDetails.RoleGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.Group),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RoleGroup),
				Filters.UniversalSecurityGroupFilter
			}));
			dictionary.Add(RecipientTypeDetails.NonUniversalGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.Group),
				Filters.NonUniversalGroupFilter
			}));
			dictionary.Add(RecipientTypeDetails.DisabledUser, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.User),
				Filters.UserDisabledFilter,
				new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue)),
					Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.DisabledUser)
				})
			}));
			key = (RecipientTypeDetails.UserMailbox | RecipientTypeDetails.LinkedMailbox | RecipientTypeDetails.SharedMailbox | RecipientTypeDetails.LegacyMailbox | RecipientTypeDetails.RoomMailbox | RecipientTypeDetails.EquipmentMailbox | RecipientTypeDetails.MailContact | RecipientTypeDetails.MailUser | RecipientTypeDetails.MailForestContact | RecipientTypeDetails.DiscoveryMailbox | RecipientTypeDetails.RemoteUserMailbox | RecipientTypeDetails.RemoteRoomMailbox | RecipientTypeDetails.RemoteEquipmentMailbox | RecipientTypeDetails.RemoteSharedMailbox | RecipientTypeDetails.TeamMailbox | RecipientTypeDetails.RemoteTeamMailbox | RecipientTypeDetails.GroupMailbox | RecipientTypeDetails.LinkedRoomMailbox | RecipientTypeDetails.RemoteGroupMailbox);
			dictionary.Add(key, new AndFilter(new QueryFilter[]
			{
				Filters.CreateMailEnabledFilter(true),
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox)),
				new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan))
			}));
			dictionary.Add(RecipientTypeDetails.MailUser, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				new OrFilter(new QueryFilter[]
				{
					Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailUser),
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue))
				})
			}));
			dictionary.Add(RecipientTypeDetails.MailUniversalDistributionGroup, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalDistributionGroup),
				new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientTypeDetailsValue)),
					Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailUniversalDistributionGroup)
				})
			}));
			dictionary.Add(RecipientTypeDetails.MailUniversalSecurityGroup, Filters.CreateRecipientTypeFilter(RecipientType.MailUniversalSecurityGroup));
			dictionary.Add(RecipientTypeDetails.MailNonUniversalGroup, Filters.CreateRecipientTypeFilter(RecipientType.MailNonUniversalGroup));
			dictionary.Add(RecipientTypeDetails.DynamicDistributionGroup, Filters.CreateRecipientTypeFilter(RecipientType.DynamicDistributionGroup));
			dictionary.Add(RecipientTypeDetails.PublicFolder, Filters.CreateRecipientTypeFilter(RecipientType.PublicFolder));
			dictionary.Add(RecipientTypeDetails.MicrosoftExchange, Filters.CreateRecipientTypeFilter(RecipientType.MicrosoftExchange));
			dictionary.Add(RecipientTypeDetails.SystemAttendantMailbox, Filters.CreateRecipientTypeFilter(RecipientType.SystemAttendantMailbox));
			dictionary.Add(RecipientTypeDetails.SystemMailbox, Filters.CreateRecipientTypeFilter(RecipientType.SystemMailbox));
			dictionary.Add((RecipientTypeDetails)((ulong)int.MinValue), new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				Filters.CreateRecipientTypeDetailsValueFilter((RecipientTypeDetails)((ulong)int.MinValue))
			}));
			dictionary.Add(RecipientTypeDetails.Computer, Filters.CreateRecipientTypeFilter(RecipientType.Computer));
			dictionary.Add(RecipientTypeDetails.RemoteRoomMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteRoomMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.RemoteEquipmentMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteEquipmentMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.RemoteTeamMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteTeamMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.RemoteSharedMailbox, new AndFilter(new QueryFilter[]
			{
				Filters.CreateRecipientTypeFilter(RecipientType.MailUser),
				Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteSharedMailbox)
			}));
			dictionary.Add(RecipientTypeDetails.PublicFolderMailbox, Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.PublicFolderMailbox));
			dictionary.Add(RecipientTypeDetails.MonitoringMailbox, Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MonitoringMailbox));
			return dictionary;
		}

		public static readonly QueryFilter DefaultRecipientFilter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "person"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchDynamicDistributionList"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "group"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "publicFolder"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchPublicMDB"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchSystemMailbox"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADMicrosoftExchangeRecipient.MostDerivedClass),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "exchangeAdminService"),
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "computer")
		});

		internal static readonly int RecipientTypeCount = Enum.GetValues(typeof(RecipientType)).Length;

		private static readonly QueryFilter UserEnabledFilter = Filters.CreateUserEnabledFilter(true);

		private static readonly QueryFilter UserDisabledFilter = Filters.CreateUserEnabledFilter(false);

		private static readonly QueryFilter NonUniversalGroupFilter = Filters.CreateUniversalGroupFilter(false);

		private static readonly QueryFilter UniversalSecurityGroupFilter = new BitMaskAndFilter(ADGroupSchema.GroupType, (ulong)-2147483640);

		private static readonly QueryFilter UniversalDistributionGroupFilter = new AndFilter(new QueryFilter[]
		{
			Filters.CreateUniversalGroupFilter(true),
			Filters.CreateSecurityGroupFilter(false)
		});

		private static readonly QueryFilter AllMailableUsersContactsDDLsUniversalGroupsFilter = new AndFilter(new QueryFilter[]
		{
			Filters.CreateMailEnabledFilter(true),
			new OrFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				ADObject.ObjectCategoryFilter(ADDynamicGroup.ObjectCategoryNameInternal),
				new AndFilter(new QueryFilter[]
				{
					ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
					Filters.CreateUniversalGroupFilter(true)
				})
			})
		});

		private static readonly QueryFilter AllRecipientsForGetRecipientTask = new AndFilter(new QueryFilter[]
		{
			Filters.CreateMailEnabledFilter(true),
			new OrFilter(new QueryFilter[]
			{
				ADObject.ObjectCategoryFilter(ADUser.ObjectCategoryNameInternal),
				ADObject.ObjectCategoryFilter(ADDynamicGroup.ObjectCategoryNameInternal),
				ADObject.ObjectCategoryFilter(ADGroup.MostDerivedClass),
				ADObject.ObjectCategoryFilter(ADPublicFolder.MostDerivedClass)
			}),
			new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)),
			new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan)),
			new NotFilter(Filters.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox))
		});

		internal static readonly QueryFilter[] RecipientTypeFilters = Filters.InitializeStaticRecipientTypeFilters();

		private static readonly Dictionary<int, QueryFilter> RecipientTypeFilterOptimizations = Filters.InitializeStaticRecipientTypeFilterOptimizations();

		private static readonly Dictionary<RecipientTypeDetails, QueryFilter> RecipientTypeDetailsFilterOptimizations = Filters.InitializeStaticRecipientTypeDetailsFilterOptimizations();

		private struct RecipientTypeBitVector32
		{
			internal int Data
			{
				get
				{
					return this.data;
				}
			}

			internal void Reset()
			{
				this.data = 0;
			}

			internal bool this[RecipientType index]
			{
				set
				{
					if (value)
					{
						this.data |= 1 << (int)index;
						return;
					}
					this.data &= ~(1 << (int)index);
				}
			}

			private int data;
		}
	}
}

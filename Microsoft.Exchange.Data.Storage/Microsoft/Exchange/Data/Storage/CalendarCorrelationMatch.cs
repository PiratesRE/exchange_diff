using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarCorrelationMatch : IComparable<CalendarCorrelationMatch>
	{
		private static SortBy[] GetRelatedItemsSortOrder(PropertyDefinition relationBase)
		{
			return new SortBy[]
			{
				new SortBy(relationBase, SortOrder.Ascending)
			};
		}

		private static SortBy[] GetRelatedItemsChronologicalSortOrder(PropertyDefinition relationBase)
		{
			return new SortBy[]
			{
				new SortBy(relationBase, SortOrder.Ascending),
				new SortBy(InternalSchema.ItemVersion, SortOrder.Descending),
				new SortBy(InternalSchema.OriginalLastModifiedTime, SortOrder.Descending)
			};
		}

		private static SortBy[] GetRelatedItemsByClassSortOrder(PropertyDefinition relationBase)
		{
			return new SortBy[]
			{
				new SortBy(relationBase, SortOrder.Ascending),
				new SortBy(InternalSchema.ItemClass, SortOrder.Ascending)
			};
		}

		private static SortBy[] GetRelatedItemsByClassChronologicalSortOrder(PropertyDefinition relationBase)
		{
			return new SortBy[]
			{
				new SortBy(relationBase, SortOrder.Ascending),
				new SortBy(InternalSchema.ItemClass, SortOrder.Ascending),
				new SortBy(InternalSchema.ItemVersion, SortOrder.Descending),
				new SortBy(InternalSchema.OriginalLastModifiedTime, SortOrder.Descending)
			};
		}

		private static ICollection<PropertyDefinition> DefaultPropertySet
		{
			get
			{
				return CalendarCorrelationMatch.GetPropertySet("{651EFB55-4E21-44c3-8338-81A2502FA65D}", null, true);
			}
		}

		internal PropertyBag Properties
		{
			get
			{
				return this.properties;
			}
		}

		private static ICollection<PropertyDefinition> GetPropertySet(string schemaKey, ICollection<PropertyDefinition> additionalRequiredProperties, bool useCache)
		{
			ICollection<PropertyDefinition> collection = null;
			if (useCache)
			{
				if (schemaKey == null)
				{
					throw new ArgumentNullException("schemaKey");
				}
				lock (CalendarCorrelationMatch.threadSafetyLock)
				{
					if (!CalendarCorrelationMatch.requiredPropertiesCache.TryGetValue(schemaKey, out collection))
					{
						collection = CalendarCorrelationMatch.CreateNewPropertySet(additionalRequiredProperties);
						CalendarCorrelationMatch.requiredPropertiesCache.Add(schemaKey, collection);
					}
					return collection;
				}
			}
			collection = CalendarCorrelationMatch.CreateNewPropertySet(additionalRequiredProperties);
			return collection;
		}

		private static ICollection<PropertyDefinition> CreateNewPropertySet(ICollection<PropertyDefinition> additionalProperties)
		{
			ICollection<PropertyDefinition> collection = (additionalProperties == null || additionalProperties.Count == 0) ? CalendarCorrelationMatch.CorrelatedItemViewProperties : CalendarCorrelationMatch.CorrelatedItemViewProperties.Union(additionalProperties);
			return CalendarCorrelationMatch.GetNativeProperties(collection);
		}

		private static ICollection<PropertyDefinition> GetNativeProperties(ICollection<PropertyDefinition> properties)
		{
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.NeedForRead, properties);
			ICollection<PropertyDefinition> collection = new List<PropertyDefinition>(nativePropertyDefinitions.Count);
			foreach (PropertyDefinition item in nativePropertyDefinitions)
			{
				collection.Add(item);
			}
			return collection;
		}

		private CalendarCorrelationMatch(PropertyBag propertyBag, GlobalObjectId globalObjectId)
		{
			this.Id = propertyBag.GetValueOrDefault<VersionedId>(InternalSchema.ItemId);
			this.documentId = propertyBag.GetValueOrDefault<int>(InternalSchema.DocumentId, int.MinValue);
			byte[] goidBytes = CalendarCorrelationMatch.GetGoidBytes(propertyBag);
			this.goid = ((goidBytes == null) ? null : new GlobalObjectId(goidBytes));
			object obj = propertyBag.TryGetProperty(InternalSchema.AppointmentRecurrenceBlob);
			this.isRecurringMaster = (obj is byte[] || PropertyError.IsPropertyValueTooBig(obj));
			this.appointmentSequenceNumber = propertyBag.GetValueAsNullable<int>(InternalSchema.AppointmentSequenceNumber);
			this.lastModifiedTime = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.LastModifiedTime);
			this.ownerCriticalChangeTime = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.OwnerCriticalChangeTime);
			this.IsCorrelated = this.CheckIsCorrelated(globalObjectId, out this.isMasterMatchingTheOccurrence);
			this.properties = propertyBag;
		}

		private static byte[] GetGoidBytes(PropertyBag propertyBag)
		{
			return propertyBag.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId) ?? propertyBag.GetValueOrDefault<byte[]>(InternalSchema.CleanGlobalObjectId);
		}

		private static void ValidateAdditionalProperties(ICollection<PropertyDefinition> additionalProperties)
		{
			if (additionalProperties == null || additionalProperties.Count == 0)
			{
				return;
			}
			foreach (PropertyDefinition propertyDefinition in additionalProperties)
			{
				if (!RecurrenceManager.CanPropertyBeInExceptionData(propertyDefinition) && !RecurrenceManager.MasterOnlyProperties.Contains(propertyDefinition))
				{
					throw new ArgumentException(string.Format("[CalendarCorrelationMatch.ValidateAdditionalProperties] Property '{0}' cannot be requested because it is not in the O11 blob properties nor master property list", propertyDefinition.Name));
				}
			}
		}

		internal static List<CalendarCorrelationMatch> FindMatches(CalendarFolder folder, GlobalObjectId globalObjectId, ICollection<PropertyDefinition> additionalProperties = null)
		{
			CalendarCorrelationMatch.ValidateAdditionalProperties(additionalProperties);
			CalendarCorrelationMatch.CalendarCorrelationMatchCollection matchCollection = new CalendarCorrelationMatch.CalendarCorrelationMatchCollection();
			CalendarCorrelationMatch.QueryRelatedItems(folder, globalObjectId, CalendarCorrelationMatch.GetPropertySet(CalendarCorrelationMatch.BuildSchemaKey(additionalProperties), additionalProperties, true), delegate(PropertyBag match)
			{
				matchCollection.AddMatch(globalObjectId, match);
				return true;
			}, false, false, null, null, null);
			return matchCollection.FoundMatches;
		}

		private static string BuildSchemaKey(ICollection<PropertyDefinition> additionalProperties)
		{
			if (additionalProperties == null || additionalProperties.Count == 0)
			{
				return "{651EFB55-4E21-44c3-8338-81A2502FA65D}";
			}
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in additionalProperties)
			{
				num ^= propertyDefinition.Name.GetHashCode();
			}
			return string.Format("{0}_{1}", "{651EFB55-4E21-44c3-8338-81A2502FA65D}", num);
		}

		internal static void QuerySubjectContains(Folder folder, string subject, string schemaKey, ICollection<PropertyDefinition> propertiesToFetch, bool useCachedPropertySetIfPresent, Action<PropertyBag> matchFoundAction, ExDateTime startDate, ExDateTime endDate)
		{
			string normalizedSubject;
			string text;
			SubjectProperty.ComputeSubjectPrefix(subject, out text, out normalizedSubject);
			CalendarCorrelationMatch.QueryItemsUsingView(folder, new SortBy[]
			{
				new SortBy(InternalSchema.OriginalLastModifiedTime, SortOrder.Descending)
			}, null, CalendarCorrelationMatch.GetPropertySet(schemaKey, propertiesToFetch, useCachedPropertySetIfPresent), delegate(PropertyBag propertyBag)
			{
				string text2 = propertyBag.GetValueOrDefault<string>(InternalSchema.NormalizedSubject);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = string.Empty;
				}
				if (text2.IndexOf(normalizedSubject, StringComparison.CurrentCultureIgnoreCase) != -1 && CalendarCorrelationMatch.MatchOriginalLastModifiedTime(propertyBag, new ExDateTime?(startDate), new ExDateTime?(endDate)))
				{
					matchFoundAction(propertyBag);
				}
				return true;
			});
		}

		public static void QueryRelatedItems(Folder folder, GlobalObjectId globalObjectId, string schemaKey, ICollection<PropertyDefinition> propertiesToFetch, bool useCachedPropertySetIfPresent, Func<PropertyBag, bool> matchFoundAction, bool fetchResultsInReverseChronologicalOrder, bool sameGoidOnly, string[] itemClassFilter, ExDateTime? startDate, ExDateTime? endDate)
		{
			Util.ThrowOnNullArgument(matchFoundAction, "matchFoundAction");
			CalendarCorrelationMatch.QueryRelatedItems(folder, globalObjectId, CalendarCorrelationMatch.GetPropertySet(schemaKey, propertiesToFetch, useCachedPropertySetIfPresent), matchFoundAction, fetchResultsInReverseChronologicalOrder, sameGoidOnly, itemClassFilter, startDate, endDate);
		}

		private static void QueryRelatedItems(Folder folder, GlobalObjectId globalObjectId, ICollection<PropertyDefinition> propertySet, Func<PropertyBag, bool> matchFoundAction, bool fetchResultsInReverseChronologicalOrder, bool sameGoidOnly, string[] itemClassFilter, ExDateTime? startDate, ExDateTime? endDate)
		{
			Func<PropertyBag, bool> readRow = delegate(PropertyBag propertyBag)
			{
				if (itemClassFilter != null)
				{
					string classFromPropertyBag = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
					if (!itemClassFilter.Any((string itemClass) => ObjectClass.IsOfClass(classFromPropertyBag, itemClass)))
					{
						return false;
					}
				}
				byte[] goidBytes = CalendarCorrelationMatch.GetGoidBytes(propertyBag);
				bool flag = goidBytes != null && (sameGoidOnly ? GlobalObjectId.Equals(globalObjectId, new GlobalObjectId(goidBytes)) : GlobalObjectId.CompareCleanGlobalObjectIds(globalObjectId.Bytes, goidBytes));
				if (flag)
				{
					flag = CalendarCorrelationMatch.MatchOriginalLastModifiedTime(propertyBag, startDate, endDate);
				}
				if (flag)
				{
					flag = matchFoundAction(propertyBag);
				}
				return flag;
			};
			QueryFilter queryFilter = sameGoidOnly ? new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.GlobalObjectId, globalObjectId.Bytes) : new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.CleanGlobalObjectId, globalObjectId.CleanGlobalObjectIdBytes);
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, QueryFilter>(0L, "CalendarCorrelationMatch.QueryRelatedItems: GOID={0}. QueryFilter = {1}.", globalObjectId, queryFilter);
			SortBy[] sortBy;
			if (itemClassFilter != null)
			{
				if (sameGoidOnly)
				{
					sortBy = (fetchResultsInReverseChronologicalOrder ? CalendarCorrelationMatch.goidRestrictedRelatedItemsByClassChronologicalSortOrder : CalendarCorrelationMatch.goidRestrictedRelatedItemsByClassSortOrder);
				}
				else
				{
					sortBy = (fetchResultsInReverseChronologicalOrder ? CalendarCorrelationMatch.relatedItemsByClassChronologicalSortOrder : CalendarCorrelationMatch.relatedItemsByClassSortOrder);
				}
				QueryFilter[] array = new QueryFilter[itemClassFilter.Length];
				for (int i = 0; i < itemClassFilter.Length; i++)
				{
					array[i] = new TextFilter(InternalSchema.ItemClass, itemClassFilter[i], MatchOptions.Prefix, MatchFlags.IgnoreCase);
				}
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new OrFilter(array)
				});
			}
			else if (sameGoidOnly)
			{
				sortBy = (fetchResultsInReverseChronologicalOrder ? CalendarCorrelationMatch.goidRestrictedRelatedItemsChronologicalSortOrder : CalendarCorrelationMatch.goidRestrictedRelatedItemsSortOrder);
			}
			else
			{
				sortBy = (fetchResultsInReverseChronologicalOrder ? CalendarCorrelationMatch.relatedItemsChronologicalSortOrder : CalendarCorrelationMatch.relatedItemsSortOrder);
			}
			CalendarCorrelationMatch.QueryItemsUsingView(folder, sortBy, queryFilter, propertySet, readRow);
		}

		private static void QueryItemsUsingView(Folder folder, SortBy[] sortBy, QueryFilter condition, ICollection<PropertyDefinition> propertySet, Func<PropertyBag, bool> readRow)
		{
			long num = 0L;
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortBy, propertySet))
			{
				if (condition != null)
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, condition, (condition is ComparisonFilter) ? SeekToConditionFlags.None : SeekToConditionFlags.AllowExtendedFilters);
				}
				bool flag = false;
				while (!flag)
				{
					object[][] rows = queryResult.GetRows(50);
					flag = (rows.Length == 0);
					foreach (object[] queryResultRow in rows)
					{
						QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(folder.Session, propertySet);
						queryResultPropertyBag.SetQueryResultRow(queryResultRow);
						if (!readRow(queryResultPropertyBag) || (num += 1L) >= 1000L)
						{
							flag = true;
							break;
						}
					}
				}
			}
		}

		private static bool MatchOriginalLastModifiedTime(PropertyBag propertyBag, ExDateTime? startDate, ExDateTime? endDate)
		{
			if (startDate == null || endDate == null)
			{
				return true;
			}
			ExDateTime? valueAsNullable = propertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.OriginalLastModifiedTime);
			return valueAsNullable == null || (valueAsNullable >= startDate && valueAsNullable <= endDate);
		}

		private bool CheckIsCorrelated(GlobalObjectId globalObjectId, out bool isOccurrenceMatchedToMaster)
		{
			bool result = false;
			isOccurrenceMatchedToMaster = false;
			if (this.Id != null && this.Id.ObjectId.ObjectType == StoreObjectType.CalendarItem)
			{
				if (globalObjectId.Date == this.goid.Date)
				{
					result = true;
				}
				else if (!globalObjectId.IsCleanGlobalObjectId && this.goid.IsCleanGlobalObjectId && this.isRecurringMaster)
				{
					result = true;
					isOccurrenceMatchedToMaster = true;
				}
			}
			else if (this.Id != null && this.Id.ObjectId.ObjectType == StoreObjectType.CalendarItemSeries)
			{
				result = this.goid.Equals(globalObjectId);
			}
			return result;
		}

		internal VersionedId GetCorrelatedId(GlobalObjectId globalObjectId)
		{
			VersionedId result;
			if (this.isMasterMatchingTheOccurrence)
			{
				OccurrenceStoreObjectId itemId = new OccurrenceStoreObjectId(this.Id.ObjectId.ProviderLevelItemId, globalObjectId.Date);
				result = new VersionedId(itemId, this.Id.ChangeKeyAsByteArray());
			}
			else
			{
				result = this.Id;
			}
			return result;
		}

		public int CompareTo(CalendarCorrelationMatch other)
		{
			int num;
			if (this.Id == other.Id)
			{
				num = 0;
			}
			else
			{
				num = Nullable.Compare<int>(this.appointmentSequenceNumber, other.appointmentSequenceNumber);
				if (num == 0)
				{
					if (this.lastModifiedTime != null && other.lastModifiedTime != null)
					{
						num = this.lastModifiedTime.Value.CompareTo(other.lastModifiedTime.Value, CalendarCorrelationMatch.LastModifiedTimeTreshold);
					}
					if (num == 0)
					{
						num = Nullable.Compare<ExDateTime>(this.ownerCriticalChangeTime, other.ownerCriticalChangeTime);
						if (num == 0)
						{
							num = this.documentId.CompareTo(other.documentId);
						}
					}
				}
			}
			return num;
		}

		private const string DefaultPropertySetKey = "{651EFB55-4E21-44c3-8338-81A2502FA65D}";

		private const long MaxReturnedItems = 1000L;

		private static readonly TimeSpan LastModifiedTimeTreshold = TimeSpan.FromSeconds(5.0);

		private static readonly SortBy[] relatedItemsSortOrder = CalendarCorrelationMatch.GetRelatedItemsSortOrder(InternalSchema.CleanGlobalObjectId);

		private static readonly SortBy[] relatedItemsChronologicalSortOrder = CalendarCorrelationMatch.GetRelatedItemsChronologicalSortOrder(InternalSchema.CleanGlobalObjectId);

		private static readonly SortBy[] relatedItemsByClassSortOrder = CalendarCorrelationMatch.GetRelatedItemsByClassSortOrder(InternalSchema.CleanGlobalObjectId);

		private static readonly SortBy[] relatedItemsByClassChronologicalSortOrder = CalendarCorrelationMatch.GetRelatedItemsByClassChronologicalSortOrder(InternalSchema.CleanGlobalObjectId);

		private static readonly SortBy[] goidRestrictedRelatedItemsSortOrder = CalendarCorrelationMatch.GetRelatedItemsSortOrder(InternalSchema.GlobalObjectId);

		private static readonly SortBy[] goidRestrictedRelatedItemsChronologicalSortOrder = CalendarCorrelationMatch.GetRelatedItemsChronologicalSortOrder(InternalSchema.GlobalObjectId);

		private static readonly SortBy[] goidRestrictedRelatedItemsByClassSortOrder = CalendarCorrelationMatch.GetRelatedItemsByClassSortOrder(InternalSchema.GlobalObjectId);

		private static readonly SortBy[] goidRestrictedRelatedItemsByClassChronologicalSortOrder = CalendarCorrelationMatch.GetRelatedItemsByClassChronologicalSortOrder(InternalSchema.GlobalObjectId);

		private readonly VersionedId Id;

		private readonly PropertyBag properties;

		private readonly int documentId;

		private readonly GlobalObjectId goid;

		private readonly bool isRecurringMaster;

		private readonly int? appointmentSequenceNumber;

		private readonly ExDateTime? lastModifiedTime;

		private readonly ExDateTime? ownerCriticalChangeTime;

		private readonly bool IsCorrelated;

		private readonly bool isMasterMatchingTheOccurrence;

		private static object threadSafetyLock = new object();

		private static Dictionary<string, ICollection<PropertyDefinition>> requiredPropertiesCache = new Dictionary<string, ICollection<PropertyDefinition>>(1);

		internal static readonly ICollection<PropertyDefinition> CorrelatedItemViewProperties = new PropertyDefinition[]
		{
			InternalSchema.ItemId,
			InternalSchema.DocumentId,
			InternalSchema.GlobalObjectId,
			InternalSchema.CleanGlobalObjectId,
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.AppointmentSequenceNumber,
			InternalSchema.LastModifiedTime,
			InternalSchema.OwnerCriticalChangeTime,
			InternalSchema.OriginalLastModifiedTime
		};

		private class CalendarCorrelationMatchCollection
		{
			public List<CalendarCorrelationMatch> FoundMatches { get; private set; }

			public bool? IsMasterMatch { get; private set; }

			public CalendarCorrelationMatchCollection()
			{
				this.FoundMatches = new List<CalendarCorrelationMatch>();
			}

			public void AddMatch(GlobalObjectId globalObjectId, PropertyBag propertyBag)
			{
				CalendarCorrelationMatch matchData = new CalendarCorrelationMatch(propertyBag, globalObjectId);
				this.UpdateMatchCollection(matchData);
			}

			private void UpdateMatchCollection(CalendarCorrelationMatch matchData)
			{
				if (matchData.IsCorrelated)
				{
					bool flag = false;
					if (this.FoundMatches.Count == 0)
					{
						flag = true;
					}
					else
					{
						if (this.IsMasterMatch == null)
						{
							throw new ArgumentNullException("isPreviousMasterMatchingTheOccurrence");
						}
						if (this.IsMasterMatch.Value == matchData.isMasterMatchingTheOccurrence)
						{
							flag = true;
						}
						else if (this.IsMasterMatch.Value)
						{
							flag = true;
							this.FoundMatches.Clear();
						}
					}
					if (flag)
					{
						this.FoundMatches.Add(matchData);
						this.IsMasterMatch = new bool?(matchData.isMasterMatchingTheOccurrence);
					}
				}
			}
		}
	}
}

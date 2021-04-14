using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PropertySetMixer<CustomPropertyId, SetId>
	{
		internal PropertySetMixer() : this(new Predicate<CustomPropertyId>(PropertySetMixer<CustomPropertyId, SetId>.NeverIntercept))
		{
		}

		internal PropertySetMixer(Predicate<CustomPropertyId> shouldIntercept)
		{
			this.shouldIntercept = shouldIntercept;
			this.propsToIndicies = null;
			this.propsToIndicies = new SortedDictionary<CustomPropertyId, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex>(typeof(CustomPropertyId).GetTypeInfo().IsValueType ? null : new PropertySetMixer<CustomPropertyId, SetId>.DefaultComparer());
		}

		public static explicit operator CustomPropertyId[](PropertySetMixer<CustomPropertyId, SetId> v)
		{
			return v.GetMergedSet();
		}

		internal void AddSet(SetId setId, params CustomPropertyId[] propIds)
		{
			int num = this.maxIndex;
			PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping[] array = new PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping[propIds.Length];
			for (int i = 0; i < propIds.Length; i++)
			{
				if (this.propsToIndicies.ContainsKey(propIds[i]))
				{
					array[i] = new PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping(propIds[i], this.propsToIndicies[propIds[i]]);
				}
				else
				{
					array[i] = new PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping(propIds[i], this.maxIndex++, this.shouldIntercept(propIds[i]) ? -1 : this.maxIndexAfterInterception++);
					this.propsToIndicies[propIds[i]] = array[i].Index;
				}
			}
			this.propSetIdsToSets.Add(setId, new PropertySetMixer<CustomPropertyId, SetId>.PropertySet(array, num, this.maxIndex - num));
		}

		internal object[] FilterRow(object[] unfilteredRow, SetId setId)
		{
			PropertySetMixer<CustomPropertyId, SetId>.PropertySet propertySet = this.propSetIdsToSets[setId];
			object[] array = new object[propertySet.Mappings.Length];
			for (int i = 0; i < propertySet.Mappings.Length; i++)
			{
				array[i] = unfilteredRow[propertySet.Mappings[i].Index.BeforeInterception];
			}
			return array;
		}

		internal CustomPropertyId[] GetMergedSet()
		{
			CustomPropertyId[] array = new CustomPropertyId[this.maxIndex];
			foreach (KeyValuePair<CustomPropertyId, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex> keyValuePair in this.propsToIndicies)
			{
				array[keyValuePair.Value.BeforeInterception] = keyValuePair.Key;
			}
			return array;
		}

		internal CustomPropertyId[] GetFilteredMergedSet()
		{
			CustomPropertyId[] array = new CustomPropertyId[this.maxIndexAfterInterception];
			foreach (KeyValuePair<CustomPropertyId, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex> keyValuePair in this.propsToIndicies)
			{
				if (keyValuePair.Value.AfterInterception != -1)
				{
					array[keyValuePair.Value.AfterInterception] = keyValuePair.Key;
				}
			}
			return array;
		}

		internal object[] GetProperties(object[] unfilteredRow, params CustomPropertyId[] propIds)
		{
			object[] array = new object[propIds.Length];
			for (int i = 0; i < propIds.Length; i++)
			{
				array[i] = this.TryGetProperty(unfilteredRow, propIds[i]);
			}
			return array;
		}

		internal CustomPropertyId[] GetSet(SetId id)
		{
			PropertySetMixer<CustomPropertyId, SetId>.PropertySet propertySet = this.propSetIdsToSets[id];
			CustomPropertyId[] array = new CustomPropertyId[propertySet.Mappings.Length];
			for (int i = 0; i < propertySet.Mappings.Length; i++)
			{
				array[i] = propertySet.Mappings[i].Id;
			}
			return array;
		}

		internal void MigrateSets(PropertySetMixer<CustomPropertyId, SetId> from, params SetId[] setIds)
		{
			foreach (SetId setId in setIds)
			{
				this.AddSet(setId, from.GetSet(setId));
			}
		}

		internal object[] RemitFilteredOffProperties(object[] filteredRow)
		{
			object[] array = new object[this.maxIndex];
			foreach (KeyValuePair<CustomPropertyId, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex> keyValuePair in this.propsToIndicies)
			{
				if (keyValuePair.Value.AfterInterception != -1)
				{
					array[keyValuePair.Value.BeforeInterception] = filteredRow[keyValuePair.Value.AfterInterception];
				}
				else
				{
					array[keyValuePair.Value.BeforeInterception] = null;
				}
			}
			return array;
		}

		internal void SetProperties(object[] unfilteredRowToModify, CustomPropertyId[] propIds, object[] newValues)
		{
			for (int i = 0; i < propIds.Length; i++)
			{
				this.SetProperty(unfilteredRowToModify, propIds[i], newValues[i]);
			}
		}

		internal void SetProperty(object[] unfilteredRowToModify, CustomPropertyId propId, object newValue)
		{
			if (this.propsToIndicies.ContainsKey(propId))
			{
				unfilteredRowToModify[this.propsToIndicies[propId].BeforeInterception] = newValue;
			}
		}

		public object TryGetProperty(object[] unfilteredRow, CustomPropertyId propId)
		{
			return unfilteredRow[this.propsToIndicies[propId].BeforeInterception];
		}

		private static bool NeverIntercept(CustomPropertyId propId)
		{
			return false;
		}

		private int maxIndex;

		private int maxIndexAfterInterception;

		private IDictionary<SetId, PropertySetMixer<CustomPropertyId, SetId>.PropertySet> propSetIdsToSets = new SortedDictionary<SetId, PropertySetMixer<CustomPropertyId, SetId>.PropertySet>();

		private IDictionary<CustomPropertyId, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex> propsToIndicies;

		private Predicate<CustomPropertyId> shouldIntercept;

		private struct PropertyIndex
		{
			public PropertyIndex(int beforeInterception, int afterInterception)
			{
				this.BeforeInterception = beforeInterception;
				this.AfterInterception = afterInterception;
			}

			public readonly int AfterInterception;

			public readonly int BeforeInterception;
		}

		private struct PropertyMapping
		{
			public PropertyMapping(CustomPropertyId id, PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex index)
			{
				this.Id = id;
				this.Index = index;
			}

			public PropertyMapping(CustomPropertyId id, int totalIndex, int afterInterceptionIndex)
			{
				this = new PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping(id, new PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex(totalIndex, afterInterceptionIndex));
			}

			public readonly CustomPropertyId Id;

			public readonly PropertySetMixer<CustomPropertyId, SetId>.PropertyIndex Index;
		}

		private struct PropertySet
		{
			public PropertySet(PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping[] mappings, int deltaStartIndex, int deltaCount)
			{
				this.Mappings = mappings;
				this.DeltaStartIndex = deltaStartIndex;
				this.DeltaCount = deltaCount;
			}

			public readonly int DeltaCount;

			public readonly int DeltaStartIndex;

			public readonly PropertySetMixer<CustomPropertyId, SetId>.PropertyMapping[] Mappings;
		}

		private class DefaultComparer : IComparer<CustomPropertyId>
		{
			public int Compare(CustomPropertyId x, CustomPropertyId y)
			{
				if (!this.Equals(x, y))
				{
					if (x.GetType() == y.GetType())
					{
						IComparable comparable = x as IComparable;
						IComparable comparable2 = y as IComparable;
						if (comparable != null && comparable2 != null)
						{
							return comparable.CompareTo(comparable2);
						}
					}
					return Util.GetClassComparable(x).CompareTo(Util.GetClassComparable(y));
				}
				return 0;
			}

			public bool Equals(CustomPropertyId x, CustomPropertyId y)
			{
				return x.Equals(y);
			}
		}
	}
}

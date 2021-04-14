using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MemoryPropertyBag : PropertyBag, IDictionary<PropertyDefinition, object>, ICollection<KeyValuePair<PropertyDefinition, object>>, IEnumerable<KeyValuePair<PropertyDefinition, object>>, IEnumerable
	{
		public MemoryPropertyBag()
		{
		}

		internal MemoryPropertyBag(MemoryPropertyBag propertyBag) : base(propertyBag)
		{
			this.ExTimeZone = propertyBag.ExTimeZone;
			this.HasAllPropertiesLoaded = propertyBag.HasAllPropertiesLoaded;
			if (propertyBag.propertyValues != null && propertyBag.propertyValues.Count > 0)
			{
				this.propertyValues = new Dictionary<PropertyDefinition, object>(propertyBag.propertyValues);
			}
		}

		private void EnsureInternalDataStructuresAllocated(int capacity)
		{
			if (this.propertyValues == null)
			{
				this.propertyValues = new Dictionary<PropertyDefinition, object>(capacity);
			}
		}

		public override bool IsDirty
		{
			get
			{
				return this.changedProperties != null && this.changedProperties.Count != 0;
			}
		}

		public override PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition)
		{
			StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			if ((storePropertyDefinition.PropertyFlags & PropertyFlags.TrackChange) != PropertyFlags.TrackChange)
			{
				return PropertyValueTrackingData.PropertyValueTrackDataNotTracked;
			}
			if (this.TrackedPropertyInformation != null && this.TrackedPropertyInformation.ContainsKey(propertyDefinition))
			{
				return this.TrackedPropertyInformation[propertyDefinition];
			}
			return PropertyValueTrackingData.PropertyValueTrackDataUnchanged;
		}

		public int Count
		{
			get
			{
				if (this.propertyValues != null)
				{
					return this.propertyValues.Count;
				}
				return 0;
			}
		}

		bool ICollection<KeyValuePair<PropertyDefinition, object>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public ICollection<PropertyDefinition> Keys
		{
			get
			{
				return this.PropertyValues.Keys;
			}
		}

		ICollection<object> IDictionary<PropertyDefinition, object>.Values
		{
			get
			{
				return this.PropertyValues.Values;
			}
		}

		void ICollection<KeyValuePair<PropertyDefinition, object>>.Add(KeyValuePair<PropertyDefinition, object> keyValuePair)
		{
			throw new InvalidOperationException("Readonly ICollection implementation");
		}

		bool ICollection<KeyValuePair<PropertyDefinition, object>>.Remove(KeyValuePair<PropertyDefinition, object> keyValuePair)
		{
			throw new InvalidOperationException("Readonly ICollection implementation");
		}

		bool ICollection<KeyValuePair<PropertyDefinition, object>>.Contains(KeyValuePair<PropertyDefinition, object> keyValuePair)
		{
			return this.PropertyValues.Contains(keyValuePair);
		}

		void IDictionary<PropertyDefinition, object>.Add(PropertyDefinition key, object value)
		{
			throw new InvalidOperationException("Readonly IDictionary implementation");
		}

		bool IDictionary<PropertyDefinition, object>.Remove(PropertyDefinition key)
		{
			throw new InvalidOperationException("Readonly IDictionary implementation");
		}

		bool IDictionary<PropertyDefinition, object>.TryGetValue(PropertyDefinition key, out object value)
		{
			return this.PropertyValues.TryGetValue(key, out value);
		}

		public bool ContainsKey(PropertyDefinition key)
		{
			return this.PropertyValues.ContainsKey(key);
		}

		IEnumerator<KeyValuePair<PropertyDefinition, object>> IEnumerable<KeyValuePair<PropertyDefinition, object>>.GetEnumerator()
		{
			return this.PropertyValues.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.PropertyValues.GetEnumerator();
		}

		void ICollection<KeyValuePair<PropertyDefinition, object>>.CopyTo(KeyValuePair<PropertyDefinition, object>[] array, int index)
		{
			this.PropertyValues.CopyTo(array, index);
		}

		public override void Load(ICollection<PropertyDefinition> properties)
		{
			if (properties == null)
			{
				return;
			}
			this.EnsureInternalDataStructuresAllocated(properties.Count);
			StorePropertyDefinition.PerformActionOnNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.AllRead, properties, delegate(NativeStorePropertyDefinition nativeProperty)
			{
				if (!this.IsLoaded(nativeProperty))
				{
					this.MarkAsNotFound(nativeProperty);
				}
			});
		}

		public void LoadFromStorePropertyBag(IStorePropertyBag storePropertyBag, ICollection<PropertyDefinition> properties)
		{
			ArgumentValidator.ThrowIfNull("storePropertyBag", storePropertyBag);
			ArgumentValidator.ThrowIfNull("properties", properties);
			this.EnsureInternalDataStructuresAllocated(properties.Count);
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				object value = storePropertyBag.TryGetProperty(propertyDefinition);
				this.InternalSetValidatedStoreProperty(propertyDefinition, value);
			}
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyDefinition", 1));
			}
			if (propertyValue == null)
			{
				throw new ArgumentNullException(ServerStrings.ExNullParameter("propertyValue", 2));
			}
			this.InternalSetValidatedStoreProperty(propertyDefinition, propertyValue);
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			object obj;
			if (this.propertyValues == null || !this.propertyValues.TryGetValue(propertyDefinition, out obj))
			{
				if (!this.HasAllPropertiesLoaded)
				{
					throw new NotInBagPropertyErrorException(propertyDefinition);
				}
				return this.MarkAsNotFound(propertyDefinition);
			}
			else
			{
				if (obj is ExDateTime)
				{
					return this.ExTimeZone.ConvertDateTime((ExDateTime)obj);
				}
				return obj;
			}
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			this.EnsureInternalDataStructuresAllocated(8);
			if (!this.DeletedProperties.Contains(propertyDefinition))
			{
				this.DeletedProperties.Add(propertyDefinition);
			}
			this.ChangedProperties.TryAdd(propertyDefinition);
			this.AddTrackingInformation(propertyDefinition, PropertyTrackingInformation.Deleted, null);
			this.propertyValues[propertyDefinition] = new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			return this.changedProperties != null && this.changedProperties.Contains(propertyDefinition);
		}

		internal ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				if (this.propertyValues == null || this.propertyValues.Count == 0)
				{
					return MemoryPropertyBag.emptyNativeList;
				}
				return new MemoryPropertyBag.NativeFilter(this.propertyValues, new Action(this.MarkAsReadOnly), new Action(this.UnmarkAsReadOnly));
			}
		}

		internal ICollection<PropertyDefinition> DeleteList
		{
			get
			{
				if (this.deletedProperties == null)
				{
					return MemoryPropertyBag.emptyList;
				}
				return new ReadOnlyCollection<PropertyDefinition>(this.deletedProperties);
			}
		}

		internal ICollection<PropertyDefinition> ChangeList
		{
			get
			{
				return this.changedProperties ?? MemoryPropertyBag.emptyList;
			}
		}

		private IDictionary<PropertyDefinition, object> PropertyValues
		{
			get
			{
				this.EnsureInternalDataStructuresAllocated(8);
				return this.propertyValues;
			}
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.exTimeZone;
			}
			set
			{
				this.exTimeZone = value;
			}
		}

		public ICollection<PropertyDefinition> AllFoundProperties
		{
			get
			{
				if (this.propertyValues == null || this.propertyValues.Count == 0)
				{
					return MemoryPropertyBag.emptyList;
				}
				return new MemoryPropertyBag.FoundFilter(this.propertyValues, new Action(this.MarkAsReadOnly), new Action(this.UnmarkAsReadOnly));
			}
		}

		public void PreLoadStoreProperty<T>(ICollection<T> properties, object[] values) where T : StorePropertyDefinition
		{
			Util.ThrowOnNullArgument(properties, "properties");
			Util.ThrowOnNullArgument(values, "values");
			if (properties.Count != values.Length)
			{
				throw new ArgumentException("properties and values have mismatched elements");
			}
			ICollection<PropValue> collection = new ComputedElementCollection<T, object, PropValue>(new Func<T, object, PropValue>(PropValue.CreatePropValue<T>), properties, values, values.Length);
			this.PreLoadStoreProperties(collection);
		}

		public void PreLoadStoreProperties(ICollection<PropValue> propertyValues)
		{
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			this.EnsureInternalDataStructuresAllocated(propertyValues.Count);
			foreach (PropValue propValue in propertyValues)
			{
				if (!this.IsDirty || !this.ChangedProperties.Contains(propValue.Property))
				{
					this.PropertyValues[propValue.Property] = propValue.Value;
				}
			}
		}

		internal void PreLoadStoreProperty(PropertyDefinition prop, object propertyValue, int approxCount)
		{
			this.EnsureInternalDataStructuresAllocated(approxCount);
			if (!this.IsDirty || !this.ChangedProperties.Contains(prop))
			{
				this.PropertyValues[prop] = propertyValue;
			}
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return this.CheckIsLoaded(propertyDefinition, false);
		}

		internal bool CheckIsLoaded(PropertyDefinition propertyDefinition, bool allowCalculatedProperty)
		{
			return this.propertyValues != null && this.propertyValues.ContainsKey(propertyDefinition);
		}

		public void Clear()
		{
			if (this.propertyValues != null)
			{
				this.propertyValues.Clear();
			}
			this.HasAllPropertiesLoaded = false;
			this.ClearChangeInfo();
		}

		internal bool HasAllPropertiesLoaded
		{
			get
			{
				return this.hasLoadedAllPossibleProperties;
			}
			private set
			{
				this.hasLoadedAllPossibleProperties = value;
			}
		}

		public void SetAllPropertiesLoaded()
		{
			this.HasAllPropertiesLoaded = true;
		}

		public void ClearChangeInfo()
		{
			this.changedProperties = null;
			this.deletedProperties = null;
			this.trackedPropertyInformation = null;
		}

		internal void ClearChangeInfo(PropertyDefinition propertyDefinition)
		{
			if (this.changedProperties != null)
			{
				this.ChangedProperties.Remove(propertyDefinition);
			}
			if (this.deletedProperties != null)
			{
				this.DeletedProperties.Remove(propertyDefinition);
			}
			if (this.trackedPropertyInformation != null)
			{
				this.TrackedPropertyInformation.Remove(propertyDefinition);
			}
		}

		internal void MarkPropertyAsRequireStreamed(PropertyDefinition propertyDefinition)
		{
			this.InternalSetValidatedStoreProperty(propertyDefinition, new PropertyError(propertyDefinition, PropertyErrorCode.RequireStreamed));
		}

		internal void Unload(PropertyDefinition propertyDefinition)
		{
			this.ClearChangeInfo(propertyDefinition);
			if (this.propertyValues != null)
			{
				this.PropertyValues.Remove(propertyDefinition);
			}
		}

		private HashSet<PropertyDefinition> ChangedProperties
		{
			get
			{
				if (this.changedProperties == null)
				{
					int num = (this.propertyValues != null) ? this.propertyValues.Count : 8;
					this.changedProperties = new HashSet<PropertyDefinition>(num >> 1);
				}
				return this.changedProperties;
			}
		}

		private List<PropertyDefinition> DeletedProperties
		{
			get
			{
				if (this.deletedProperties == null)
				{
					this.deletedProperties = new List<PropertyDefinition>();
				}
				return this.deletedProperties;
			}
		}

		private Dictionary<PropertyDefinition, PropertyValueTrackingData> TrackedPropertyInformation
		{
			get
			{
				if (this.trackedPropertyInformation == null)
				{
					this.trackedPropertyInformation = new Dictionary<PropertyDefinition, PropertyValueTrackingData>(2);
				}
				return this.trackedPropertyInformation;
			}
		}

		private PropertyError MarkAsNotFound(StorePropertyDefinition propertyDefinition)
		{
			PropertyError propertyError = new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			this.PropertyValues[propertyDefinition] = propertyError;
			return propertyError;
		}

		private void AddTrackingInformation(StorePropertyDefinition propertyDefinition, PropertyTrackingInformation changeType, object originalValue)
		{
			if ((propertyDefinition.PropertyFlags & PropertyFlags.TrackChange) == PropertyFlags.TrackChange && !this.TrackedPropertyInformation.ContainsKey(propertyDefinition))
			{
				PropertyValueTrackingData value = new PropertyValueTrackingData(changeType, originalValue);
				this.TrackedPropertyInformation.Add(propertyDefinition, value);
			}
		}

		private void InternalSetValidatedStoreProperty(PropertyDefinition propertyDefinition, object value)
		{
			this.EnsureInternalDataStructuresAllocated(8);
			Array array = value as Array;
			if (array != null)
			{
				value = MemoryPropertyBag.ClonePropertyValue<Array>(array);
			}
			else if (value is DateTime)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Low, "MemoryPropertyBag.InternalSetValidatedStoreProperty: System.DateTime", new object[0]);
				value = new ExDateTime(this.ExTimeZone, (DateTime)value);
			}
			else if (value is ExDateTime)
			{
				((ExDateTime)value).CheckExpectedTimeZone(this.ExTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.High);
				value = this.ExTimeZone.ConvertDateTime((ExDateTime)value);
			}
			object originalValue = null;
			StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			bool flag = (storePropertyDefinition.PropertyFlags & PropertyFlags.TrackChange) == PropertyFlags.TrackChange;
			if (!(value is PropertyError) && flag && this.propertyValues.ContainsKey(propertyDefinition))
			{
				originalValue = this.propertyValues[propertyDefinition];
			}
			this.propertyValues[propertyDefinition] = value;
			if (this.deletedProperties != null)
			{
				this.deletedProperties.Remove(propertyDefinition);
			}
			this.ChangedProperties.TryAdd(propertyDefinition);
			this.AddTrackingInformation(storePropertyDefinition, PropertyTrackingInformation.Modified, originalValue);
		}

		private void MarkAsReadOnly()
		{
			this.activeEnumeratorCount++;
			if (!(this.propertyValues is ReadOnlyDictionary<PropertyDefinition, object>))
			{
				this.propertyValues = new ReadOnlyDictionary<PropertyDefinition, object>(this.propertyValues);
			}
		}

		private void UnmarkAsReadOnly()
		{
			if (this.activeEnumeratorCount > 0)
			{
				this.activeEnumeratorCount--;
			}
			ReadOnlyDictionary<PropertyDefinition, object> readOnlyDictionary = this.propertyValues as ReadOnlyDictionary<PropertyDefinition, object>;
			if (readOnlyDictionary != null && this.activeEnumeratorCount == 0)
			{
				this.propertyValues = readOnlyDictionary.WrappedDictionary;
			}
		}

		private static object ClonePropertyValue<T>(T propertyValue) where T : ICloneable
		{
			if (propertyValue == null)
			{
				return propertyValue;
			}
			return propertyValue.Clone();
		}

		private static readonly NativeStorePropertyDefinition[] emptyNativeList = Array<NativeStorePropertyDefinition>.Empty;

		private static readonly ICollection<PropertyDefinition> emptyList = new ReadOnlyCollection<PropertyDefinition>(Array<PropertyDefinition>.Empty);

		private bool hasLoadedAllPossibleProperties;

		private IDictionary<PropertyDefinition, object> propertyValues;

		private int activeEnumeratorCount;

		private List<PropertyDefinition> deletedProperties;

		private HashSet<PropertyDefinition> changedProperties;

		private Dictionary<PropertyDefinition, PropertyValueTrackingData> trackedPropertyInformation;

		private ExTimeZone exTimeZone = ExTimeZone.UtcTimeZone;

		private class NativeFilter : ICollection<NativeStorePropertyDefinition>, IEnumerable<NativeStorePropertyDefinition>, IEnumerable
		{
			public NativeFilter(IDictionary<PropertyDefinition, object> properties, Action onEnumeratorCreate, Action onEnumeratorDispose)
			{
				ArgumentValidator.ThrowIfNull("properties", properties);
				ArgumentValidator.ThrowIfNull("onEnumeratorCreate", onEnumeratorCreate);
				ArgumentValidator.ThrowIfNull("onEnumeratorDispose", onEnumeratorDispose);
				this.properties = properties;
				this.onEnumeratorCreate = onEnumeratorCreate;
				this.onEnumeratorDispose = onEnumeratorDispose;
			}

			public int Count
			{
				get
				{
					if (this.count == -1)
					{
						this.count = 0;
						foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in this)
						{
							this.count++;
						}
					}
					return this.count;
				}
			}

			bool ICollection<NativeStorePropertyDefinition>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(NativeStorePropertyDefinition item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Remove(NativeStorePropertyDefinition item)
			{
				throw new NotSupportedException();
			}

			public bool Contains(NativeStorePropertyDefinition item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				return this.properties.ContainsKey(item);
			}

			public void CopyTo(NativeStorePropertyDefinition[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0 || index > array.Length - this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in this.properties)
				{
					NativeStorePropertyDefinition nativeStorePropertyDefinition = keyValuePair.Key as NativeStorePropertyDefinition;
					if (nativeStorePropertyDefinition != null)
					{
						array[index++] = nativeStorePropertyDefinition;
					}
				}
			}

			public MemoryPropertyBag.NativeFilter.Enumerator GetEnumerator()
			{
				MemoryPropertyBag.NativeFilter.Enumerator result = new MemoryPropertyBag.NativeFilter.Enumerator(this.properties.GetEnumerator(), this.onEnumeratorDispose);
				this.onEnumeratorCreate();
				return result;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator<NativeStorePropertyDefinition> IEnumerable<NativeStorePropertyDefinition>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private readonly IDictionary<PropertyDefinition, object> properties;

			private readonly Action onEnumeratorCreate;

			private readonly Action onEnumeratorDispose;

			private int count = -1;

			[Serializable]
			public class Enumerator : DisposableObject, IEnumerator<NativeStorePropertyDefinition>, IDisposable, IEnumerator
			{
				internal Enumerator(IEnumerator<KeyValuePair<PropertyDefinition, object>> parent, Action onDispose)
				{
					this.parentDictionaryEnumerator = parent;
					this.currentItem = null;
					this.onDispose = onDispose;
				}

				public NativeStorePropertyDefinition Current
				{
					get
					{
						return this.currentItem;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public bool MoveNext()
				{
					while (this.parentDictionaryEnumerator.MoveNext())
					{
						KeyValuePair<PropertyDefinition, object> keyValuePair = this.parentDictionaryEnumerator.Current;
						this.currentItem = (keyValuePair.Key as NativeStorePropertyDefinition);
						if (this.currentItem != null)
						{
							return true;
						}
					}
					return false;
				}

				protected override void InternalDispose(bool disposing)
				{
					if (disposing)
					{
						this.onDispose();
					}
					base.InternalDispose(disposing);
				}

				protected override DisposeTracker GetDisposeTracker()
				{
					return DisposeTracker.Get<MemoryPropertyBag.NativeFilter.Enumerator>(this);
				}

				void IEnumerator.Reset()
				{
					this.parentDictionaryEnumerator.Reset();
					this.currentItem = null;
				}

				private readonly IEnumerator<KeyValuePair<PropertyDefinition, object>> parentDictionaryEnumerator;

				private readonly Action onDispose;

				private NativeStorePropertyDefinition currentItem;
			}
		}

		private class FoundFilter : ICollection<PropertyDefinition>, IEnumerable<PropertyDefinition>, IEnumerable
		{
			public FoundFilter(IDictionary<PropertyDefinition, object> properties, Action onEnumeratorCreate, Action onEnumeratorDispose)
			{
				ArgumentValidator.ThrowIfNull("properties", properties);
				ArgumentValidator.ThrowIfNull("onEnumeratorCreate", onEnumeratorCreate);
				ArgumentValidator.ThrowIfNull("onEnumeratorDispose", onEnumeratorDispose);
				this.properties = properties;
				this.onEnumeratorCreate = onEnumeratorCreate;
				this.onEnumeratorDispose = onEnumeratorDispose;
			}

			public int Count
			{
				get
				{
					if (this.count == -1)
					{
						this.count = 0;
						foreach (PropertyDefinition propertyDefinition in this)
						{
							this.count++;
						}
					}
					return this.count;
				}
			}

			bool ICollection<PropertyDefinition>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(PropertyDefinition item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Remove(PropertyDefinition item)
			{
				throw new NotSupportedException();
			}

			public bool Contains(PropertyDefinition item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				return this.properties.ContainsKey(item);
			}

			public void CopyTo(PropertyDefinition[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0 || index > array.Length - this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				foreach (PropertyDefinition propertyDefinition in this)
				{
					array[index++] = propertyDefinition;
				}
			}

			public MemoryPropertyBag.FoundFilter.Enumerator GetEnumerator()
			{
				MemoryPropertyBag.FoundFilter.Enumerator result = new MemoryPropertyBag.FoundFilter.Enumerator(this.properties.GetEnumerator(), this.onEnumeratorDispose);
				this.onEnumeratorCreate();
				return result;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator<PropertyDefinition> IEnumerable<PropertyDefinition>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private readonly IDictionary<PropertyDefinition, object> properties;

			private readonly Action onEnumeratorCreate;

			private readonly Action onEnumeratorDispose;

			private int count = -1;

			[Serializable]
			public class Enumerator : DisposableObject, IEnumerator<PropertyDefinition>, IDisposable, IEnumerator
			{
				internal Enumerator(IEnumerator<KeyValuePair<PropertyDefinition, object>> parent, Action onDispose)
				{
					this.parentDictionaryEnumerator = parent;
					this.currentItem = null;
					this.onDispose = onDispose;
				}

				public PropertyDefinition Current
				{
					get
					{
						return this.currentItem;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public bool MoveNext()
				{
					while (this.parentDictionaryEnumerator.MoveNext())
					{
						KeyValuePair<PropertyDefinition, object> keyValuePair = this.parentDictionaryEnumerator.Current;
						if (!PropertyError.IsPropertyNotFound(keyValuePair.Value))
						{
							KeyValuePair<PropertyDefinition, object> keyValuePair2 = this.parentDictionaryEnumerator.Current;
							this.currentItem = keyValuePair2.Key;
							return true;
						}
					}
					return false;
				}

				protected override void InternalDispose(bool disposing)
				{
					if (disposing)
					{
						this.onDispose();
					}
					base.InternalDispose(disposing);
				}

				protected override DisposeTracker GetDisposeTracker()
				{
					return DisposeTracker.Get<MemoryPropertyBag.FoundFilter.Enumerator>(this);
				}

				void IEnumerator.Reset()
				{
					this.parentDictionaryEnumerator.Reset();
					this.currentItem = null;
				}

				private readonly IEnumerator<KeyValuePair<PropertyDefinition, object>> parentDictionaryEnumerator;

				private readonly Action onDispose;

				private PropertyDefinition currentItem;
			}
		}
	}
}

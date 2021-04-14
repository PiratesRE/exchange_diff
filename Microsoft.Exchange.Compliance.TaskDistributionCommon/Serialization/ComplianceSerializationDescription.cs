using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	public class ComplianceSerializationDescription<T> where T : new()
	{
		public byte ComplianceStructureId { get; set; }

		public int TotalByteFields
		{
			get
			{
				return this.byteGetters.Count;
			}
		}

		public int TotalShortFields
		{
			get
			{
				return this.shortGetters.Count;
			}
		}

		public int TotalIntegerFields
		{
			get
			{
				return this.integerGetters.Count;
			}
		}

		public int TotalLongFields
		{
			get
			{
				return this.longGetters.Count;
			}
		}

		public int TotalDoubleFields
		{
			get
			{
				return this.doubleGetters.Count;
			}
		}

		public int TotalGuidFields
		{
			get
			{
				return this.guidGetters.Count;
			}
		}

		public int TotalStringFields
		{
			get
			{
				return this.stringGetters.Count;
			}
		}

		public int TotalBlobFields
		{
			get
			{
				return this.blobGetters.Count;
			}
		}

		public int TotalCollectionFields
		{
			get
			{
				return this.collectionItemTypeGetters.Count;
			}
		}

		public void RegisterBytePropertyGetterAndSetter(byte fieldIndex, Func<T, byte> getter, Action<T, byte> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.byteGetters.Add(getter);
			this.byteSetters.Add(setter);
		}

		public void RegisterShortPropertyGetterAndSetter(byte fieldIndex, Func<T, short> getter, Action<T, short> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.shortGetters.Add(getter);
			this.shortSetters.Add(setter);
		}

		public void RegisterIntegerPropertyGetterAndSetter(byte fieldIndex, Func<T, int> getter, Action<T, int> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.integerGetters.Add(getter);
			this.integerSetters.Add(setter);
		}

		public void RegisterLongPropertyGetterAndSetter(byte fieldIndex, Func<T, long> getter, Action<T, long> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.longGetters.Add(getter);
			this.longSetters.Add(setter);
		}

		public void RegisterDoublePropertyGetterAndSetter(byte fieldIndex, Func<T, double> getter, Action<T, double> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.doubleGetters.Add(getter);
			this.doubleSetters.Add(setter);
		}

		public void RegisterGuidPropertyGetterAndSetter(byte fieldIndex, Func<T, Guid> getter, Action<T, Guid> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.guidGetters.Add(getter);
			this.guidSetters.Add(setter);
		}

		public void RegisterStringPropertyGetterAndSetter(byte fieldIndex, Func<T, string> getter, Action<T, string> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.stringGetters.Add(getter);
			this.stringSetters.Add(setter);
		}

		public void RegisterBlobPropertyGetterAndSetter(byte fieldIndex, Func<T, byte[]> getter, Action<T, byte[]> setter)
		{
			if (getter == null || setter == null)
			{
				throw new ArgumentNullException();
			}
			this.blobGetters.Add(getter);
			this.blobSetters.Add(setter);
		}

		public void RegisterCollectionPropertyAccessors(byte fieldIndex, Func<CollectionItemType> itemTypeGetter, Func<T, int> itemCountGetter, Func<T, int, object> itemGetter, Action<T, object, int> itemAdder)
		{
			this.RegisterCollectionPropertyAccessors(fieldIndex, itemTypeGetter, itemCountGetter, itemGetter, itemAdder, null);
		}

		public void RegisterCollectionPropertyAccessors(byte fieldIndex, Func<CollectionItemType> itemTypeGetter, Func<T, int> itemCountGetter, Func<T, int, object> itemGetter, Action<T, object, int> itemAdder, Action<T, int> collectionInitializer)
		{
			if (itemTypeGetter == null || itemCountGetter == null || itemGetter == null || itemAdder == null)
			{
				throw new ArgumentNullException();
			}
			this.collectionItemTypeGetters.Add(itemTypeGetter);
			this.collectionItemCountGetters.Add(itemCountGetter);
			this.collectionItemGetters.Add(itemGetter);
			this.collectionItemAdders.Add(itemAdder);
			this.collectionInitializers.Add(collectionInitializer);
		}

		public void RegisterComplexCollectionAccessor<I>(byte fieldIndex, Func<T, int> itemCountGetter, Func<T, int, I> itemGetter, Action<T, I, int> itemAdder, ComplianceSerializationDescription<I> listItemDescription) where I : class, new()
		{
			this.RegisterCollectionPropertyAccessors(fieldIndex, () => CollectionItemType.Blob, itemCountGetter, (T item, int index) => ComplianceSerializer.Serialize<I>(listItemDescription, itemGetter(item, index)), delegate(T item, object obj, int index)
			{
				itemAdder(item, ComplianceSerializer.DeSerialize<I>(listItemDescription, (byte[])obj), index);
			});
		}

		public void RegisterComplexPropertyAsBlobGetterAndSetter<I>(byte fieldIndex, Func<T, I> getter, Action<T, I> setter, ComplianceSerializationDescription<I> itemDescription) where I : class, new()
		{
			this.RegisterBlobPropertyGetterAndSetter(fieldIndex, (T item) => ComplianceSerializer.Serialize<I>(itemDescription, getter(item)), delegate(T item, byte[] obj)
			{
				setter(item, ComplianceSerializer.DeSerialize<I>(itemDescription, obj));
			});
		}

		public bool TryGetByteProperty(T obj, byte fieldIndex, out byte value)
		{
			value = 0;
			if ((int)fieldIndex >= this.byteGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.byteGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetByteProperty(T obj, byte fieldIndex, byte value)
		{
			if ((int)fieldIndex >= this.byteSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.byteSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetShortProperty(T obj, byte fieldIndex, out short value)
		{
			value = 0;
			if ((int)fieldIndex >= this.shortGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.shortGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetShortProperty(T obj, byte fieldIndex, short value)
		{
			if ((int)fieldIndex >= this.shortSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.shortSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetIntegerProperty(T obj, byte fieldIndex, out int value)
		{
			value = 0;
			if ((int)fieldIndex >= this.integerGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.integerGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetIntegerProperty(T obj, byte fieldIndex, int value)
		{
			if ((int)fieldIndex >= this.integerSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.integerSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetLongProperty(T obj, byte fieldIndex, out long value)
		{
			value = 0L;
			if ((int)fieldIndex >= this.longGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.longGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetLongProperty(T obj, byte fieldIndex, long value)
		{
			if ((int)fieldIndex >= this.longSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.longSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetDoubleProperty(T obj, byte fieldIndex, out double value)
		{
			value = 0.0;
			if ((int)fieldIndex >= this.doubleGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.doubleGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetDoubleProperty(T obj, byte fieldIndex, double value)
		{
			if ((int)fieldIndex >= this.doubleSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.doubleSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetGuidProperty(T obj, byte fieldIndex, out Guid value)
		{
			value = Guid.Empty;
			if ((int)fieldIndex >= this.guidGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.guidGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetGuidProperty(T obj, byte fieldIndex, Guid value)
		{
			if ((int)fieldIndex >= this.guidSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.guidSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetStringProperty(T obj, byte fieldIndex, out string value)
		{
			value = string.Empty;
			if ((int)fieldIndex >= this.stringGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.stringGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetStringProperty(T obj, byte fieldIndex, string value)
		{
			if ((int)fieldIndex >= this.stringSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.stringSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetBlobProperty(T obj, byte fieldIndex, out byte[] value)
		{
			value = null;
			if ((int)fieldIndex >= this.blobGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			value = this.blobGetters[(int)fieldIndex](obj);
			return true;
		}

		public bool TrySetBlobProperty(T obj, byte fieldIndex, byte[] value)
		{
			if ((int)fieldIndex >= this.blobSetters.Count || fieldIndex < 0)
			{
				return false;
			}
			this.blobSetters[(int)fieldIndex](obj, value);
			return true;
		}

		public bool TryGetCollectionPropertyItemType(byte fieldIndex, out CollectionItemType type)
		{
			type = CollectionItemType.NotDefined;
			if ((int)fieldIndex >= this.collectionItemTypeGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			type = this.collectionItemTypeGetters[(int)fieldIndex]();
			return true;
		}

		public IEnumerable<object> GetCollectionItems(T obj, byte fieldIndex)
		{
			List<object> list = new List<object>();
			if ((int)fieldIndex >= this.collectionItemTypeGetters.Count || fieldIndex < 0)
			{
				return list;
			}
			int num = this.collectionItemCountGetters[(int)fieldIndex](obj);
			for (int i = 0; i < num; i++)
			{
				object item = this.collectionItemGetters[(int)fieldIndex](obj, i);
				list.Add(item);
			}
			return list;
		}

		public bool TrySetCollectionItems(T obj, byte fieldIndex, IList<object> items)
		{
			if ((int)fieldIndex >= this.collectionItemTypeGetters.Count || fieldIndex < 0)
			{
				return false;
			}
			if (this.collectionInitializers[(int)fieldIndex] != null)
			{
				this.collectionInitializers[(int)fieldIndex](obj, items.Count);
			}
			int num = 0;
			foreach (object arg in items)
			{
				this.collectionItemAdders[(int)fieldIndex](obj, arg, num++);
			}
			return true;
		}

		private List<Action<T, byte>> byteSetters = new List<Action<T, byte>>();

		private List<Func<T, byte>> byteGetters = new List<Func<T, byte>>();

		private List<Action<T, short>> shortSetters = new List<Action<T, short>>();

		private List<Func<T, short>> shortGetters = new List<Func<T, short>>();

		private List<Action<T, int>> integerSetters = new List<Action<T, int>>();

		private List<Func<T, int>> integerGetters = new List<Func<T, int>>();

		private List<Action<T, long>> longSetters = new List<Action<T, long>>();

		private List<Func<T, long>> longGetters = new List<Func<T, long>>();

		private List<Action<T, double>> doubleSetters = new List<Action<T, double>>();

		private List<Func<T, double>> doubleGetters = new List<Func<T, double>>();

		private List<Action<T, Guid>> guidSetters = new List<Action<T, Guid>>();

		private List<Func<T, Guid>> guidGetters = new List<Func<T, Guid>>();

		private List<Action<T, string>> stringSetters = new List<Action<T, string>>();

		private List<Func<T, string>> stringGetters = new List<Func<T, string>>();

		private List<Action<T, byte[]>> blobSetters = new List<Action<T, byte[]>>();

		private List<Func<T, byte[]>> blobGetters = new List<Func<T, byte[]>>();

		private List<Func<CollectionItemType>> collectionItemTypeGetters = new List<Func<CollectionItemType>>();

		private List<Func<T, int>> collectionItemCountGetters = new List<Func<T, int>>();

		private List<Func<T, int, object>> collectionItemGetters = new List<Func<T, int, object>>();

		private List<Action<T, object, int>> collectionItemAdders = new List<Action<T, object, int>>();

		private List<Action<T, int>> collectionInitializers = new List<Action<T, int>>();
	}
}

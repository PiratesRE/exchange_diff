using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.Common
{
	[Serializable]
	public class PropertyBag : IDictionary, ICollection, IEnumerable
	{
		public static PropertyBag Synchronized(PropertyBag oldBag)
		{
			PropertyBag.SynchronizedPropertyBag synchronizedPropertyBag = null;
			if (oldBag != null)
			{
				synchronizedPropertyBag = new PropertyBag.SynchronizedPropertyBag(oldBag.Count);
				foreach (object obj in oldBag.FieldDictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					synchronizedPropertyBag.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				}
				synchronizedPropertyBag.readOnly = oldBag.readOnly;
			}
			else
			{
				synchronizedPropertyBag = new PropertyBag.SynchronizedPropertyBag(0);
			}
			return synchronizedPropertyBag;
		}

		public PropertyBag(int initialSize)
		{
			this.readOnly = false;
			this.fieldDictionary = new HybridDictionary(initialSize);
		}

		public PropertyBag() : this(0)
		{
		}

		protected PropertyBag(bool isSynchronized)
		{
			this.readOnly = false;
		}

		public int Count
		{
			get
			{
				return this.FieldDictionary.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.FieldDictionary.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.FieldDictionary.SyncRoot;
			}
		}

		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			foreach (object obj in this.FieldDictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array.SetValue(new DictionaryEntry(dictionaryEntry.Key, ((Field)dictionaryEntry.Value).Data), index);
				index++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new PropertyBag.PropertyBagEnumerator(this);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public virtual object this[object key]
		{
			get
			{
				Field field = (Field)this.FieldDictionary[key];
				if (field == null)
				{
					return null;
				}
				return field.Data;
			}
			set
			{
				ExTraceGlobals.PropertyBagTracer.Information((long)this.GetHashCode(), "PropertyBag[{0}]={1}.", new object[]
				{
					key,
					value
				});
				if (this.IsReadOnly)
				{
					throw new ReadOnlyPropertyBagException();
				}
				Field field = (Field)this.FieldDictionary[key];
				if (field == null)
				{
					field = (this.FieldDictionary[key] = new Field(null));
				}
				field.Data = value;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				object[] array = new object[this.Count];
				int num = 0;
				foreach (object obj in this.FieldDictionary.Keys)
				{
					array[num] = obj;
					num++;
				}
				return array;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				object[] array = new object[this.Count];
				int num = 0;
				foreach (object obj in this.FieldDictionary.Values)
				{
					array[num] = ((Field)obj).Data;
					num++;
				}
				return array;
			}
		}

		public void Add(object key, object value)
		{
			this[key] = value;
		}

		public virtual void Clear()
		{
			ExTraceGlobals.PropertyBagTracer.Information((long)this.GetHashCode(), "PropertyBag::Clear()");
			if (this.IsReadOnly)
			{
				throw new ReadOnlyPropertyBagException();
			}
			this.FieldDictionary.Clear();
		}

		public bool Contains(object key)
		{
			return this.FieldDictionary.Contains(key);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new PropertyBag.PropertyBagEnumerator(this);
		}

		public virtual void Remove(object key)
		{
			if (this.IsReadOnly)
			{
				throw new ReadOnlyPropertyBagException();
			}
			this.FieldDictionary.Remove(key);
		}

		public bool IsChanged(object key)
		{
			Field field = (Field)this.FieldDictionary[key];
			return field != null && field.IsChanged;
		}

		public bool IsModified(object key)
		{
			Field field = (Field)this.FieldDictionary[key];
			return field != null && field.IsModified;
		}

		public virtual void ResetChangeTracking()
		{
			ExTraceGlobals.PropertyBagTracer.Information((long)this.GetHashCode(), "PropertyBag::ResetChangeTracking()");
			if (this.IsReadOnly)
			{
				throw new ReadOnlyPropertyBagException();
			}
			ICollection values = this.FieldDictionary.Values;
			foreach (object obj in values)
			{
				((Field)obj).ResetChangeTracking();
			}
		}

		public virtual void MakeReadOnly()
		{
			ExTraceGlobals.PropertyBagTracer.Information((long)this.GetHashCode(), "DesynchedPropertyBag::MakeReadOnly()");
			this.readOnly = true;
		}

		public virtual void MakeReadWrite()
		{
			ExTraceGlobals.PropertyBagTracer.Information((long)this.GetHashCode(), "DesynchedPropertyBag::MakeReadWrite()");
			this.readOnly = false;
		}

		internal IDictionary FieldDictionary
		{
			get
			{
				return (IDictionary)this.fieldDictionary;
			}
		}

		protected bool readOnly;

		protected object fieldDictionary;

		private sealed class PropertyBagEnumerator : IDictionaryEnumerator, IEnumerator
		{
			public PropertyBagEnumerator(PropertyBag bag)
			{
				this.fieldEnumerator = bag.FieldDictionary.GetEnumerator();
			}

			public object Current
			{
				get
				{
					DictionaryEntry entry = this.FieldEnumerator.Entry;
					return new DictionaryEntry(entry.Key, ((Field)entry.Value).Data);
				}
			}

			public bool MoveNext()
			{
				return this.FieldEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.FieldEnumerator.Reset();
			}

			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry)this.Current;
				}
			}

			public object Key
			{
				get
				{
					return this.FieldEnumerator.Key;
				}
			}

			public object Value
			{
				get
				{
					return ((Field)this.FieldEnumerator.Value).Data;
				}
			}

			private IDictionaryEnumerator FieldEnumerator
			{
				get
				{
					return this.fieldEnumerator;
				}
			}

			private IDictionaryEnumerator fieldEnumerator;
		}

		private sealed class SynchronizedPropertyBag : PropertyBag
		{
			public SynchronizedPropertyBag(int initialSize) : base(true)
			{
				this.fieldDictionary = Hashtable.Synchronized(new Hashtable(initialSize));
			}

			public override void CopyTo(Array array, int index)
			{
				lock (base.SyncRoot)
				{
					base.CopyTo(array, index);
				}
			}

			public override object this[object key]
			{
				set
				{
					lock (base.SyncRoot)
					{
						base[key] = value;
					}
				}
			}

			public override ICollection Keys
			{
				get
				{
					ICollection keys;
					lock (base.SyncRoot)
					{
						keys = base.Keys;
					}
					return keys;
				}
			}

			public override ICollection Values
			{
				get
				{
					ICollection values;
					lock (base.SyncRoot)
					{
						values = base.Values;
					}
					return values;
				}
			}

			public override void Clear()
			{
				lock (base.SyncRoot)
				{
					base.Clear();
				}
			}

			public override void Remove(object key)
			{
				lock (base.SyncRoot)
				{
					base.Remove(key);
				}
			}

			public override void ResetChangeTracking()
			{
				lock (base.SyncRoot)
				{
					base.ResetChangeTracking();
				}
			}

			public override void MakeReadOnly()
			{
				lock (base.SyncRoot)
				{
					base.MakeReadOnly();
				}
			}

			public override void MakeReadWrite()
			{
				lock (base.SyncRoot)
				{
					base.MakeReadWrite();
				}
			}
		}
	}
}

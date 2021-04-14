using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal sealed class CLRIReferenceArrayImpl<T> : CLRIPropertyValueImpl, IReferenceArray<T>, IPropertyValue, ICustomPropertyProvider, IList, ICollection, IEnumerable
	{
		public CLRIReferenceArrayImpl(PropertyType type, T[] obj) : base(type, obj)
		{
			this._value = obj;
			this._list = this._value;
		}

		public T[] Value
		{
			get
			{
				return this._value;
			}
		}

		public override string ToString()
		{
			if (this._value != null)
			{
				return this._value.ToString();
			}
			return base.ToString();
		}

		ICustomProperty ICustomPropertyProvider.GetCustomProperty(string name)
		{
			return ICustomPropertyProviderImpl.CreateProperty(this._value, name);
		}

		ICustomProperty ICustomPropertyProvider.GetIndexedProperty(string name, Type indexParameterType)
		{
			return ICustomPropertyProviderImpl.CreateIndexedProperty(this._value, name, indexParameterType);
		}

		string ICustomPropertyProvider.GetStringRepresentation()
		{
			return this._value.ToString();
		}

		Type ICustomPropertyProvider.Type
		{
			get
			{
				return this._value.GetType();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._value.GetEnumerator();
		}

		object IList.this[int index]
		{
			get
			{
				return this._list[index];
			}
			set
			{
				this._list[index] = value;
			}
		}

		int IList.Add(object value)
		{
			return this._list.Add(value);
		}

		bool IList.Contains(object value)
		{
			return this._list.Contains(value);
		}

		void IList.Clear()
		{
			this._list.Clear();
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this._list.IsReadOnly;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this._list.IsFixedSize;
			}
		}

		int IList.IndexOf(object value)
		{
			return this._list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			this._list.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			this._list.Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			this._list.RemoveAt(index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		int ICollection.Count
		{
			get
			{
				return this._list.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this._list.IsSynchronized;
			}
		}

		[FriendAccessAllowed]
		internal static object UnboxHelper(object wrapper)
		{
			IReferenceArray<T> referenceArray = (IReferenceArray<T>)wrapper;
			return referenceArray.Value;
		}

		private T[] _value;

		private IList _list;
	}
}

using System;
using System.Collections;

namespace System.Runtime.Remoting.Channels
{
	internal class AggregateDictionary : IDictionary, ICollection, IEnumerable
	{
		public AggregateDictionary(ICollection dictionaries)
		{
			this._dictionaries = dictionaries;
		}

		public virtual object this[object key]
		{
			get
			{
				foreach (object obj in this._dictionaries)
				{
					IDictionary dictionary = (IDictionary)obj;
					if (dictionary.Contains(key))
					{
						return dictionary[key];
					}
				}
				return null;
			}
			set
			{
				foreach (object obj in this._dictionaries)
				{
					IDictionary dictionary = (IDictionary)obj;
					if (dictionary.Contains(key))
					{
						dictionary[key] = value;
					}
				}
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				foreach (object obj in this._dictionaries)
				{
					IDictionary dictionary = (IDictionary)obj;
					ICollection keys = dictionary.Keys;
					if (keys != null)
					{
						foreach (object value in keys)
						{
							arrayList.Add(value);
						}
					}
				}
				return arrayList;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				foreach (object obj in this._dictionaries)
				{
					IDictionary dictionary = (IDictionary)obj;
					ICollection values = dictionary.Values;
					if (values != null)
					{
						foreach (object value in values)
						{
							arrayList.Add(value);
						}
					}
				}
				return arrayList;
			}
		}

		public virtual bool Contains(object key)
		{
			foreach (object obj in this._dictionaries)
			{
				IDictionary dictionary = (IDictionary)obj;
				if (dictionary.Contains(key))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public virtual void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		public virtual void Clear()
		{
			throw new NotSupportedException();
		}

		public virtual void Remove(object key)
		{
			throw new NotSupportedException();
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new DictionaryEnumeratorByKeys(this);
		}

		public virtual void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public virtual int Count
		{
			get
			{
				int num = 0;
				foreach (object obj in this._dictionaries)
				{
					IDictionary dictionary = (IDictionary)obj;
					num += dictionary.Count;
				}
				return num;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DictionaryEnumeratorByKeys(this);
		}

		private ICollection _dictionaries;
	}
}

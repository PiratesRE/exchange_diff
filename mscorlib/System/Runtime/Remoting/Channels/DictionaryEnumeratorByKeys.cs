using System;
using System.Collections;

namespace System.Runtime.Remoting.Channels
{
	internal class DictionaryEnumeratorByKeys : IDictionaryEnumerator, IEnumerator
	{
		public DictionaryEnumeratorByKeys(IDictionary properties)
		{
			this._properties = properties;
			this._keyEnum = properties.Keys.GetEnumerator();
		}

		public bool MoveNext()
		{
			return this._keyEnum.MoveNext();
		}

		public void Reset()
		{
			this._keyEnum.Reset();
		}

		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		public DictionaryEntry Entry
		{
			get
			{
				return new DictionaryEntry(this.Key, this.Value);
			}
		}

		public object Key
		{
			get
			{
				return this._keyEnum.Current;
			}
		}

		public object Value
		{
			get
			{
				return this._properties[this.Key];
			}
		}

		private IDictionary _properties;

		private IEnumerator _keyEnum;
	}
}

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Remoting.Channels
{
	[SecurityCritical]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
	[Serializable]
	public class TransportHeaders : ITransportHeaders
	{
		public TransportHeaders()
		{
			this._headerList = new ArrayList(6);
		}

		public object this[object key]
		{
			[SecurityCritical]
			get
			{
				string strB = (string)key;
				foreach (object obj in this._headerList)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (string.Compare((string)dictionaryEntry.Key, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return dictionaryEntry.Value;
					}
				}
				return null;
			}
			[SecurityCritical]
			set
			{
				if (key == null)
				{
					return;
				}
				string strB = (string)key;
				for (int i = this._headerList.Count - 1; i >= 0; i--)
				{
					string strA = (string)((DictionaryEntry)this._headerList[i]).Key;
					if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this._headerList.RemoveAt(i);
						break;
					}
				}
				if (value != null)
				{
					this._headerList.Add(new DictionaryEntry(key, value));
				}
			}
		}

		[SecurityCritical]
		public IEnumerator GetEnumerator()
		{
			return this._headerList.GetEnumerator();
		}

		private ArrayList _headerList;
	}
}

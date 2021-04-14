using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class AttachmentDataCollection<T> : IEnumerable<T>, IEnumerable where T : AttachmentData
	{
		public int Count
		{
			get
			{
				int num = 0;
				foreach (T t in this.list)
				{
					if (t != null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public List<T> InternalList
		{
			[DebuggerStepThrough]
			get
			{
				return this.list;
			}
		}

		public AttachmentDataCollection()
		{
			this.list = new List<T>();
		}

		public int Add(T item)
		{
			int count = this.list.Count;
			this.list.Add(item);
			return count;
		}

		public bool RemoveAtPrivateIndex(int index)
		{
			T t = this.list[index];
			this.list[index] = default(T);
			return t != null;
		}

		public void Clear()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				this.list[i] = default(T);
			}
		}

		public void Reset()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				MimeAttachmentData mimeAttachmentData = this.list[i] as MimeAttachmentData;
				if (mimeAttachmentData != null)
				{
					mimeAttachmentData.Referenced = false;
				}
			}
		}

		public T GetDataAtPrivateIndex(int privateIndex)
		{
			return this.list[privateIndex];
		}

		public T GetDataAtPublicIndex(int publicIndex)
		{
			int privateIndex = this.GetPrivateIndex(publicIndex);
			if (privateIndex >= 0)
			{
				return this.list[privateIndex];
			}
			return default(T);
		}

		public int GetPrivateIndex(int publicIndex)
		{
			int num = 0;
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.list[i] != null)
				{
					if (publicIndex == num)
					{
						return i;
					}
					num++;
				}
			}
			return -1;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int index = 0; index < this.list.Count; index++)
			{
				T data = this.list[index];
				if (data != null)
				{
					yield return data;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private List<T> list;
	}
}

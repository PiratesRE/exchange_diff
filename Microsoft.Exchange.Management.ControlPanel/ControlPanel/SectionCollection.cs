using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class SectionCollection : IList, ICollection, IEnumerable<Section>, IEnumerable
	{
		internal SectionCollection(WebControl parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent", "Parent control cannot be null.");
			}
			this.parent = parent;
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (object obj in this.parent.Controls)
				{
					Control control = (Control)obj;
					if (control is Section)
					{
						num++;
					}
				}
				return num;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public Section this[int index]
		{
			get
			{
				return (Section)this.parent.Controls[this.ToRawIndex(index)];
			}
		}

		public Section this[string id]
		{
			get
			{
				for (int i = 0; i < this.parent.Controls.Count; i++)
				{
					Section section = this.parent.Controls[i] as Section;
					if (section != null && section.ID == id)
					{
						return section;
					}
				}
				return null;
			}
		}

		private int ToRawIndex(int paneIndex)
		{
			if (paneIndex < 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "paneIndex = {0} : paneIndex should not be negative", new object[]
				{
					paneIndex
				}));
			}
			int num = -1;
			for (int i = 0; i < this.parent.Controls.Count; i++)
			{
				if (this.parent.Controls[i] is Section && ++num == paneIndex)
				{
					return i;
				}
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No Section at position {0}", new object[]
			{
				paneIndex
			}));
		}

		private int FromRawIndex(int index)
		{
			if (index < 0)
			{
				return -1;
			}
			if (index >= this.parent.Controls.Count || !(this.parent.Controls[index] is Section))
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "No Section at position {0}", new object[]
				{
					index
				}));
			}
			int num = -1;
			while (index >= 0)
			{
				if (this.parent.Controls[index--] is Section)
				{
					num++;
				}
			}
			return num;
		}

		public void Add(Section item)
		{
			this.parent.Controls.Add(item);
			this.version++;
		}

		public void Clear()
		{
			for (int i = this.parent.Controls.Count - 1; i >= 0; i--)
			{
				if (this.parent.Controls[i] is Section)
				{
					this.parent.Controls.RemoveAt(i);
				}
			}
			this.version++;
		}

		public bool Contains(Section item)
		{
			return this.parent.Controls.Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			Section[] array2 = array as Section[];
			if (array2 == null)
			{
				throw new ArgumentException("Expected an array of Sections.");
			}
			this.CopyTo(array2, index);
		}

		public void CopyTo(Section[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Cannot copy into a null array.");
			}
			int num = 0;
			for (int i = 0; i < this.parent.Controls.Count; i++)
			{
				Section section = this.parent.Controls[i] as Section;
				if (section != null)
				{
					if (num + index == array.Length)
					{
						throw new ArgumentException("Array is not large enough for the Sections");
					}
					array[num + index] = section;
					num++;
				}
			}
		}

		public int IndexOf(Section item)
		{
			return this.FromRawIndex(this.parent.Controls.IndexOf(item));
		}

		public void Insert(int index, Section item)
		{
			this.parent.Controls.AddAt(this.ToRawIndex(index), item);
			this.version++;
		}

		public void Remove(Section item)
		{
			this.parent.Controls.Remove(item);
			this.version++;
		}

		public void RemoveAt(int index)
		{
			this.parent.Controls.RemoveAt(this.ToRawIndex(index));
			this.version++;
		}

		int IList.Add(object value)
		{
			this.Add((Section)value);
			return this.IndexOf((Section)value);
		}

		bool IList.Contains(object value)
		{
			return this.Contains((Section)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((Section)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (Section)value);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		void IList.Remove(object value)
		{
			this.Remove((Section)value);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SectionCollection.SectionEnumerator(this);
		}

		public IEnumerator<Section> GetEnumerator()
		{
			return new SectionCollection.SectionEnumerator(this);
		}

		private WebControl parent;

		private int version;

		private class SectionEnumerator : IEnumerator<Section>, IDisposable, IEnumerator
		{
			public SectionEnumerator(SectionCollection parent)
			{
				this.collection = parent;
				this.parentEnumerator = parent.parent.Controls.GetEnumerator();
				this.version = parent.version;
			}

			private void CheckVersion()
			{
				if (this.version != this.collection.version)
				{
					throw new InvalidOperationException("Enumeration can't continue because the collection has been modified.");
				}
			}

			public void Dispose()
			{
				this.parentEnumerator = null;
				this.collection = null;
			}

			public Section Current
			{
				get
				{
					this.CheckVersion();
					return this.parentEnumerator.Current as Section;
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
				this.CheckVersion();
				bool flag = this.parentEnumerator.MoveNext();
				if (flag && !(this.parentEnumerator.Current is Section))
				{
					flag = this.MoveNext();
				}
				return flag;
			}

			public void Reset()
			{
				this.CheckVersion();
				this.parentEnumerator.Reset();
			}

			private SectionCollection collection;

			private IEnumerator parentEnumerator;

			private int version;
		}
	}
}

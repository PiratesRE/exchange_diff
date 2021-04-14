using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadParameterCollection : DbParameterCollection
	{
		public override int Count
		{
			get
			{
				if (this.items == null)
				{
					return 0;
				}
				return this.items.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return ((IList)this.InnerList).IsFixedSize;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return ((IList)this.InnerList).IsReadOnly;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return ((ICollection)this.InnerList).IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return ((ICollection)this.InnerList).SyncRoot;
			}
		}

		private List<MonadParameter> InnerList
		{
			get
			{
				if (this.items == null)
				{
					this.items = new List<MonadParameter>();
				}
				return this.items;
			}
		}

		public MonadParameter Add(MonadParameter value)
		{
			this.InnerList.Add(value);
			return value;
		}

		public MonadParameter AddWithValue(string parameterName, object value)
		{
			return this.Add(new MonadParameter(parameterName, value));
		}

		public MonadParameter AddSwitch(string parameterName)
		{
			return this.Add(new MonadParameter(parameterName)
			{
				IsSwitch = true
			});
		}

		public void AddSwitchAsNeeded(string parameterName, bool needed)
		{
			if (needed)
			{
				this.AddSwitch(parameterName);
			}
		}

		public void Remove(string parameterName)
		{
			int index;
			while (-1 != (index = this.IndexOf(parameterName)))
			{
				this.RemoveAt(index);
			}
		}

		public override int Add(object value)
		{
			this.InnerList.Add((MonadParameter)value);
			return this.Count - 1;
		}

		public override void AddRange(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			foreach (object obj in values)
			{
				MonadParameter item = (MonadParameter)obj;
				this.InnerList.Add(item);
			}
		}

		public override void Clear()
		{
			this.InnerList.Clear();
		}

		public override bool Contains(string value)
		{
			return -1 != this.IndexOf(value);
		}

		public override bool Contains(object value)
		{
			return -1 != this.IndexOf(value);
		}

		public override void CopyTo(Array array, int index)
		{
			((ICollection)this.InnerList).CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return ((IEnumerable)this.InnerList).GetEnumerator();
		}

		public override int IndexOf(string parameterName)
		{
			return MonadParameterCollection.IndexOf(this.InnerList, parameterName);
		}

		public override int IndexOf(object value)
		{
			return this.InnerList.IndexOf((MonadParameter)value);
		}

		public override void Insert(int index, object value)
		{
			this.InnerList.Insert(index, (MonadParameter)value);
		}

		public override void Remove(object value)
		{
			int num = this.IndexOf(value);
			if (-1 != num)
			{
				this.RemoveAt(num);
			}
		}

		public override void RemoveAt(int index)
		{
			this.InnerList.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			int num = this.IndexOf(parameterName);
			if (num < 0)
			{
				throw new ArgumentException();
			}
			this.RemoveAt(num);
		}

		protected internal static int IndexOf(IEnumerable items, string parameterName)
		{
			if (items != null)
			{
				int num = 0;
				foreach (object obj in items)
				{
					DbParameter dbParameter = (DbParameter)obj;
					if (string.Compare(parameterName, dbParameter.ParameterName) == 0)
					{
						return num;
					}
					num++;
				}
				num = 0;
				foreach (object obj2 in items)
				{
					DbParameter dbParameter2 = (DbParameter)obj2;
					if (string.Compare(parameterName, dbParameter2.ParameterName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return num;
					}
					num++;
				}
				return -1;
			}
			return -1;
		}

		protected override DbParameter GetParameter(int index)
		{
			return this.InnerList[index];
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return this.GetParameter(this.IndexOf(parameterName));
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this.InnerList[index] = (MonadParameter)value;
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			this.SetParameter(this.IndexOf(parameterName), value);
		}

		private List<MonadParameter> items;
	}
}

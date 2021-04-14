using System;

namespace System.Runtime.Serialization
{
	internal class ObjectHolderList
	{
		internal ObjectHolderList() : this(8)
		{
		}

		internal ObjectHolderList(int startingSize)
		{
			this.m_count = 0;
			this.m_values = new ObjectHolder[startingSize];
		}

		internal virtual void Add(ObjectHolder value)
		{
			if (this.m_count == this.m_values.Length)
			{
				this.EnlargeArray();
			}
			ObjectHolder[] values = this.m_values;
			int count = this.m_count;
			this.m_count = count + 1;
			values[count] = value;
		}

		internal ObjectHolderListEnumerator GetFixupEnumerator()
		{
			return new ObjectHolderListEnumerator(this, true);
		}

		private void EnlargeArray()
		{
			int num = this.m_values.Length * 2;
			if (num < 0)
			{
				if (num == 2147483647)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_TooManyElements"));
				}
				num = int.MaxValue;
			}
			ObjectHolder[] array = new ObjectHolder[num];
			Array.Copy(this.m_values, array, this.m_count);
			this.m_values = array;
		}

		internal int Version
		{
			get
			{
				return this.m_count;
			}
		}

		internal int Count
		{
			get
			{
				return this.m_count;
			}
		}

		internal const int DefaultInitialSize = 8;

		internal ObjectHolder[] m_values;

		internal int m_count;
	}
}

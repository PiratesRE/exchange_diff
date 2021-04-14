using System;
using System.Diagnostics;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class SerStack
	{
		internal SerStack()
		{
			this.stackId = "System";
		}

		internal SerStack(string stackId)
		{
			this.stackId = stackId;
		}

		internal void Push(object obj)
		{
			if (this.top == this.objects.Length - 1)
			{
				this.IncreaseCapacity();
			}
			object[] array = this.objects;
			int num = this.top + 1;
			this.top = num;
			array[num] = obj;
		}

		internal object Pop()
		{
			if (this.top < 0)
			{
				return null;
			}
			object result = this.objects[this.top];
			object[] array = this.objects;
			int num = this.top;
			this.top = num - 1;
			array[num] = null;
			return result;
		}

		internal void IncreaseCapacity()
		{
			int num = this.objects.Length * 2;
			object[] destinationArray = new object[num];
			Array.Copy(this.objects, 0, destinationArray, 0, this.objects.Length);
			this.objects = destinationArray;
		}

		internal object Peek()
		{
			if (this.top < 0)
			{
				return null;
			}
			return this.objects[this.top];
		}

		internal object PeekPeek()
		{
			if (this.top < 1)
			{
				return null;
			}
			return this.objects[this.top - 1];
		}

		internal int Count()
		{
			return this.top + 1;
		}

		internal bool IsEmpty()
		{
			return this.top <= 0;
		}

		[Conditional("SER_LOGGING")]
		internal void Dump()
		{
			for (int i = 0; i < this.Count(); i++)
			{
				object obj = this.objects[i];
			}
		}

		internal object[] objects = new object[5];

		internal string stackId;

		internal int top = -1;

		internal int next;
	}
}

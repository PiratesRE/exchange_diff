using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy
{
	internal sealed class Breadcrumbs<T> : IEnumerable<T>, IEnumerable
	{
		public Breadcrumbs(int size)
		{
			if (size <= 32)
			{
				if (size != 8 && size != 16 && size != 32)
				{
					goto IL_4F;
				}
			}
			else if (size != 64 && size != 128 && size != 256)
			{
				goto IL_4F;
			}
			this.trail = new T[size];
			this.mask = size - 1;
			return;
			IL_4F:
			throw new ArgumentException("The size of the breadcrumb trail must be either 8, 16, 32, 64, 128, or 256", "size");
		}

		public int LastFilledIndex
		{
			get
			{
				return this.index & this.mask;
			}
		}

		public T[] BreadCrumb
		{
			get
			{
				return this.trail;
			}
		}

		public void Drop(T bc)
		{
			int num = Interlocked.Increment(ref this.index);
			this.trail[num & this.mask] = bc;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.trail.Length * 32);
			int num = this.index;
			int num2 = Math.Max(0, num - this.trail.Length);
			stringBuilder.AppendFormat("LastIndex: {0}{1}", num, Environment.NewLine);
			for (int i = num2; i <= num; i++)
			{
				stringBuilder.AppendFormat("{0}: {1}{2}", i, this.trail[i & this.mask], Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			int last = this.index;
			int first = Math.Max(0, last - this.trail.Length);
			for (int i = first; i <= last; i++)
			{
				yield return this.trail[i & this.mask];
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!0>)this).GetEnumerator();
		}

		private readonly int mask;

		private int index = -1;

		private T[] trail;
	}
}

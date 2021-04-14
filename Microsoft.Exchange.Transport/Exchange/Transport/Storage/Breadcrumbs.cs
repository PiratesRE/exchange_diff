using System;
using System.Threading;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class Breadcrumbs
	{
		internal Breadcrumbs(int size)
		{
			if (size <= 32)
			{
				if (size == 8 || size == 16 || size == 32)
				{
					goto IL_4E;
				}
			}
			else if (size == 64 || size == 128 || size == 256)
			{
				goto IL_4E;
			}
			throw new ArgumentException(Strings.BreadCrumbSize, "size");
			IL_4E:
			this.trail = new Breadcrumb[size];
			this.mask = size - 1;
		}

		private Breadcrumbs()
		{
		}

		public int LastFilledIndex
		{
			get
			{
				return this.index & this.mask;
			}
		}

		public void Drop(Breadcrumb bc)
		{
			int num = Interlocked.Increment(ref this.index);
			this.trail[num & this.mask] = bc;
		}

		private readonly int mask;

		private Breadcrumb[] trail;

		private int index = -1;
	}
}

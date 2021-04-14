using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public struct PropertyCategories
	{
		public PropertyCategories(int c1)
		{
			this.categoryBitmask = 1UL << c1;
		}

		public PropertyCategories(int c1, int c2)
		{
			this.categoryBitmask = (1UL << c1 | 1UL << c2);
		}

		public PropertyCategories(int c1, int c2, int c3)
		{
			this.categoryBitmask = (1UL << c1 | 1UL << c2 | 1UL << c3);
		}

		public PropertyCategories(int c1, int c2, int c3, int c4)
		{
			this.categoryBitmask = (1UL << c1 | 1UL << c2 | 1UL << c3 | 1UL << c4);
		}

		public PropertyCategories(int c1, int c2, int c3, int c4, int c5)
		{
			this.categoryBitmask = (1UL << c1 | 1UL << c2 | 1UL << c3 | 1UL << c4 | 1UL << c5);
		}

		public PropertyCategories(params int[] c)
		{
			ulong num = 0UL;
			for (int i = 0; i < c.Length; i++)
			{
				num |= 1UL << c[i];
			}
			this.categoryBitmask = num;
		}

		public bool CheckCategory(int categoryNumber)
		{
			return 0UL != (this.categoryBitmask & 1UL << categoryNumber);
		}

		public static readonly PropertyCategories Empty = default(PropertyCategories);

		private ulong categoryBitmask;
	}
}

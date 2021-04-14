using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SortOrder : IEquatable<SortOrder>
	{
		public SortOrder()
		{
			this.sorts = new List<SortOrder.Sort>();
			this.expandCount = 0;
			this.categoriesCount = 0;
		}

		public SortOrder(PropTag propTag, SortFlags sortFlag) : this()
		{
			this.Add(propTag, sortFlag);
		}

		public unsafe SortOrder(byte[] sortosBlob) : this()
		{
			if (sortosBlob.Length < SSortOrderSet.SizeOf)
			{
				MapiExceptionHelper.ThrowIfError("Invalid SORTOS blob", -2147024809);
			}
			fixed (byte* ptr = sortosBlob)
			{
				SSortOrderSet* ptr2 = (SSortOrderSet*)ptr;
				if (sortosBlob.Length < (int)Marshal.OffsetOf(typeof(SSortOrderSet), "aSorts") + ptr2->cSorts * SSortOrder.SizeOf)
				{
					MapiExceptionHelper.ThrowIfError("Invalid SORTOS blob", -2147024809);
				}
				SSortOrder* ptr3 = &ptr2->aSorts;
				for (int i = 0; i < ptr2->cSorts; i++)
				{
					if (this.categoriesCount < ptr2->cCategories)
					{
						this.AddCategory((PropTag)ptr3[i].ulPropTag, (SortFlags)ptr3[i].ulOrder);
						if (i + 1 < ptr2->cSorts && (ptr3[i + 1].ulOrder == 4 || ptr3[i + 1].ulOrder == 8))
						{
							this.Add((PropTag)ptr3[i + 1].ulPropTag, (SortFlags)ptr3[i + 1].ulOrder);
							i++;
						}
					}
					else
					{
						this.Add((PropTag)ptr3[i].ulPropTag, (SortFlags)ptr3[i].ulOrder);
					}
				}
			}
		}

		public int CategoriesCount
		{
			get
			{
				return this.categoriesCount;
			}
		}

		public int ExpandCount
		{
			get
			{
				return this.expandCount;
			}
			set
			{
				this.expandCount = value;
			}
		}

		public void Add(PropTag propTag, SortFlags sortFlag)
		{
			this.sorts.Add(new SortOrder.Sort(propTag, sortFlag));
		}

		public void AddCategory(PropTag propTag, SortFlags sortFlag)
		{
			this.sorts.Add(new SortOrder.Sort(propTag, sortFlag));
			this.categoriesCount++;
		}

		public int GetSortCount()
		{
			return this.sorts.Count;
		}

		public void EnumerateSortOrder(SortOrder.EnumSortOrderDelegate del, object ctx)
		{
			int num = 0;
			for (int i = 0; i < this.sorts.Count; i++)
			{
				if (num < this.categoriesCount)
				{
					del(this.sorts[i].PropertyTag, this.sorts[i].Direction, true, ctx);
					if (i + 1 < this.sorts.Count && (this.sorts[i + 1].Direction == SortFlags.CategoryMax || this.sorts[i + 1].Direction == SortFlags.CategoryMin))
					{
						del(this.sorts[i + 1].PropertyTag, this.sorts[i + 1].Direction, false, ctx);
						i++;
					}
					num++;
				}
				else
				{
					del(this.sorts[i].PropertyTag, this.sorts[i].Direction, false, ctx);
				}
			}
		}

		public override bool Equals(object comparand)
		{
			return comparand is SortOrder && this.Equals((SortOrder)comparand);
		}

		public bool Equals(SortOrder comparand)
		{
			return this.IsEqualTo(comparand);
		}

		public static bool Equals(SortOrder v1, SortOrder v2)
		{
			return v1.Equals(v2);
		}

		public override int GetHashCode()
		{
			return this.sorts.GetHashCode() + this.categoriesCount;
		}

		internal bool IsEqualTo(SortOrder other)
		{
			if (this.categoriesCount != other.categoriesCount || this.ExpandCount != other.ExpandCount)
			{
				return false;
			}
			if (this.sorts.Count != other.sorts.Count)
			{
				return false;
			}
			for (int i = 0; i < this.sorts.Count; i++)
			{
				if (this.sorts[i].PropertyTag != other.sorts[i].PropertyTag || this.sorts[i].Direction != other.sorts[i].Direction)
				{
					return false;
				}
			}
			return true;
		}

		internal int GetBytesToMarshal()
		{
			return SSortOrderSet.SizeOf + this.GetSortCount() * SSortOrder.SizeOf;
		}

		internal unsafe void MarshalToNative(SSortOrderSet* pssortset)
		{
			pssortset->cSorts = this.sorts.Count;
			pssortset->cCategories = this.categoriesCount;
			pssortset->cExpanded = this.ExpandCount;
			SSortOrder* ptr = &pssortset->aSorts;
			int i = 0;
			while (i < this.sorts.Count)
			{
				ptr->ulPropTag = (int)this.sorts[i].PropertyTag;
				ptr->ulOrder = (int)this.sorts[i].Direction;
				i++;
				ptr++;
			}
		}

		private readonly List<SortOrder.Sort> sorts;

		private int expandCount;

		private int categoriesCount;

		private struct Sort
		{
			public Sort(PropTag propertyTag, SortFlags direction)
			{
				this.propertyTag = propertyTag;
				this.direction = direction;
			}

			public PropTag PropertyTag
			{
				get
				{
					return this.propertyTag;
				}
			}

			public SortFlags Direction
			{
				get
				{
					return this.direction;
				}
			}

			private readonly PropTag propertyTag;

			private readonly SortFlags direction;
		}

		public delegate void EnumSortOrderDelegate(PropTag ptag, SortFlags flags, bool isCategory, object ctx);
	}
}

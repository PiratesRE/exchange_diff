using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemCategoryList : ICollection<string>, IEnumerable<string>, IEnumerable
	{
		public ItemCategoryList(IItem item)
		{
			this.item = item;
			this.initialValue = this.GetNativeValue();
			Array.Sort<string>(this.initialValue, Category.NameComparer);
		}

		public int Count
		{
			get
			{
				return this.GetNativeValue().Length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void AddRange(ICollection<string> categoryNames)
		{
			if (categoryNames == null)
			{
				throw new ArgumentNullException("categoryNames");
			}
			HashSet<string> hashSet = new HashSet<string>(this);
			foreach (string text in categoryNames)
			{
				if (text == null)
				{
					throw new ArgumentNullException("categoryNames");
				}
				if (hashSet.Contains(text))
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Category {0} already exists on the item", new object[]
					{
						text
					}), "categoryName");
				}
				hashSet.Add(text);
			}
			this.SetNativeValue(Util.MergeArrays<string>(new ICollection<string>[]
			{
				categoryNames,
				this.GetNativeValue()
			}));
		}

		public void AddRange(params string[] categoryNames)
		{
			this.AddRange((ICollection<string>)categoryNames);
		}

		public void Add(string categoryName)
		{
			this.AddRange(new string[]
			{
				categoryName
			});
		}

		public void Clear()
		{
			this.SetNativeValue(Array<string>.Empty);
		}

		public bool Contains(string categoryName)
		{
			return -1 != ItemCategoryList.IndexOf(this.GetNativeValue(), categoryName);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			this.GetNativeValue().CopyTo(array, arrayIndex);
		}

		public bool Remove(string item)
		{
			string[] nativeValue = this.GetNativeValue();
			int num = ItemCategoryList.IndexOf(nativeValue, item);
			if (num != -1)
			{
				this.SetNativeValue(Util.RemoveArrayElements<string>(nativeValue, new int[]
				{
					num
				}));
				return true;
			}
			return false;
		}

		public IEnumerator<string> GetEnumerator()
		{
			return ((IList<string>)this.GetNativeValue()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetNativeValue().GetEnumerator();
		}

		internal IEnumerable<string> GetNewCategories()
		{
			if (this.initialValue.Length == 0)
			{
				return this.InternalGetNewCategories();
			}
			return this;
		}

		private static int IndexOf(string[] categories, string categoryName)
		{
			for (int i = 0; i < categories.Length; i++)
			{
				if (Category.NameComparer.Compare(categories[i], categoryName) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private string[] GetNativeValue()
		{
			return (string[])this.item.GetValueOrDefault<string[]>(InternalSchema.Categories, Array<string>.Empty).Clone();
		}

		private IEnumerable<string> InternalGetNewCategories()
		{
			foreach (string assignedCategory in this)
			{
				if (Array.BinarySearch<string>(this.initialValue, assignedCategory, Category.NameComparer) == -1)
				{
					yield return assignedCategory;
				}
			}
			yield break;
		}

		private void SetNativeValue(string[] newValue)
		{
			this.item[InternalSchema.Categories] = newValue;
		}

		private readonly IItem item;

		private readonly string[] initialValue;
	}
}

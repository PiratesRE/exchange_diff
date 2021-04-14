using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MostRecentlyUsedCategories
	{
		public static MostRecentlyUsedCategories Create(MasterCategoryList masterCategoryList, OutlookModule outlookModule)
		{
			return new MostRecentlyUsedCategories(masterCategoryList, outlookModule);
		}

		private MostRecentlyUsedCategories(MasterCategoryList masterCategoryList, OutlookModule outlookModule)
		{
			Category[] array = masterCategoryList.ToArray();
			Array.Sort<Category>(array, MasterCategoryList.CreateUsageBasedComparer(outlookModule));
			int num = 10;
			if (array.Length < 10)
			{
				num = array.Length;
			}
			this.mostRecentCategories = new Category[num];
			Array.Copy(array, this.mostRecentCategories, num);
			Array.Sort<Category>(this.mostRecentCategories, new MostRecentlyUsedCategories.CategoryNameComparer());
			int num2 = array.Length - num;
			if (0 < num2)
			{
				this.otherCategories = new Category[num2];
				Array.Copy(array, num, this.otherCategories, 0, num2);
				Array.Sort<Category>(this.otherCategories, new MostRecentlyUsedCategories.CategoryNameComparer());
			}
		}

		public Category[] MostRecentCategories
		{
			get
			{
				return this.mostRecentCategories;
			}
		}

		public Category[] OtherCategories
		{
			get
			{
				return this.otherCategories;
			}
		}

		private const int MaxMostRecentCategoryCount = 10;

		private Category[] mostRecentCategories;

		private Category[] otherCategories;

		public class CategoryNameComparer : IComparer<Category>
		{
			public int Compare(Category categoryX, Category categoryY)
			{
				if (categoryX == null)
				{
					throw new ArgumentNullException("categoryX");
				}
				if (categoryY == null)
				{
					throw new ArgumentNullException("categoryY");
				}
				return categoryX.Name.CompareTo(categoryY.Name);
			}
		}
	}
}

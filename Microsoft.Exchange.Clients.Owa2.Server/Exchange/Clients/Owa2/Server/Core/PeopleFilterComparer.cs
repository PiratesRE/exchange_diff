using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PeopleFilterComparer : IComparer<PeopleFilter>
	{
		internal PeopleFilterComparer(CultureInfo culture)
		{
			this.culture = (culture ?? CultureInfo.CurrentUICulture);
		}

		public int Compare(PeopleFilter x, PeopleFilter y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return 0;
			}
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			int num = this.CompareSortGroups(x, y);
			if (num != 0)
			{
				return num;
			}
			return this.CompareDisplayNames(x, y);
		}

		private int CompareSortGroups(PeopleFilter x, PeopleFilter y)
		{
			return Comparer<int>.Default.Compare(x.SortGroupPriority, y.SortGroupPriority);
		}

		private int CompareDisplayNames(PeopleFilter x, PeopleFilter y)
		{
			return string.Compare(x.DisplayName, y.DisplayName, this.culture, CompareOptions.IgnoreCase);
		}

		private readonly CultureInfo culture;
	}
}

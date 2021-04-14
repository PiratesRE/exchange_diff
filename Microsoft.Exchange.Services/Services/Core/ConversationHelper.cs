using System;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ConversationHelper
	{
		internal static T[] MergeArray<T>(T[] a, T[] b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return a.Union(b).ToArray<T>();
		}

		internal static int? MergeInts(int? a, int? b)
		{
			if (a != null && b != null)
			{
				int? num = a;
				int? num2 = b;
				if (!(num != null & num2 != null))
				{
					return null;
				}
				return new int?(num.GetValueOrDefault() + num2.GetValueOrDefault());
			}
			else
			{
				if (a != null)
				{
					return a;
				}
				return b;
			}
		}

		internal static string MergeDates(string a, string b)
		{
			if (string.IsNullOrEmpty(a))
			{
				return b;
			}
			if (string.IsNullOrEmpty(b))
			{
				return a;
			}
			DateTime dateTime = DateTime.Parse(a);
			DateTime value = DateTime.Parse(b);
			if (dateTime.CompareTo(value) > 0)
			{
				return a;
			}
			return b;
		}

		internal static bool? MergeBoolNullable(bool? a, bool? b)
		{
			if (a == null)
			{
				return b;
			}
			if (b != null)
			{
				return new bool?(a.Value || b.Value);
			}
			return a;
		}

		internal static ConversationTypeEqualityComparer ConversationTypeEqualityComparer = new ConversationTypeEqualityComparer();

		internal static ConversationNodeEqualityComparer ConversationNodeEqualityComparer = new ConversationNodeEqualityComparer();

		internal static DateTimeStringComparer DateTimeStringComparer = new DateTimeStringComparer();
	}
}

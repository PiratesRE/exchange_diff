using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal static class ExtensionHelper
	{
		public static TSource AggregateOfDefault<TSource>(this IEnumerable<TSource> sources, Func<TSource, TSource, TSource> func)
		{
			return sources.DefaultIfEmpty<TSource>().Aggregate(func);
		}

		public static void ForEach<TSource>(this IEnumerable<TSource> sources, Action<TSource> func)
		{
			if (sources != null)
			{
				foreach (TSource obj in sources)
				{
					func(obj);
				}
			}
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> sources)
		{
			return sources == null || sources.Count<TSource>() == 0;
		}

		public static string ValueOrDefault(this string str, string defVal)
		{
			if (!str.IsNullOrEmpty())
			{
				return str;
			}
			return defVal;
		}

		public static string DomainUserName(this ADObjectId adId)
		{
			return adId.DomainId.Name + "\\" + adId.Name;
		}

		public static string ToLabelTag(this Globals.LogFields logField)
		{
			return "{Label" + logField.ToString() + "}";
		}

		public static string ToValueTag(this Globals.LogFields logField)
		{
			return "{" + logField.ToString() + "}";
		}

		public static StoreId GetSubFolderIdByName(this Folder parentFolder, string folderName)
		{
			StoreId result = null;
			using (QueryResult queryResult = parentFolder.FolderQuery(FolderQueryFlags.None, new TextFilter(FolderSchema.DisplayName, folderName, MatchOptions.ExactPhrase, MatchFlags.IgnoreCase), null, new PropertyDefinition[]
			{
				FolderSchema.Id,
				FolderSchema.DisplayName
			}))
			{
				foreach (Pair<StoreId, string> pair in queryResult.Enumerator<StoreId, string>())
				{
					if (pair.First != null && folderName.Equals(pair.Second, StringComparison.OrdinalIgnoreCase))
					{
						result = pair.First;
						break;
					}
				}
			}
			return result;
		}

		public static List<Pair<StoreId, string>> GetSubFoldersWithIdAndName(this Folder parentFolder)
		{
			List<Pair<StoreId, string>> result;
			using (QueryResult queryResult = parentFolder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
			{
				FolderSchema.Id,
				FolderSchema.DisplayName
			}))
			{
				result = (from x in queryResult.Enumerator<StoreId, string>()
				where x != null
				select x).ToList<Pair<StoreId, string>>();
			}
			return result;
		}
	}
}

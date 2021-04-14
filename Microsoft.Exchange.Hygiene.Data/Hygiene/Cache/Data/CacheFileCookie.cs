using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheFileCookie : IEquatable<CacheFileCookie>
	{
		public CacheFileCookie()
		{
			this.EntityName = string.Empty;
			this.PartitionIndex = 0;
			this.Cookie = string.Empty;
			this.CacheCookieRows = new SortedList<int, CacheCookieRow>();
			this.StartFileOffset = 0L;
			this.EndFileOffset = 0L;
			this.PreCookieOffset = -1L;
			this.NextCookieOffset = -1L;
			this.CookieOffset = -1L;
		}

		public CacheFileCookie(CacheFileCookie copy)
		{
			this.EntityName = copy.EntityName;
			this.PartitionIndex = copy.PartitionIndex;
			this.Cookie = copy.Cookie;
			this.CacheCookieRows = CacheFileCookie.CopyCacheCookieRows(copy.CacheCookieRows);
			this.StartFileOffset = copy.StartFileOffset;
			this.EndFileOffset = copy.EndFileOffset;
			this.PreCookieOffset = copy.PreCookieOffset;
			this.NextCookieOffset = copy.NextCookieOffset;
			this.CookieOffset = copy.CookieOffset;
		}

		public CacheFileCookie(string entityName, int partitionIndex) : this()
		{
			this.EntityName = entityName;
			this.PartitionIndex = partitionIndex;
		}

		public string EntityName { get; set; }

		public int PartitionIndex { get; set; }

		public string Cookie { get; set; }

		public SortedList<int, CacheCookieRow> CacheCookieRows { get; set; }

		public long StartFileOffset { get; set; }

		public long EndFileOffset { get; set; }

		public long PreCookieOffset { get; set; }

		public long NextCookieOffset { get; set; }

		public long CookieOffset { get; set; }

		public static SortedList<int, CacheCookieRow> CopyCacheCookieRows(SortedList<int, CacheCookieRow> crList2)
		{
			if (crList2 == null)
			{
				return null;
			}
			SortedList<int, CacheCookieRow> sortedList = new SortedList<int, CacheCookieRow>();
			foreach (CacheCookieRow cacheCookieRow in crList2.Values)
			{
				sortedList.Add(cacheCookieRow.CopyIndex, cacheCookieRow);
			}
			return sortedList;
		}

		public static bool CacheCookieRowEquals(SortedList<int, CacheCookieRow> crList1, SortedList<int, CacheCookieRow> crList2)
		{
			if (crList1 == null || crList2 == null)
			{
				return crList1 == null && crList2 == null;
			}
			if (crList1.Count<KeyValuePair<int, CacheCookieRow>>() != crList2.Count<KeyValuePair<int, CacheCookieRow>>())
			{
				return false;
			}
			int num = 0;
			foreach (CacheCookieRow cacheCookieRow in crList1.Values)
			{
				if (!cacheCookieRow.Equals(crList2[num]))
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public bool Equals(CacheFileCookie c2)
		{
			return c2 != null && (this.EntityName == c2.EntityName && this.PartitionIndex == c2.PartitionIndex && this.Cookie == c2.Cookie && this.StartFileOffset == c2.StartFileOffset && this.EndFileOffset == c2.EndFileOffset && this.PreCookieOffset == c2.PreCookieOffset && this.NextCookieOffset == c2.NextCookieOffset && this.CookieOffset == c2.CookieOffset) && CacheFileCookie.CacheCookieRowEquals(this.CacheCookieRows, c2.CacheCookieRows);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is CacheFileCookie && this.Equals(obj as CacheFileCookie);
		}

		public override int GetHashCode()
		{
			string text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this.EntityName,
				this.PartitionIndex,
				this.Cookie,
				this.StartFileOffset,
				this.EndFileOffset,
				this.CookieOffset
			});
			return text.GetHashCode();
		}
	}
}

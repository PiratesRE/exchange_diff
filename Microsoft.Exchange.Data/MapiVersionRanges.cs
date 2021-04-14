using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal class MapiVersionRanges
	{
		public MapiVersionRanges(string s)
		{
			if (s == null || s.Trim().Length == 0)
			{
				return;
			}
			string[] array = s.Split(MapiVersionRanges.separators);
			foreach (string s2 in array)
			{
				this.Ranges.Add(MapiVersionRanges.MapiVersionRange.Parse(s2));
			}
		}

		public bool IsIncluded(MapiVersion version)
		{
			foreach (MapiVersionRanges.MapiVersionRange mapiVersionRange in this.Ranges)
			{
				if (mapiVersionRange.IsInlcuded(version))
				{
					return true;
				}
			}
			return false;
		}

		internal readonly List<MapiVersionRanges.MapiVersionRange> Ranges = new List<MapiVersionRanges.MapiVersionRange>();

		private static readonly char[] separators = new char[]
		{
			',',
			';'
		};

		public struct MapiVersionRange
		{
			public static MapiVersionRanges.MapiVersionRange Parse(string s)
			{
				string[] array = s.Split(new char[]
				{
					'-'
				});
				string text;
				string text2;
				if (array.Length == 2)
				{
					text = array[0];
					text2 = array[1];
				}
				else
				{
					if (array.Length != 1)
					{
						throw new FormatException();
					}
					text2 = (text = array[0]);
				}
				bool flag = false;
				bool flag2 = false;
				MapiVersion start;
				if (text.Trim().Length == 0)
				{
					start = MapiVersion.Min;
				}
				else
				{
					start = MapiVersion.Parse(text);
					flag = true;
				}
				MapiVersion end;
				if (text2.Trim().Length == 0)
				{
					end = MapiVersion.Max;
				}
				else
				{
					end = MapiVersion.Parse(text2);
					flag2 = true;
				}
				if (!flag && !flag2)
				{
					throw new FormatException();
				}
				return new MapiVersionRanges.MapiVersionRange(start, end);
			}

			private MapiVersionRange(MapiVersion start, MapiVersion end)
			{
				this.Start = start;
				this.End = end;
			}

			public bool IsInlcuded(MapiVersion version)
			{
				return this.Start <= version && version <= this.End;
			}

			internal readonly MapiVersion Start;

			internal readonly MapiVersion End;
		}
	}
}

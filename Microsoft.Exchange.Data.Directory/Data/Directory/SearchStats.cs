using System;
using System.DirectoryServices.Protocols;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class SearchStats
	{
		private SearchStats()
		{
		}

		public static SearchStats Parse(byte[] value)
		{
			SearchStats searchStats = null;
			try
			{
				object[] array = BerConverter.Decode("{iiiiiiiiiaia}", value);
				searchStats = new SearchStats();
				searchStats.callTime = (int)array[3];
				searchStats.entriesReturned = (int)array[5];
				searchStats.entriesVisited = (int)array[7];
				searchStats.filter = (string)array[9];
				searchStats.index = (string)array[11];
			}
			catch (BerConversionException)
			{
			}
			catch (InvalidCastException)
			{
			}
			catch (DecoderFallbackException)
			{
			}
			if (searchStats != null)
			{
				if (!string.IsNullOrEmpty(searchStats.filter))
				{
					SearchStats searchStats2 = searchStats;
					string text = searchStats.filter;
					char[] trimChars = new char[1];
					searchStats2.filter = text.TrimEnd(trimChars);
				}
				if (string.IsNullOrEmpty(searchStats.filter))
				{
					searchStats.filter = "<null>";
				}
				if (!string.IsNullOrEmpty(searchStats.index))
				{
					SearchStats searchStats3 = searchStats;
					string text2 = searchStats.index;
					char[] trimChars2 = new char[1];
					searchStats3.index = text2.TrimEnd(trimChars2);
				}
				if (string.IsNullOrEmpty(searchStats.index))
				{
					searchStats.index = "<null>";
				}
			}
			return searchStats;
		}

		public int EntriesReturned
		{
			get
			{
				return this.entriesReturned;
			}
		}

		public int EntriesVisited
		{
			get
			{
				return this.entriesVisited;
			}
		}

		public string Index
		{
			get
			{
				return this.index;
			}
		}

		public string Filter
		{
			get
			{
				return this.filter;
			}
		}

		public int CallTime
		{
			get
			{
				return this.callTime;
			}
		}

		private int entriesReturned;

		private int entriesVisited;

		private string filter;

		private string index;

		private int callTime;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class IndexInfo
	{
		internal IndexInfo(string name, CultureInfo cultureInfo, CompareOptions compareOptions, IndexSegment[] indexSegments, CreateIndexGrbit grbit, int keys, int entries, int pages)
		{
			this.name = name;
			this.cultureInfo = cultureInfo;
			this.compareOptions = compareOptions;
			this.indexSegments = new ReadOnlyCollection<IndexSegment>(indexSegments);
			this.grbit = grbit;
			this.keys = keys;
			this.entries = entries;
			this.pages = pages;
		}

		public string Name
		{
			[DebuggerStepThrough]
			get
			{
				return this.name;
			}
		}

		public CultureInfo CultureInfo
		{
			[DebuggerStepThrough]
			get
			{
				return this.cultureInfo;
			}
		}

		public CompareOptions CompareOptions
		{
			[DebuggerStepThrough]
			get
			{
				return this.compareOptions;
			}
		}

		public IList<IndexSegment> IndexSegments
		{
			[DebuggerStepThrough]
			get
			{
				return this.indexSegments;
			}
		}

		public CreateIndexGrbit Grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.grbit;
			}
		}

		public int Keys
		{
			[DebuggerStepThrough]
			get
			{
				return this.keys;
			}
		}

		public int Entries
		{
			[DebuggerStepThrough]
			get
			{
				return this.entries;
			}
		}

		public int Pages
		{
			[DebuggerStepThrough]
			get
			{
				return this.pages;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Name);
			stringBuilder.Append(" (");
			foreach (IndexSegment indexSegment in this.IndexSegments)
			{
				stringBuilder.Append(indexSegment.ToString());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private readonly string name;

		private readonly CultureInfo cultureInfo;

		private readonly CompareOptions compareOptions;

		private readonly ReadOnlyCollection<IndexSegment> indexSegments;

		private readonly CreateIndexGrbit grbit;

		private readonly int keys;

		private readonly int entries;

		private readonly int pages;
	}
}

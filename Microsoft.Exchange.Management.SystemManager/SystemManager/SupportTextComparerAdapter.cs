using System;
using System.Collections;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class SupportTextComparerAdapter : IComparer
	{
		public SupportTextComparerAdapter(ISupportTextComparer supportTextComparer, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			this.SupportTextComparer = supportTextComparer;
			this.CustomFormatter = customFormatter;
			this.FormatProvider = formatProvider;
			this.FormatString = formatString;
			this.DefaultEmptyText = defaultEmptyText;
		}

		public int Compare(object x, object y)
		{
			return this.SupportTextComparer.Compare(x, y, this.CustomFormatter, this.FormatProvider, this.FormatString, this.DefaultEmptyText);
		}

		public ISupportTextComparer SupportTextComparer { get; private set; }

		public ICustomFormatter CustomFormatter { get; private set; }

		public IFormatProvider FormatProvider { get; private set; }

		public string FormatString { get; private set; }

		public string DefaultEmptyText { get; private set; }
	}
}

using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface ISupportTextComparer
	{
		int Compare(object x, object y, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText);
	}
}

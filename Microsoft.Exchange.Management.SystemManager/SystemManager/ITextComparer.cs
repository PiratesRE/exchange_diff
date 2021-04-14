using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface ITextComparer : ISupportTextComparer
	{
		string Format(object item, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText);
	}
}

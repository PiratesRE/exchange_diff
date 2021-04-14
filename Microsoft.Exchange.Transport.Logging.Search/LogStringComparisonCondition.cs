using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("StringCompare")]
	public class LogStringComparisonCondition : LogComparisonCondition
	{
		public bool IgnoreCase;
	}
}

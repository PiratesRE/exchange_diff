using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("Or")]
	public class LogOrCondition : LogCompoundCondition
	{
	}
}

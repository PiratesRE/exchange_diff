using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("And")]
	public class LogAndCondition : LogCompoundCondition
	{
	}
}

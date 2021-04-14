using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("EndsWith")]
	public class LogStringEndsWithCondition : LogBinaryStringOperatorCondition
	{
	}
}

using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("Contains")]
	public class LogStringContainsCondition : LogBinaryStringOperatorCondition
	{
	}
}

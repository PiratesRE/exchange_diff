using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("StartsWith")]
	public class LogStringStartsWithCondition : LogBinaryStringOperatorCondition
	{
	}
}

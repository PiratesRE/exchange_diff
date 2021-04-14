using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("IsNullOrEmpty")]
	public class LogIsNullOrEmptyCondition : LogUnaryOperatorCondition
	{
	}
}

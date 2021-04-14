using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("Constant")]
	public class LogConditionConstant : LogConditionOperand
	{
		public object Value;
	}
}

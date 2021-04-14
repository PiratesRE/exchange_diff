using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("Field")]
	public class LogConditionField : LogConditionOperand
	{
		public string Name;
	}
}

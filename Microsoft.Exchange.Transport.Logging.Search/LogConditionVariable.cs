using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlType("Variable")]
	public class LogConditionVariable : LogConditionOperand
	{
		public string Name;
	}
}

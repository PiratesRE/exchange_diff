using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogIsNullOrEmptyCondition))]
	public abstract class LogUnaryOperatorCondition : LogCondition
	{
		public LogConditionOperand Operand;
	}
}

using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogComparisonCondition))]
	[XmlInclude(typeof(LogBinaryStringOperatorCondition))]
	public abstract class LogBinaryOperatorCondition : LogCondition
	{
		public LogConditionOperand Left;

		public LogConditionOperand Right;
	}
}

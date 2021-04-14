using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogStringComparisonCondition))]
	[XmlType("Compare")]
	public class LogComparisonCondition : LogBinaryOperatorCondition
	{
		public LogComparisonOperator Operator;
	}
}

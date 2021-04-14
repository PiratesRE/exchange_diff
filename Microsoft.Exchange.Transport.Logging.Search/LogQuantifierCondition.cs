using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogForEveryCondition))]
	[XmlInclude(typeof(LogForAnyCondition))]
	public abstract class LogQuantifierCondition : LogUnaryCondition
	{
		public LogConditionField Field;

		public LogConditionVariable Variable;
	}
}

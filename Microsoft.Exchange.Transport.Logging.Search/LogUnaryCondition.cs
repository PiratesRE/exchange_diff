using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogQuantifierCondition))]
	[XmlInclude(typeof(LogNotCondition))]
	public abstract class LogUnaryCondition : LogCondition
	{
		public LogCondition Condition;
	}
}

using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogConditionField))]
	[XmlInclude(typeof(LogConditionVariable))]
	[XmlInclude(typeof(LogConditionConstant))]
	public abstract class LogConditionOperand
	{
	}
}

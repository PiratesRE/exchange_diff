using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogStringEndsWithCondition))]
	[XmlInclude(typeof(LogStringContainsCondition))]
	[XmlInclude(typeof(LogStringStartsWithCondition))]
	public abstract class LogBinaryStringOperatorCondition : LogBinaryOperatorCondition
	{
		public bool IgnoreCase;
	}
}

using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogCompoundCondition))]
	[XmlInclude(typeof(LogBinaryOperatorCondition))]
	[XmlInclude(typeof(LogUnaryOperatorCondition))]
	[XmlInclude(typeof(LogUnaryCondition))]
	[XmlInclude(typeof(LogTrueCondition))]
	[XmlInclude(typeof(LogFalseCondition))]
	public abstract class LogCondition
	{
		public static LogTrueCondition True
		{
			get
			{
				return LogCondition.trueCondition;
			}
		}

		public static LogFalseCondition False
		{
			get
			{
				return LogCondition.falseCondition;
			}
		}

		private static readonly LogTrueCondition trueCondition = new LogTrueCondition();

		private static readonly LogFalseCondition falseCondition = new LogFalseCondition();
	}
}

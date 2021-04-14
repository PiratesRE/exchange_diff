using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[XmlInclude(typeof(LogAndCondition))]
	[XmlInclude(typeof(LogOrCondition))]
	public abstract class LogCompoundCondition : LogCondition
	{
		[XmlArrayItem(ElementName = "Condition")]
		public List<LogCondition> Conditions
		{
			get
			{
				return this.conditions;
			}
		}

		private readonly List<LogCondition> conditions = new List<LogCondition>();
	}
}

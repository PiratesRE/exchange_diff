using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal abstract class LogSerializer
	{
		public LogSerializer()
		{
		}

		public LogSerializer(IEnumerable<PropertyDefinition> propertyMask)
		{
			this.propertyMask = propertyMask;
		}

		public string Serialize(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, IEnumerable<PropertyDefinition> maskedProperties = null)
		{
			if (maskedProperties == null)
			{
				maskedProperties = new List<PropertyDefinition>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			properties = properties.Except(maskedProperties);
			this.SerializeHeader(logs, properties, ref stringBuilder);
			this.SerializeBody(logs, properties, ref stringBuilder);
			this.SerializeFooter(logs, properties, ref stringBuilder);
			return stringBuilder.ToString();
		}

		protected virtual void SerializeHeader(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
		}

		protected virtual void SerializeBody(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
		}

		protected virtual void SerializeFooter(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
		}

		protected IEnumerable<PropertyDefinition> propertyMask = new List<PropertyDefinition>();
	}
}

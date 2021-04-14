using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Data
{
	internal class RedactionProcessor : IDataProcessor
	{
		private RedactionProcessor(Type targetType)
		{
			this.propertiesNeedingRedaction = Schema.Utilities.GetProperties<RedactAttribute>(targetType);
		}

		internal static RedactionProcessor Create<TRedactionTargetType>()
		{
			return new RedactionProcessor(typeof(TRedactionTargetType));
		}

		public object Process(object input)
		{
			if (this.propertiesNeedingRedaction.Count > 0)
			{
				Schema.Utilities.Redact(input, this.propertiesNeedingRedaction);
			}
			return input;
		}

		private readonly IList<Tuple<PropertyInfo, RedactAttribute>> propertiesNeedingRedaction;
	}
}

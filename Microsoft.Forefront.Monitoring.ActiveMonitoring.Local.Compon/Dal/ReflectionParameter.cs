using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class ReflectionParameter
	{
		[XmlAttribute]
		public string Value { get; set; }

		[XmlAttribute]
		public string Type { get; set; }

		public object Evaluate(IDictionary<string, object> variables)
		{
			object obj = DalProbeOperation.GetValue(this.Value, variables);
			if (obj != null && this.Type != null)
			{
				Type type = DalProbeOperation.ResolveDataType(this.Type);
				if (type != null && !type.IsInstanceOfType(obj))
				{
					obj = DalHelper.ConvertFromStoreObject(obj, type);
				}
			}
			return obj;
		}
	}
}

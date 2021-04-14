using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public abstract class ReflectionOperation : DalProbeOperation
	{
		[XmlAttribute]
		public string Type { get; set; }

		[XmlArrayItem("Parameter")]
		public ReflectionParameter[] Parameters { get; set; }

		protected object[] GetParameterValues(IDictionary<string, object> variables)
		{
			if (this.Parameters == null)
			{
				return new object[0];
			}
			return (from parameter in this.Parameters
			select parameter.Evaluate(variables)).ToArray<object>();
		}
	}
}

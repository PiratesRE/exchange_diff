using System;
using System.Collections.Generic;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class ReflectionCreateOperation : ReflectionOperation
	{
		public override void Execute(IDictionary<string, object> variables)
		{
			Type type = DalProbeOperation.ResolveDataType(base.Type);
			object[] parameterValues = base.GetParameterValues(variables);
			object value = Activator.CreateInstance(type, parameterValues);
			if (!string.IsNullOrEmpty(base.Return))
			{
				variables[base.Return] = value;
			}
		}
	}
}

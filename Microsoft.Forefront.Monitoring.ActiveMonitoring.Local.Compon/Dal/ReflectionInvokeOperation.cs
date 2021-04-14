using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class ReflectionInvokeOperation : ReflectionOperation
	{
		[XmlAttribute]
		public string This { get; set; }

		[XmlAttribute]
		public string Method { get; set; }

		[XmlAttribute]
		public string ParameterTypes { get; set; }

		public override void Execute(IDictionary<string, object> variables)
		{
			object obj = null;
			if (!string.IsNullOrEmpty(this.This))
			{
				obj = DalProbeOperation.GetValue(this.This, variables);
			}
			Type type = (obj != null) ? obj.GetType() : DalProbeOperation.ResolveDataType(base.Type);
			object[] parameterValues = base.GetParameterValues(variables);
			object value = type.InvokeMember(this.Method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, new DalProbeBinder(), obj, parameterValues);
			if (!string.IsNullOrEmpty(base.Return))
			{
				variables[base.Return] = value;
			}
			for (int i = 0; i < parameterValues.Length; i++)
			{
				if (DalProbeOperation.IsVariable(base.Parameters[i].Value))
				{
					variables[base.Parameters[i].Value] = parameterValues[i];
				}
			}
		}
	}
}

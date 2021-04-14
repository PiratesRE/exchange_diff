using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DDIParameterAttribute : DDIValidateAttribute
	{
		public DDIParameterAttribute() : base("DDIParameterAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			List<string> list = new List<string>();
			if (target != null)
			{
				Parameter parameter = target as Parameter;
				if (parameter == null)
				{
					throw new ArgumentException("DDIParameterAttribute can only be applied to Parameter object");
				}
				if (!string.IsNullOrWhiteSpace(parameter.Reference) && parameter.Value != null)
				{
					list.Add(string.Format("Cannot both specify Reference and Value at the same time", target));
				}
			}
			return list;
		}
	}
}

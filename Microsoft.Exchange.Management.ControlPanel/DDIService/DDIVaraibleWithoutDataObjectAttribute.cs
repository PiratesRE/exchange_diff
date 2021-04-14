using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIVaraibleWithoutDataObjectAttribute : DDIValidateAttribute
	{
		public DDIVaraibleWithoutDataObjectAttribute() : base("DDIVaraibleWithoutDataObjectAttribute ")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIVaraibleWithoutDataObjectAttribute  can only apply to String property");
			}
			string columnName = target as string;
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(columnName) && profile.Variables.Any((Variable columnProfile) => columnProfile.Name.Equals(columnName) && !string.IsNullOrWhiteSpace(columnProfile.DataObjectName)))
			{
				list.Add(string.Format("OutputVariable must refer to Variable which don't specify value for the property DataObjectName.", target));
			}
			return list;
		}
	}
}

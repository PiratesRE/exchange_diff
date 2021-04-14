using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIVariableNameExistAttribute : DDIValidateAttribute
	{
		public DDIVariableNameExistAttribute() : base("DDIVariableNameExistAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIVariableNameExistAttribute can only apply to String property");
			}
			string columnName = target as string;
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(columnName))
			{
				if (profile.Variables.All((Variable columnProfile) => !columnProfile.Name.Equals(columnName)))
				{
					if (profile.ExtendedColumns.All((DataColumn dataColumn) => dataColumn.ColumnName != columnName))
					{
						list.Add(string.Format("{0} is not a valid variable name", target));
					}
				}
			}
			return list;
		}
	}
}

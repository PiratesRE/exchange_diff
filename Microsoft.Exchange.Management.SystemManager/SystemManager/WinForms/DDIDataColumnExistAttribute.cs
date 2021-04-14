using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIDataColumnExistAttribute : DDIValidateAttribute
	{
		public DDIDataColumnExistAttribute() : base("DDIDataColumnExistAttribute")
		{
		}

		public override List<string> Validate(object target, PageConfigurableProfile profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIDataColumnExistAttribute can only apply to String property");
			}
			string columnName = target as string;
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(columnName) && profile.ColumnProfiles.All((ColumnProfile columnProfile) => !columnProfile.Name.Equals(columnName)))
			{
				list.Add(string.Format("{0} is not a valid column name", target));
			}
			return list;
		}
	}
}

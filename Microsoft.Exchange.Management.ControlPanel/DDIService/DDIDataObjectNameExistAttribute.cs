using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIDataObjectNameExistAttribute : DDIValidateAttribute
	{
		public DDIDataObjectNameExistAttribute() : base("DDIDataObjectNameExistAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIDataObjectNameExistAttribute can only apply to String property");
			}
			string dataObjectName = target as string;
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(dataObjectName) && profile.DataObjects.All((DataObject dataObject) => !dataObject.Name.Equals(dataObjectName)))
			{
				list.Add(string.Format("{0} is not a valid data object name", target));
			}
			return list;
		}
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIMandatoryValueAttribute : DDIValidateAttribute
	{
		public DDIMandatoryValueAttribute() : base("DDIMandatoryValueAttribute")
		{
		}

		public override List<string> Validate(object target, PageConfigurableProfile profile)
		{
			List<string> list = new List<string>();
			if (WinformsHelper.IsEmptyValue(target))
			{
				list.Add("cannot be null or empty");
			}
			return list;
		}
	}
}

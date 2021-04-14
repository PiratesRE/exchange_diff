using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIMandatoryValueAttribute : DDIValidateAttribute
	{
		public DDIMandatoryValueAttribute() : base("DDIMandatoryValueAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
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

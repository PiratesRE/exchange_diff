using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidRoleAttribute : DDIValidateAttribute
	{
		static DDIValidRoleAttribute()
		{
			RbacModule.RegisterQueryProcessors();
		}

		public DDIValidRoleAttribute() : base("DDIValidRoleAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			return DDIValidRoleAttribute.rule.Validate(target, profile);
		}

		private static ValidRoleRule rule = new ValidRoleRule();
	}
}

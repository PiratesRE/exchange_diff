using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Validate", "FfoWebServiceRole")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class ValidateFfoWebServiceRole : ValidatingTask
	{
		public ValidateFfoWebServiceRole()
		{
			base.ValidationTests = new ValidatingCondition[]
			{
				new ValidatingCondition(new ValidationDelegate(this.RoleIsInstalled), Strings.VerifyRoleInstalled, true)
			};
		}

		private bool RoleIsInstalled()
		{
			return new RoleIsInstalledCondition
			{
				RoleInQuestion = RoleManager.GetRoleByName("FfoWebServiceRole")
			}.Verify();
		}
	}
}

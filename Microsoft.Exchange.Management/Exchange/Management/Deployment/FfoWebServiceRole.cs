using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FfoWebServiceRole : Role
	{
		public FfoWebServiceRole()
		{
			this.roleName = "FfoWebServiceRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.FfoWebService;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallFfoWebServiceRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return null;
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallFfoWebServiceRoleRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateFfoWebServiceRole();
			}
		}
	}
}

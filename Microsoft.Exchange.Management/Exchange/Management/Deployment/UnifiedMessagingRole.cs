using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UnifiedMessagingRole : Role
	{
		public UnifiedMessagingRole()
		{
			this.roleName = "UnifiedMessagingRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.UnifiedMessaging;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallUnifiedMessagingRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryUnifiedMessagingRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallUnifiedMessagingRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateUnifiedMessagingRole();
			}
		}
	}
}

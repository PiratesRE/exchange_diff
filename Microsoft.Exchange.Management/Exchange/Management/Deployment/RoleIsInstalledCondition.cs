using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RoleIsInstalledCondition : Condition
	{
		public Role RoleInQuestion
		{
			get
			{
				return this.role;
			}
			set
			{
				this.role = value;
			}
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			bool isInstalled = this.RoleInQuestion.IsInstalled;
			TaskLogger.LogExit();
			return isInstalled;
		}

		private Role role;
	}
}

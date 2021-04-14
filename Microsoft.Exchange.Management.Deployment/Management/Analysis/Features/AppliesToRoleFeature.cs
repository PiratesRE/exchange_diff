using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal class AppliesToRoleFeature : Feature
	{
		public AppliesToRoleFeature(SetupRole roles) : base(true, true)
		{
			this.Role = roles;
		}

		public SetupRole Role { get; private set; }

		public bool Contains(SetupRole role)
		{
			return (this.Role & role) > SetupRole.None;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.Role.ToString());
		}
	}
}

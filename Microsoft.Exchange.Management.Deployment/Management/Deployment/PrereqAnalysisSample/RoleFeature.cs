using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal sealed class RoleFeature : Feature
	{
		public RoleFeature(SetupRole roles)
		{
			this.role = roles;
		}

		public SetupRole Role
		{
			get
			{
				return this.role;
			}
		}

		public bool Contains(SetupRole role)
		{
			return (this.role & role) > SetupRole.None;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.role.ToString());
		}

		private readonly SetupRole role;
	}
}

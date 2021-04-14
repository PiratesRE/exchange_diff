using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Start", "PostSetup", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class StartPostSetup : ManageSetupBindingTasks
	{
		public StartPostSetup()
		{
			base.ImplementsResume = false;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartPostSetupDescription;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			base.Fields["SetupAssemblyPath"] = ConfigurationContext.Setup.AssemblyPath;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			base.ComponentInfoFileNames.Reverse();
		}
	}
}

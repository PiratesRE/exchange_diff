using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Start", "PreFileCopy", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class StartPreFileCopy : ManageSetupBindingTasks
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartPreFileCopyDescription;
			}
		}

		protected override void PopulateComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.Add("setup\\data\\AllRolesPreFileCopyComponent.xml");
		}
	}
}

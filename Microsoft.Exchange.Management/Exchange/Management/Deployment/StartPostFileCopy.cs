using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Start", "PostFileCopy", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class StartPostFileCopy : ManageSetupBindingTasks
	{
		public StartPostFileCopy()
		{
			base.ImplementsResume = false;
			string[] fileSearchPath = new string[]
			{
				ExchangeSetupContext.BinPath,
				Path.Combine(ExchangeSetupContext.BinPath, "FIP-FS\\Bin")
			};
			base.SetFileSearchPath(fileSearchPath);
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartPostFileCopyDescription;
			}
		}

		protected override void PopulateComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.Add("setup\\data\\AllRolesPostFileCopyComponent.xml");
		}
	}
}

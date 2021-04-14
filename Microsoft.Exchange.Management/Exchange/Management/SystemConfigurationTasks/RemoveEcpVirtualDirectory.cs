using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "EcpVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveEcpVirtualDirectory : RemoveExchangeVirtualDirectory<ADEcpVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveEcpVirtualDirectory(this.Identity.ToString());
			}
		}

		protected override ICollection ChildVirtualDirectoryNames
		{
			get
			{
				return RemoveEcpVirtualDirectory.ChildVirtualDirectory;
			}
		}

		private const string ReportingWebService = "ReportingWebService";

		private static readonly string[] ChildVirtualDirectory = new string[]
		{
			"ReportingWebService"
		};
	}
}

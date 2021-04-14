using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("remove", "umlanguagepackregistry")]
	public sealed class RemoveUMLanguagePackRegistry : ManageUMLanaguagePackRegistry
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.languagePack.RemoveProductCodesFromRegistry();
			}
			catch (RegistryInsufficientPermissionException exception)
			{
				base.WriteError(exception, ErrorCategory.PermissionDenied, null);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}

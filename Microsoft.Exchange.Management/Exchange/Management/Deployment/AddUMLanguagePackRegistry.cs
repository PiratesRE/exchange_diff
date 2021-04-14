using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("add", "umlanguagepackregistry")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class AddUMLanguagePackRegistry : ManageUMLanaguagePackRegistry
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.languagePack.AddProductCodesToRegistry();
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

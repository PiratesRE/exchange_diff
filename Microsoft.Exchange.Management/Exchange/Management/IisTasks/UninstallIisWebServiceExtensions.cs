using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.IisTasks
{
	public abstract class UninstallIisWebServiceExtensions : ManageIisWebServiceExtensions
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				using (IsapiExtensionList isapiExtensionList = new IsapiExtensionList(this.HostName))
				{
					for (int i = 0; i < this.ExtensionCount; i++)
					{
						IisWebServiceExtension iisWebServiceExtension = this[i];
						List<int> list = isapiExtensionList.FindMatchingExtensions(this.GroupID, iisWebServiceExtension.ExecutableName);
						for (int j = list.Count - 1; j >= 0; j--)
						{
							isapiExtensionList.RemoveAt(list[j]);
						}
					}
					isapiExtensionList.CommitChanges();
					IisUtility.CommitMetabaseChanges(this.HostName);
				}
				OwaIsapiFilter.RemoveFilters(this.HostName);
			}
			catch (IISNotInstalledException exception)
			{
				base.WriteError(exception, ErrorCategory.NotInstalled, this);
			}
			TaskLogger.LogExit();
		}
	}
}

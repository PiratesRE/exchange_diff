using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.IisTasks
{
	public abstract class InstallIisWebServiceExtensions : ManageIisWebServiceExtensions
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string text = ConfigurationContext.Setup.InstallPath.TrimEnd(new char[]
			{
				'\\'
			});
			try
			{
				using (IsapiExtensionList isapiExtensionList = new IsapiExtensionList(this.HostName))
				{
					bool flag = false;
					for (int i = 0; i < this.ExtensionCount; i++)
					{
						IisWebServiceExtension iisWebServiceExtension = this[i];
						string text2 = string.Concat(new object[]
						{
							text,
							'\\',
							iisWebServiceExtension.RelativePath,
							'\\',
							iisWebServiceExtension.ExecutableName
						});
						if (isapiExtensionList.Exists(this.GroupID, text2))
						{
							base.WriteVerbose(Strings.InstallIisWebServiceExtensionExists(this.GroupID, text2));
						}
						else
						{
							base.WriteVerbose(Strings.InstallIisWebServiceExtensionAdding(this.GroupID, text2));
							isapiExtensionList.Add(new IsapiExtension(text2, this.GroupID, this.GroupDescription, iisWebServiceExtension.Allow, iisWebServiceExtension.UiDeletable));
							flag = true;
						}
					}
					if (flag)
					{
						base.WriteVerbose(Strings.InstallIisWebServiceExtensionCommitting);
						isapiExtensionList.CommitChanges();
						IisUtility.CommitMetabaseChanges(this.HostName);
					}
					else
					{
						base.WriteVerbose(Strings.InstallIisWebServiceExtensionNoChanges);
					}
				}
			}
			catch (IISNotInstalledException exception)
			{
				base.WriteError(exception, ErrorCategory.NotInstalled, this);
			}
			TaskLogger.LogExit();
		}
	}
}

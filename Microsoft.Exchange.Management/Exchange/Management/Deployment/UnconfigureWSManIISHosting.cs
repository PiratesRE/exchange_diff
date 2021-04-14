using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Unconfigure", "WSManIISHosting", SupportsShouldProcess = true)]
	public sealed class UnconfigureWSManIISHosting : ConfigureWSManIISHostingBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUnconfigureWSManIISHosting;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			if (File.Exists(base.IISConfigFilePath))
			{
				using (ServerManager serverManager = new ServerManager())
				{
					bool flag = false;
					SectionGroup sectionGroup = serverManager.GetApplicationHostConfiguration().RootSectionGroup.SectionGroups["system.webServer"];
					SectionDefinition sectionDefinition = sectionGroup.Sections["system.management.wsmanagement.config"];
					if (sectionDefinition != null)
					{
						base.WriteVerbose(Strings.VerboseRemoveWSManConfigSection(base.IISConfigFilePath));
						sectionGroup.Sections.Remove("system.management.wsmanagement.config");
						flag = true;
					}
					ConfigurationElementCollection collection = serverManager.GetApplicationHostConfiguration().GetSection("system.webServer/globalModules").GetCollection();
					for (int i = 0; i < collection.Count; i++)
					{
						ConfigurationElement configurationElement = collection[i];
						object attributeValue = configurationElement.GetAttributeValue("name");
						if (attributeValue != null && string.Equals("WSMan", attributeValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
						{
							base.WriteVerbose(Strings.VerboseRemoveWSManGlobalModule("WSMan", base.IISConfigFilePath));
							collection.Remove(configurationElement);
							flag = true;
							break;
						}
					}
					foreach (Site site in serverManager.Sites)
					{
						if (site.Id == 1L)
						{
							base.DefaultSiteName = site.Name;
						}
					}
					if (flag)
					{
						serverManager.CommitChanges();
						if (base.DefaultSiteName != null)
						{
							base.RestartDefaultWebSite();
						}
					}
				}
			}
			if (File.Exists(base.WSManCfgSchemaFilePath))
			{
				try
				{
					File.Delete(base.WSManCfgSchemaFilePath);
				}
				catch (SystemException ex)
				{
					this.WriteWarning(Strings.ErrorFailedToRemoveWinRMSchemaFile(base.WSManCfgSchemaFilePath, ex.ToString()));
				}
			}
			TaskLogger.LogEnter();
		}
	}
}

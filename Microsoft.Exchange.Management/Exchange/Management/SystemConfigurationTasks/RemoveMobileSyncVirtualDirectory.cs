using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "ActiveSyncVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMobileSyncVirtualDirectory : RemoveExchangeVirtualDirectory<ADMobileVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMobileSyncVirtualDirectory(this.Identity.ToString());
			}
		}

		protected override void DeleteFromMetabase()
		{
			if (string.IsNullOrEmpty(base.DataObject.MetabasePath))
			{
				string webSiteRoot = IisUtility.CreateAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot, this.Identity.Server, IisUtility.FindWebSiteRootPath(this.Identity.Name, this.Identity.Server), null);
				string virtualDirectoryName = "Microsoft-Server-ActiveSync";
				DeleteVirtualDirectory.DeleteFromMetabase(webSiteRoot, virtualDirectoryName, this.ChildVirtualDirectoryNames);
				return;
			}
			base.DeleteFromMetabase();
		}

		protected override void PreDeleteFromMetabase()
		{
			if (!DirectoryEntry.Exists(base.DataObject.MetabasePath))
			{
				return;
			}
			try
			{
				MobileSyncVirtualDirectoryHelper.UninstallIsapiFilter(base.DataObject);
			}
			catch (Exception)
			{
				this.WriteWarning(Strings.ActiveSyncMetabaseIsapiUninstallFailure);
				throw;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.DataObject.ExchangeVersion.IsOlderThan(ADMobileVirtualDirectory.MinimumSupportedExchangeObjectVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorRemoveOlderVirtualDirectory(base.DataObject.Identity.ToString(), ADMobileVirtualDirectory.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}

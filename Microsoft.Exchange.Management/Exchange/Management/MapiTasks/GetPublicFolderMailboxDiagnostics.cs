using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "PublicFolderMailboxDiagnostics", SupportsShouldProcess = true)]
	public sealed class GetPublicFolderMailboxDiagnostics : RecipientObjectActionTask<MailboxIdParameter, ADRecipient>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeDumpsterInfo { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeHierarchyInfo { get; set; }

		protected override void InternalProcessRecord()
		{
			ADUser aduser = this.DataObject as ADUser;
			if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
			{
				base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, null);
			}
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(base.CurrentOrganizationId);
			if (value.GetLocalMailboxRecipient(aduser.ExchangeGuid) == null)
			{
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.CurrentOrganizationId);
			}
			DiagnosticsLoadFlags diagnosticsLoadFlags = DiagnosticsLoadFlags.Default;
			if (this.IncludeDumpsterInfo)
			{
				diagnosticsLoadFlags |= DiagnosticsLoadFlags.DumpsterInfo;
			}
			if (this.IncludeHierarchyInfo)
			{
				diagnosticsLoadFlags |= DiagnosticsLoadFlags.HierarchyInfo;
			}
			PublicFolderMailboxDiagnosticsInfo sendToPipeline = PublicFolderMailboxDiagnosticsInfo.Load(base.CurrentOrganizationId, aduser.ExchangeGuid, diagnosticsLoadFlags, new Action<LocalizedString, LocalizedString, int>(base.WriteProgress));
			base.WriteObject(sendToPipeline);
		}
	}
}

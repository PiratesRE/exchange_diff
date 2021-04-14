using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ExportMailboxChanges : DataSourceService, IExportMailboxChanges, INewObjectService<ExportMailboxChangesRow, ExportMailboxChangesParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "New-MailboxAuditLogSearch?StartDate&EndDate&Mailboxes&StatusMailRecipients&LogonTypes&ExternalAccess&ShowDetails@R:Organization")]
		public PowerShellResults<ExportMailboxChangesRow> NewObject(ExportMailboxChangesParameters parameters)
		{
			string logonTypes;
			if ((logonTypes = parameters.LogonTypes) != null)
			{
				if (!(logonTypes == "AllNonOwners"))
				{
					if (!(logonTypes == "OutsideUsers"))
					{
						if (!(logonTypes == "InternalUsers"))
						{
							if (logonTypes == "NonDelegateUsers")
							{
								parameters.LogonTypes = "Admin";
								parameters.ExternalAccess = new bool?(false);
							}
						}
						else
						{
							parameters.LogonTypes = "Admin,Delegate";
							parameters.ExternalAccess = new bool?(false);
						}
					}
					else
					{
						parameters.LogonTypes = null;
						parameters.ExternalAccess = new bool?(true);
					}
				}
				else
				{
					parameters.LogonTypes = "Admin,Delegate";
					parameters.ExternalAccess = null;
				}
			}
			if (parameters.EndDate.IsNullOrBlank() || parameters.StartDate.IsNullOrBlank())
			{
				throw new FaultException(ClientStrings.DatesNotDefined);
			}
			if (parameters.StatusMailRecipients.IsNullOrBlank())
			{
				throw new FaultException(Strings.MailRecipientNotDefined);
			}
			parameters.ShowDetails = true;
			return base.NewObject<ExportMailboxChangesRow, ExportMailboxChangesParameters>("New-MailboxAuditLogSearch", parameters);
		}

		internal const string WriteScope = "@R:Organization";

		internal const string NewCmdlet = "New-MailboxAuditLogSearch";

		private const string NewObjectRole = "New-MailboxAuditLogSearch?StartDate&EndDate&Mailboxes&StatusMailRecipients&LogonTypes&ExternalAccess&ShowDetails@R:Organization";
	}
}

using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Export", "MailboxDiagnosticLogs", DefaultParameterSetName = "MailboxLogParameterSet", SupportsShouldProcess = true)]
	public sealed class ExportMailboxDiagnosticLogs : GetXsoObjectWithIdentityTaskBase<MailboxDiagnosticLogs, ADRecipient>
	{
		[Parameter(Mandatory = true, ParameterSetName = "ExtendedPropertiesParameterSet", ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "MailboxLogParameterSet", ValueFromPipelineByPropertyName = true, Position = 0)]
		public new MailUserOrGeneralMailboxIdParameter Identity
		{
			get
			{
				return (MailUserOrGeneralMailboxIdParameter)base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MailboxLogParameterSet")]
		public string ComponentName
		{
			get
			{
				return (string)(base.Fields["ComponentName"] ?? string.Empty);
			}
			set
			{
				base.Fields["ComponentName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExtendedPropertiesParameterSet")]
		public SwitchParameter ExtendedProperties
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExtendedProperties"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExtendedProperties"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxLogParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ExtendedPropertiesParameterSet")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationExportMailboxDiagnosticLogs;
			}
		}

		protected override bool ShouldProcessArchive
		{
			get
			{
				return this.Archive.IsPresent;
			}
		}

		internal override IConfigDataProvider CreateXsoMailboxDataProvider(ExchangePrincipal principal, ISecurityAccessToken userToken)
		{
			if (this.ExtendedProperties.IsPresent)
			{
				return new MailboxDiagnosticLogsDataProvider(principal, "ExportMailboxDiagnosticLogs");
			}
			return new MailboxDiagnosticLogsDataProvider(this.ComponentName, principal, "ExportMailboxDiagnosticLogs");
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is ObjectNotFoundException || base.IsKnownException(exception);
		}

		public const string Name = "MailboxDiagnosticLogs";

		public const string MailboxLogParameterSet = "MailboxLogParameterSet";

		public const string ExtendedPropertiesParameterSet = "ExtendedPropertiesParameterSet";

		public const string Export = "Export";
	}
}

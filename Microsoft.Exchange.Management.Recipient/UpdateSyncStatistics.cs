using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "SyncStatistics", SupportsShouldProcess = true)]
	public sealed class UpdateSyncStatistics : SetMultitenancySingletonSystemConfigurationObjectTask<GALSyncOrganization>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateSyncStatistics;
			}
		}

		internal new OrganizationIdParameter Identity
		{
			get
			{
				return null;
			}
		}

		[Parameter]
		[ValidateRange(0, 2147483647)]
		public int NumberOfMailboxesCreated
		{
			get
			{
				return (int)(base.Fields["NumberOfMailboxesCreated"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfMailboxesCreated"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfMailboxesToCreate
		{
			get
			{
				return (int)(base.Fields["NumberOfMailboxesToCreate"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfMailboxesToCreate"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int MailboxCreationElapsedMilliseconds
		{
			get
			{
				return (int)(base.Fields["MailboxCreationElapsedMilliseconds"] ?? 0);
			}
			set
			{
				base.Fields["MailboxCreationElapsedMilliseconds"] = value;
			}
		}

		[Parameter]
		[ValidateRange(0, 2147483647)]
		public int NumberOfExportSyncRuns
		{
			get
			{
				return (int)(base.Fields["NumberOfExportSyncRuns"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfExportSyncRuns"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfImportSyncRuns
		{
			get
			{
				return (int)(base.Fields["NumberOfImportSyncRuns"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfImportSyncRuns"] = value;
			}
		}

		[Parameter]
		[ValidateRange(0, 2147483647)]
		public int NumberOfSucessfulExportSyncRuns
		{
			get
			{
				return (int)(base.Fields["NumberOfSucessfulExportSyncRuns"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfSucessfulExportSyncRuns"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfSucessfulImportSyncRuns
		{
			get
			{
				return (int)(base.Fields["NumberOfSucessfulImportSyncRuns"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfSucessfulImportSyncRuns"] = value;
			}
		}

		[Parameter]
		[ValidateRange(0, 2147483647)]
		public int NumberOfConnectionErrors
		{
			get
			{
				return (int)(base.Fields["NumberOfConnectionErrors"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfConnectionErrors"] = value;
			}
		}

		[Parameter]
		[ValidateRange(0, 2147483647)]
		public int NumberOfPermissionErrors
		{
			get
			{
				return (int)(base.Fields["NumberOfPermissionErrors"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfPermissionErrors"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfIlmLogicErrors
		{
			get
			{
				return (int)(base.Fields["NumberOfIlmLogicErrors"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfIlmLogicErrors"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfIlmOtherErrors
		{
			get
			{
				return (int)(base.Fields["NumberOfIlmOtherErrors"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfIlmOtherErrors"] = value;
			}
		}

		[ValidateRange(0, 2147483647)]
		[Parameter]
		public int NumberOfLiveIdErrors
		{
			get
			{
				return (int)(base.Fields["NumberOfLiveIdErrors"] ?? 0);
			}
			set
			{
				base.Fields["NumberOfLiveIdErrors"] = value;
			}
		}

		[Parameter]
		public string ClientData
		{
			get
			{
				return (string)base.Fields["CustomerData"];
			}
			set
			{
				base.Fields["CustomerData"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			GALSyncOrganization galsyncOrganization = (GALSyncOrganization)base.PrepareDataObject();
			if (string.IsNullOrEmpty(this.ClientData))
			{
				galsyncOrganization.GALSyncClientData = string.Format("<xml><Version>14.01.0098.00</Version><LastRun>{0}</LastRun></xml>", ExDateTime.Now.ToUtc());
			}
			else
			{
				galsyncOrganization.GALSyncClientData = this.ClientData;
			}
			TaskLogger.LogExit();
			return galsyncOrganization;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.Fields.IsChanged("NumberOfMailboxesCreated"))
			{
				GalsyncCounters.ClientReportedNumberOfMailboxesCreated.IncrementBy((long)this.NumberOfMailboxesCreated);
			}
			if (base.Fields.IsChanged("NumberOfMailboxesToCreate"))
			{
				GalsyncCounters.ClientReportedNumberOfMailboxesToCreate.IncrementBy((long)this.NumberOfMailboxesToCreate);
			}
			if (base.Fields.IsChanged("MailboxCreationElapsedMilliseconds"))
			{
				GalsyncCounters.ClientReportedMailboxCreationElapsedMilliseconds.IncrementBy((long)this.MailboxCreationElapsedMilliseconds);
			}
			if (base.Fields.IsChanged("NumberOfExportSyncRuns"))
			{
				GalsyncCounters.NumberOfExportSyncRuns.IncrementBy((long)this.NumberOfExportSyncRuns);
			}
			if (base.Fields.IsChanged("NumberOfImportSyncRuns"))
			{
				GalsyncCounters.NumberOfImportSyncRuns.IncrementBy((long)this.NumberOfImportSyncRuns);
			}
			if (base.Fields.IsChanged("NumberOfSucessfulExportSyncRuns"))
			{
				GalsyncCounters.NumberOfSucessfulExportSyncRuns.IncrementBy((long)this.NumberOfSucessfulExportSyncRuns);
			}
			if (base.Fields.IsChanged("NumberOfSucessfulImportSyncRuns"))
			{
				GalsyncCounters.NumberOfSucessfulImportSyncRuns.IncrementBy((long)this.NumberOfSucessfulImportSyncRuns);
			}
			if (base.Fields.IsChanged("NumberOfConnectionErrors"))
			{
				GalsyncCounters.NumberOfConnectionErrors.IncrementBy((long)this.NumberOfConnectionErrors);
			}
			if (base.Fields.IsChanged("NumberOfPermissionErrors"))
			{
				GalsyncCounters.NumberOfPermissionErrors.IncrementBy((long)this.NumberOfPermissionErrors);
			}
			if (base.Fields.IsChanged("NumberOfIlmLogicErrors"))
			{
				GalsyncCounters.NumberOfILMLogicErrors.IncrementBy((long)this.NumberOfIlmLogicErrors);
			}
			if (base.Fields.IsChanged("NumberOfIlmOtherErrors"))
			{
				GalsyncCounters.NumberOfILMOtherErrors.IncrementBy((long)this.NumberOfIlmOtherErrors);
			}
			if (base.Fields.IsChanged("NumberOfLiveIdErrors"))
			{
				GalsyncCounters.NumberOfLiveIdErrors.IncrementBy((long)this.NumberOfLiveIdErrors);
			}
		}

		private const string MailboxesCreatedPara = "NumberOfMailboxesCreated";

		private const string MailboxesToCreatePara = "NumberOfMailboxesToCreate";

		private const string MailboxCreationElapsedMillisecondsPara = "MailboxCreationElapsedMilliseconds";

		private const string ExportSyncRunsPara = "NumberOfExportSyncRuns";

		private const string ImportSyncRunsPara = "NumberOfImportSyncRuns";

		private const string SucessfulExportSyncRunsPara = "NumberOfSucessfulExportSyncRuns";

		private const string SucessfulImportSyncRunsPara = "NumberOfSucessfulImportSyncRuns";

		private const string ConnectionErrorsPara = "NumberOfConnectionErrors";

		private const string PermissionErrorsPara = "NumberOfPermissionErrors";

		private const string IlmLogicErrorsPara = "NumberOfIlmLogicErrors";

		private const string IlmOtherErrorsPara = "NumberOfIlmOtherErrors";

		private const string LiveIdErrorsPara = "NumberOfLiveIdErrors";

		private const string ClientDataPara = "CustomerData";

		private const string ClientDataStringFmt = "<xml><Version>14.01.0098.00</Version><LastRun>{0}</LastRun></xml>";
	}
}

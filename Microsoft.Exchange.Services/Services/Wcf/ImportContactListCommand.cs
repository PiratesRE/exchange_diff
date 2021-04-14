using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class ImportContactListCommand : SingleCmdletCommandBase<ImportContactListRequest, ImportContactListResponse, Microsoft.Exchange.Management.Aggregation.ImportContactList, ImportContactListResult>
	{
		public ImportContactListCommand(CallContext callContext, ImportContactListRequest request) : base(callContext, request, "Import-ContactList", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			Microsoft.Exchange.Management.Aggregation.ImportContactList task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("CSV", task, new SwitchParameter(true));
			if (this.request != null && this.request.ImportedContactList.CSVData != null)
			{
				this.cmdletRunner.SetTaskParameter("CSVData", task, this.request.ImportedContactList.CSVData);
			}
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(ImportContactListResponse response)
		{
			ImportContactListResult result = this.cmdletRunner.TaskWrapper.Result;
			response.NumberOfContactsImported = result.ContactsImported;
		}

		protected override PSLocalTask<Microsoft.Exchange.Management.Aggregation.ImportContactList, ImportContactListResult> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateImportContactListTask(base.CallContext.AccessingPrincipal);
		}
	}
}

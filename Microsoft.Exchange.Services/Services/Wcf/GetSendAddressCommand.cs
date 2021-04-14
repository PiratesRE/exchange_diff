using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetSendAddressCommand : SingleCmdletCommandBase<object, GetSendAddressResponse, GetSendAddress, SendAddress>
	{
		public GetSendAddressCommand(CallContext callContext) : base(callContext, null, "Get-SendAddress", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetSendAddress task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Mailbox", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetSendAddressResponse response)
		{
			IEnumerable<SendAddress> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			response.SendAddressCollection.SendAddresses = (from r in allResults
			select new SendAddressData
			{
				AddressId = r.AddressId,
				DisplayName = r.DisplayName
			}).ToArray<SendAddressData>();
		}

		protected override PSLocalTask<GetSendAddress, SendAddress> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetSendAddressTask(base.CallContext.AccessingPrincipal);
		}
	}
}

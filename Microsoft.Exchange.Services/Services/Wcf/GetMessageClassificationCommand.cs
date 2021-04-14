using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.SecureMail;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMessageClassificationCommand : SingleCmdletCommandBase<object, GetMessageClassificationResponse, GetMessageClassification, Microsoft.Exchange.Data.Directory.SystemConfiguration.MessageClassification>
	{
		public GetMessageClassificationCommand(CallContext callContext) : base(callContext, null, "Get-MessageClassification", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateResponseData(GetMessageClassificationResponse response)
		{
			IEnumerable<Microsoft.Exchange.Data.Directory.SystemConfiguration.MessageClassification> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			IEnumerable<Microsoft.Exchange.Services.Wcf.Types.MessageClassification> source = from e in allResults
			where e.PermissionMenuVisible
			select new Microsoft.Exchange.Services.Wcf.Types.MessageClassification
			{
				DisplayName = e.DisplayName,
				Guid = e.Guid
			};
			response.MessageClassificationCollection.MessageClassifications = source.ToArray<Microsoft.Exchange.Services.Wcf.Types.MessageClassification>();
		}

		protected override PSLocalTask<GetMessageClassification, Microsoft.Exchange.Data.Directory.SystemConfiguration.MessageClassification> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMessageClassificationTask(base.CallContext.AccessingPrincipal);
		}
	}
}

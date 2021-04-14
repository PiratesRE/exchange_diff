using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMessageCategoryCommand : SingleCmdletCommandBase<object, GetMessageCategoryResponse, GetMessageCategory, Microsoft.Exchange.Data.Storage.Management.MessageCategory>
	{
		public GetMessageCategoryCommand(CallContext callContext) : base(callContext, null, "Get-MessageCategory", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateResponseData(GetMessageCategoryResponse response)
		{
			PSLocalTask<GetMessageCategory, Microsoft.Exchange.Data.Storage.Management.MessageCategory> taskWrapper = this.cmdletRunner.TaskWrapper;
			IEnumerable<Microsoft.Exchange.Services.Wcf.Types.MessageCategory> source = from t in taskWrapper.AllResults
			select new Microsoft.Exchange.Services.Wcf.Types.MessageCategory
			{
				Color = t.Color,
				Name = t.Name
			};
			response.MessageCategoryCollection.MessageCategories = source.ToArray<Microsoft.Exchange.Services.Wcf.Types.MessageCategory>();
		}

		protected override PSLocalTask<GetMessageCategory, Microsoft.Exchange.Data.Storage.Management.MessageCategory> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMessageCategoryTask(base.CallContext.AccessingPrincipal);
		}
	}
}

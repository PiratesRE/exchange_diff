using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetTaskFoldersCommand : ServiceCommand<GetTaskFoldersResponse>
	{
		public GetTaskFoldersCommand(CallContext context) : base(context)
		{
		}

		protected override GetTaskFoldersResponse InternalExecute()
		{
			return new GetTaskFolders(base.MailboxIdentityMailboxSession).Execute();
		}
	}
}

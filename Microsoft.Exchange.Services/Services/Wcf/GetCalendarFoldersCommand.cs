using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarFoldersCommand : ServiceCommand<GetCalendarFoldersResponse>
	{
		public GetCalendarFoldersCommand(CallContext callContext) : base(callContext)
		{
		}

		protected override GetCalendarFoldersResponse InternalExecute()
		{
			IConstraintProvider context = base.MailboxIdentityMailboxSession.MailboxOwner.GetContext(null);
			bool enabled = VariantConfiguration.GetSnapshot(context, null, null).OwaClientServer.OwaPublicFolders.Enabled;
			return new GetCalendarFolders(base.MailboxIdentityMailboxSession, base.CallContext.ADRecipientSessionContext.GetADRecipientSession(), enabled).Execute();
		}
	}
}

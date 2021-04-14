using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class GetMasterCategoryListCommand : ServiceCommand<MasterCategoryListActionResponse>
	{
		public GetMasterCategoryListCommand(CallContext context, GetMasterCategoryListRequest request) : base(context)
		{
			this.request = request;
			this.request.ValidateRequest();
		}

		protected override MasterCategoryListActionResponse InternalExecute()
		{
			MailboxSession mailboxSession;
			try
			{
				mailboxSession = this.GetMailboxSession(this.request.SmtpAddress.ToString());
			}
			catch (NonExistentMailboxException)
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug<SmtpAddress>(0L, "Not able to access mailbox to retrieve the MasterCategoryList for {0}", this.request.SmtpAddress);
				return new MasterCategoryListActionResponse(MasterCategoryListActionError.MasterCategoryListErrorUnableToAccessMclOwnerMailbox);
			}
			if (mailboxSession == null)
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug<SmtpAddress>(0L, "Not able to access mailbox to retrieve the MasterCategoryList for {0}", this.request.SmtpAddress);
				return new MasterCategoryListActionResponse(MasterCategoryListActionError.MasterCategoryListErrorUnableToAccessMclOwnerMailbox);
			}
			MasterCategoryListType masterCategoryListType = new GetMasterCategoryList(mailboxSession).Execute();
			if (masterCategoryListType == null)
			{
				return new MasterCategoryListActionResponse(MasterCategoryListActionError.MasterCategoryListErrorUnableToLoad);
			}
			return new MasterCategoryListActionResponse(MasterCategoryListHelper.GetMasterList(masterCategoryListType));
		}

		protected MailboxSession GetMailboxSession(string smtpAddress)
		{
			return base.CallContext.SessionCache.GetMailboxSessionBySmtpAddress(smtpAddress);
		}

		private GetMasterCategoryListRequest request;
	}
}

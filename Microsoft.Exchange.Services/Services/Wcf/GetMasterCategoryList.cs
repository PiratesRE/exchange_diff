using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMasterCategoryList
	{
		public GetMasterCategoryList(MailboxSession mclOwnerMailboxSession)
		{
			this.mclOwnerMailboxSession = mclOwnerMailboxSession;
		}

		public MasterCategoryListType Execute()
		{
			MasterCategoryListType result = null;
			if (!this.CanAccessCalendarFolder(this.mclOwnerMailboxSession))
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug<string>(0L, "Not able to access calendar folder to retrieve the MasterCategoryListType for {0}", this.mclOwnerMailboxSession.DisplayAddress);
			}
			else
			{
				try
				{
					result = MasterCategoryListHelper.GetMasterCategoryListType(this.mclOwnerMailboxSession, this.mclOwnerMailboxSession.Culture);
				}
				catch (AccessDeniedException ex)
				{
					ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug<string, string>(0L, "Not able to access calendar folder to retrieve the MasterCategoryListType for {0}. Exception:{1}", this.mclOwnerMailboxSession.DisplayAddress, ex.ToString());
				}
			}
			return result;
		}

		internal bool CanAccessCalendarFolder(MailboxSession mailboxSession)
		{
			return mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar) != null;
		}

		private MailboxSession mclOwnerMailboxSession;
	}
}

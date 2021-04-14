using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTagMailboxProvider : IPolicyTagProvider
	{
		public PolicyTag[] PolicyTags
		{
			get
			{
				PolicyTagList policyTagList = this.mailboxSession.GetPolicyTagList((RetentionActionType)0);
				PolicyTagMailboxProvider.Tracer.TraceDebug<int>((long)this.GetHashCode(), "PolicyTagStoreResolver resolving {0} RetentionPolicyTags", (policyTagList == null) ? 0 : policyTagList.Count);
				if (policyTagList != null)
				{
					return policyTagList.Values.ToArray<PolicyTag>();
				}
				return null;
			}
		}

		public PolicyTagMailboxProvider(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			this.mailboxSession = mailboxSession;
		}

		private MailboxSession mailboxSession;

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;
	}
}

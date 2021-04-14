using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActivityLogFactory
	{
		internal static Hookable<ActivityLogFactory> Instance
		{
			get
			{
				return ActivityLogFactory.HookableInstance;
			}
		}

		public static ActivityLogFactory Current
		{
			get
			{
				return ActivityLogFactory.HookableInstance.Value;
			}
		}

		public virtual IActivityLog Bind(MailboxSession mailboxSession)
		{
			return new AppendOnlyActivityLog(mailboxSession);
		}

		private static readonly Hookable<ActivityLogFactory> HookableInstance = Hookable<ActivityLogFactory>.Create(true, new ActivityLogFactory());
	}
}

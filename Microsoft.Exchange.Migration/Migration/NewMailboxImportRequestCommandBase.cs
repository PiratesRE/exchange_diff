using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NewMailboxImportRequestCommandBase : NewMrsRequestCommandBase
	{
		protected NewMailboxImportRequestCommandBase(string cmdletName, ICollection<Type> ignoredExceptions) : base(cmdletName, ignoredExceptions)
		{
		}

		public Fqdn RemoteHostName
		{
			set
			{
				base.AddParameter("RemoteHostName", value);
			}
		}

		public PSCredential RemoteCredential
		{
			set
			{
				if (value != null)
				{
					base.AddParameter("RemoteCredential", value);
				}
			}
		}

		public bool AutoCleanup
		{
			set
			{
				base.AddParameter("CompletedRequestAgeLimit", value ? EnhancedTimeSpan.FromDays(1.0) : EnhancedTimeSpan.FromDays(7.0));
			}
		}
	}
}

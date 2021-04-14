using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NewSyncRequestCommandBase : NewMrsRequestCommandBase
	{
		protected NewSyncRequestCommandBase(string cmdletName, ICollection<Type> ignoredExceptions) : base(cmdletName, ignoredExceptions)
		{
		}

		public string RemoteServerName
		{
			set
			{
				base.AddParameter("RemoteServerName", value);
			}
		}

		public int RemoteServerPort
		{
			set
			{
				base.AddParameter("RemoteServerPort", value);
			}
		}

		public SecureString Password
		{
			set
			{
				base.AddParameter("Password", value);
			}
		}

		public IMAPSecurityMechanism Security
		{
			set
			{
				base.AddParameter("Security", value);
			}
		}

		public AuthenticationMethod Authentication
		{
			set
			{
				base.AddParameter("Authentication", value);
			}
		}

		public DateTime? StartAfter
		{
			set
			{
				base.AddParameter("StartAfter", value);
			}
		}

		public DateTime? CompleteAfter
		{
			set
			{
				base.AddParameter("CompleteAfter", value);
			}
		}

		internal const string StartAfterParameter = "StartAfter";

		internal const string CompleteAfterParameter = "CompleteAfter";
	}
}

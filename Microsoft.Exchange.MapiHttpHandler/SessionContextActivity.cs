using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionContextActivity : BaseObject
	{
		private SessionContextActivity(SessionContext sessionContext)
		{
			this.sessionContext = sessionContext;
		}

		public SessionContext SessionContext
		{
			get
			{
				base.CheckDisposed();
				return this.sessionContext;
			}
		}

		public static bool TryCreate(SessionContext sessionContext, out SessionContextActivity sessionContextActivity)
		{
			sessionContextActivity = null;
			bool flag = false;
			if (!sessionContext.TryAddReference())
			{
				return false;
			}
			try
			{
				sessionContextActivity = new SessionContextActivity(sessionContext);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					sessionContext.ReleaseReference();
				}
			}
			return true;
		}

		protected override void InternalDispose()
		{
			this.sessionContext.ReleaseReference();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SessionContextActivity>(this);
		}

		private readonly SessionContext sessionContext;
	}
}

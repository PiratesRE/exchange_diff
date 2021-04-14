using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserContextActivity : BaseObject
	{
		public UserContextActivity(UserContext userContext)
		{
			this.userContext = userContext;
			this.userContext.AddReference();
		}

		public UserContext UserContext
		{
			get
			{
				base.CheckDisposed();
				return this.userContext;
			}
		}

		protected override void InternalDispose()
		{
			this.userContext.ReleaseReference();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UserContextActivity>(this);
		}

		private readonly UserContext userContext;
	}
}

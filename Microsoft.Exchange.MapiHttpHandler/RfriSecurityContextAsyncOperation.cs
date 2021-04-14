using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MapiHttp
{
	internal abstract class RfriSecurityContextAsyncOperation : RfriAsyncOperation
	{
		protected RfriSecurityContextAsyncOperation(HttpContextBase context, AsyncOperationCookieFlags cookieFlags) : base(context, cookieFlags)
		{
		}

		protected MapiHttpClientBinding ClientBinding
		{
			get
			{
				return this.clientBinding;
			}
		}

		public override void Prepare()
		{
			base.Prepare();
			this.initialCachedClientSecurityContext = base.GetInitialCachedClientSecurityContext();
			this.clientBinding = base.GetMapiHttpClientBinding(new Func<ClientSecurityContext>(this.GetClientSecurityContext));
		}

		protected ClientSecurityContext GetClientSecurityContext()
		{
			ClientSecurityContext result = null;
			if (this.initialCachedClientSecurityContext != null)
			{
				lock (this.securityContextLock)
				{
					if (this.initialCachedClientSecurityContext != null)
					{
						result = this.initialCachedClientSecurityContext;
						this.initialCachedClientSecurityContext = null;
					}
				}
			}
			return result;
		}

		protected override void InternalDispose()
		{
			if (this.clientBinding != null)
			{
				this.clientBinding.ClearClientSecurityContextGetter();
			}
			if (this.initialCachedClientSecurityContext != null)
			{
				lock (this.securityContextLock)
				{
					if (this.initialCachedClientSecurityContext != null)
					{
						this.initialCachedClientSecurityContext.Dispose();
						this.initialCachedClientSecurityContext = null;
					}
				}
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriSecurityContextAsyncOperation>(this);
		}

		private ClientSecurityContext initialCachedClientSecurityContext;

		private object securityContextLock = new object();

		private MapiHttpClientBinding clientBinding;
	}
}

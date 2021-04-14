using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class InstantMessageCommandBase<T> : ServiceCommand<T>
	{
		public InstantMessageCommandBase(CallContext callContext) : base(callContext)
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingLogMetadata), new Type[0]);
		}

		protected InstantMessageProvider Provider
		{
			get
			{
				InstantMessageManager instantMessageManager = this.InstantMessageManager;
				if (instantMessageManager != null)
				{
					return instantMessageManager.Provider;
				}
				return null;
			}
		}

		protected InstantMessageManager InstantMessageManager
		{
			get
			{
				UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
				if (userContext.IsInstantMessageEnabled)
				{
					return userContext.InstantMessageManager;
				}
				return null;
			}
		}
	}
}

using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CancelAttachment : ServiceCommand<bool>
	{
		public CancelAttachment(CallContext callContext, string cancellationId) : base(callContext)
		{
			if (string.IsNullOrEmpty(cancellationId))
			{
				throw new ArgumentException("The parameter cannot be null or empty.", "cancellationId");
			}
			this.cancellationId = cancellationId;
		}

		protected override bool InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachment.InternalExecute", userContext, "InternalExecute", string.Format("Attempting to cancel. CancellationId: {0}", this.cancellationId)));
			return userContext.CancelAttachmentManager.CancelAttachment(this.cancellationId, 30000);
		}

		private readonly string cancellationId;
	}
}

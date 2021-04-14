using System;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetDlpPolicyTipsAsyncResult : AsyncResult
	{
		public GetDlpPolicyTipsAsyncResult(AsyncCallback callback, object context) : base(callback, context)
		{
		}

		public GetDlpPolicyTipsResponse Response { get; set; }

		public void GetDlpPolicyTipsCommand(GetDlpPolicyTipsRequest request)
		{
			if (Interlocked.Increment(ref GetDlpPolicyTipsAsyncResult.PendingRequestCount) > 50)
			{
				Interlocked.Decrement(ref GetDlpPolicyTipsAsyncResult.PendingRequestCount);
				this.Response = GetDlpPolicyTipsResponse.TooManyPendingRequestResponse;
				base.Callback(this);
				return;
			}
			CallContext callContext = CallContext.Current;
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						CallContext.SetCurrent(callContext);
						this.Response = new GetDlpPolicyTipsCommand(callContext, request).Execute();
						this.Callback(this);
					});
				}
				catch (GrayException arg)
				{
					ExTraceGlobals.OwaRulesEngineTracer.TraceError<GrayException>(0L, "GetDlpPolicyTipsAsyncResult GrayException occured while evaluating for Dlp.  exception {0}", arg);
				}
				finally
				{
					CallContext.SetCurrent(null);
					Interlocked.Decrement(ref GetDlpPolicyTipsAsyncResult.PendingRequestCount);
				}
			}, base.AsyncState);
		}

		public static int PendingRequestCount;
	}
}

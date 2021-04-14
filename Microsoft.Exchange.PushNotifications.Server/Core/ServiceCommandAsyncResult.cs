using System;
using System.Net;
using System.ServiceModel.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal class ServiceCommandAsyncResult<TResult> : BasicAsyncResult<TResult>
	{
		public ServiceCommandAsyncResult(AsyncCallback asyncCallback, object state) : base(asyncCallback, state)
		{
		}

		protected override bool ShouldThrowCallbackException(Exception ex)
		{
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "ServiceCommandAsyncResult.ShouldThrowCallbackException Exception processed.\n{0}.", ex.ToTraceString());
			}
			return base.ShouldThrowCallbackException(ex);
		}

		protected override Exception CreateEndException(Exception currentException)
		{
			this.LogException(currentException);
			return new WebFaultException<PushNotificationFault>(new PushNotificationFault(currentException, ServiceConfig.IsWriteStackTraceOnResponseEnabled, 0, true), HttpStatusCode.InternalServerError);
		}

		private void LogException(Exception ex)
		{
			PushNotificationsCrimsonEvents.ServiceCommandException.LogPeriodic<string>(ex.ToString(), CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, ex.ToTraceString());
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "A service command execution reported an error.\n{0}.", ex.ToTraceString());
			}
		}
	}
}

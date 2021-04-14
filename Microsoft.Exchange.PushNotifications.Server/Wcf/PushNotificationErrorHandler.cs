using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	public class PushNotificationErrorHandler : IErrorHandler
	{
		public bool HandleError(Exception error)
		{
			if (error is PushNotificationPermanentException || error is PushNotificationTransientException)
			{
				if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "PushNotificationErrorHandler.HandleError: Error reported is known Exception {0}.", error.ToTraceString());
				}
				return false;
			}
			PushNotificationsCrimsonEvents.ErrorHandlerException.LogPeriodic<string>(error.ToString(), CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, error.ToTraceString());
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "PushNotificationErrorHandler.HandleError: Unknown error reported '{0}'.", error.ToTraceString());
			}
			if (this.IsGrayException(error))
			{
				ExWatson.SendReport(error, ReportOptions.DoNotFreezeThreads, null);
			}
			return false;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
		}

		private bool IsGrayException(Exception ex)
		{
			return GrayException.IsGrayException(ex);
		}
	}
}

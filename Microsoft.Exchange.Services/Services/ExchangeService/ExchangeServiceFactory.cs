using System;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class ExchangeServiceFactory
	{
		public static ExchangeServiceFactory Default
		{
			get
			{
				return ExchangeServiceFactory.defaultInstance;
			}
		}

		public virtual IExchangeService CreateForStandaloneHttpService(HttpContext httpContext, IActivityScope activityScope, IStandardBudget budget)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			ArgumentValidator.ThrowIfNull("activityScope", activityScope);
			ArgumentValidator.ThrowIfNull("budget", budget);
			return new HttpExchangeService(httpContext, activityScope, budget);
		}

		public virtual IExchangeService CreateForEws(CallContext callContext)
		{
			ArgumentValidator.ThrowIfNull("callContext", callContext);
			return new EwsExchangeService(callContext);
		}

		public virtual IPushNotificationService CreateOutlookPushNotificationService()
		{
			return new OutlookPushNotificationService();
		}

		private static readonly ExchangeServiceFactory defaultInstance = new ExchangeServiceFactory();
	}
}

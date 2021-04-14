using System;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class HttpExchangeService : ExchangeServiceBase
	{
		public HttpExchangeService(HttpContext httpContext, IActivityScope activityScope, IStandardBudget budget)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			ArgumentValidator.ThrowIfNull("activityScope", activityScope);
			ArgumentValidator.ThrowIfNull("budget", budget);
			this.HttpContext = httpContext;
			base.ActivityScope = activityScope;
			this.Budget = budget;
			base.CallWithExceptionHandling(ExecutionOption.Default, new Action(this.InitializeCallContext));
		}

		private protected HttpContext HttpContext { protected get; private set; }

		private protected IStandardBudget Budget { protected get; private set; }

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(base.CallContext.HttpContext);
				if (current != null)
				{
					current.Dispose();
				}
				if (base.CallContext != null)
				{
					base.CallContext.DisposeForExchangeService();
					base.CallContext = null;
				}
			}
		}

		private void InitializeCallContext()
		{
			HttpContext.Current = this.HttpContext;
			HttpExchangeService.EwsGlobals.InitIfNeeded(this.HttpContext);
			if (RequestDetailsLogger.Current != null && RequestDetailsLogger.Current.IsDisposed)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(HttpContext.Current, null);
				CallContext.SetCurrent(null);
				HttpContext.Current.Items["CallContext"] = null;
			}
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger(base.ActivityScope);
			requestDetailsLogger.EndActivityContext = false;
			requestDetailsLogger.SkipLogging = true;
			BudgetAdapter budget = new BudgetAdapter(this.Budget);
			base.CallContext = CallContext.CreateForExchangeService(this.HttpContext, HttpExchangeService.EwsGlobals.AppWideStoreSessionCache, HttpExchangeService.EwsGlobals.AcceptedDomainCache, HttpExchangeService.EwsGlobals.UserWorkloadManager, budget, Thread.CurrentThread.CurrentCulture);
		}

		private static class EwsGlobals
		{
			public static void InitIfNeeded(HttpContext httpContext)
			{
				if (!HttpExchangeService.EwsGlobals.initialized)
				{
					lock (HttpExchangeService.EwsGlobals.staticLock)
					{
						if (!HttpExchangeService.EwsGlobals.initialized)
						{
							HttpApplicationState application = httpContext.Application;
							ADIdentityInformationCache.Initialize(HttpExchangeService.EwsGlobals.ADIdentityCacheSize.Value);
							application["WS_APPWideMailboxCacheKey"] = (HttpExchangeService.EwsGlobals.AppWideStoreSessionCache = new AppWideStoreSessionCache());
							application["WS_AcceptedDomainCacheKey"] = (HttpExchangeService.EwsGlobals.AcceptedDomainCache = new AcceptedDomainCache());
							application["WS_WorkloadManagerKey"] = (HttpExchangeService.EwsGlobals.UserWorkloadManager = UserWorkloadManager.Singleton);
							HttpExchangeService.EwsGlobals.ADIdentityInformationCache = ADIdentityInformationCache.Singleton;
							HttpExchangeService.EwsGlobals.initialized = true;
						}
					}
				}
			}

			public static AppWideStoreSessionCache AppWideStoreSessionCache { get; private set; }

			public static AcceptedDomainCache AcceptedDomainCache { get; private set; }

			public static UserWorkloadManager UserWorkloadManager { get; private set; }

			public static ADIdentityInformationCache ADIdentityInformationCache { get; private set; }

			private static readonly IntAppSettingsEntry ADIdentityCacheSize = new IntAppSettingsEntry("HttpExchangeService.ADIdentityCacheSize", 4000, null);

			private static object staticLock = new object();

			private static bool initialized = false;
		}
	}
}

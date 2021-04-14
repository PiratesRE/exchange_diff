using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RightsManagementAsyncResult : LazyAsyncResult
	{
		internal RmsClientManagerContext Context
		{
			get
			{
				return this.context;
			}
		}

		internal Breadcrumbs<Constants.State> BreadCrumbs
		{
			get
			{
				return this.breadcrumbs;
			}
		}

		internal RmsClientManager.SaveContextOnAsyncQueryCallback SaveContextCallback
		{
			get
			{
				return this.saveContextCallback;
			}
		}

		internal IRmsLatencyTracker LatencyTracker
		{
			get
			{
				return this.context.LatencyTracker;
			}
		}

		internal RightsManagementAsyncResult(RmsClientManagerContext context, object callerState, AsyncCallback callerCallback) : base(null, callerState, RightsManagementAsyncResult.WrapCallbackWithUnhandledExceptionAndCrash(callerCallback))
		{
			ArgumentValidator.ThrowIfNull("context", context);
			this.context = context;
			this.adSession = RmsClientManager.ADSession;
			this.saveContextCallback = RmsClientManager.SaveContextCallback;
			RmsClientManager.SaveContextCallback = null;
		}

		internal void AddBreadCrumb(Constants.State value)
		{
			RightsManagementAsyncResult rightsManagementAsyncResult = base.AsyncState as RightsManagementAsyncResult;
			if (rightsManagementAsyncResult == null || rightsManagementAsyncResult == this)
			{
				if (this.breadcrumbs == null)
				{
					this.breadcrumbs = new Breadcrumbs<Constants.State>(32);
				}
				this.breadcrumbs.Drop(value);
				return;
			}
			rightsManagementAsyncResult.AddBreadCrumb(value);
		}

		internal void InvokeSaveContextCallback()
		{
			RightsManagementAsyncResult rightsManagementAsyncResult = base.AsyncState as RightsManagementAsyncResult;
			if (rightsManagementAsyncResult == null || rightsManagementAsyncResult == this)
			{
				if (this.saveContextCallback != null)
				{
					this.saveContextCallback(base.AsyncState);
				}
				RmsClientManager.ADSession = this.adSession;
				return;
			}
			rightsManagementAsyncResult.InvokeSaveContextCallback();
		}

		private static AsyncCallback WrapCallbackWithUnhandledExceptionAndCrash(AsyncCallback callback)
		{
			if (callback == null)
			{
				return null;
			}
			return delegate(IAsyncResult asyncResult)
			{
				RightsManagementAsyncResult asyncResultRM = asyncResult as RightsManagementAsyncResult;
				try
				{
					if (asyncResultRM != null)
					{
						asyncResultRM.InvokeSaveContextCallback();
					}
					callback(asyncResult);
				}
				catch (Exception exception)
				{
					Exception exception2;
					ExWatson.SendReportAndCrashOnAnotherThread(exception2, ReportOptions.None, delegate(Exception exception, int threadId)
					{
						if (asyncResultRM != null)
						{
							asyncResultRM.InvokeSaveContextCallback();
						}
					}, null);
					throw;
				}
			};
		}

		private readonly IConfigurationSession adSession;

		private readonly RmsClientManager.SaveContextOnAsyncQueryCallback saveContextCallback;

		private readonly RmsClientManagerContext context;

		private Breadcrumbs<Constants.State> breadcrumbs;
	}
}

using System;
using System.Data.Services;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class RbacAuthorizationManager : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			if (RbacAuthorizationManager.NewAuthZMethodEnabled.Value)
			{
				if (HttpContext.Current.Items.Contains(RbacAuthorizationManager.DataServiceExceptionKey))
				{
					throw (DataServiceException)HttpContext.Current.Items[RbacAuthorizationManager.DataServiceExceptionKey];
				}
				operationContext.SetRbacPrincipal((RbacPrincipal)HttpContext.Current.User);
				return base.CheckAccessCore(operationContext);
			}
			else
			{
				RbacPrincipal rbacPrincipal = null;
				ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.RbacPrincipalAcquireLatency, delegate
				{
					rbacPrincipal = RbacPrincipalManager.Instance.AcquireRbacPrincipal(HttpContext.Current);
				});
				if (rbacPrincipal == null)
				{
					return false;
				}
				HttpContext.Current.User = rbacPrincipal;
				rbacPrincipal.SetCurrentThreadPrincipal();
				if (OperationContext.Current != null)
				{
					OperationContext.Current.SetRbacPrincipal(rbacPrincipal);
				}
				return base.CheckAccessCore(operationContext);
			}
		}

		public static readonly string DataServiceExceptionKey = "DataServiceExceptionKey";

		private static readonly BoolAppSettingsEntry NewAuthZMethodEnabled = new BoolAppSettingsEntry("NewAuthZMethodEnabled", false, ExTraceGlobals.ReportingWebServiceTracer);
	}
}

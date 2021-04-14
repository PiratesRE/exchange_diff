using System;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Web;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	public abstract class ServiceBase
	{
		internal ServiceBase(IUserWorkloadManager userWorkloadManager)
		{
			ArgumentValidator.ThrowIfNull("userWorkloadManager", userWorkloadManager);
			this.UserWorkloadManager = userWorkloadManager;
		}

		internal IUserWorkloadManager UserWorkloadManager { get; private set; }

		internal IAsyncResult BeginServiceCommand(IServiceCommand command)
		{
			ExAssert.RetailAssert(Thread.CurrentPrincipal.Identity.IsAuthenticated, "BeginServiceCommand call must be authenticated.");
			IStandardBudget standardBudget = this.AcquireAndCheckBudget(command);
			command.Initialize(standardBudget);
			if (!this.UserWorkloadManager.TrySubmitNewTask(command))
			{
				ServiceBusyException ex = new ServiceBusyException(command.Description);
				command.Complete(ex);
				this.ThrowServiceBusyException(command.Description, standardBudget.Owner, ex);
			}
			return command.CommandAsyncResult;
		}

		internal abstract IStandardBudget AcquireBudget(IServiceCommand serviceCommand);

		internal T EndServiceCommand<T>(IAsyncResult asyncResult)
		{
			ExAssert.RetailAssert(Thread.CurrentPrincipal.Identity.IsAuthenticated, "BeginServiceCommand call must be authenticated.");
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			ServiceCommandAsyncResult<T> serviceCommandAsyncResult = asyncResult as ServiceCommandAsyncResult<T>;
			return serviceCommandAsyncResult.End();
		}

		internal void ThrowServiceBusyException(string command, BudgetKey budgetKey, OverBudgetException overBudget)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("command", command);
			ArgumentValidator.ThrowIfNull("budgetKey", budgetKey);
			ArgumentValidator.ThrowIfNull("overBudget", overBudget);
			this.ThrowServiceBusyException(string.Format("{0}-{1}", command, budgetKey.ToString()), new PushNotificationFault(overBudget, overBudget.BackoffTime, true), overBudget);
		}

		internal void ThrowServiceBusyException(string command, BudgetKey budgetKey, Exception ex)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("command", command);
			ArgumentValidator.ThrowIfNull("budgetKey", budgetKey);
			ArgumentValidator.ThrowIfNull("ex", ex);
			this.ThrowServiceBusyException(string.Format("{0}-{1}", command, budgetKey.ToString()), ex);
		}

		protected void ThrowServiceBusyException(string userId, Exception ex)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("userId", userId);
			ArgumentValidator.ThrowIfNull("ex", ex);
			this.ThrowServiceBusyException(userId, new PushNotificationFault(ex), ex);
		}

		private void ThrowServiceBusyException(string userId, PushNotificationFault fault, Exception ex)
		{
			PushNotificationsCrimsonEvents.ServerBusy.LogPeriodic<string, string>(userId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, userId, ex.ToTraceString());
			if (ExTraceGlobals.PushNotificationServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "The service is too busy to process the request '{0}'.", string.Format("{0}-{1}", userId, ex.ToTraceString()));
			}
			throw new WebFaultException<PushNotificationFault>(fault, HttpStatusCode.Forbidden);
		}

		private IStandardBudget AcquireAndCheckBudget(IServiceCommand serviceCommand)
		{
			IStandardBudget standardBudget = this.AcquireBudget(serviceCommand);
			try
			{
				standardBudget.CheckOverBudget();
				standardBudget.StartConnection("ServiceBase.AcquireAndCheckBudget");
			}
			catch (OverBudgetException overBudget)
			{
				BudgetKey owner = standardBudget.Owner;
				standardBudget.Dispose();
				this.ThrowServiceBusyException(serviceCommand.Description, standardBudget.Owner, overBudget);
			}
			return standardBudget;
		}

		protected const int DefaultWlmMaxRequestsQueued = 500;

		protected const int DefaultWlmMaxWorkerThreadsPerProc = 10;

		internal static readonly SidBudgetKey LocalSystemBudgetKey = new SidBudgetKey(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), BudgetType.PushNotificationTenant, true, ADSessionSettings.FromRootOrgScopeSet());
	}
}

using System;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class EwsBudget : StandardBudget
	{
		public static IEwsBudget Acquire(CallContext callContext)
		{
			IEwsBudget result;
			try
			{
				ExternalCallContext externalCallContext = callContext as ExternalCallContext;
				BudgetKey key;
				if (externalCallContext != null)
				{
					key = EwsBudget.GetExternalCallContextBudgetKey(externalCallContext);
				}
				else
				{
					key = EwsBudget.GetCallContextBudgetKey(callContext);
				}
				IEwsBudget ewsBudget = EwsBudget.Acquire(key);
				result = ewsBudget;
			}
			catch (OverBudgetException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			return result;
		}

		public new static IEwsBudget Acquire(BudgetKey key)
		{
			EwsBudget innerBudget = EwsBudgetCache.Singleton.Get(key);
			return new EwsBudgetWrapper(innerBudget);
		}

		public CostHandle StartHangingConnection(Action<CostHandle> onRelease)
		{
			lock (base.SyncRoot)
			{
				if (!base.ThrottlingPolicy.MaxConcurrency.IsUnlimited && this.hangingConnections + 1 > Global.HangingConnectionLimit)
				{
					throw base.CreateOverBudgetException("MaxStreamingConcurrency", Global.HangingConnectionLimit.ToString(), 0);
				}
				this.hangingConnections++;
			}
			return new CostHandle(this, CostType.HangingConnection, onRelease, "EwsBudgetCache.StartHangingConnection", default(TimeSpan));
		}

		protected override void AccountForCostHandle(CostHandle costHandle)
		{
			if (costHandle.CostType == CostType.HangingConnection)
			{
				lock (base.SyncRoot)
				{
					if (this.hangingConnections <= 0)
					{
						throw new InvalidOperationException("[EwsBudget.AccountForCostHandle] End for HangingConnections was called, but there are no outstanding HangingConnections.");
					}
					this.hangingConnections--;
					return;
				}
			}
			base.AccountForCostHandle(costHandle);
		}

		public int HangingConnections
		{
			get
			{
				return this.hangingConnections;
			}
		}

		internal static CostType GetConnectionCostType()
		{
			if (EwsOperationContextBase.Current != null)
			{
				object obj = null;
				if (EwsOperationContextBase.Current.RequestMessage.Properties.TryGetValue("ConnectionCostType", out obj))
				{
					return (CostType)obj;
				}
			}
			return CostType.Connection;
		}

		private static BudgetType GetBudgetTypeForMethodCalled(CallContext callContext)
		{
			BudgetType result = Global.BudgetType;
			if (Global.BulkOperationThrottlingEnabled && Global.BulkOperationMethods.Contains(callContext.MethodName))
			{
				result = Global.BulkOperationBudgetType;
			}
			if (Global.NonInteractiveThrottlingEnabled && Global.NonInteractiveOperationMethods.Contains(callContext.MethodName))
			{
				result = Global.NonInteractiveOperationBudgetType;
			}
			return result;
		}

		private static BudgetKey GetExternalCallContextBudgetKey(ExternalCallContext externalCallContext)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<SmtpAddress>(0L, "[EwsBudget.GetExternalCallContextBudgetKey] Getting budget key for external caller {0}", externalCallContext.EmailAddress);
			return new StringBudgetKey(externalCallContext.EmailAddress.ToString(), false, EwsBudget.GetBudgetTypeForMethodCalled(externalCallContext));
		}

		private static BudgetKey GetCallContextBudgetKey(CallContext callContext)
		{
			ADSessionSettings settings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(callContext.ADRecipientSessionContext.OrganizationId);
			BudgetType budgetTypeForMethodCalled = EwsBudget.GetBudgetTypeForMethodCalled(callContext);
			if (EwsBudget.IsLongRunningScenarioContext(callContext))
			{
				if (!Global.LongRunningScenarioNonBackgroundTasks.Contains(callContext.MethodName))
				{
					callContext.BackgroundLoad = true;
				}
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Ews.LongRunningScenarioThrottling.Enabled)
				{
					callContext.IsLongRunningScenario = true;
				}
				return new UnthrottledBudgetKey(callContext.OriginalCallerContext.IdentifierString, budgetTypeForMethodCalled);
			}
			if (EwsBudget.IsWellKnownClientsBackgroundSyncScenario(callContext))
			{
				return new UnthrottledBudgetKey(callContext.OriginalCallerContext.IdentifierString, budgetTypeForMethodCalled);
			}
			if (callContext.MailboxAccessType == MailboxAccessType.ExchangeImpersonation || (callContext.MailboxAccessType == MailboxAccessType.ApplicationAction && callContext.EffectiveCaller.ClientSecurityContext != null))
			{
				if (EwsBudget.GetConnectionCostType() == CostType.HangingConnection && callContext.IsPartnerUser)
				{
					ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget::InitializeFromCallContext] EI call for act as account '{0}', caller is FPO partner {1}. However for streaming notification, we grant unthrottled policy.", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid), callContext.OriginalCallerContext.IdentifierString);
					return new UnthrottledBudgetKey(callContext.OriginalCallerContext.IdentifierString, budgetTypeForMethodCalled, false);
				}
				ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget::InitializeFromCallContext] EI call for act as account '{0}', caller is '{1}'.  Using service account budget for act as account.", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid), (callContext.OriginalCallerContext.Sid == null) ? "<null>" : SidToAccountMap.Singleton.Get(callContext.OriginalCallerContext.Sid));
				return new SidBudgetKey(callContext.EffectiveCallerSid, budgetTypeForMethodCalled, true, settings);
			}
			else if (callContext.MailboxAccessType == MailboxAccessType.ServerToServer)
			{
				if (callContext.AllowUnthrottledBudget)
				{
					ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget.InitializeFromCallContext] Admin/service S2S call for account '{0}', caller '{1}'.  Using unthrottled budget.", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid), SidToAccountMap.Singleton.Get(callContext.OriginalCallerContext.Sid));
					return new UnthrottledBudgetKey(callContext.OriginalCallerContext.IdentifierString, budgetTypeForMethodCalled, true);
				}
				ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget.InitializeFromCallContext] S2S call for act as account '{0}', caller '{1}'.  Using service account budget for act as account.", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid), SidToAccountMap.Singleton.Get(callContext.OriginalCallerContext.Sid));
				return new SidBudgetKey(callContext.EffectiveCallerSid, budgetTypeForMethodCalled, true, settings);
			}
			else
			{
				if (callContext.EffectiveCaller.ClientSecurityContext != null)
				{
					ExTraceGlobals.ThrottlingTracer.TraceDebug<string>(0L, "[EwsBudget.InitializeFromCallContext] Getting normal budget for caller {0}", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid));
					return new SidBudgetKey(callContext.EffectiveCallerSid, budgetTypeForMethodCalled, false, settings);
				}
				ExTraceGlobals.ThrottlingTracer.TraceDebug<OrganizationId>(0L, "[EwsBudget.InitializeFromCallContext] Getting tenant budget for caller org {0}", callContext.OriginalCallerContext.OrganizationId);
				return new TenantBudgetKey(callContext.OriginalCallerContext.OrganizationId, budgetTypeForMethodCalled);
			}
		}

		private static bool IsWellKnownClientsBackgroundSyncScenario(CallContext callContext)
		{
			return Global.BackgroundSyncTasksForWellKnownClientsEnabled && (Global.BackgroundSyncTasksForWellKnownClients.Contains("*") || Global.BackgroundSyncTasksForWellKnownClients.Any((string x) => string.Equals(x, callContext.MethodName, StringComparison.OrdinalIgnoreCase))) && !string.IsNullOrEmpty(callContext.UserAgent) && Global.WellKnownClientsForBackgroundSync.Any((string x) => callContext.UserAgent.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private static bool IsLongRunningScenarioContext(CallContext callContext)
		{
			if (Global.LongRunningScenarioTasks.Contains(callContext.MethodName) && !string.IsNullOrEmpty(callContext.UserAgent) && Global.LongRunningScenarioEnabledUserAgents.IsMatch(callContext.UserAgent))
			{
				if (callContext.LogonTypeSource == LogonTypeSource.OpenAsAdminOrSystemServiceHeader)
				{
					ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget::InitializeFromCallContext] (IsLongRunningScenarioContext) Granting unlimited budget to {0} calling as system because the call is part of a long running process {1} ", (callContext.EffectiveCallerSid != null) ? SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid) : "NA", callContext.UserAgent);
					return true;
				}
				if (callContext.ManagementRole != null && (callContext.ManagementRole.HasUserRoles || callContext.ManagementRole.HasApplicationRoles))
				{
					foreach (string[] array2 in new string[][]
					{
						callContext.ManagementRole.ApplicationRoles,
						callContext.ManagementRole.UserRoles
					})
					{
						if (array2 != null)
						{
							foreach (string text in array2)
							{
								RoleType item;
								if (Enum.TryParse<RoleType>(text, out item) && Global.LongRunningScenarioEnabledRoleTypes.Contains(item))
								{
									ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget::InitializeFromCallContext] (IsLongRunningScenarioContext) Granting unlimited budget when calling with role {0} because the call is part of a long running process {1} ", text, callContext.UserAgent);
									return true;
								}
							}
						}
					}
				}
				if (callContext.EffectiveCaller != null && callContext.EffectiveCaller.ObjectSid != null)
				{
					try
					{
						ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(callContext.EffectiveCaller, null, false);
						if (exchangeRunspaceConfiguration != null && Global.LongRunningScenarioEnabledRoleTypes.Intersect(exchangeRunspaceConfiguration.RoleTypes).Any<RoleType>())
						{
							ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>(0L, "[EwsBudget::InitializeFromCallContext] (IsLongRunningScenarioContext) Granting unlimited budget to {0} because the call is part of a long running process {1} ", SidToAccountMap.Singleton.Get(callContext.EffectiveCallerSid), callContext.UserAgent);
							return true;
						}
					}
					catch (ImpersonateUserDeniedException)
					{
						ExTraceGlobals.ThrottlingTracer.TraceDebug<string>(0L, "[EwsBudget::InitializeFromCallContext] (IsLongRunningScenarioContext) Not granting unlimited budget because impersonation was invalid {0} ", callContext.UserAgent);
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public EwsBudget(BudgetKey budgetKey, IThrottlingPolicy throttlingPolicy) : base(budgetKey, throttlingPolicy)
		{
		}

		public override string ToString()
		{
			return string.Format("{0},HangingConn:{1},{2}", base.ToString(), this.hangingConnections, this.GetSubscriptionState());
		}

		private string GetSubscriptionState()
		{
			if (Subscriptions.Singleton != null)
			{
				int subscriptionCountForUser = Subscriptions.Singleton.GetSubscriptionCountForUser(base.Owner);
				return string.Format("Sub:{0},MaxSub:{1};", subscriptionCountForUser, base.ThrottlingPolicy.FullPolicy.EwsMaxSubscriptions);
			}
			return "NoSub";
		}

		internal static void LogOverBudgetToIIS(OverBudgetException overBudgetException)
		{
			if (CallContext.Current != null && CallContext.Current.HttpContext != null && CallContext.Current.HttpContext.Response != null)
			{
				string value = string.Format(";OverBudget({0}/{1}),Owner:{2}[{3}]", new object[]
				{
					overBudgetException.IsServiceAccountBudget ? "ServiceAccount" : "Normal",
					overBudgetException.PolicyPart,
					overBudgetException.Owner,
					overBudgetException.Snapshot
				});
				RequestDetailsLogger.Current.AppendGenericError("OverBudget", value);
			}
		}

		public const string MaxStreamingConcurrencyPart = "MaxStreamingConcurrency";

		private int hangingConnections;
	}
}

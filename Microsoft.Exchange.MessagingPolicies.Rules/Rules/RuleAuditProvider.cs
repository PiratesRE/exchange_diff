using System;
using System.ComponentModel;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RuleAuditProvider : AuditProvider
	{
		private RuleAuditProvider() : base(RuleAuditProvider.EtrEventSourceName)
		{
		}

		private static void Init()
		{
			if (!RuleAuditProvider.init)
			{
				lock (RuleAuditProvider.lockVar)
				{
					if (!RuleAuditProvider.init)
					{
						try
						{
							RuleAuditProvider.provider = new RuleAuditProvider();
							RuleAuditProvider.skip = false;
						}
						catch (UnauthorizedAccessException)
						{
							RuleAuditProvider.skip = true;
						}
						catch (PrivilegeNotHeldException)
						{
							RuleAuditProvider.skip = true;
						}
						RuleAuditProvider.init = true;
					}
				}
			}
		}

		public static void LogSuccess(string ruleInfo)
		{
			try
			{
				RuleAuditProvider.Init();
				if (!RuleAuditProvider.skip)
				{
					RuleAuditProvider.provider.ReportAudit(1, true, new object[]
					{
						ruleInfo
					});
				}
			}
			catch (Win32Exception)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, "audit logging for a success rule loading failed");
			}
		}

		public static void LogFailure(string ruleInfo)
		{
			try
			{
				RuleAuditProvider.Init();
				if (!RuleAuditProvider.skip)
				{
					RuleAuditProvider.provider.ReportAudit(2, false, new object[]
					{
						ruleInfo
					});
				}
			}
			catch (Win32Exception)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, "audit logging for a failed rule loading failed");
			}
		}

		internal static readonly string EtrEventSourceName = "MSExchange Messaging Policies";

		private static RuleAuditProvider provider;

		private static object lockVar = new object();

		private static bool init;

		private static bool skip;
	}
}

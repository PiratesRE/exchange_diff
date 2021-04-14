using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class AdminLogAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			if (TaskLogger.IsSetupLogging)
			{
				if (AdminLogAgentClassFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					AdminLogAgentClassFactory.Tracer.TraceDebug<AdminLogAgentClassFactory>((long)this.GetHashCode(), "{0} GetSupportedCmdlets called. Returning the empty string array since we do not want to audit cmdlets executed during setup", this);
				}
				return AdminLogAgentClassFactory.emptyCmdlets;
			}
			if (AdminLogAgentClassFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				AdminLogAgentClassFactory.Tracer.TraceDebug<AdminLogAgentClassFactory, string>((long)this.GetHashCode(), "{0} GetSupportedCmdlets called. Returning '{1}'", this, AdminLogAgentClassFactory.allCmdlets[0]);
			}
			return AdminLogAgentClassFactory.allCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			if (AdminLogAgentClassFactory.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				AdminLogAgentClassFactory.Tracer.TraceDebug<AdminLogAgentClassFactory>((long)this.GetHashCode(), "{0} Return the admin audit log agent handler.", this);
			}
			if (!AdminLogAgentClassFactory.isDiagnosticsInitialized)
			{
				AdminLogAgentClassFactory.isDiagnosticsInitialized = true;
				ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents(AdminAuditLogHealthHandler.GetInstance());
			}
			return new AdminLogProvisioningHandler(this.configurationCache);
		}

		public override string ToString()
		{
			return "AdminLogAgentClassFactory: ";
		}

		private const string toString = "AdminLogAgentClassFactory: ";

		private static readonly string[] allCmdlets = new string[]
		{
			"*"
		};

		private static readonly string[] emptyCmdlets = new string[0];

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;

		private readonly ConfigurationCache configurationCache = new ConfigurationCache();

		private static bool isDiagnosticsInitialized;
	}
}

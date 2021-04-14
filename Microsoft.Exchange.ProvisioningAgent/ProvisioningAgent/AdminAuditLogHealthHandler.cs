using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AdminAuditLogHealthHandler : ExchangeDiagnosableWrapper<AdminAuditLogHealth>
	{
		private AdminAuditLogHealthHandler()
		{
		}

		public static AdminAuditLogHealthHandler GetInstance()
		{
			return AdminAuditLogHealthHandler.instance.Value;
		}

		internal AdminAuditLogHealth Health
		{
			get
			{
				return this.health.Value;
			}
		}

		internal override AdminAuditLogHealth GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			return this.Health;
		}

		protected override string ComponentName
		{
			get
			{
				return "AdminAuditLogHealth";
			}
		}

		private static readonly Lazy<AdminAuditLogHealthHandler> instance = new Lazy<AdminAuditLogHealthHandler>(() => new AdminAuditLogHealthHandler(), true);

		private readonly Lazy<AdminAuditLogHealth> health = new Lazy<AdminAuditLogHealth>(true);
	}
}

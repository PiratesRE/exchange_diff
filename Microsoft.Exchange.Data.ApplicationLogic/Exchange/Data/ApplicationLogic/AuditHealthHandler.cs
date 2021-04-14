using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class AuditHealthHandler : ExchangeDiagnosableWrapper<AuditHealthInfo>
	{
		public static AuditHealthHandler Instance
		{
			get
			{
				return Singleton<AuditHealthHandler>.Instance;
			}
		}

		public static AuditHealthInfo Health
		{
			get
			{
				return AuditHealthInfo.Instance;
			}
		}

		protected override string ComponentName
		{
			get
			{
				return "AuditHealthInfo";
			}
		}

		internal override AuditHealthInfo GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			return AuditHealthHandler.Health;
		}

		public static void Register()
		{
			if (!AuditHealthHandler.isRegistered)
			{
				AuditHealthHandler.isRegistered = true;
				ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents(AuditHealthHandler.Instance);
			}
		}

		private static bool isRegistered;
	}
}

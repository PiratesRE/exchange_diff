using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers
{
	public class FaultDiagnosticsComponent : ExchangeDiagnosableWrapper<FaultDiagnosticsInfo>
	{
		protected override string UsageText
		{
			get
			{
				return "This handler is generic exception handler for all diagnostics compoenents. This handler is not registered explicitly.";
			}
		}

		protected override string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		internal override FaultDiagnosticsInfo GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			return this.outputMessage;
		}

		internal void SetComponentNameAndMessage(string componentName, int errorCode, string errorMessage)
		{
			this.componentName = componentName;
			this.outputMessage = new FaultDiagnosticsInfo(errorCode, errorMessage);
		}

		private string componentName;

		private FaultDiagnosticsInfo outputMessage;
	}
}

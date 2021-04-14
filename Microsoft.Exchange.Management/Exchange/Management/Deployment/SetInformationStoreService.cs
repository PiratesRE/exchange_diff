using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "InformationStoreService")]
	public class SetInformationStoreService : ConfigureService
	{
		public SetInformationStoreService()
		{
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.FailureActionsFlag = true;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeIS";
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.ConfigureFailureActions();
			base.ConfigureFailureActionsFlag();
			TaskLogger.LogExit();
		}

		private const string ServiceShortName = "MSExchangeIS";
	}
}

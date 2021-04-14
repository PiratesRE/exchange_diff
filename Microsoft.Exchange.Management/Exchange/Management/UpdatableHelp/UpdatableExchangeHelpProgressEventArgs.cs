using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	internal class UpdatableExchangeHelpProgressEventArgs : EventArgs
	{
		internal UpdatableExchangeHelpProgressEventArgs(UpdatePhase phase, LocalizedString subTask, int numerator, int denominator)
		{
			LocalizedString value = LocalizedString.Empty;
			switch (phase)
			{
			case UpdatePhase.Checking:
				value = UpdatableHelpStrings.UpdatePhaseChecking;
				break;
			case UpdatePhase.Downloading:
				value = UpdatableHelpStrings.UpdatePhaseDownloading;
				break;
			case UpdatePhase.Extracting:
				value = UpdatableHelpStrings.UpdatePhaseExtracting;
				break;
			case UpdatePhase.Validating:
				value = UpdatableHelpStrings.UpdatePhaseValidating;
				break;
			case UpdatePhase.Installing:
				value = UpdatableHelpStrings.UpdatePhaseInstalling;
				break;
			case UpdatePhase.Finalizing:
				value = UpdatableHelpStrings.UpdatePhaseFinalizing;
				break;
			case UpdatePhase.Rollback:
				value = UpdatableHelpStrings.UpdatePhaseRollback;
				break;
			}
			this.ProgressStatus = ((!subTask.Equals(LocalizedString.Empty)) ? UpdatableHelpStrings.UpdateStatus2(value, subTask) : UpdatableHelpStrings.UpdateStatus1(value));
			this.Activity = UpdatableHelpStrings.UpdateModuleName;
			if (denominator != 0)
			{
				this.PercentCompleted = Math.Abs(numerator) * 100 / Math.Abs(denominator);
				return;
			}
			this.PercentCompleted = 0;
		}

		internal LocalizedString Activity { get; private set; }

		internal LocalizedString ProgressStatus { get; private set; }

		internal int PercentCompleted { get; private set; }
	}
}

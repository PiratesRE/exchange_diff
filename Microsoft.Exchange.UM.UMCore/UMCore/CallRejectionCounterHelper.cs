using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CallRejectionCounterHelper
	{
		public static CallRejectionCounterHelper Instance { get; private set; }

		public abstract void SetCounters(Exception ex, Action<bool> userDelegate, bool arg, bool isDiagnosticCall);

		static CallRejectionCounterHelper()
		{
			if (CommonConstants.UseDataCenterLogging)
			{
				CallRejectionCounterHelper.Instance = new CallRejectionCounterHelper.DatacenterCallRejectionCounterHelper();
				return;
			}
			CallRejectionCounterHelper.Instance = new CallRejectionCounterHelper.EnterpriseCallRejectionCounterHelper();
		}

		private class EnterpriseCallRejectionCounterHelper : CallRejectionCounterHelper
		{
			public override void SetCounters(Exception ex, Action<bool> userDelegate, bool arg, bool isDiagnosticCall)
			{
				if (!isDiagnosticCall)
				{
					userDelegate(arg);
				}
			}
		}

		private class DatacenterCallRejectionCounterHelper : CallRejectionCounterHelper
		{
			public override void SetCounters(Exception ex, Action<bool> userDelegate, bool arg, bool isDiagnosticCall)
			{
				if (!isDiagnosticCall)
				{
					if (ex != null)
					{
						CallRejectedException ex2 = ex as CallRejectedException;
						if (ex2 != null && ex2.Reason.Category == 1)
						{
							return;
						}
					}
					userDelegate(arg);
				}
			}
		}
	}
}

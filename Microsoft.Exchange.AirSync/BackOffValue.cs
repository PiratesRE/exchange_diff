using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class BackOffValue : IComparable
	{
		static BackOffValue()
		{
			BackOffValue.backOffTypeMapping.Add(BackOffType.Low, "L");
			BackOffValue.backOffTypeMapping.Add(BackOffType.Medium, "M");
			BackOffValue.backOffTypeMapping.Add(BackOffType.High, "H");
		}

		public double BackOffDuration { get; set; }

		public BackOffType BackOffType { get; set; }

		public string BackOffReason { get; set; }

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			BackOffValue backOffValue = obj as BackOffValue;
			if (backOffValue != null)
			{
				return this.BackOffDuration.CompareTo(backOffValue.BackOffDuration);
			}
			throw new ArgumentException("Object is not a valid BackOffValue");
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", BackOffValue.backOffTypeMapping[this.BackOffType], this.BackOffDuration);
		}

		public static BackOffValue GetEffectiveBackOffValue(BackOffValue budgetBackOff, BackOffValue abBackOff)
		{
			BackOffValue backOffValue = new BackOffValue();
			backOffValue.BackOffDuration = Math.Ceiling(Math.Max(budgetBackOff.BackOffDuration, abBackOff.BackOffDuration));
			backOffValue.BackOffType = ((budgetBackOff.BackOffType >= abBackOff.BackOffType) ? budgetBackOff.BackOffType : abBackOff.BackOffType);
			backOffValue.BackOffReason = ((budgetBackOff.BackOffType >= abBackOff.BackOffType) ? budgetBackOff.BackOffReason : abBackOff.BackOffReason);
			if (backOffValue.BackOffDuration > GlobalSettings.MaxBackOffDuration.TotalSeconds)
			{
				AirSyncDiagnostics.TraceDebug<double, TimeSpan>(ExTraceGlobals.RequestsTracer, null, "Calculated backoff time exceed max allowed backoff time, using predefined MaxBackOffDuration. CalculatedbackOff:{0} sec, MaxValue as per Settings:{1}", backOffValue.BackOffDuration, GlobalSettings.MaxBackOffDuration);
				backOffValue.BackOffDuration = GlobalSettings.MaxBackOffDuration.TotalSeconds;
			}
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.SuggestedBackOffValue, string.Format("BBkOff:{0}, ABBkOff:{1}, EffBkOff:{2}", budgetBackOff.ToString(), abBackOff.ToString(), backOffValue.ToString()));
			}
			return backOffValue;
		}

		public static BackOffValue NoBackOffValue = new BackOffValue
		{
			BackOffDuration = Math.Ceiling(-1.0 * ThrottlingPolicyDefaults.EasMaxBurst.Value / 1000.0),
			BackOffType = BackOffType.Low,
			BackOffReason = string.Empty
		};

		public static Dictionary<BackOffType, string> backOffTypeMapping = new Dictionary<BackOffType, string>();
	}
}

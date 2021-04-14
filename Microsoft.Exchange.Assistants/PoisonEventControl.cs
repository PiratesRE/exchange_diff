using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class PoisonEventControl : PoisonControl
	{
		public PoisonEventControl(PoisonControlMaster master, DatabaseInfo databaseInfo) : base(master, databaseInfo, "Event")
		{
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "Poison event control control for " + base.DatabaseInfo.ToString();
			}
			return this.toString;
		}

		public bool IsPoisonEvent(MapiEvent mapiEvent)
		{
			return this.GetCrashCount(mapiEvent) >= base.Master.PoisonCrashCount && base.Master.Enabled;
		}

		public bool IsToxicEvent(MapiEvent mapiEvent)
		{
			return this.GetCrashCount(mapiEvent) >= base.Master.ToxicCrashCount && base.Master.Enabled;
		}

		public int GetCrashCount(MapiEvent mapiEvent)
		{
			int result;
			if (this.crashCounts.TryGetValue(mapiEvent.EventCounter, out result))
			{
				return result;
			}
			return 0;
		}

		protected override void LoadCrashData(string subKeyName, int crashCount)
		{
			Exception ex = null;
			try
			{
				long key = long.Parse(subKeyName, NumberStyles.HexNumber);
				this.crashCounts.Add(key, crashCount);
			}
			catch (FormatException ex2)
			{
				ex = ex2;
			}
			catch (OverflowException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.PoisonControlTracer.TraceError<PoisonEventControl, string, Exception>((long)this.GetHashCode(), "{0}: Unable to load crash data from {1}. Exception: {2}", this, subKeyName, ex);
			}
		}

		protected override void HandleUnhandledException(object exception, EmergencyKit kit)
		{
			int num = this.GetCrashCount(kit.MapiEvent) + 1;
			base.SaveCrashData(kit.MapiEvent.EventCounter.ToString("x16"), num);
			ExTraceGlobals.PoisonControlTracer.TraceError((long)this.GetHashCode(), "{0}: Unhandled Exception while processing eventCounter: {1}, crashCount: {2}, exception: {3}", new object[]
			{
				this,
				kit.MapiEvent.EventCounter,
				num,
				exception
			});
			if (num < base.Master.PoisonCrashCount || !base.Master.Enabled)
			{
				base.LogEvent(AssistantsEventLogConstants.Tuple_CrashEvent, null, new object[]
				{
					kit.AssistantName,
					num,
					kit.MapiEvent.EventCounter,
					base.DatabaseInfo.DisplayName,
					kit.MailboxDisplayName,
					exception
				});
				return;
			}
			base.LogEvent(AssistantsEventLogConstants.Tuple_PoisonEvent, null, new object[]
			{
				kit.MapiEvent.EventCounter,
				base.DatabaseInfo.DisplayName,
				kit.MailboxDisplayName,
				kit.AssistantName,
				num,
				exception
			});
		}

		private Dictionary<long, int> crashCounts = new Dictionary<long, int>();

		private string toString;
	}
}

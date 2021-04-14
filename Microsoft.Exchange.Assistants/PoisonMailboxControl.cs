using System;
using System.Collections.Generic;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class PoisonMailboxControl : PoisonControl
	{
		public PoisonMailboxControl(PoisonControlMaster master, DatabaseInfo databaseInfo) : base(master, databaseInfo, "Mailbox")
		{
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "Poison mailbox control for " + base.DatabaseInfo.ToString();
			}
			return this.toString;
		}

		public bool IsPoisonMailbox(Guid mailboxGuid)
		{
			return this.GetCrashCount(mailboxGuid) >= base.Master.PoisonCrashCount && base.Master.Enabled;
		}

		public int GetCrashCount(Guid mailboxGuid)
		{
			int result;
			if (!this.crashCounts.TryGetValue(mailboxGuid, out result))
			{
				return 0;
			}
			return result;
		}

		public void Clear()
		{
			base.RemoveDatabaseKey();
			this.crashCounts = new Dictionary<Guid, int>();
		}

		protected override void LoadCrashData(string subKeyName, int crashCount)
		{
			Exception ex = null;
			try
			{
				Guid key = new Guid(subKeyName);
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
			if (ex != null)
			{
				ExTraceGlobals.PoisonControlTracer.TraceError<PoisonMailboxControl, string, Exception>((long)this.GetHashCode(), "{0}: Unable to load crash data from {1}. Exception: {2}", this, subKeyName, ex);
			}
		}

		protected override void HandleUnhandledException(object exception, EmergencyKit kit)
		{
			int num = this.GetCrashCount(kit.MailboxGuid) + 1;
			base.SaveCrashData(kit.MailboxGuid.ToString(), num);
			ExTraceGlobals.PoisonControlTracer.TraceError((long)this.GetHashCode(), "{0}: Unhandled exception while processing mailbox: {1}, crashCount: {2}, exception: {3}", new object[]
			{
				this,
				kit.MailboxDisplayName,
				num,
				exception
			});
			if (num < base.Master.PoisonCrashCount || !base.Master.Enabled)
			{
				base.LogEvent(AssistantsEventLogConstants.Tuple_CrashMailbox, null, new object[]
				{
					kit.AssistantName,
					num,
					kit.MailboxDisplayName,
					base.DatabaseInfo.DisplayName,
					exception
				});
				return;
			}
			base.LogEvent(AssistantsEventLogConstants.Tuple_PoisonMailbox, null, new object[]
			{
				kit.AssistantName,
				num,
				kit.MailboxDisplayName,
				base.DatabaseInfo.DisplayName,
				exception
			});
		}

		private Dictionary<Guid, int> crashCounts = new Dictionary<Guid, int>();

		private string toString;
	}
}

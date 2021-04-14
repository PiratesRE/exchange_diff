using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver.Configuration
{
	internal sealed class StoreDriverConfig
	{
		private StoreDriverConfig()
		{
			StoreDriverParameters storeDriverParameters = new StoreDriverParameters();
			storeDriverParameters.Load(new Dictionary<string, StoreDriverParameterHandler>
			{
				{
					"AlwaysSetReminderOnAppointment",
					new StoreDriverParameterHandler(this.StoreDriverParameterParser)
				},
				{
					"IsAutoAcceptForGroupAndSelfForwardedEventEnabled",
					new StoreDriverParameterHandler(this.StoreDriverGroupMailboxProcessFlagParser)
				},
				{
					"IsGroupEscalationAgentEnabled",
					new StoreDriverParameterHandler(this.StoreDriverGroupEscalationAgentFlagParser)
				},
				{
					"MeetingHijackPreventionEnabled",
					new StoreDriverParameterHandler(this.StoreDriverMeetingHijackPreventionEnabledFlagParser)
				}
			});
		}

		internal static StoreDriverConfig Instance
		{
			get
			{
				if (StoreDriverConfig.instance == null)
				{
					lock (StoreDriverConfig.syncRoot)
					{
						if (StoreDriverConfig.instance == null)
						{
							StoreDriverConfig.instance = new StoreDriverConfig();
						}
					}
				}
				return StoreDriverConfig.instance;
			}
		}

		internal bool AlwaysSetReminderOnAppointment
		{
			get
			{
				return this.alwaysSetReminderOnAppointment;
			}
		}

		internal bool IsAutoAcceptForGroupAndSelfForwardedEventEnabled
		{
			get
			{
				return this.isAutoAcceptForGroupAndSelfForwardedEventEnabled;
			}
		}

		internal bool IsGroupEscalationAgentEnabled
		{
			get
			{
				return this.isGroupEscalationAgentEnabled;
			}
		}

		internal bool MeetingHijackPreventionEnabled
		{
			get
			{
				return this.meetingHijackPreventionEnabled;
			}
		}

		private void StoreDriverParameterParser(string key, string value)
		{
			if (string.Compare("AlwaysSetReminderOnAppointment", key, StringComparison.OrdinalIgnoreCase) == 0 && !bool.TryParse(value, out this.alwaysSetReminderOnAppointment))
			{
				this.alwaysSetReminderOnAppointment = true;
			}
		}

		private void StoreDriverGroupMailboxProcessFlagParser(string key, string value)
		{
			if (string.Compare("IsAutoAcceptForGroupAndSelfForwardedEventEnabled", key, StringComparison.OrdinalIgnoreCase) == 0 && !bool.TryParse(value, out this.isAutoAcceptForGroupAndSelfForwardedEventEnabled))
			{
				this.isAutoAcceptForGroupAndSelfForwardedEventEnabled = true;
			}
		}

		private void StoreDriverGroupEscalationAgentFlagParser(string key, string value)
		{
			if (string.Compare("IsGroupEscalationAgentEnabled", key, StringComparison.OrdinalIgnoreCase) == 0 && !bool.TryParse(value, out this.isGroupEscalationAgentEnabled))
			{
				this.isGroupEscalationAgentEnabled = true;
			}
		}

		private void StoreDriverMeetingHijackPreventionEnabledFlagParser(string key, string value)
		{
			if (string.Compare("MeetingHijackPreventionEnabled", key, StringComparison.OrdinalIgnoreCase) == 0 && !bool.TryParse(value, out this.meetingHijackPreventionEnabled))
			{
				this.meetingHijackPreventionEnabled = true;
			}
		}

		private const string IsAutoAcceptForGroupAndSelfForwardedEventEnabledKey = "IsAutoAcceptForGroupAndSelfForwardedEventEnabled";

		private const string AlwaysSetReminderOnAppointmentKey = "AlwaysSetReminderOnAppointment";

		private const string IsGroupEscalationAgentEnabledKey = "IsGroupEscalationAgentEnabled";

		private const string MeetingHijackPreventionEnabledKey = "MeetingHijackPreventionEnabled";

		private static readonly Trace diag = ExTraceGlobals.StoreDriverTracer;

		private static object syncRoot = new object();

		private static StoreDriverConfig instance;

		private bool alwaysSetReminderOnAppointment = true;

		private bool isAutoAcceptForGroupAndSelfForwardedEventEnabled = true;

		private bool isGroupEscalationAgentEnabled = true;

		private bool meetingHijackPreventionEnabled = true;
	}
}

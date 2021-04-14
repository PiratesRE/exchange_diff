using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class WinRMRequestTracker
	{
		internal static bool TryReviseAction(WinRMInfo winRMInfo, int status, int subStatus, out string revisedAction)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction(0L, "[WinRMRequestTracker::TryReviseAction] Enter");
			revisedAction = null;
			if (status == 500 && subStatus == 687)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<int, int>(0L, "[WinRMRequestTracker::TryReviseAction] Ping detected. Status = {0} SubStatus = {1}", status, subStatus);
				revisedAction = "Ping";
				return true;
			}
			if (winRMInfo == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug(0L, "[WinRMRequestTracker::TryReviseAction] winRMInfo = null.");
				return false;
			}
			string text = winRMInfo.SessionId ?? winRMInfo.ShellId;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			string action = winRMInfo.Action;
			if ("Remove-PSSession".Equals(action))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>(0L, "[WinRMRequestTracker::TryReviseAction] End tracking session {0} in cache.", text);
				WinRMRequestTracker.SessionIdToStateCache.InsertSliding(text, 20, WinRMRequestTracker.SlidingTimeSpanAfterRemoveSession, null);
				return false;
			}
			if ("New-PSSession".Equals(action) || ("Receive".Equals(action) && !WinRMRequestTracker.SessionIdToStateCache.Contains(text)))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>(0L, "[WinRMRequestTracker::TryReviseAction] Start tracking session {0} in cache.", text);
				WinRMRequestTracker.SessionIdToStateCache.InsertSliding(text, 0, WinRMRequestTracker.SlidingTimeSpan, null);
			}
			string commandId = winRMInfo.CommandId;
			if (!string.IsNullOrEmpty(commandId))
			{
				int num;
				if (WinRMRequestTracker.SessionIdToStateCache.TryGetValue(text, out num) && num != 10)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string>(0L, "[WinRMRequestTracker::TryReviseAction] Change session {0} state to command in cache.", text);
					WinRMRequestTracker.SessionIdToStateCache.InsertSliding(text, 10, WinRMRequestTracker.SlidingTimeSpan, null);
				}
				if (!string.IsNullOrEmpty(action) && action.EndsWith("Terminate"))
				{
					bool result = false;
					string arg;
					if ("Terminate".Equals(action) && WinRMRequestTracker.CommandIdToCommandNameCache.TryGetValue(commandId, out arg))
					{
						revisedAction = arg + ':' + "Terminate";
						result = true;
					}
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string>(0L, "[WinRMRequestTracker::TryReviseAction] End tracking command {0} in cache.", commandId);
					WinRMRequestTracker.CommandIdToCommandNameCache.Remove(commandId);
					return result;
				}
				if (!string.IsNullOrEmpty(winRMInfo.CommandName) && !WinRMRequestTracker.CommandIdToCommandNameCache.Contains(commandId))
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string, string>(0L, "[WinRMRequestTracker::TryReviseAction] Start tracking command {0} - {1} in cache.", commandId, winRMInfo.CommandName);
					WinRMRequestTracker.CommandIdToCommandNameCache.InsertSliding(commandId, winRMInfo.CommandName, WinRMRequestTracker.SlidingTimeSpan, null);
				}
				string str;
				if (("Receive".Equals(action) || "Command:Receive".Equals(action)) && WinRMRequestTracker.CommandIdToCommandNameCache.TryGetValue(commandId, out str))
				{
					revisedAction = str + ":Receive";
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string, string>(0L, "[WinRMRequestTracker::TryReviseAction] Revise action from {0} to {1}.", action, revisedAction);
					return true;
				}
				return false;
			}
			else
			{
				int num2;
				if ("Receive".Equals(action) && status == 500 && WinRMRequestTracker.SessionIdToStateCache.TryGetValue(text, out num2) && num2 >= 10)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug<int, int>(0L, "[WinRMRequestTracker::TryReviseAction] Ping detected. Status = {0}, curSessionState = {1}", status, num2);
					revisedAction = "Ping";
					if (num2 == 20)
					{
						ExTraceGlobals.HttpModuleTracer.TraceDebug<string>(0L, "[WinRMRequestTracker::TryReviseAction] Remove session state. sessionId = {1}", text);
						WinRMRequestTracker.SessionIdToStateCache.Remove(text);
					}
					return true;
				}
				int num3;
				if ("Receive".Equals(action) && WinRMRequestTracker.SessionIdToStateCache.TryGetValue(text, out num3) && num3 < 10 && num3 < 3)
				{
					revisedAction = "New-PSSession:Receive";
					num3++;
					WinRMRequestTracker.SessionIdToStateCache.InsertSliding(text, num3, WinRMRequestTracker.SlidingTimeSpan, null);
					ExTraceGlobals.HttpModuleTracer.TraceDebug<string, string, int>(0L, "[WinRMRequestTracker::TryReviseAction] Revise session from {0} to {1}. curSessionState = {2}", action, revisedAction, num3);
					return true;
				}
				return false;
			}
		}

		private const int SessionStateAfterCommandExecuted = 10;

		private const int SessionStateAfterSessionRemoved = 20;

		private const int MaxReceiveForNewPSSession = 3;

		internal static readonly TimeoutCache<string, int> SessionIdToStateCache = new TimeoutCache<string, int>(20, 5000, false);

		private static readonly TimeoutCache<string, string> CommandIdToCommandNameCache = new TimeoutCache<string, string>(20, 5000, false);

		private static readonly TimeSpan SlidingTimeSpan = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan SlidingTimeSpanAfterRemoveSession = TimeSpan.FromSeconds(5.0);
	}
}

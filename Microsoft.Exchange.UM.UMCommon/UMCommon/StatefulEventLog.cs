using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class StatefulEventLog
	{
		private StatefulEventLog()
		{
		}

		internal static StatefulEventLog Instance
		{
			get
			{
				return StatefulEventLog.instance;
			}
		}

		public void LogGreenEvent(string key, ExEventLog.EventTuple tuple, string periodicKey, bool forceLog, params object[] args)
		{
			this.LogEvent(key, HealthState.Green, tuple, periodicKey, forceLog, args);
		}

		public void LogYellowEvent(string key, ExEventLog.EventTuple tuple, string periodicKey, bool forceLog, params object[] args)
		{
			this.LogEvent(key, HealthState.Red, tuple, periodicKey, forceLog, args);
		}

		public void LogRedEvent(string key, ExEventLog.EventTuple tuple, string periodicKey, bool forceLog, params object[] args)
		{
			this.LogEvent(key, HealthState.Red, tuple, periodicKey, forceLog, args);
		}

		private void LogEvent(string key, HealthState newState, ExEventLog.EventTuple tuple, string periodicKey, bool forceLog, params object[] args)
		{
			bool flag = false;
			lock (StatefulEventLog.lockObj)
			{
				HealthState healthState;
				if (!this.healthstate.TryGetValue(key, out healthState))
				{
					this.healthstate.Add(key, newState);
					flag = true;
				}
				else
				{
					this.healthstate[key] = newState;
					flag = (healthState != newState);
				}
			}
			if (flag || forceLog)
			{
				UmGlobals.ExEvent.LogEvent(tuple, periodicKey, args);
			}
		}

		private static object lockObj = new object();

		private static StatefulEventLog instance = new StatefulEventLog();

		private Dictionary<string, HealthState> healthstate = new Dictionary<string, HealthState>();
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaExceptionEventManager
	{
		public OwaExceptionEventManager(int eventFrequency, int occurrenceThreshold)
		{
			this.eventFrequencyInSeconds = eventFrequency;
			this.occurrenceThreshold = occurrenceThreshold;
			this.ScheduleNextEvent();
		}

		public bool RegisterException(Exception e, string serverFqdn, out string[] exceptionSummary)
		{
			Exception innerException = (e.InnerException != null) ? e.InnerException : e;
			if (string.IsNullOrEmpty(serverFqdn))
			{
				serverFqdn = "Unknown";
			}
			serverFqdn = serverFqdn.ToUpperInvariant();
			bool result;
			lock (this.lockObject)
			{
				this.PrivateRegisterException(innerException, e, serverFqdn);
				if (this.ShouldLogEventSummary())
				{
					exceptionSummary = this.RetrieveEventSummary();
					result = true;
				}
				else
				{
					exceptionSummary = null;
					result = false;
				}
			}
			return result;
		}

		private bool ShouldLogEventSummary()
		{
			if (DateTime.UtcNow < this.nextEventTime)
			{
				return false;
			}
			if (this.totalExceptionCount < this.occurrenceThreshold)
			{
				this.ScheduleNextEvent();
				return false;
			}
			return true;
		}

		private void ScheduleNextEvent()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (this.nextEventTime == null || this.nextEventTime < utcNow)
			{
				this.totalExceptionCount = 0;
				this.exceptionInformation.Clear();
				this.serverFqdns.Clear();
				this.exceptionTimingSlots.Clear();
				this.exceptionKeys.Clear();
				this.nextEventTime = new DateTime?(DateTime.UtcNow.AddSeconds((double)this.eventFrequencyInSeconds));
			}
		}

		private string[] RetrieveEventSummary()
		{
			string[] result = this.GenerateEventSummary();
			this.ScheduleNextEvent();
			return result;
		}

		private void PrivateRegisterException(Exception innerException, Exception exceptionToLog, string serverFqdn)
		{
			Type type = innerException.GetType();
			string text = type.FullName;
			MapiRetryableException ex = innerException as MapiRetryableException;
			if (ex != null && ex.DiagCtx != null && ex.DiagCtx.Length > 0)
			{
				for (int i = 0; i < ex.DiagCtx.Length; i++)
				{
					if (ex.DiagCtx[i].Layout != DiagRecordLayout.dwParam)
					{
						text = text + "+" + ex.DiagCtx[i].Lid;
						break;
					}
				}
			}
			if (!this.exceptionKeys.Contains(text))
			{
				this.exceptionKeys.Add(text);
			}
			OwaExceptionEventManager.ExceptionInfo exceptionInfo;
			if (!this.exceptionInformation.TryGetValue(text, out exceptionInfo))
			{
				exceptionInfo = new OwaExceptionEventManager.ExceptionInfo();
				this.exceptionInformation.Add(text, exceptionInfo);
			}
			exceptionInfo.ExceptionObject = exceptionToLog;
			exceptionInfo.TotalCount++;
			if (exceptionInfo.PerServerCount.ContainsKey(serverFqdn))
			{
				Dictionary<string, int> perServerCount;
				(perServerCount = exceptionInfo.PerServerCount)[serverFqdn] = perServerCount[serverFqdn] + 1;
			}
			else
			{
				exceptionInfo.PerServerCount.Add(serverFqdn, 1);
			}
			if (this.serverFqdns.ContainsKey(serverFqdn))
			{
				SortedList<string, int> sortedList;
				(sortedList = this.serverFqdns)[serverFqdn] = sortedList[serverFqdn] + 1;
			}
			else
			{
				this.serverFqdns.Add(serverFqdn, 1);
			}
			this.totalExceptionCount++;
			DateTime utcNow = DateTime.UtcNow;
			OwaExceptionEventManager.ExceptionTimingInfo exceptionTimingInfo;
			if (this.exceptionTimingSlots.Count == 0 || utcNow > this.exceptionTimingSlots[this.exceptionTimingSlots.Count - 1].EndTime)
			{
				exceptionTimingInfo = new OwaExceptionEventManager.ExceptionTimingInfo();
				exceptionTimingInfo.StartTime = utcNow;
				exceptionTimingInfo.EndTime = utcNow.AddSeconds(60.0);
				exceptionTimingInfo.Count = 0;
				this.exceptionTimingSlots.Add(exceptionTimingInfo);
			}
			else
			{
				exceptionTimingInfo = this.exceptionTimingSlots[this.exceptionTimingSlots.Count - 1];
			}
			if (exceptionTimingInfo.HitsPerExceptionType.ContainsKey(text))
			{
				Dictionary<string, int> hitsPerExceptionType;
				string key;
				(hitsPerExceptionType = exceptionTimingInfo.HitsPerExceptionType)[key = text] = hitsPerExceptionType[key] + 1;
			}
			else
			{
				exceptionTimingInfo.HitsPerExceptionType.Add(text, 1);
			}
			exceptionTimingInfo.Count++;
		}

		private string[] GenerateEventSummary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("-----------------------------------------");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Exception\\Server");
			foreach (string value in this.serverFqdns.Keys)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(value);
			}
			stringBuilder.Append(",");
			stringBuilder.Append("Total");
			stringBuilder.Append(Environment.NewLine);
			foreach (string text in this.exceptionInformation.Keys)
			{
				OwaExceptionEventManager.ExceptionInfo exceptionInfo = this.exceptionInformation[text];
				stringBuilder.Append(text);
				foreach (string key in this.serverFqdns.Keys)
				{
					stringBuilder.Append(",");
					if (exceptionInfo.PerServerCount.ContainsKey(key))
					{
						stringBuilder.Append(exceptionInfo.PerServerCount[key]);
					}
					else
					{
						stringBuilder.Append(0);
					}
				}
				stringBuilder.Append(",");
				stringBuilder.Append(exceptionInfo.TotalCount);
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.Append("Total");
			foreach (string key2 in this.serverFqdns.Keys)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(this.serverFqdns[key2]);
			}
			stringBuilder.Append(",");
			stringBuilder.Append(this.totalExceptionCount);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("-----------------------------------------");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Time slot\\Exception type,Total");
			foreach (string value2 in this.exceptionKeys)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(value2);
			}
			stringBuilder.Append(",");
			stringBuilder.Append(Environment.NewLine);
			foreach (OwaExceptionEventManager.ExceptionTimingInfo exceptionTimingInfo in this.exceptionTimingSlots)
			{
				stringBuilder.AppendFormat("{0} - {1},{2}", exceptionTimingInfo.StartTime.ToString(), exceptionTimingInfo.EndTime.ToString(), exceptionTimingInfo.Count);
				foreach (string key3 in this.exceptionKeys)
				{
					stringBuilder.Append(",");
					if (exceptionTimingInfo.HitsPerExceptionType.ContainsKey(key3))
					{
						stringBuilder.Append(exceptionTimingInfo.HitsPerExceptionType[key3]);
					}
				}
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("-----------------------------------------");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			foreach (string text2 in this.exceptionInformation.Keys)
			{
				stringBuilder.Append(text2);
				stringBuilder.Append(":");
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this.exceptionInformation[text2].ExceptionObject.ToString());
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Environment.NewLine);
			}
			string text3 = stringBuilder.ToString();
			int num = (int)Math.Ceiling((double)text3.Length / 13000.0);
			if (num <= 1)
			{
				return new string[]
				{
					text3
				};
			}
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				if (i == num - 1)
				{
					array[i] = text3.Substring(i * 13000);
				}
				else
				{
					array[i] = text3.Substring(i * 13000, 13000);
				}
			}
			return array;
		}

		private const int ExceptionSlotsTimeIntervalInSeconds = 60;

		private int eventFrequencyInSeconds;

		private int occurrenceThreshold;

		private DateTime? nextEventTime;

		private int totalExceptionCount;

		private Dictionary<string, OwaExceptionEventManager.ExceptionInfo> exceptionInformation = new Dictionary<string, OwaExceptionEventManager.ExceptionInfo>();

		private SortedList<string, int> serverFqdns = new SortedList<string, int>();

		private List<string> exceptionKeys = new List<string>();

		private List<OwaExceptionEventManager.ExceptionTimingInfo> exceptionTimingSlots = new List<OwaExceptionEventManager.ExceptionTimingInfo>();

		private object lockObject = new object();

		private class ExceptionInfo
		{
			internal Exception ExceptionObject;

			internal int TotalCount;

			internal Dictionary<string, int> PerServerCount = new Dictionary<string, int>();
		}

		private class ExceptionTimingInfo
		{
			internal DateTime StartTime;

			internal DateTime EndTime;

			internal int Count;

			internal Dictionary<string, int> HitsPerExceptionType = new Dictionary<string, int>();
		}
	}
}

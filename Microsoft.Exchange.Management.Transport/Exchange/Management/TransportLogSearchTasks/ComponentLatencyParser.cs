using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	internal sealed class ComponentLatencyParser : LatencyParser
	{
		public ComponentLatencyParser() : base(ExTraceGlobals.TaskTracer)
		{
			this.currentServerName = string.Empty;
			this.componentSequenceNumber = 0;
		}

		public bool TryParse(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			DateTime dateTime;
			int num;
			ComponentLatencyParser.TryParseOriginalArrivalTime(s, out dateTime, out num);
			return num < s.Length && base.TryParse(s, num, s.Length - num);
		}

		public IEnumerable<LatencyComponent> Components
		{
			get
			{
				return this.components ?? ComponentLatencyParser.EmptyComponentArray;
			}
		}

		protected override bool HandleLocalServerFqdn(string s, int startIndex, int count)
		{
			this.currentServerName = s.Substring(startIndex, count);
			this.componentSequenceNumber = 0;
			return true;
		}

		protected override bool HandleServerFqdn(string s, int startIndex, int count)
		{
			this.currentServerName = s.Substring(startIndex, count);
			this.componentSequenceNumber = 0;
			return true;
		}

		protected override bool HandleTotalLatency(string s, int startIndex, int count)
		{
			ushort latency;
			if (LatencyParser.TryParseLatency(s, startIndex, count, out latency))
			{
				this.AddComponent(this.currentServerName, "TOTAL", latency);
			}
			return true;
		}

		protected override bool HandleComponentLatency(string s, int componentNameStart, int componentNameLength, int latencyStart, int latencyLength)
		{
			ushort latency;
			if (LatencyParser.TryParseLatency(s, latencyStart, latencyLength, out latency))
			{
				this.AddComponent(this.currentServerName, s.Substring(componentNameStart, componentNameLength), latency);
			}
			return true;
		}

		internal static bool TryParseOriginalArrivalTime(string s, out DateTime originalArrivalTime, out int latencyInfoStartIndex)
		{
			int num = s.IndexOf(';');
			if (num < 0)
			{
				DateTime dateTime;
				if (LatencyParser.TryParseDateTime(s, 0, s.Length, out dateTime))
				{
					originalArrivalTime = dateTime;
					latencyInfoStartIndex = s.Length;
					return true;
				}
				originalArrivalTime = DateTime.MinValue;
				latencyInfoStartIndex = 0;
				return false;
			}
			else
			{
				DateTime dateTime2;
				if (LatencyParser.TryParseDateTime(s, 0, num, out dateTime2))
				{
					originalArrivalTime = dateTime2;
					latencyInfoStartIndex = num + 1;
					return true;
				}
				originalArrivalTime = DateTime.MinValue;
				latencyInfoStartIndex = 0;
				return false;
			}
		}

		private void AddComponent(string serverName, string code, ushort latency)
		{
			if (this.components == null)
			{
				this.components = new LinkedList<LatencyComponent>();
			}
			LocalizedString fullName = LatencyTracker.GetFullName(code);
			if (LocalizedString.Empty.Equals(fullName))
			{
				fullName = new LocalizedString(code);
			}
			LatencyComponent value = new LatencyComponent(serverName, code, fullName, latency, this.componentSequenceNumber++);
			this.components.AddLast(value);
		}

		private const string TotalComponentCode = "TOTAL";

		private static readonly IEnumerable<LatencyComponent> EmptyComponentArray = new LatencyComponent[0];

		private LinkedList<LatencyComponent> components;

		private string currentServerName;

		private int componentSequenceNumber;
	}
}

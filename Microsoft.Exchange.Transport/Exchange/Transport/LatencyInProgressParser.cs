using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class LatencyInProgressParser : LatencyParser
	{
		public LatencyInProgressParser() : base(ExTraceGlobals.SmtpReceiveTracer)
		{
		}

		public DateTime PreDeliveryTime
		{
			get
			{
				return this.preDeliveryTime;
			}
		}

		public int PreDeliveryTimeSeparatorIndex
		{
			get
			{
				return this.preDeliveryTimeSeparatorIndex;
			}
		}

		public int ServerFqdnStartIndex
		{
			get
			{
				return this.serverFqdnStartIndex;
			}
		}

		public int ServerFqdnLength
		{
			get
			{
				return this.serverFqdnLength;
			}
		}

		public int ComponentsStartIndex
		{
			get
			{
				return this.componentsStartIndex;
			}
		}

		public int ComponentsLength
		{
			get
			{
				return this.componentsLength;
			}
		}

		public float TotalSeconds
		{
			get
			{
				return this.totalSeconds;
			}
		}

		public int PendingStartIndex
		{
			get
			{
				return this.pendingStartIndex;
			}
		}

		public string PreviousHopProcess
		{
			get
			{
				return this.previousHopProcess;
			}
			private set
			{
				this.previousHopProcess = value;
			}
		}

		public bool TryParse(string s)
		{
			if (!this.TryParsePreDeliveryTime(s))
			{
				return false;
			}
			this.serverFqdnStartIndex = -1;
			this.serverFqdnLength = -1;
			this.componentsStartIndex = -1;
			this.componentsLength = -1;
			this.totalSeconds = 65535f;
			this.pendingStartIndex = -1;
			return base.TryParse(s, 0, this.preDeliveryTimeSeparatorIndex);
		}

		protected override bool HandleLocalServerFqdn(string s, int startIndex, int count)
		{
			this.serverFqdnStartIndex = startIndex;
			this.serverFqdnLength = count;
			base.Tracer.TraceDebug<int, int, string>(0L, "LatencyInProgress Parser: found server FQDN at position {0} (length={1}) in string '{2}'", this.serverFqdnStartIndex, this.serverFqdnLength, base.StringToParse);
			return true;
		}

		protected override bool HandleServerFqdn(string s, int startIndex, int count)
		{
			base.Tracer.TraceError<int, int, string>(0L, "LatencyInProgress Parser: non-local server FQDN found at position {0} (length={1}) in string '{2}'", startIndex, count, base.StringToParse);
			return false;
		}

		protected override bool HandleComponentLatency(string s, int componentNameStart, int componentNameLength, int latencyStart, int latencyLength)
		{
			if (this.componentsStartIndex == -1)
			{
				this.componentsStartIndex = componentNameStart;
				base.Tracer.TraceDebug<int, string>(0L, "LatencyInProgress Parser: components start at position {0} in string '{1}'", this.componentsStartIndex, base.StringToParse);
			}
			this.componentsLength = latencyStart + latencyLength - this.componentsStartIndex;
			return true;
		}

		protected override bool HandleTotalLatency(string s, int startIndex, int count)
		{
			if (!LatencyParser.TryParseLatency(s, startIndex, count, out this.totalSeconds))
			{
				base.Tracer.TraceError<int, int, string>(0L, "LatencyInProgress Parser: invalid TOTAL value at position {0} (length={1}) in string '{2}'", startIndex, count, base.StringToParse);
				return false;
			}
			base.Tracer.TraceDebug<float, string>(0L, "LatencyInProgress Parser: found TOTAL value {0} in string '{1}'", this.totalSeconds, base.StringToParse);
			return true;
		}

		protected override void HandleTotalComponent(string s, int startIndex, int count)
		{
			string text = s.Substring(startIndex, count);
			string[] array = text.Split(new char[]
			{
				'-'
			});
			if (array.Length == 2)
			{
				this.PreviousHopProcess = array[1];
			}
		}

		protected override bool HandlePendingComponent(string s, int startIndex, int count)
		{
			if (this.pendingStartIndex == -1)
			{
				this.pendingStartIndex = startIndex;
				base.Tracer.TraceDebug<int, string>(0L, "LatencyInProgress Parser: pending components start at position {0} in string '{1}'", this.pendingStartIndex, base.StringToParse);
			}
			return true;
		}

		private bool TryParsePreDeliveryTime(string s)
		{
			this.preDeliveryTime = DateTime.MinValue;
			this.preDeliveryTimeSeparatorIndex = -1;
			this.preDeliveryTimeSeparatorIndex = s.LastIndexOf(';');
			if (this.preDeliveryTimeSeparatorIndex == -1)
			{
				base.Tracer.TraceError<string>(0L, "LatencyInProgress Parser: Cannot find the timestamp field in string '{0}'", s);
				return false;
			}
			int num = LatencyParser.SkipWhitespaces(s, this.preDeliveryTimeSeparatorIndex + 1, s.Length - this.preDeliveryTimeSeparatorIndex - 1);
			if (num == -1)
			{
				base.Tracer.TraceError<string>(0L, "LatencyInProgress Parser: Cannot find the timestamp value in string '{0}'", s);
				return false;
			}
			if (!LatencyParser.TryParseDateTime(s, num, s.Length - num, out this.preDeliveryTime))
			{
				base.Tracer.TraceError<int, string>(0L, "LatencyInProgress Parser: Failed to parse the timestamp value at position {0} in string '{1}'", num, s);
				return false;
			}
			return true;
		}

		private DateTime preDeliveryTime;

		private int preDeliveryTimeSeparatorIndex;

		private int serverFqdnStartIndex;

		private int serverFqdnLength;

		private int componentsStartIndex;

		private int componentsLength;

		private float totalSeconds;

		private int pendingStartIndex;

		private string previousHopProcess = string.Empty;
	}
}

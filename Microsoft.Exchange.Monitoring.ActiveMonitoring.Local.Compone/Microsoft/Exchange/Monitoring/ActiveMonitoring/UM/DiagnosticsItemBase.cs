using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemBase
	{
		public int ErrorId { get; internal set; }

		public string Source
		{
			get
			{
				return this.GetValue("source");
			}
		}

		public string Reason
		{
			get
			{
				return this.GetValue("reason");
			}
		}

		public DateTime LocalTime { get; protected set; }

		public bool IsValid
		{
			get
			{
				return this.ErrorId != -1;
			}
		}

		internal bool ContainsKey(string key)
		{
			return this.data.ContainsKey(key);
		}

		internal void Add(string key, string value)
		{
			if (this.data.ContainsKey(key))
			{
				this.data[key] = value;
				return;
			}
			this.data.Add(key, value);
		}

		public string GetValue(string key)
		{
			if (this.data.ContainsKey(key))
			{
				return this.data[key];
			}
			return string.Empty;
		}

		public string this[string key]
		{
			get
			{
				return this.GetValue(key);
			}
		}

		protected DiagnosticsItemBase()
		{
			this.LocalTime = DateTime.UtcNow;
			this.ErrorId = -1;
			this.data = new Dictionary<string, string>(3);
		}

		public const int InvalidErrorId = -1;

		private Dictionary<string, string> data;
	}
}

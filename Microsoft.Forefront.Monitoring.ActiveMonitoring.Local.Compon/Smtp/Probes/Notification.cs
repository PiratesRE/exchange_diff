using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class Notification
	{
		internal string Category { get; set; }

		internal string Type { get; set; }

		internal string Value { get; set; }

		internal bool Mandatory { get; set; }

		internal bool MatchExpected { get; set; }

		internal MatchType Method { get; set; }

		public override string ToString()
		{
			return string.Format("Type={0}, Value={1}, Method={2}, Category={3}, Mandatory={4}, MatchExpected={5}", new object[]
			{
				this.Type,
				this.Value,
				this.Method,
				this.Category,
				this.Mandatory,
				this.MatchExpected
			});
		}
	}
}

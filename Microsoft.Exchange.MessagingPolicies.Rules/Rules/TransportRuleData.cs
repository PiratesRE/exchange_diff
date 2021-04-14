using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class TransportRuleData
	{
		public TransportRuleData(TransportRule rule)
		{
			this.xml = rule.Xml;
			this.priority = rule.Priority;
			this.name = rule.Name;
			this.ImmutableId = rule.ImmutableId;
			this.whenChangedUTC = rule.WhenChangedUTCCopy;
		}

		public string Xml
		{
			get
			{
				return this.xml;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return this.whenChangedUTC;
			}
			set
			{
				this.whenChangedUTC = value;
			}
		}

		public Guid ImmutableId { get; private set; }

		public long ItemSize
		{
			get
			{
				long num = 0L;
				num += 4L;
				num += 8L;
				num += 16L;
				if (this.xml != null)
				{
					num += (long)(this.xml.Length * 2);
				}
				if (this.name != null)
				{
					num += (long)(this.name.Length * 2);
				}
				return num + 36L;
			}
		}

		internal const int FixedObjectOverhead = 18;

		private string xml;

		private string name;

		private int priority;

		private DateTime? whenChangedUTC;
	}
}

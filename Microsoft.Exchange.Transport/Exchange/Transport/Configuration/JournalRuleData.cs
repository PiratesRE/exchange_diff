using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class JournalRuleData
	{
		public JournalRuleData(TransportRule rule)
		{
			this.immutableId = rule.ImmutableId;
			this.xml = rule.Xml;
			this.priority = rule.Priority;
			this.name = rule.Name;
		}

		public Guid ImmutableId
		{
			get
			{
				return this.immutableId;
			}
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

		public long ItemSize
		{
			get
			{
				long num = 0L;
				num += 4L;
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

		private readonly Guid immutableId;

		private string xml;

		private string name;

		private int priority;
	}
}

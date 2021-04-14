using System;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class AgentRecord : ICloneable
	{
		public AgentRecord(string id, string name, string type, int sequenceNumber, bool isInternal)
		{
			this.id = id;
			this.isInternal = isInternal;
			this.name = name;
			this.type = type;
			this.sequenceNumber = sequenceNumber;
		}

		public bool IsInternal
		{
			get
			{
				return this.isInternal;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public int SequenceNumber
		{
			get
			{
				return this.sequenceNumber;
			}
		}

		public Agent Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public AgentRecord Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		private readonly string id;

		private readonly string name;

		private readonly string type;

		private readonly int sequenceNumber;

		private readonly bool isInternal;

		private Agent instance;

		private bool enabled;

		private AgentRecord next;
	}
}

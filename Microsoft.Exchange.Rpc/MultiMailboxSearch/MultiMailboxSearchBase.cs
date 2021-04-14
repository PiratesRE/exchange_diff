using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal abstract class MultiMailboxSearchBase
	{
		internal int Version
		{
			get
			{
				return this.version;
			}
		}

		internal Guid CorrelationId
		{
			get
			{
				return this.queryCorrelationId;
			}
			set
			{
				this.queryCorrelationId = value;
			}
		}

		protected MultiMailboxSearchBase()
		{
			this.version = MultiMailboxSearchBase.CurrentVersion;
		}

		protected MultiMailboxSearchBase(int version)
		{
			this.version = version;
		}

		private readonly int version;

		private Guid queryCorrelationId;

		protected static int CurrentVersion = 1;
	}
}

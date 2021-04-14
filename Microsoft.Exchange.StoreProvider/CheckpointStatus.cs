using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CheckpointStatus
	{
		internal CheckpointStatus(ref CHECKPOINTSTATUSRAW pCheckpointStatus)
		{
			this.guidMdb = pCheckpointStatus.guidMdb;
			this.ulBeginCheckpointDepth = pCheckpointStatus.ulBeginCheckpointDepth;
			this.ulEndCheckpointDepth = pCheckpointStatus.ulEndCheckpointDepth;
		}

		internal CheckpointStatus(Guid _guidMdb, uint _ulBeginCheckpointDepth, uint _ulEndCheckpointDepth)
		{
			this.guidMdb = _guidMdb;
			this.ulBeginCheckpointDepth = _ulBeginCheckpointDepth;
			this.ulEndCheckpointDepth = _ulEndCheckpointDepth;
		}

		public Guid MdbGuid
		{
			get
			{
				return this.guidMdb;
			}
		}

		public uint BeginCheckpointDepth
		{
			get
			{
				return this.ulBeginCheckpointDepth;
			}
		}

		public uint EndCheckpointDepth
		{
			get
			{
				return this.ulEndCheckpointDepth;
			}
		}

		public override string ToString()
		{
			return string.Format("guidMdb {0} : ", this.guidMdb) + string.Format("ulBeginCheckpointDepth {0}, ", this.ulBeginCheckpointDepth) + string.Format("ulEndCheckpointDepth {0} ", this.ulEndCheckpointDepth);
		}

		private Guid guidMdb;

		private uint ulBeginCheckpointDepth;

		private uint ulEndCheckpointDepth;
	}
}

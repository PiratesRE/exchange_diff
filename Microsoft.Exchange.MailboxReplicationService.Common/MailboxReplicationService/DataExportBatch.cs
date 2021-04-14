using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class DataExportBatch
	{
		[DataMember(EmitDefaultValue = false)]
		public int Opcode
		{
			get
			{
				return this.opcode;
			}
			set
			{
				this.opcode = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public byte[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool FlushAfterImport
		{
			get
			{
				return this.flushAfterImport;
			}
			set
			{
				this.flushAfterImport = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool IsLastBatch
		{
			get
			{
				return this.isLastBatch;
			}
			set
			{
				this.isLastBatch = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public long DataExportHandle
		{
			get
			{
				return this.dataExportHandle;
			}
			set
			{
				this.dataExportHandle = value;
			}
		}

		private int opcode;

		private byte[] data;

		private bool flushAfterImport;

		private bool isLastBatch;

		private long dataExportHandle;
	}
}

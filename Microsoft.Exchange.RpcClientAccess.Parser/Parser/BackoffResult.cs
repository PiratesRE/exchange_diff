using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BackoffResult : Result
	{
		internal BackoffResult(byte logonId, uint duration, BackoffRopData[] ropData, byte[] additionalData, Encoding string8Encoding) : base(RopId.Backoff)
		{
			if (ropData == null)
			{
				throw new ArgumentNullException("ropData");
			}
			if (ropData.Length > 255)
			{
				throw new ArgumentException("Cannot contain more than 255 entries", "ropData");
			}
			this.logonId = logonId;
			this.duration = duration;
			this.ropData = ropData;
			this.additionalData = additionalData;
			base.String8Encoding = string8Encoding;
		}

		internal BackoffResult(Reader reader) : base(reader)
		{
			this.logonId = reader.ReadByte();
			this.duration = reader.ReadUInt32();
			byte b = reader.ReadByte();
			this.ropData = new BackoffRopData[(int)b];
			for (int i = 0; i < (int)b; i++)
			{
				this.ropData[i] = new BackoffRopData(reader);
			}
			this.additionalData = reader.ReadSizeAndByteArray();
		}

		public byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		public uint Duration
		{
			get
			{
				return this.duration;
			}
		}

		public BackoffRopData[] RopData
		{
			get
			{
				return this.ropData;
			}
		}

		public byte[] AdditionalData
		{
			get
			{
				return this.additionalData;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(this.LogonId);
			writer.WriteUInt32(this.Duration);
			writer.WriteByte((byte)this.ropData.Length);
			foreach (BackoffRopData backoffRopData in this.RopData)
			{
				backoffRopData.Serialize(writer);
			}
			writer.WriteSizedBytes(this.AdditionalData);
		}

		private readonly byte logonId;

		private readonly uint duration;

		private readonly BackoffRopData[] ropData;

		private readonly byte[] additionalData;
	}
}

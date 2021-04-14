using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfProcessInfoAuxiliaryBlock : AuxiliaryBlock
	{
		internal PerfProcessInfoAuxiliaryBlock(byte blockVersion, ushort blockSessionId, Guid blockProcessGuid, string blockProcessName) : base(blockVersion, AuxiliaryBlockTypes.PerfProcessInfo)
		{
			if (blockVersion != 1 && blockVersion != 2)
			{
				throw new ArgumentException("Version must 1 or 2", "blockVersion");
			}
			this.processId = blockSessionId;
			this.processGuid = blockProcessGuid;
			this.processName = blockProcessName;
		}

		internal PerfProcessInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.processId = reader.ReadUInt16();
			reader.ReadUInt16();
			this.processGuid = reader.ReadGuid();
			ushort offset = reader.ReadUInt16();
			this.processName = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset);
		}

		public string ProcessName
		{
			get
			{
				return this.processName;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.processId);
			writer.WriteUInt16(0);
			writer.WriteGuid(this.processGuid);
			long position = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16(0);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.processName, position);
		}

		private readonly ushort processId;

		private readonly Guid processGuid;

		private readonly string processName;
	}
}

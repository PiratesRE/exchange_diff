using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ClientActivityAuxiliaryBlock : AuxiliaryBlock
	{
		public ClientActivityAuxiliaryBlock(Guid activityId, uint testCaseId, string componentName, string protocolName, string actionString) : base(1, AuxiliaryBlockTypes.ClientActivity)
		{
			this.activityId = activityId;
			this.testCaseId = testCaseId;
			this.componentName = componentName;
			this.protocolName = protocolName;
			this.actionString = actionString;
		}

		internal ClientActivityAuxiliaryBlock(Reader reader) : base(reader)
		{
			if (base.Version >= 1)
			{
				this.activityId = reader.ReadGuid();
				this.testCaseId = reader.ReadUInt32();
				ushort offset = reader.ReadUInt16();
				ushort offset2 = reader.ReadUInt16();
				ushort offset3 = reader.ReadUInt16();
				reader.ReadUInt16();
				this.componentName = AuxiliaryBlock.ReadAsciiStringAtPosition(reader, offset);
				this.protocolName = AuxiliaryBlock.ReadAsciiStringAtPosition(reader, offset2);
				this.actionString = AuxiliaryBlock.ReadAsciiStringAtPosition(reader, offset3);
			}
		}

		public Guid ActivityId
		{
			get
			{
				return this.activityId;
			}
		}

		public uint TestCaseId
		{
			get
			{
				return this.testCaseId;
			}
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public string ProtocolName
		{
			get
			{
				return this.protocolName;
			}
		}

		public string ActionString
		{
			get
			{
				return this.actionString;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteGuid(this.ActivityId);
			writer.WriteUInt32(this.TestCaseId);
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			writer.WriteUInt16(0);
			long position3 = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16(0);
			AuxiliaryBlock.WriteAsciiStringAndUpdateOffset(writer, this.ComponentName, position);
			AuxiliaryBlock.WriteAsciiStringAndUpdateOffset(writer, this.ProtocolName, position2);
			AuxiliaryBlock.WriteAsciiStringAndUpdateOffset(writer, this.ActionString, position3);
		}

		private const byte BlockVersion1 = 1;

		private const byte CurrentBlockVersion = 1;

		private readonly Guid activityId;

		private readonly uint testCaseId;

		private readonly string componentName;

		private readonly string protocolName;

		private readonly string actionString;
	}
}

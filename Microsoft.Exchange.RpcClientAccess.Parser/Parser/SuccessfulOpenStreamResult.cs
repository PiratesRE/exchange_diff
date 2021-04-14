using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SuccessfulOpenStreamResult : RopResult
	{
		internal SuccessfulOpenStreamResult(IServerObject serverObject, uint streamSize) : base(RopId.OpenStream, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			this.streamSize = streamSize;
		}

		internal SuccessfulOpenStreamResult(Reader reader) : base(reader)
		{
			this.streamSize = reader.ReadUInt32();
		}

		internal static SuccessfulOpenStreamResult Parse(Reader reader)
		{
			return new SuccessfulOpenStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.streamSize);
		}

		public uint StreamSize
		{
			get
			{
				return this.streamSize;
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Size=0x").Append(this.StreamSize.ToString("X"));
		}

		private readonly uint streamSize;
	}
}

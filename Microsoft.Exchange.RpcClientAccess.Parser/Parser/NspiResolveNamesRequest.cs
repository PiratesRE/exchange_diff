using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiResolveNamesRequest : MapiHttpRequest
	{
		public NspiResolveNamesRequest(NspiResolveNamesFlags flags, NspiState state, PropertyTag[] columns, string[] names, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.columns = columns;
			this.names = names;
		}

		public NspiResolveNamesRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiResolveNamesFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			this.names = reader.ReadNullableCountAndUnicodeStringList(StringFlags.IncludeNull, FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiResolveNamesFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		public string[] Names
		{
			get
			{
				return this.names;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			writer.WriteNullableCountAndUnicodeStringList(this.names, StringFlags.IncludeNull, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiResolveNamesFlags flags;

		private readonly NspiState state;

		private readonly PropertyTag[] columns;

		private readonly string[] names;
	}
}

using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulAddressTypesResult : RopResult
	{
		internal SuccessfulAddressTypesResult(string[] addressTypes) : base(RopId.AddressTypes, ErrorCode.None, null)
		{
			if (addressTypes == null)
			{
				throw new ArgumentNullException("addressTypes");
			}
			this.addressTypes = addressTypes;
		}

		internal SuccessfulAddressTypesResult(Reader reader) : base(reader)
		{
			ushort num = reader.ReadUInt16();
			ushort estimateCount = reader.ReadUInt16();
			reader.CheckBoundary((uint)num, 1U);
			reader.CheckBoundary((uint)estimateCount, 1U);
			this.addressTypes = new string[(int)num];
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				this.addressTypes[(int)num2] = reader.ReadAsciiString(StringFlags.IncludeNull);
			}
		}

		internal string[] AddressTypes
		{
			get
			{
				return this.addressTypes;
			}
		}

		internal static SuccessfulAddressTypesResult Parse(Reader reader)
		{
			return new SuccessfulAddressTypesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16((ushort)this.addressTypes.Length);
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			foreach (string value in this.addressTypes)
			{
				writer.WriteAsciiString(value, StringFlags.IncludeNull);
			}
			long position3 = writer.Position;
			writer.Position = position;
			writer.WriteUInt16((ushort)(position3 - position2));
			writer.Position = position3;
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Types=[");
			Util.AppendToString<string>(stringBuilder, this.addressTypes);
			stringBuilder.Append("]");
		}

		private string[] addressTypes;
	}
}

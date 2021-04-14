using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetOptionsDataResult : RopResult
	{
		internal SuccessfulGetOptionsDataResult(byte[] optionsInfo, byte[] helpFileData, string helpFileName) : base(RopId.GetOptionsData, ErrorCode.None, null)
		{
			if (optionsInfo == null)
			{
				throw new ArgumentNullException("optionsInfo");
			}
			if (helpFileData == null)
			{
				throw new ArgumentNullException("helpFileData");
			}
			this.optionsInfo = optionsInfo;
			this.helpFileData = helpFileData;
			this.helpFileName = helpFileName;
		}

		internal SuccessfulGetOptionsDataResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			reader.ReadByte();
			this.optionsInfo = reader.ReadSizeAndByteArray();
			this.helpFileData = reader.ReadSizeAndByteArray();
			if (this.helpFileData.Length > 0)
			{
				this.helpFileName = reader.ReadString8(string8Encoding, StringFlags.IncludeNull);
			}
		}

		internal byte[] OptionsInfo
		{
			get
			{
				return this.optionsInfo;
			}
		}

		internal byte[] HelpFileData
		{
			get
			{
				return this.helpFileData;
			}
		}

		internal string HelpFileName
		{
			get
			{
				return this.helpFileName;
			}
		}

		internal static SuccessfulGetOptionsDataResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulGetOptionsDataResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(1);
			writer.WriteSizedBytes(this.optionsInfo);
			writer.WriteSizedBytes(this.helpFileData);
			if (!string.IsNullOrEmpty(this.helpFileName))
			{
				writer.WriteString8(this.helpFileName, base.String8Encoding, StringFlags.IncludeNull);
			}
		}

		private byte[] optionsInfo;

		private byte[] helpFileData;

		private string helpFileName;
	}
}

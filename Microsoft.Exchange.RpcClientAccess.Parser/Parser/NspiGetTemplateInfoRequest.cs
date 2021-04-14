using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetTemplateInfoRequest : MapiHttpRequest
	{
		public NspiGetTemplateInfoRequest(NspiGetTemplateInfoFlags flags, uint displayType, string templateDn, uint codePage, uint localeId, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.displayType = displayType;
			this.templateDn = templateDn;
			this.codePage = codePage;
			this.localeId = localeId;
		}

		public NspiGetTemplateInfoRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiGetTemplateInfoFlags)reader.ReadUInt32();
			this.displayType = reader.ReadUInt32();
			this.templateDn = reader.ReadNullableAsciiString(StringFlags.IncludeNull);
			this.codePage = reader.ReadUInt32();
			this.localeId = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiGetTemplateInfoFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public uint DisplayType
		{
			get
			{
				return this.displayType;
			}
		}

		public string TemplateDn
		{
			get
			{
				return this.templateDn;
			}
		}

		public uint CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public uint LocaleId
		{
			get
			{
				return this.localeId;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUInt32(this.displayType);
			writer.WriteNullableAsciiString(this.templateDn);
			writer.WriteUInt32(this.codePage);
			writer.WriteUInt32(this.localeId);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiGetTemplateInfoFlags flags;

		private readonly uint displayType;

		private readonly string templateDn;

		private readonly uint codePage;

		private readonly uint localeId;
	}
}

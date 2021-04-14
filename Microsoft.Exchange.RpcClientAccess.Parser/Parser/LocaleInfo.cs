using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct LocaleInfo
	{
		public LocaleInfo(int stringLocaleId, int sortLocaleId, int codePageId)
		{
			this.stringLocaleId = stringLocaleId;
			this.sortLocaleId = sortLocaleId;
			this.codePageId = codePageId;
		}

		public int StringLocaleId
		{
			get
			{
				return this.stringLocaleId;
			}
		}

		public int SortLocaleId
		{
			get
			{
				return this.sortLocaleId;
			}
		}

		public int CodePageId
		{
			get
			{
				return this.codePageId;
			}
		}

		public static LocaleInfo Parse(Reader reader)
		{
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			int num3 = reader.ReadInt32();
			return new LocaleInfo(num, num2, num3);
		}

		public override string ToString()
		{
			return "LocaleInfo: " + this.ToBareString();
		}

		public string ToBareString()
		{
			return string.Format("String[{0}] Sort[{1}] CodePage[{2}]", this.sortLocaleId, this.stringLocaleId, this.codePageId);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteInt32(this.stringLocaleId);
			writer.WriteInt32(this.sortLocaleId);
			writer.WriteInt32(this.codePageId);
		}

		private readonly int stringLocaleId;

		private readonly int sortLocaleId;

		private readonly int codePageId;
	}
}

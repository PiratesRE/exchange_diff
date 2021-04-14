using System;

namespace Microsoft.Exchange.Data.Mime
{
	public struct DecodingResults
	{
		public string CharsetName
		{
			get
			{
				return this.charsetName;
			}
			internal set
			{
				this.charsetName = value;
			}
		}

		public string CultureName
		{
			get
			{
				return this.cultureName;
			}
			internal set
			{
				this.cultureName = value;
			}
		}

		public EncodingScheme EncodingScheme
		{
			get
			{
				return this.encodingScheme;
			}
			internal set
			{
				this.encodingScheme = value;
			}
		}

		public bool DecodingFailed
		{
			get
			{
				return this.decodingFailed;
			}
			internal set
			{
				this.decodingFailed = value;
			}
		}

		private string charsetName;

		private string cultureName;

		private EncodingScheme encodingScheme;

		private bool decodingFailed;
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class OrganizationContentConversionProperties
	{
		public int PreferredInternetCodePageForShiftJis
		{
			get
			{
				return this.preferredInternetCodePageForShiftJis;
			}
			internal set
			{
				this.preferredInternetCodePageForShiftJis = value;
			}
		}

		public int RequiredCharsetCoverage
		{
			get
			{
				return this.requiredCharsetCoverage;
			}
			internal set
			{
				this.requiredCharsetCoverage = value;
			}
		}

		public int ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return this.byteEncoderTypeFor7BitCharsets;
			}
			internal set
			{
				this.byteEncoderTypeFor7BitCharsets = value;
			}
		}

		public bool ValidOrganization
		{
			get
			{
				return this.validOrganization;
			}
			internal set
			{
				this.validOrganization = value;
			}
		}

		private int preferredInternetCodePageForShiftJis;

		private int requiredCharsetCoverage;

		private int byteEncoderTypeFor7BitCharsets;

		private bool validOrganization = true;
	}
}

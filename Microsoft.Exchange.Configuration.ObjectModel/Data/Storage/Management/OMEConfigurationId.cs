using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class OMEConfigurationId : ObjectId
	{
		internal byte[] Image { get; private set; }

		internal string EmailText { get; private set; }

		internal string PortalText { get; private set; }

		internal string DisclaimerText { get; private set; }

		internal bool OTPEnabled { get; private set; }

		internal OMEConfigurationId(byte[] image, string emailText, string portalText, string disclaimerText, bool otpEnabled)
		{
			this.Image = image;
			this.EmailText = emailText;
			this.PortalText = portalText;
			this.DisclaimerText = disclaimerText;
			this.OTPEnabled = otpEnabled;
		}

		public override byte[] GetBytes()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.EmailText);
			stringBuilder.Append(this.PortalText);
			stringBuilder.Append(this.DisclaimerText);
			stringBuilder.Append(this.OTPEnabled);
			List<byte> list = new List<byte>();
			list.AddRange(this.Image);
			list.AddRange(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			return list.ToArray();
		}

		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.EmailText);
			stringBuilder.Append(this.PortalText);
			stringBuilder.Append(this.DisclaimerText);
			stringBuilder.Append(this.OTPEnabled);
			return stringBuilder.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			OMEConfigurationId omeconfigurationId = obj as OMEConfigurationId;
			return omeconfigurationId != null && (this.ByteArrayEqual(this.Image, omeconfigurationId.Image) && string.Equals(this.EmailText, omeconfigurationId.EmailText, StringComparison.Ordinal) && string.Equals(this.PortalText, omeconfigurationId.PortalText, StringComparison.Ordinal) && string.Equals(this.DisclaimerText, omeconfigurationId.DisclaimerText, StringComparison.Ordinal)) && this.OTPEnabled == omeconfigurationId.OTPEnabled;
		}

		public override string ToString()
		{
			return "OME Configuration";
		}

		private bool ByteArrayEqual(byte[] one, byte[] two)
		{
			byte[] first = (one == null) ? new byte[0] : one;
			byte[] second = (two == null) ? new byte[0] : two;
			return first.SequenceEqual(second);
		}

		internal const string OMEConfiguration = "OME Configuration";
	}
}

using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.ApplicationLogic.E4E
{
	[Serializable]
	public class EncryptionConfigurationData
	{
		[XmlElement]
		public string ImageBase64 { get; set; }

		[XmlElement]
		public string EmailText { get; set; }

		[XmlElement]
		public string PortalText { get; set; }

		[XmlElement]
		public string DisclaimerText { get; set; }

		[XmlElement]
		public bool OTPEnabled { get; set; }

		public EncryptionConfigurationData()
		{
			this.ImageBase64 = string.Empty;
			this.EmailText = string.Empty;
			this.PortalText = string.Empty;
			this.DisclaimerText = string.Empty;
			this.OTPEnabled = true;
		}

		public EncryptionConfigurationData(string imageBase64, string emailText, string portalText, string disclaimerText, bool otpEnabled)
		{
			this.ImageBase64 = imageBase64;
			this.EmailText = emailText;
			this.PortalText = portalText;
			this.DisclaimerText = disclaimerText;
			this.OTPEnabled = otpEnabled;
			this.OTPEnabled = true;
		}

		internal string Serialize()
		{
			if (this.ImageBase64 == null)
			{
				this.ImageBase64 = string.Empty;
			}
			if (this.EmailText == null)
			{
				this.EmailText = string.Empty;
			}
			if (this.PortalText == null)
			{
				this.PortalText = string.Empty;
			}
			if (this.DisclaimerText == null)
			{
				this.DisclaimerText = string.Empty;
			}
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(base.GetType());
				safeXmlSerializer.Serialize(stringWriter, this);
				stringWriter.Flush();
				result = stringWriter.ToString();
			}
			return result;
		}

		internal static EncryptionConfigurationData Deserialize(string serializedXML)
		{
			EncryptionConfigurationData result;
			using (StringReader stringReader = new StringReader(serializedXML))
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(EncryptionConfigurationData));
				EncryptionConfigurationData encryptionConfigurationData = (EncryptionConfigurationData)safeXmlSerializer.Deserialize(stringReader);
				result = encryptionConfigurationData;
			}
			return result;
		}
	}
}

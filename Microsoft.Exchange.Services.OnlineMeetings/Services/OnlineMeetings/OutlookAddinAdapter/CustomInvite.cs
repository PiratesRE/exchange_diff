using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[DataContract(Name = "CustomInvite")]
	[XmlType("CustomInvite")]
	[KnownType(typeof(CustomInvite))]
	public class CustomInvite
	{
		[DataMember(Name = "help-url", EmitDefaultValue = false)]
		[XmlElement("help-url")]
		public string HelpUrl { get; set; }

		[XmlElement("logo-url")]
		[DataMember(Name = "logo-url", EmitDefaultValue = false)]
		public string LogoUrl { get; set; }

		[XmlElement("legal-url")]
		[DataMember(Name = "legal-url", EmitDefaultValue = false)]
		public string LegalUrl { get; set; }

		[XmlElement("footer-text")]
		[DataMember(Name = "footer-text", EmitDefaultValue = false)]
		public string FooterText { get; set; }

		internal static CustomInvite ConvertFrom(CustomizationValues customizationValues)
		{
			if (customizationValues != null && (!string.IsNullOrEmpty(customizationValues.InvitationHelpUrl) || !string.IsNullOrEmpty(customizationValues.InvitationLogoUrl) || !string.IsNullOrEmpty(customizationValues.InvitationLegalUrl) || !string.IsNullOrEmpty(customizationValues.InvitationFooterText)))
			{
				return new CustomInvite
				{
					HelpUrl = customizationValues.InvitationHelpUrl,
					LogoUrl = customizationValues.InvitationLogoUrl,
					LegalUrl = customizationValues.InvitationLegalUrl,
					FooterText = customizationValues.InvitationFooterText
				};
			}
			return null;
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("user")]
	[Get(typeof(OnlineMeetingInvitationCustomizationResource))]
	[DataContract(Name = "OnlineMeetingInvitationCustomizationResource")]
	internal class OnlineMeetingInvitationCustomizationResource : OnlineMeetingCapabilityResource
	{
		public OnlineMeetingInvitationCustomizationResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "enterpriseHelpUrl", EmitDefaultValue = false)]
		public string EnterpriseHelpUrl
		{
			get
			{
				return base.GetValue<string>("enterpriseHelpUrl");
			}
			set
			{
				base.SetValue<string>("enterpriseHelpUrl", value);
			}
		}

		[DataMember(Name = "invitationFooterText", EmitDefaultValue = false)]
		public string InvitationFooterText
		{
			get
			{
				return base.GetValue<string>("invitationFooterText");
			}
			set
			{
				base.SetValue<string>("invitationFooterText", value);
			}
		}

		[DataMember(Name = "invitationHelpUrl", EmitDefaultValue = false)]
		public string InvitationHelpUrl
		{
			get
			{
				return base.GetValue<string>("invitationHelpUrl");
			}
			set
			{
				base.SetValue<string>("invitationHelpUrl", value);
			}
		}

		[DataMember(Name = "invitationLegalUrl", EmitDefaultValue = false)]
		public string InvitationLegalUrl
		{
			get
			{
				return base.GetValue<string>("invitationLegalUrl");
			}
			set
			{
				base.SetValue<string>("invitationLegalUrl", value);
			}
		}

		[DataMember(Name = "invitationLogoUrl", EmitDefaultValue = false)]
		public string InvitationLogoUrl
		{
			get
			{
				return base.GetValue<string>("invitationLogoUrl");
			}
			set
			{
				base.SetValue<string>("invitationLogoUrl", value);
			}
		}

		public const string Token = "onlineMeetingInvitationCustomization";

		private const string EnterpriseHelpUrlPropertyName = "enterpriseHelpUrl";

		private const string InvitationFooterTextPropertyName = "invitationFooterText";

		private const string InvitationHelpUrlPropertyName = "invitationHelpUrl";

		private const string InvitationLegalUrlPropertyName = "invitationLegalUrl";

		private const string InvitationLogoUrlPropertyName = "invitationLogoUrl";
	}
}

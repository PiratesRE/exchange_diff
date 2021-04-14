using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "SetCompanyContactInformationRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class SetCompanyContactInformationRequest : Request
	{
		[DataMember]
		public string[] MarketingNotificationEmails
		{
			get
			{
				return this.MarketingNotificationEmailsField;
			}
			set
			{
				this.MarketingNotificationEmailsField = value;
			}
		}

		[DataMember]
		public string[] TechnicalNotificationEmails
		{
			get
			{
				return this.TechnicalNotificationEmailsField;
			}
			set
			{
				this.TechnicalNotificationEmailsField = value;
			}
		}

		private string[] MarketingNotificationEmailsField;

		private string[] TechnicalNotificationEmailsField;
	}
}

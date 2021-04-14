using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class RightsManagementLicenseDataType
	{
		public int RightsManagedMessageDecryptionStatus;

		[XmlIgnore]
		public bool RightsManagedMessageDecryptionStatusSpecified;

		public string RmsTemplateId;

		public string TemplateName;

		public string TemplateDescription;

		public bool EditAllowed;

		[XmlIgnore]
		public bool EditAllowedSpecified;

		public bool ReplyAllowed;

		[XmlIgnore]
		public bool ReplyAllowedSpecified;

		public bool ReplyAllAllowed;

		[XmlIgnore]
		public bool ReplyAllAllowedSpecified;

		public bool ForwardAllowed;

		[XmlIgnore]
		public bool ForwardAllowedSpecified;

		public bool ModifyRecipientsAllowed;

		[XmlIgnore]
		public bool ModifyRecipientsAllowedSpecified;

		public bool ExtractAllowed;

		[XmlIgnore]
		public bool ExtractAllowedSpecified;

		public bool PrintAllowed;

		[XmlIgnore]
		public bool PrintAllowedSpecified;

		public bool ExportAllowed;

		[XmlIgnore]
		public bool ExportAllowedSpecified;

		public bool ProgrammaticAccessAllowed;

		[XmlIgnore]
		public bool ProgrammaticAccessAllowedSpecified;

		public bool IsOwner;

		[XmlIgnore]
		public bool IsOwnerSpecified;

		public string ContentOwner;

		public string ContentExpiryDate;
	}
}

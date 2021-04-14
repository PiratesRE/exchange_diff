using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetSharingMetadataType : BaseRequestType
	{
		public FolderIdType IdOfFolderToShare
		{
			get
			{
				return this.idOfFolderToShareField;
			}
			set
			{
				this.idOfFolderToShareField = value;
			}
		}

		public string SenderSmtpAddress
		{
			get
			{
				return this.senderSmtpAddressField;
			}
			set
			{
				this.senderSmtpAddressField = value;
			}
		}

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Recipients
		{
			get
			{
				return this.recipientsField;
			}
			set
			{
				this.recipientsField = value;
			}
		}

		private FolderIdType idOfFolderToShareField;

		private string senderSmtpAddressField;

		private string[] recipientsField;
	}
}

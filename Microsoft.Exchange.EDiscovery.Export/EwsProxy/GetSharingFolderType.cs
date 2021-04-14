using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetSharingFolderType : BaseRequestType
	{
		public string SmtpAddress
		{
			get
			{
				return this.smtpAddressField;
			}
			set
			{
				this.smtpAddressField = value;
			}
		}

		public SharingDataType DataType
		{
			get
			{
				return this.dataTypeField;
			}
			set
			{
				this.dataTypeField = value;
			}
		}

		[XmlIgnore]
		public bool DataTypeSpecified
		{
			get
			{
				return this.dataTypeFieldSpecified;
			}
			set
			{
				this.dataTypeFieldSpecified = value;
			}
		}

		public string SharedFolderId
		{
			get
			{
				return this.sharedFolderIdField;
			}
			set
			{
				this.sharedFolderIdField = value;
			}
		}

		private string smtpAddressField;

		private SharingDataType dataTypeField;

		private bool dataTypeFieldSpecified;

		private string sharedFolderIdField;
	}
}

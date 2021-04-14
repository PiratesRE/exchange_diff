using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class GetSharingMetadataResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("EncryptedSharedFolderData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public EncryptedSharedFolderDataType[] EncryptedSharedFolderDataCollection
		{
			get
			{
				return this.encryptedSharedFolderDataCollectionField;
			}
			set
			{
				this.encryptedSharedFolderDataCollectionField = value;
			}
		}

		[XmlArrayItem("InvalidRecipient", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public InvalidRecipientType[] InvalidRecipients
		{
			get
			{
				return this.invalidRecipientsField;
			}
			set
			{
				this.invalidRecipientsField = value;
			}
		}

		private EncryptedSharedFolderDataType[] encryptedSharedFolderDataCollectionField;

		private InvalidRecipientType[] invalidRecipientsField;
	}
}

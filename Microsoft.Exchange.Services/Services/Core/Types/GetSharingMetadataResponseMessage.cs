using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetSharingMetadataResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetSharingMetadataResponseMessage : ResponseMessage
	{
		public GetSharingMetadataResponseMessage()
		{
		}

		internal GetSharingMetadataResponseMessage(ServiceResultCode code, ServiceError error, EncryptionResults encryptionResults) : base(code, error)
		{
			if (encryptionResults != null)
			{
				this.encryptedSharedFolderDataCollection = encryptionResults.EncryptedSharedFolderDataCollection;
				this.invalidRecipients = encryptionResults.InvalidRecipients;
			}
		}

		[XmlArrayItem("EncryptedSharedFolderData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(EncryptedSharedFolderData))]
		[XmlArray("EncryptedSharedFolderDataCollection", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public EncryptedSharedFolderData[] EncryptedSharedFolderDataCollection
		{
			get
			{
				return this.encryptedSharedFolderDataCollection;
			}
			set
			{
				this.encryptedSharedFolderDataCollection = value;
			}
		}

		[XmlArray("InvalidRecipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("InvalidRecipient", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(InvalidRecipient))]
		public InvalidRecipient[] InvalidRecipients
		{
			get
			{
				return this.invalidRecipients;
			}
			set
			{
				this.invalidRecipients = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetSharingMetadataResponseMessage;
		}

		private EncryptedSharedFolderData[] encryptedSharedFolderDataCollection;

		private InvalidRecipient[] invalidRecipients;
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentPolicyType
	{
		[DataMember]
		public string[] AllowedFileTypes
		{
			get
			{
				return this.allowedFileTypes;
			}
			set
			{
				this.allowedFileTypes = value;
			}
		}

		[DataMember]
		public string[] AllowedMimeTypes
		{
			get
			{
				return this.allowedMimeTypes;
			}
			set
			{
				this.allowedMimeTypes = value;
			}
		}

		[DataMember]
		public string[] ForceSaveFileTypes
		{
			get
			{
				return this.forceSaveFileTypes;
			}
			set
			{
				this.forceSaveFileTypes = value;
			}
		}

		[DataMember]
		public string[] ForceSaveMimeTypes
		{
			get
			{
				return this.forceSaveMimeTypes;
			}
			set
			{
				this.forceSaveMimeTypes = value;
			}
		}

		[DataMember]
		public string[] BlockedFileTypes
		{
			get
			{
				return this.blockedFileTypes;
			}
			set
			{
				this.blockedFileTypes = value;
			}
		}

		[DataMember]
		public string[] BlockedMimeTypes
		{
			get
			{
				return this.blockedMimeTypes;
			}
			set
			{
				this.blockedMimeTypes = value;
			}
		}

		[DataMember]
		public string ActionForUnknownFileAndMIMETypes
		{
			get
			{
				return this.actionForUnknownFileAndMIMETypes;
			}
			set
			{
				this.actionForUnknownFileAndMIMETypes = value;
			}
		}

		[DataMember]
		public string[] WacViewableFileTypes
		{
			get
			{
				return this.wacViewableFileTypes;
			}
			set
			{
				this.wacViewableFileTypes = value;
			}
		}

		[DataMember]
		public string[] WacEditableFileTypes
		{
			get
			{
				return this.wacEditableFileTypes;
			}
			set
			{
				this.wacEditableFileTypes = value;
			}
		}

		[DataMember]
		public bool WacViewingOnPublicComputersEnabled
		{
			get
			{
				return this.wacViewingOnPublicComputersEnabled;
			}
			set
			{
				this.wacViewingOnPublicComputersEnabled = value;
			}
		}

		[DataMember]
		public bool WacViewingOnPrivateComputersEnabled
		{
			get
			{
				return this.wacViewingOnPrivateComputersEnabled;
			}
			set
			{
				this.wacViewingOnPrivateComputersEnabled = value;
			}
		}

		[DataMember]
		public bool ForceWacViewingFirstOnPublicComputers
		{
			get
			{
				return this.forceWacViewingFirstOnPublicComputers;
			}
			set
			{
				this.forceWacViewingFirstOnPublicComputers = value;
			}
		}

		[DataMember]
		public bool ForceWacViewingFirstOnPrivateComputers
		{
			get
			{
				return this.forceWacViewingFirstOnPrivateComputers;
			}
			set
			{
				this.forceWacViewingFirstOnPrivateComputers = value;
			}
		}

		[DataMember]
		public bool ForceWebReadyDocumentViewingFirstOnPublicComputers
		{
			get
			{
				return this.forceWebReadyDocumentViewingFirstOnPublicComputers;
			}
			set
			{
				this.forceWebReadyDocumentViewingFirstOnPublicComputers = value;
			}
		}

		[DataMember]
		public bool ForceWebReadyDocumentViewingFirstOnPrivateComputers
		{
			get
			{
				return this.forceWebReadyDocumentViewingFirstOnPrivateComputers;
			}
			set
			{
				this.forceWebReadyDocumentViewingFirstOnPrivateComputers = value;
			}
		}

		[DataMember]
		public bool WebReadyDocumentViewingOnPublicComputersEnabled
		{
			get
			{
				return this.webReadyDocumentViewingOnPublicComputersEnabled;
			}
			set
			{
				this.webReadyDocumentViewingOnPublicComputersEnabled = value;
			}
		}

		[DataMember]
		public bool WebReadyDocumentViewingOnPrivateComputersEnabled
		{
			get
			{
				return this.webReadyDocumentViewingOnPrivateComputersEnabled;
			}
			set
			{
				this.webReadyDocumentViewingOnPrivateComputersEnabled = value;
			}
		}

		[DataMember]
		public bool DirectFileAccessOnPublicComputersEnabled
		{
			get
			{
				return this.directFileAccessOnPublicComputersEnabled;
			}
			set
			{
				this.directFileAccessOnPublicComputersEnabled = value;
			}
		}

		[DataMember]
		public bool DirectFileAccessOnPrivateComputersEnabled
		{
			get
			{
				return this.directFileAccessOnPrivateComputersEnabled;
			}
			set
			{
				this.directFileAccessOnPrivateComputersEnabled = value;
			}
		}

		[DataMember]
		public bool WebReadyDocumentViewingForAllSupportedTypes
		{
			get
			{
				return this.webReadyDocumentViewingForAllSupportedTypes;
			}
			set
			{
				this.webReadyDocumentViewingForAllSupportedTypes = value;
			}
		}

		[DataMember]
		public bool AttachmentDataProviderAvailable
		{
			get
			{
				return this.attachmentDataProviderAvailable;
			}
			set
			{
				this.attachmentDataProviderAvailable = value;
			}
		}

		[DataMember]
		public string[] WebReadyFileTypes
		{
			get
			{
				return this.webReadyFileTypes;
			}
			set
			{
				this.webReadyFileTypes = value;
			}
		}

		[DataMember]
		public string[] WebReadyMimeTypes
		{
			get
			{
				return this.webReadyMimeTypes;
			}
			set
			{
				this.webReadyMimeTypes = value;
			}
		}

		[DataMember]
		public string[] WebReadyDocumentViewingSupportedFileTypes
		{
			get
			{
				return this.webReadyDocumentViewingSupportedFileTypes;
			}
			set
			{
				this.webReadyDocumentViewingSupportedFileTypes = value;
			}
		}

		[DataMember]
		public string[] WebReadyDocumentViewingSupportedMimeTypes
		{
			get
			{
				return this.webReadyDocumentViewingSupportedMimeTypes;
			}
			set
			{
				this.webReadyDocumentViewingSupportedMimeTypes = value;
			}
		}

		private string[] allowedFileTypes;

		private string[] allowedMimeTypes;

		private string[] forceSaveFileTypes;

		private string[] forceSaveMimeTypes;

		private string[] blockedFileTypes;

		private string[] blockedMimeTypes;

		private string actionForUnknownFileAndMIMETypes;

		private string[] wacViewableFileTypes;

		private string[] wacEditableFileTypes;

		private bool forceWacViewingFirstOnPublicComputers;

		private bool forceWacViewingFirstOnPrivateComputers;

		private bool wacViewingOnPublicComputersEnabled;

		private bool wacViewingOnPrivateComputersEnabled;

		private bool forceWebReadyDocumentViewingFirstOnPublicComputers;

		private bool forceWebReadyDocumentViewingFirstOnPrivateComputers;

		private bool webReadyDocumentViewingOnPublicComputersEnabled;

		private bool webReadyDocumentViewingOnPrivateComputersEnabled;

		private bool webReadyDocumentViewingForAllSupportedTypes;

		private bool directFileAccessOnPrivateComputersEnabled;

		private bool directFileAccessOnPublicComputersEnabled;

		private bool attachmentDataProviderAvailable;

		private string[] webReadyFileTypes;

		private string[] webReadyMimeTypes;

		private string[] webReadyDocumentViewingSupportedFileTypes;

		private string[] webReadyDocumentViewingSupportedMimeTypes;
	}
}

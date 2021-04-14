using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "KeyInformation", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	public class KeyInformation : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string strID
		{
			get
			{
				return this.strIDField;
			}
			set
			{
				this.strIDField = value;
			}
		}

		[DataMember]
		public string strIDType
		{
			get
			{
				return this.strIDTypeField;
			}
			set
			{
				this.strIDTypeField = value;
			}
		}

		[DataMember(Order = 2)]
		public int nCSPType
		{
			get
			{
				return this.nCSPTypeField;
			}
			set
			{
				this.nCSPTypeField = value;
			}
		}

		[DataMember(Order = 3)]
		public string strCSPName
		{
			get
			{
				return this.strCSPNameField;
			}
			set
			{
				this.strCSPNameField = value;
			}
		}

		[DataMember(Order = 4)]
		public string strKeyContainerName
		{
			get
			{
				return this.strKeyContainerNameField;
			}
			set
			{
				this.strKeyContainerNameField = value;
			}
		}

		[DataMember(Order = 5)]
		public int nKeyNumber
		{
			get
			{
				return this.nKeyNumberField;
			}
			set
			{
				this.nKeyNumberField = value;
			}
		}

		[DataMember(Order = 6)]
		public string strEncryptedPrivateKey
		{
			get
			{
				return this.strEncryptedPrivateKeyField;
			}
			set
			{
				this.strEncryptedPrivateKeyField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string strIDField;

		private string strIDTypeField;

		private int nCSPTypeField;

		private string strCSPNameField;

		private string strKeyContainerNameField;

		private int nKeyNumberField;

		private string strEncryptedPrivateKeyField;
	}
}

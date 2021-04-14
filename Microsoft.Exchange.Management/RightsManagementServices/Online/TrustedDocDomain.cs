using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[DataContract(Name = "TrustedDocDomain", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class TrustedDocDomain : IExtensibleDataObject
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
		public KeyInformation m_ttdki
		{
			get
			{
				return this.m_ttdkiField;
			}
			set
			{
				this.m_ttdkiField = value;
			}
		}

		[DataMember(Order = 1)]
		public string[] m_strLicensorCertChain
		{
			get
			{
				return this.m_strLicensorCertChainField;
			}
			set
			{
				this.m_strLicensorCertChainField = value;
			}
		}

		[DataMember(Order = 2)]
		public string[] m_astrRightsTemplates
		{
			get
			{
				return this.m_astrRightsTemplatesField;
			}
			set
			{
				this.m_astrRightsTemplatesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private KeyInformation m_ttdkiField;

		private string[] m_strLicensorCertChainField;

		private string[] m_astrRightsTemplatesField;
	}
}

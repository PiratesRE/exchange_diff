using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CredentialValue
	{
		[XmlAttribute]
		public CredentialType CredentialType
		{
			get
			{
				return this.credentialTypeField;
			}
			set
			{
				this.credentialTypeField = value;
			}
		}

		[XmlAttribute]
		public string KeyStoreId
		{
			get
			{
				return this.keyStoreIdField;
			}
			set
			{
				this.keyStoreIdField = value;
			}
		}

		[XmlAttribute]
		public string KeyGroupId
		{
			get
			{
				return this.keyGroupIdField;
			}
			set
			{
				this.keyGroupIdField = value;
			}
		}

		private CredentialType credentialTypeField;

		private string keyStoreIdField;

		private string keyGroupIdField;
	}
}

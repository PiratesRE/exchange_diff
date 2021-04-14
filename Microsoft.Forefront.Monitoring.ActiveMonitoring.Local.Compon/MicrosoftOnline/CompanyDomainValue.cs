using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(CompanyVerifiedDomainValue))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlInclude(typeof(CompanyUnverifiedDomainValue))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class CompanyDomainValue
	{
		public CompanyDomainValue()
		{
			this.liveTypeField = LiveNamespaceType.None;
			this.capabilitiesField = 0;
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[DefaultValue(LiveNamespaceType.None)]
		[XmlAttribute]
		public LiveNamespaceType LiveType
		{
			get
			{
				return this.liveTypeField;
			}
			set
			{
				this.liveTypeField = value;
			}
		}

		[XmlAttribute(DataType = "hexBinary")]
		public byte[] LiveNetId
		{
			get
			{
				return this.liveNetIdField;
			}
			set
			{
				this.liveNetIdField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(0)]
		public int Capabilities
		{
			get
			{
				return this.capabilitiesField;
			}
			set
			{
				this.capabilitiesField = value;
			}
		}

		[XmlAttribute]
		public string MailTargetKey
		{
			get
			{
				return this.mailTargetKeyField;
			}
			set
			{
				this.mailTargetKeyField = value;
			}
		}

		private string nameField;

		private LiveNamespaceType liveTypeField;

		private byte[] liveNetIdField;

		private int capabilitiesField;

		private string mailTargetKeyField;
	}
}

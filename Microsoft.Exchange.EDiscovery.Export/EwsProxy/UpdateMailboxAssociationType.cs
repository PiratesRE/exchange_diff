using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateMailboxAssociationType : BaseRequestType
	{
		public MailboxAssociationType Association
		{
			get
			{
				return this.associationField;
			}
			set
			{
				this.associationField = value;
			}
		}

		public MasterMailboxType Master
		{
			get
			{
				return this.masterField;
			}
			set
			{
				this.masterField = value;
			}
		}

		private MailboxAssociationType associationField;

		private MasterMailboxType masterField;
	}
}

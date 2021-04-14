using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionRequest
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[XmlType(Namespace = "DeltaSyncV2:")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AccountType
	{
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

		public AccountTypeType Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		public string PartnerID
		{
			get
			{
				return this.partnerIDField;
			}
			set
			{
				this.partnerIDField = value;
			}
		}

		private string nameField;

		private AccountTypeType typeField;

		private string partnerIDField;
	}
}

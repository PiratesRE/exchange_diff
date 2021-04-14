using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionRequest
{
	[XmlRoot(Namespace = "DeltaSyncV2:", IsNullable = false)]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[Serializable]
	public class Provision
	{
		[XmlArrayItem("Account", IsNullable = false)]
		public AccountType[] Add
		{
			get
			{
				return this.addField;
			}
			set
			{
				this.addField = value;
			}
		}

		[XmlArrayItem("Account", IsNullable = false)]
		public AccountType[] Delete
		{
			get
			{
				return this.deleteField;
			}
			set
			{
				this.deleteField = value;
			}
		}

		[XmlArrayItem("Account", IsNullable = false)]
		public AccountType[] Read
		{
			get
			{
				return this.readField;
			}
			set
			{
				this.readField = value;
			}
		}

		private AccountType[] addField;

		private AccountType[] deleteField;

		private AccountType[] readField;
	}
}

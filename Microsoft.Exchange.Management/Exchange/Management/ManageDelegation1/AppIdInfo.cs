using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.ManageDelegation1
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://domains.live.com/Service/ManageDelegation/V1.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AppIdInfo
	{
		public string AppId
		{
			get
			{
				return this.appIdField;
			}
			set
			{
				this.appIdField = value;
			}
		}

		public string AdminKey
		{
			get
			{
				return this.adminKeyField;
			}
			set
			{
				this.adminKeyField = value;
			}
		}

		private string appIdField;

		private string adminKeyField;
	}
}

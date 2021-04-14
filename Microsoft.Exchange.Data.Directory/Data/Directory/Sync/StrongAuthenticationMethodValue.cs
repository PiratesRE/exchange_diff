using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class StrongAuthenticationMethodValue
	{
		public StrongAuthenticationMethodValue()
		{
			this.defaultField = false;
		}

		[XmlAttribute]
		public int MethodType
		{
			get
			{
				return this.methodTypeField;
			}
			set
			{
				this.methodTypeField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool Default
		{
			get
			{
				return this.defaultField;
			}
			set
			{
				this.defaultField = value;
			}
		}

		private int methodTypeField;

		private bool defaultField;
	}
}

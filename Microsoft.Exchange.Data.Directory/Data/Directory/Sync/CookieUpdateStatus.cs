using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[Serializable]
	public class CookieUpdateStatus
	{
		[XmlElement(IsNullable = true, Order = 0)]
		public string StatusMessage
		{
			get
			{
				return this.statusMessageField;
			}
			set
			{
				this.statusMessageField = value;
			}
		}

		[XmlAttribute]
		public int StatusCode
		{
			get
			{
				return this.statusCodeField;
			}
			set
			{
				this.statusCodeField = value;
			}
		}

		private string statusMessageField;

		private int statusCodeField;
	}
}

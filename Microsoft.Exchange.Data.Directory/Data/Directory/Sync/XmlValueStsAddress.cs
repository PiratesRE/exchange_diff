using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class XmlValueStsAddress
	{
		[XmlElement(Order = 0)]
		public StsAddressValue StsAddress
		{
			get
			{
				return this.stsAddressField;
			}
			set
			{
				this.stsAddressField = value;
			}
		}

		private StsAddressValue stsAddressField;
	}
}

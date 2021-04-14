using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class XmlValueCompanyPartnership
	{
		[XmlArrayItem("Partnership", IsNullable = false)]
		[XmlArray(Order = 0)]
		public PartnershipValue[] Partnerships
		{
			get
			{
				return this.partnershipsField;
			}
			set
			{
				this.partnershipsField = value;
			}
		}

		private PartnershipValue[] partnershipsField;
	}
}

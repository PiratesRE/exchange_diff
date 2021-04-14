using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class XmlValueLicenseUnitsDetail
	{
		[XmlElement(Order = 0)]
		public LicenseUnitsDetailValue LicenseUnitsDetail
		{
			get
			{
				return this.licenseUnitsDetailField;
			}
			set
			{
				this.licenseUnitsDetailField = value;
			}
		}

		private LicenseUnitsDetailValue licenseUnitsDetailField;
	}
}

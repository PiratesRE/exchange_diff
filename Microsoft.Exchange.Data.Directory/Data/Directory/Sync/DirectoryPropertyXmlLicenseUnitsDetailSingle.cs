using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class DirectoryPropertyXmlLicenseUnitsDetailSingle : DirectoryPropertyXmlLicenseUnitsDetail
	{
		public override IList GetValues()
		{
			if (base.Value != null)
			{
				return base.Value;
			}
			return DirectoryProperty.EmptyValues;
		}

		public sealed override void SetValues(IList values)
		{
			if (values == DirectoryProperty.EmptyValues)
			{
				base.Value = null;
				return;
			}
			base.Value = new XmlValueLicenseUnitsDetail[values.Count];
			values.CopyTo(base.Value, 0);
		}
	}
}

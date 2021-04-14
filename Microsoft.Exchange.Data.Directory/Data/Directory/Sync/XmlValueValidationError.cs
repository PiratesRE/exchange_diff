using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class XmlValueValidationError
	{
		[XmlElement(Order = 0)]
		public ValidationErrorValue ErrorInfo
		{
			get
			{
				return this.errorInfoField;
			}
			set
			{
				this.errorInfoField = value;
			}
		}

		private ValidationErrorValue errorInfoField;
	}
}

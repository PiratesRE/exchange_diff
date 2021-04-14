using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class XmlValueSharedKeyReference
	{
		[XmlElement(Order = 0)]
		public SharedKeyReferenceValue SharedKeyReference
		{
			get
			{
				return this.sharedKeyReferenceField;
			}
			set
			{
				this.sharedKeyReferenceField = value;
			}
		}

		private SharedKeyReferenceValue sharedKeyReferenceField;
	}
}

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FractionalPageViewType : BasePagingType
	{
		[XmlAttribute]
		public int Numerator
		{
			get
			{
				return this.numeratorField;
			}
			set
			{
				this.numeratorField = value;
			}
		}

		[XmlAttribute]
		public int Denominator
		{
			get
			{
				return this.denominatorField;
			}
			set
			{
				this.denominatorField = value;
			}
		}

		private int numeratorField;

		private int denominatorField;
	}
}

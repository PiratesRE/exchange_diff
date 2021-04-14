using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class EffectiveRightsType
	{
		public bool CreateAssociated;

		public bool CreateContents;

		public bool CreateHierarchy;

		public bool Delete;

		public bool Modify;

		public bool Read;

		public bool ViewPrivateItems;

		[XmlIgnore]
		public bool ViewPrivateItemsSpecified;
	}
}

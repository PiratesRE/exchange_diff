using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class UpdateItemResponseMessageType : ItemInfoResponseMessageType
	{
		public ConflictResultsType ConflictResults
		{
			get
			{
				return this.conflictResultsField;
			}
			set
			{
				this.conflictResultsField = value;
			}
		}

		private ConflictResultsType conflictResultsField;
	}
}

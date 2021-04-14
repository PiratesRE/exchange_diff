using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserAvailabilityResponseType
	{
		[XmlArrayItem("FreeBusyResponse", IsNullable = false)]
		public FreeBusyResponseType[] FreeBusyResponseArray
		{
			get
			{
				return this.freeBusyResponseArrayField;
			}
			set
			{
				this.freeBusyResponseArrayField = value;
			}
		}

		public SuggestionsResponseType SuggestionsResponse
		{
			get
			{
				return this.suggestionsResponseField;
			}
			set
			{
				this.suggestionsResponseField = value;
			}
		}

		private FreeBusyResponseType[] freeBusyResponseArrayField;

		private SuggestionsResponseType suggestionsResponseField;
	}
}

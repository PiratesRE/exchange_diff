using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class VotingInformationType
	{
		[XmlArrayItem("VotingOptionData", IsNullable = false)]
		public VotingOptionDataType[] UserOptions
		{
			get
			{
				return this.userOptionsField;
			}
			set
			{
				this.userOptionsField = value;
			}
		}

		public string VotingResponse
		{
			get
			{
				return this.votingResponseField;
			}
			set
			{
				this.votingResponseField = value;
			}
		}

		private VotingOptionDataType[] userOptionsField;

		private string votingResponseField;
	}
}

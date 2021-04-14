using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RetentionPolicyTagType
	{
		public string DisplayName;

		public string RetentionId;

		public int RetentionPeriod;

		public ElcFolderType Type;

		public RetentionActionType RetentionAction;

		public string Description;

		public bool IsVisible;

		public bool OptedInto;

		public bool IsArchive;
	}
}

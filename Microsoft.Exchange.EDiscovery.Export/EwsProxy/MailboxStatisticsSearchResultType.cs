using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class MailboxStatisticsSearchResultType
	{
		public UserMailboxType UserMailbox
		{
			get
			{
				return this.userMailboxField;
			}
			set
			{
				this.userMailboxField = value;
			}
		}

		public KeywordStatisticsSearchResultType KeywordStatisticsSearchResult
		{
			get
			{
				return this.keywordStatisticsSearchResultField;
			}
			set
			{
				this.keywordStatisticsSearchResultField = value;
			}
		}

		private UserMailboxType userMailboxField;

		private KeywordStatisticsSearchResultType keywordStatisticsSearchResultField;
	}
}

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetNonIndexableItemStatisticsType : BaseRequestType
	{
		[XmlArrayItem("LegacyDN", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Mailboxes
		{
			get
			{
				return this.mailboxesField;
			}
			set
			{
				this.mailboxesField = value;
			}
		}

		public bool SearchArchiveOnly
		{
			get
			{
				return this.searchArchiveOnlyField;
			}
			set
			{
				this.searchArchiveOnlyField = value;
			}
		}

		[XmlIgnore]
		public bool SearchArchiveOnlySpecified
		{
			get
			{
				return this.searchArchiveOnlyFieldSpecified;
			}
			set
			{
				this.searchArchiveOnlyFieldSpecified = value;
			}
		}

		private string[] mailboxesField;

		private bool searchArchiveOnlyField;

		private bool searchArchiveOnlyFieldSpecified;
	}
}

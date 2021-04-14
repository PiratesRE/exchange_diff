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
	public class GetNonIndexableItemDetailsType : BaseRequestType
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

		public int PageSize
		{
			get
			{
				return this.pageSizeField;
			}
			set
			{
				this.pageSizeField = value;
			}
		}

		[XmlIgnore]
		public bool PageSizeSpecified
		{
			get
			{
				return this.pageSizeFieldSpecified;
			}
			set
			{
				this.pageSizeFieldSpecified = value;
			}
		}

		public string PageItemReference
		{
			get
			{
				return this.pageItemReferenceField;
			}
			set
			{
				this.pageItemReferenceField = value;
			}
		}

		public SearchPageDirectionType PageDirection
		{
			get
			{
				return this.pageDirectionField;
			}
			set
			{
				this.pageDirectionField = value;
			}
		}

		[XmlIgnore]
		public bool PageDirectionSpecified
		{
			get
			{
				return this.pageDirectionFieldSpecified;
			}
			set
			{
				this.pageDirectionFieldSpecified = value;
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

		private int pageSizeField;

		private bool pageSizeFieldSpecified;

		private string pageItemReferenceField;

		private SearchPageDirectionType pageDirectionField;

		private bool pageDirectionFieldSpecified;

		private bool searchArchiveOnlyField;

		private bool searchArchiveOnlyFieldSpecified;
	}
}

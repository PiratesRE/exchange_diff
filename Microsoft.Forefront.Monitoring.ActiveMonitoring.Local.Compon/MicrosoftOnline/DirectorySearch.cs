using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(SearchForUsersValue))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[Serializable]
	public abstract class DirectorySearch
	{
		public DirectorySearch()
		{
			this.propertyField = new string[0];
			this.includeMetadataField = false;
		}

		[XmlAttribute]
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

		[XmlAttribute]
		public int SortBufferSize
		{
			get
			{
				return this.sortBufferSizeField;
			}
			set
			{
				this.sortBufferSizeField = value;
			}
		}

		[XmlAttribute]
		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrderField;
			}
			set
			{
				this.sortOrderField = value;
			}
		}

		[XmlAttribute]
		public SortProperty SortProperty
		{
			get
			{
				return this.sortPropertyField;
			}
			set
			{
				this.sortPropertyField = value;
			}
		}

		[XmlAttribute]
		public string[] Property
		{
			get
			{
				return this.propertyField;
			}
			set
			{
				this.propertyField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool IncludeMetadata
		{
			get
			{
				return this.includeMetadataField;
			}
			set
			{
				this.includeMetadataField = value;
			}
		}

		private int pageSizeField;

		private int sortBufferSizeField;

		private SortOrder sortOrderField;

		private SortProperty sortPropertyField;

		private string[] propertyField;

		private bool includeMetadataField;
	}
}

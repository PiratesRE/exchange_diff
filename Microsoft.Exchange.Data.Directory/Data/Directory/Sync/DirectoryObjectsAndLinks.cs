using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DirectoryObjectsAndLinks
	{
		[XmlIgnore]
		public Guid BatchId
		{
			get
			{
				return this.batchId;
			}
		}

		[XmlArray(Order = 0)]
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", IsNullable = false)]
		public DirectoryObject[] Objects
		{
			get
			{
				return this.objectsField;
			}
			set
			{
				this.objectsField = value;
			}
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", IsNullable = false)]
		[XmlArray(Order = 1)]
		public DirectoryLink[] Links
		{
			get
			{
				return this.linksField;
			}
			set
			{
				this.linksField = value;
			}
		}

		[XmlArray(Order = 2)]
		[XmlArrayItem(IsNullable = false)]
		public DirectoryObjectError[] Errors
		{
			get
			{
				return this.errorsField;
			}
			set
			{
				this.errorsField = value;
			}
		}

		[XmlElement(DataType = "base64Binary", Order = 3)]
		public byte[] NextPageToken
		{
			get
			{
				return this.nextPageTokenField;
			}
			set
			{
				this.nextPageTokenField = value;
			}
		}

		[XmlElement(Order = 4)]
		public bool More
		{
			get
			{
				return this.moreField;
			}
			set
			{
				this.moreField = value;
			}
		}

		private Guid batchId = Guid.NewGuid();

		private DirectoryObject[] objectsField;

		private DirectoryLink[] linksField;

		private DirectoryObjectError[] errorsField;

		private byte[] nextPageTokenField;

		private bool moreField;
	}
}

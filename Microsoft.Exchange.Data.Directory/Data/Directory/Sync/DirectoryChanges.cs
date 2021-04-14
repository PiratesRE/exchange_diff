using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class DirectoryChanges
	{
		[XmlIgnore]
		public Guid BatchId
		{
			get
			{
				return this.batchId;
			}
		}

		public DirectoryChanges()
		{
			this.operationResultCodeField = OperationResultCode.Success;
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", IsNullable = false)]
		[XmlArray(Order = 0)]
		public DirectoryContext[] Contexts
		{
			get
			{
				return this.contextsField;
			}
			set
			{
				this.contextsField = value;
			}
		}

		[XmlArray(Order = 1)]
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
		[XmlArray(Order = 2)]
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

		[XmlElement(DataType = "base64Binary", Order = 3)]
		public byte[] NextCookie
		{
			get
			{
				return this.nextCookieField;
			}
			set
			{
				this.nextCookieField = value;
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

		[DefaultValue(OperationResultCode.Success)]
		[XmlAttribute]
		public OperationResultCode OperationResultCode
		{
			get
			{
				return this.operationResultCodeField;
			}
			set
			{
				this.operationResultCodeField = value;
			}
		}

		private Guid batchId = Guid.NewGuid();

		private DirectoryContext[] contextsField;

		private DirectoryObject[] objectsField;

		private DirectoryLink[] linksField;

		private byte[] nextCookieField;

		private bool moreField;

		private OperationResultCode operationResultCodeField;
	}
}

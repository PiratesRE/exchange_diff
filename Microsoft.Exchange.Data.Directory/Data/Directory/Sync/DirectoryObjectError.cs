using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class DirectoryObjectError
	{
		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[XmlAttribute]
		public DirectoryObjectClass ObjectClass
		{
			get
			{
				return this.objectClassField;
			}
			set
			{
				this.objectClassField = value;
			}
		}

		[XmlIgnore]
		public bool ObjectClassSpecified
		{
			get
			{
				return this.objectClassFieldSpecified;
			}
			set
			{
				this.objectClassFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string ObjectId
		{
			get
			{
				return this.objectIdField;
			}
			set
			{
				this.objectIdField = value;
			}
		}

		[XmlAttribute]
		public DirectoryObjectErrorCode ErrorCode
		{
			get
			{
				return this.errorCodeField;
			}
			set
			{
				this.errorCodeField = value;
			}
		}

		[XmlAttribute]
		public string ErrorDetail
		{
			get
			{
				return this.errorDetailField;
			}
			set
			{
				this.errorDetailField = value;
			}
		}

		private string contextIdField;

		private DirectoryObjectClass objectClassField;

		private bool objectClassFieldSpecified;

		private string objectIdField;

		private DirectoryObjectErrorCode errorCodeField;

		private string errorDetailField;
	}
}

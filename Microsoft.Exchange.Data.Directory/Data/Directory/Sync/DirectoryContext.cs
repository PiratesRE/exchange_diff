using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DirectoryContext
	{
		public DirectoryContext()
		{
			this.inScopeField = true;
		}

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

		[DefaultValue(true)]
		[XmlAttribute]
		public bool InScope
		{
			get
			{
				return this.inScopeField;
			}
			set
			{
				this.inScopeField = value;
			}
		}

		private string contextIdField;

		private bool inScopeField;
	}
}

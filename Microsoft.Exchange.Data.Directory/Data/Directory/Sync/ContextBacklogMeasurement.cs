using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class ContextBacklogMeasurement
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
		public uint Objects
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

		[XmlAttribute]
		public uint Links
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

		[XmlAttribute]
		public int StreamCode
		{
			get
			{
				return this.streamCodeField;
			}
			set
			{
				this.streamCodeField = value;
			}
		}

		private string contextIdField;

		private uint objectsField;

		private uint linksField;

		private int streamCodeField;
	}
}

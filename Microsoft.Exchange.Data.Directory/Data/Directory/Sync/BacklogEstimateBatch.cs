using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[DesignerCategory("code")]
	[Serializable]
	public class BacklogEstimateBatch
	{
		[XmlArray(IsNullable = true, Order = 0)]
		[XmlArrayItem(IsNullable = false)]
		public ContextBacklogMeasurement[] ContextBacklogs
		{
			get
			{
				return this.contextBacklogsField;
			}
			set
			{
				this.contextBacklogsField = value;
			}
		}

		[XmlElement(DataType = "base64Binary", IsNullable = true, Order = 1)]
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

		[XmlAttribute]
		public int StatusCode
		{
			get
			{
				return this.statusCodeField;
			}
			set
			{
				this.statusCodeField = value;
			}
		}

		private ContextBacklogMeasurement[] contextBacklogsField;

		private byte[] nextPageTokenField;

		private int statusCodeField;
	}
}

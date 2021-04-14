using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class AlternateIdType : AlternateIdBaseType
	{
		[XmlAttribute]
		public string Id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		[XmlAttribute]
		public string Mailbox
		{
			get
			{
				return this.mailboxField;
			}
			set
			{
				this.mailboxField = value;
			}
		}

		[XmlAttribute]
		public bool IsArchive
		{
			get
			{
				return this.isArchiveField;
			}
			set
			{
				this.isArchiveField = value;
			}
		}

		[XmlIgnore]
		public bool IsArchiveSpecified
		{
			get
			{
				return this.isArchiveFieldSpecified;
			}
			set
			{
				this.isArchiveFieldSpecified = value;
			}
		}

		private string idField;

		private string mailboxField;

		private bool isArchiveField;

		private bool isArchiveFieldSpecified;
	}
}

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(UserLocatorType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(GroupLocatorType))]
	[DebuggerStepThrough]
	[Serializable]
	public class MailboxLocatorType
	{
		public string ExternalDirectoryObjectId
		{
			get
			{
				return this.externalDirectoryObjectIdField;
			}
			set
			{
				this.externalDirectoryObjectIdField = value;
			}
		}

		public string LegacyDn
		{
			get
			{
				return this.legacyDnField;
			}
			set
			{
				this.legacyDnField = value;
			}
		}

		private string externalDirectoryObjectIdField;

		private string legacyDnField;
	}
}

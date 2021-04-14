using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ModifiedEventType : BaseObjectChangedEventType
	{
		public int UnreadCount
		{
			get
			{
				return this.unreadCountField;
			}
			set
			{
				this.unreadCountField = value;
			}
		}

		[XmlIgnore]
		public bool UnreadCountSpecified
		{
			get
			{
				return this.unreadCountFieldSpecified;
			}
			set
			{
				this.unreadCountFieldSpecified = value;
			}
		}

		private int unreadCountField;

		private bool unreadCountFieldSpecified;
	}
}

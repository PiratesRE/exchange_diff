using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class InvitationTicketValue
	{
		[XmlAttribute]
		public int Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlAttribute]
		public string Ticket
		{
			get
			{
				return this.ticketField;
			}
			set
			{
				this.ticketField = value;
			}
		}

		private int typeField;

		private string ticketField;
	}
}

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class XmlValueInvitationTicket
	{
		[XmlElement(Order = 0)]
		public InvitationTicketValue InvitationTicket
		{
			get
			{
				return this.invitationTicketField;
			}
			set
			{
				this.invitationTicketField = value;
			}
		}

		private InvitationTicketValue invitationTicketField;
	}
}

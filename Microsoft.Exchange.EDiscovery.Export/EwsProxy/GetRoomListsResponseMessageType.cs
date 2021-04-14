using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class GetRoomListsResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Address", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public EmailAddressType[] RoomLists
		{
			get
			{
				return this.roomListsField;
			}
			set
			{
				this.roomListsField = value;
			}
		}

		private EmailAddressType[] roomListsField;
	}
}

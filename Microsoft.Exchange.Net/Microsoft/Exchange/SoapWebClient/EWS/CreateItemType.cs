using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class CreateItemType : BaseRequestType
	{
		public TargetFolderIdType SavedItemFolderId;

		public NonEmptyArrayOfAllItemsType Items;

		[XmlAttribute]
		public MessageDispositionType MessageDisposition;

		[XmlIgnore]
		public bool MessageDispositionSpecified;

		[XmlAttribute]
		public CalendarItemCreateOrDeleteOperationType SendMeetingInvitations;

		[XmlIgnore]
		public bool SendMeetingInvitationsSpecified;
	}
}

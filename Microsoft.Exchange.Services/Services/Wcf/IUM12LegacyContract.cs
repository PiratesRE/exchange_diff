using System;
using System.ServiceModel;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Wcf
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	internal interface IUM12LegacyContract
	{
		[OperationContract(Action = "*", ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/IsUMEnabled")]
		[XmlSerializerFormat]
		[return: MessageParameter(Name = "IsUMEnabledResponse")]
		bool IsUMEnabled();

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/GetUMProperties")]
		[return: MessageParameter(Name = "GetUMPropertiesResponse")]
		UMProperties GetUMProperties();

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/SetOofStatus")]
		void SetOofStatus(bool status);

		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/SetPlayOnPhoneDialString")]
		[XmlSerializerFormat]
		void SetPlayOnPhoneDialString(string dialString);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/SetTelephoneAccessFolderEmail")]
		void SetTelephoneAccessFolderEmail(string base64FolderId);

		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/SetMissedCallNotificationEnabled")]
		[XmlSerializerFormat]
		void SetMissedCallNotificationEnabled(bool status);

		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/ResetPIN")]
		[XmlSerializerFormat]
		void ResetPIN();

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/PlayOnPhoneResponse")]
		[return: MessageParameter(Name = "PlayOnPhoneResponse")]
		string PlayOnPhone([MessageParameter(Name = "entryId")] string base64ObjectId, [MessageParameter(Name = "DialString")] string dialString);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/GetCallInfo")]
		[return: MessageParameter(Name = "GetCallInfoResponse")]
		UMCallInfo GetCallInfo([MessageParameter(Name = "CallId")] string callId);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/Disconnect")]
		void Disconnect([MessageParameter(Name = "CallId")] string callId);

		[XmlSerializerFormat]
		[OperationContract(ReplyAction = "http://schemas.microsoft.com/exchange/services/2006/messages/PlayOnPhoneGreetingResponse")]
		[return: MessageParameter(Name = "PlayOnPhoneGreetingResponse")]
		string PlayOnPhoneGreeting([MessageParameter(Name = "GreetingType")] UMGreetingType greetingType, [MessageParameter(Name = "DialString")] string dialString);
	}
}

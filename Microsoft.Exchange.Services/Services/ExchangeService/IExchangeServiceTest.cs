using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[ServiceContract]
	public interface IExchangeServiceTest
	{
		[OperationContract]
		void CreateItem(string folderId, string subject, string body);

		[OperationContract]
		string CreateUnifiedMailbox();

		[OperationContract]
		bool IsOffice365Domain(string emailAddress);

		[OperationContract]
		AggregatedAccountType AddAggregatedAccount(string emailAddress, string userName, string password, string incomingServer, string incomingPort, string incomingProtocol, string security, string authentication, string outgoingServer, string outgoingPort, string outgoingProtocol, string interval);

		[OperationContract]
		void RemoveAggregatedAccount(string emailAddress);

		[OperationContract]
		void SetAggregatedAccount(string authentication, string emailAddress, string userName, string password, string interval, string incomingServer, string incomingPort, string incomingProtocol, string security);

		[OperationContract]
		AggregatedAccountType[] GetAggregatedAccount();

		[OperationContract(Name = "FindConversation")]
		ConversationType[] FindConversation(string parentFolderId);

		[OperationContract(Name = "FindConversationForUnifiedMailbox")]
		ConversationType[] FindConversation(Guid[] aggregatedMailboxGuids, DistinguishedFolderIdName defaultFolder);

		[OperationContract(Name = "FindItem")]
		ItemType[] FindItem(string parentFolderId);

		[OperationContract(Name = "FindItemForUnifiedMailbox")]
		ItemType[] FindItem(string[] folderIds);

		[OperationContract(Name = "FindFolder")]
		BaseFolderType[] FindFolder(DistinguishedFolderIdName distinguishedFolder);

		[OperationContract(Name = "FindFolderByMailboxGuid")]
		BaseFolderType[] FindFolder(string mailboxGuid);

		[OperationContract]
		Guid GetMailboxGuid();

		[OperationContract]
		void SubscribeToConversationChanges(string subscriptionId, string parentFolderId);

		[OperationContract(Name = "SubscribeToConversationChangesForUnifiedMailbox")]
		void SubscribeToConversationChanges(string subscriptionId, Guid[] aggregatedMailboxGuids, DistinguishedFolderIdName defaultFolder);

		[OperationContract]
		ConversationNotification GetNextConversationChange(string subscriptionId);

		[OperationContract]
		void SubscribeToCalendarChanges(string subscriptionId, string parentFolderId);

		[OperationContract]
		CalendarChangeNotificationType? GetNextCalendarChange(string subscriptionId);

		[OperationContract]
		InstantSearchPayloadType PerformInstantSearch(string deviceId, string searchSessionId, string kqlQuery, FolderId[] folderScope);

		[OperationContract]
		InstantSearchPayloadType GetNextInstantSearchPayload(string sessionId);

		[OperationContract]
		bool EndInstantSearchSession(string deviceId, string sessionId);

		[OperationContract]
		bool GetFolderFidAndMailboxFromEwsId(string ewsId, out long fid, out Guid mailboxGuid);

		[OperationContract]
		long GetFolderFidFromEwsId(string ewsId);

		[OperationContract]
		string GetEwsIdFromFolderFid(long fid, Guid mailboxGuid);

		[OperationContract]
		void SubscribeToHierarchyChanges(string subscriptionId, Guid mailboxGuid);

		[OperationContract]
		HierarchyNotification GetNextHierarchyChange(string subscriptionId);

		[OperationContract]
		void SubscribeToMessageChanges(string subscriptionId, string parentFolderId);

		[OperationContract(Name = "SubscribeToMessageChangesForUnifiedMailbox")]
		void SubscribeToMessageChanges(string subscriptionId, Guid[] aggregatedMailboxGuids, DistinguishedFolderIdName defaultFolder);

		[OperationContract]
		MessageNotification GetNextMessageChange(string subscriptionId);
	}
}

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	internal interface IMailboxReplicationService
	{
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MoveRequestInfo GetMoveRequestInfo(Guid requestGuid);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void SyncNow(List<SyncNowNotification> notifications);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void RefreshMoveRequest(Guid requestGuid, Guid mdbGuid, MoveRequestNotification op);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void RefreshMoveRequest2(Guid requestGuid, Guid mdbGuid, int requestFlags, MoveRequestNotification op);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MailboxInformation GetMailboxInformation2(Guid primaryMailboxGuid, Guid physicalMailboxGuid, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MailboxInformation GetMailboxInformation3(Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		MailboxInformation GetMailboxInformation4(string requestJobXml, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid targetMdbGuid, string targetMdbName, string remoteHostName, string remoteOrgName, string remoteDCName, string username, string password, string domain);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		string ValidateAndPopulateRequestJob(string requestJobXML, out string reportEntryXMLs);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void AttemptConnectToMRSProxy(string remoteHostName, Guid mbxGuid, string username, string password, string domain);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void PingMRSProxy(string serverFqdn, string username, string password, string domain, bool useHttps);
	}
}

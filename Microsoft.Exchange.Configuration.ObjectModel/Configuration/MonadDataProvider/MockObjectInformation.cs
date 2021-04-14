using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MockObjectInformation
	{
		internal static object CreateDummyObject(Type type)
		{
			return MockObjectInformation.GetCreator(type.FullName).CreateDummyObject(type);
		}

		internal static MockObjectCreator GetCreator(string fullName)
		{
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.Mailbox")
			{
				return new MailboxCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.User")
			{
				return new UserCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.CASMailbox")
			{
				return new CASMailboxCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.UMMailbox")
			{
				return new UMMailboxCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Mapi.MailboxStatistics")
			{
				return new MailboxStatisticsCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Storage.CalendarConfiguration")
			{
				return new CalendarConfigurationCreator();
			}
			if (fullName == "Microsoft.Exchange.Management.Tasks.UM.UMMailboxPin")
			{
				return new UMMailboxPinCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.DistributionGroup")
			{
				return new DistributionGroupCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.WindowsGroup")
			{
				return new WindowsGroupCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.DynamicDistributionGroup")
			{
				return new DynamicDistributionGroupCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.MailUser")
			{
				return new MailUserCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.MailContact")
			{
				return new MailContactCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.Contact")
			{
				return new ContactCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.SharingPolicy")
			{
				return new SharingPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.SharingPolicyDomain")
			{
				return new SharingPolicyDomainCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicy")
			{
				return new RetentionPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.OwaMailboxPolicy")
			{
				return new OwaMailboxPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.MobileMailboxPolicy")
			{
				return new MobileMailboxPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.ActiveSyncMailboxPolicy")
			{
				return new ActiveSyncMailboxPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.TransportConfigContainer")
			{
				return new TransportConfigContainerCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.DomainContentConfig")
			{
				return new DomainContentConfigCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain")
			{
				return new AcceptedDomainCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.UMDialPlan")
			{
				return new UMDialPlanCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.UMIPGateway")
			{
				return new UMIPGatewayCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.UMMailboxPolicy")
			{
				return new UMMailboxPolicyCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.UMAutoAttendant")
			{
				return new UMAutoAttendantCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.SystemConfiguration.FederationTrust")
			{
				return new FederationTrustCreator();
			}
			if (fullName == "Microsoft.Exchange.Data.Directory.Management.FederatedOrganizationIdWithDomainStatus")
			{
				return new FederatedOrganizationIdWithDomainStatusCreator();
			}
			if (fullName == "Microsoft.Exchange.Management.SystemConfigurationTasks.PresentationRetentionPolicyTag")
			{
				return new PresentationRetentionPolicyTagCreator();
			}
			if (fullName == "Microsoft.Exchange.MailboxReplicationService.MoveRequestStatistics")
			{
				return new MoveRequestStatisticsCreator();
			}
			if (fullName == "Microsoft.Exchange.Management.RecipientTasks.MailboxAcePresentationObject")
			{
				return new MailboxAcePresentationObjectCreator();
			}
			throw new NotSupportedException(fullName + " is not supported by MockEngine!");
		}

		internal static object TranslateToMockObject(Type type, PSObject psObject)
		{
			return MockObjectInformation.GetCreator(type.FullName).TranslateToMockObject(type, psObject);
		}
	}
}

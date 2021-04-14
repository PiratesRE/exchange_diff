using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAirSyncUser
	{
		IAirSyncContext Context { get; }

		Guid DeviceBehaviorCacheGuid { get; }

		IStandardBudget Budget { get; }

		IIdentity Identity { get; }

		WindowsIdentity WindowsIdentity { get; }

		byte[] SID { get; }

		ExchangePrincipal ExchangePrincipal { get; }

		WindowsPrincipal WindowsPrincipal { get; }

		bool IsEnabled { get; }

		ActiveSyncMiniRecipient ADUser { get; }

		OrganizationId OrganizationId { get; }

		bool MailboxIsOnE12Server { get; set; }

		bool IsMonitoringTestUser { get; }

		ClientSecurityContextWrapper ClientSecurityContextWrapper { get; }

		string Name { get; }

		string ServerFullyQualifiedDomainName { get; }

		Guid MailboxGuid { get; }

		string DisplayName { get; }

		string SmtpAddress { get; }

		bool IrmEnabled { get; }

		string WindowsLiveId { get; }

		IEasFeaturesManager Features { get; }

		BudgetKey BudgetKey { get; }

		bool IsConsumerOrganizationUser { get; }

		BackOffValue GetBudgetBackOffValue();

		void DisposeBudget();

		void PrepareToHang();

		void AcquireBudget();

		void SetBudgetDiagnosticValues(bool start);

		void InitializeADUser();

		ITokenBucket GetBudgetTokenBucket();
	}
}

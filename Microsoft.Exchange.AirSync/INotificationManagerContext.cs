using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.AirSync
{
	internal interface INotificationManagerContext
	{
		BudgetKey BudgetKey { get; }

		string SmtpAddress { get; }

		DeviceIdentity DeviceIdentity { get; }

		Guid MdbGuid { get; }

		int MailboxPolicyHash { get; }

		uint PolicyKey { get; }

		int AirSyncVersion { get; }

		ExDateTime RequestTime { get; }

		Guid MailboxGuid { get; }

		CommandType CommandType { get; }

		IActivityScope ActivityScope { get; set; }

		IAccountValidationContext AccountValidationContext { get; }
	}
}

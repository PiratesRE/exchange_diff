using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal abstract class ExComplianceItemContainer : ComplianceItemContainer
	{
		internal abstract MailboxSession Session { get; }

		internal abstract ComplianceItemPagedReader ComplianceItemPagedReader { get; }
	}
}

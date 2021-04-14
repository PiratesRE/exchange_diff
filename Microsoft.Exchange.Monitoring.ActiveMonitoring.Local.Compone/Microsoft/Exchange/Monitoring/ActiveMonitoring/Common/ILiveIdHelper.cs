using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal interface ILiveIdHelper
	{
		NetID CreateMember(SmtpAddress smtpAddress, IConfigurationSession session, string password);

		NetID GetMember(SmtpAddress smtpAddress, IConfigurationSession session);

		void ResetPassword(SmtpAddress smtpAddress, NetID netId, IConfigurationSession session, string password);
	}
}

using System;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal interface IConnectionSettingsWriteProvider
	{
		string SourceId { get; }

		bool SetConnectionSettingsMatchingEmail(SmtpAddress email, ConnectionSettings connectionSettings);
	}
}

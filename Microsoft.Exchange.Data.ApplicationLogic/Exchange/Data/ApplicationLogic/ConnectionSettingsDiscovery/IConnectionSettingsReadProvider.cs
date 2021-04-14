using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal interface IConnectionSettingsReadProvider
	{
		string SourceId { get; }

		IEnumerable<ConnectionSettings> GetConnectionSettingsMatchingEmail(SmtpAddress email);
	}
}

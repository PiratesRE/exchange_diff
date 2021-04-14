using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal interface ISubjectFactory
	{
		ISubject CreateSubject(string serverName);
	}
}

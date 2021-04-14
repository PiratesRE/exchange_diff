using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class SubjectFactory : ISubjectFactory
	{
		public ISubject CreateSubject(string serverName)
		{
			return new Subject(serverName);
		}
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public interface IPop3Client : IDisposable
	{
		void Connect(string server, int port, string username, string password, bool enableSsl, int readTimeout);

		void Disconnect();

		List<Pop3Message> List();

		void RetrieveHeader(Pop3Message message);

		void Retrieve(Pop3Message message);

		void Retrieve(List<Pop3Message> messages);

		void Delete(Pop3Message message);
	}
}

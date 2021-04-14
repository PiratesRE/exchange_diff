using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IMailboxDatabaseCollectionManager : IDisposable
	{
		IMailboxDatabaseConnectionManager GetConnectionManager(Guid mdbGuid);

		XElement GetDiagnosticInfo(XElement parentElement);

		void UpdateMdbThreadCounters();
	}
}

using System;
using System.Xml.Linq;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DatabaseHealthBreadcrumb
	{
		internal DatabaseHealthBreadcrumb()
		{
			this.RecordCreation = ExDateTime.UtcNow;
		}

		internal ExDateTime RecordCreation { get; set; }

		internal int DatabaseHealth { get; set; }

		internal Guid DatabaseGuid { get; set; }

		public override string ToString()
		{
			return string.Format("Created: {0}, Database: {1}, DatabaseHealth: {2}", this.RecordCreation, this.DatabaseGuid, this.DatabaseHealth);
		}

		public XElement GetDiagnosticInfo()
		{
			return new XElement("HealthEntry", new object[]
			{
				new XElement("creationTimestamp", this.RecordCreation),
				new XElement("DatabaseGuid", this.DatabaseGuid),
				new XElement("DatabaseHealth", this.DatabaseHealth)
			});
		}
	}
}

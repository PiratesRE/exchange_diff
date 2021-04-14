using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class Breadcrumb
	{
		public BreadcrumbKind Kind { get; set; }

		public byte Source { get; set; }

		public byte Operation { get; set; }

		public byte Client { get; set; }

		public int Database { get; set; }

		public int Mailbox { get; set; }

		public DateTime Time { get; set; }

		public int DataValue { get; set; }

		public object DataObject { get; set; }
	}
}

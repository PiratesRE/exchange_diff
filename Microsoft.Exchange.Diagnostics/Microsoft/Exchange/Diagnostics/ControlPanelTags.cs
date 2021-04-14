using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ControlPanelTags
	{
		public const int EventLog = 0;

		public const int RBAC = 1;

		public const int Proxy = 2;

		public const int Redirect = 3;

		public const int WebService = 4;

		public const int Performance = 5;

		public const int UserPhotos = 6;

		public const int DDI = 7;

		public const int LinkedIn = 8;

		public static Guid guid = new Guid("EDD5672C-EB31-485A-9880-6E1F3BFCE4EB");
	}
}

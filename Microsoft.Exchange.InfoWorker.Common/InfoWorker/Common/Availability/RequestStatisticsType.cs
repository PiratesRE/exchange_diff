using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal struct RequestStatisticsType
	{
		private RequestStatisticsType(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public static readonly RequestStatisticsType LocalElapsedTimeLongPole = new RequestStatisticsType("LLPE");

		public static readonly RequestStatisticsType LocalFirstThreadExecute = new RequestStatisticsType("LFTE");

		public static readonly RequestStatisticsType RequestCPUMain = new RequestStatisticsType("RCM");

		public static readonly RequestStatisticsType AD = new RequestStatisticsType("AD");

		public static readonly RequestStatisticsType MailboxRPC = new RequestStatisticsType("MRPC");

		public static readonly RequestStatisticsType ThreadCPULongPole = new RequestStatisticsType("TCLP");

		public static readonly RequestStatisticsType Local = new RequestStatisticsType("LLP");

		public static readonly RequestStatisticsType TotalLocal = new RequestStatisticsType("LT");

		public static readonly RequestStatisticsType FederatedToken = new RequestStatisticsType("FTCL");

		public static readonly RequestStatisticsType AutoDiscoverRequest = new RequestStatisticsType("ADLPR");

		public static readonly RequestStatisticsType IntraSiteProxy = new RequestStatisticsType("ISLP");

		public static readonly RequestStatisticsType CrossSiteProxy = new RequestStatisticsType("CSLP");

		public static readonly RequestStatisticsType CrossForestProxy = new RequestStatisticsType("CFLP");

		public static readonly RequestStatisticsType FederatedProxy = new RequestStatisticsType("FCFLP");

		public static readonly RequestStatisticsType OAuthProxy = new RequestStatisticsType("OCFLP");

		private string name;
	}
}

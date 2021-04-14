using System;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[Serializable]
	public class HaRpcExceptionInfo
	{
		internal string OriginatingServer { get; set; }

		internal string OriginatingStackTrace { get; set; }

		public string ErrorMessage { get; set; }

		internal string DatabaseName { get; set; }
	}
}

using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ArchiveConnectivityResult
	{
		public ArchiveConnectivityResult()
		{
		}

		public ArchiveConnectivityResult(ArchiveConnectivityResultEnum result)
		{
			this.result = result;
		}

		public ArchiveConnectivityResultEnum Value
		{
			get
			{
				return this.result;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			switch (this.result)
			{
			case ArchiveConnectivityResultEnum.Undefined:
				text = Strings.ArchiveConnectivityResultUndefined;
				break;
			case ArchiveConnectivityResultEnum.Success:
				text = Strings.ArchiveConnectivityResultSuccess;
				break;
			case ArchiveConnectivityResultEnum.ArchiveFailure:
				text = Strings.LogonFailure;
				break;
			case ArchiveConnectivityResultEnum.PrimaryFailure:
				text = Strings.ArchiveConnectivityResultPrimaryFailure;
				break;
			}
			return text;
		}

		private ArchiveConnectivityResultEnum result;
	}
}

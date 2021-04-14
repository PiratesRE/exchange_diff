using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LicensingRpcResults : RpcParameters
	{
		public OverallRpcResult OverallRpcResult
		{
			get
			{
				if (this.overallRpcResult == null)
				{
					this.overallRpcResult = (base.GetParameterValue("OverallRpcResult") as OverallRpcResult);
					if (this.overallRpcResult == null)
					{
						throw new ArgumentNullException("OverallRpcResult");
					}
				}
				return this.overallRpcResult;
			}
		}

		public LicensingRpcResults(byte[] data) : base(data)
		{
		}

		public LicensingRpcResults(OverallRpcResult overallRpcResult)
		{
			if (overallRpcResult == null)
			{
				throw new ArgumentNullException("overallRpcResult");
			}
			base.SetParameterValue("OverallRpcResult", overallRpcResult);
		}

		private const string OverallRpcResultParameterName = "OverallRpcResult";

		private OverallRpcResult overallRpcResult;
	}
}

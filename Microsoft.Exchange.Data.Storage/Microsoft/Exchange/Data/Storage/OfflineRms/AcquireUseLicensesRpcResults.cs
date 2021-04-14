using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcquireUseLicensesRpcResults : LicensingRpcResults
	{
		public UseLicenseRpcResult[] UseLicenseRpcResults
		{
			get
			{
				if (this.useLicenseRpcResults == null)
				{
					this.useLicenseRpcResults = (base.GetParameterValue("UseLicenseRpcResult") as UseLicenseRpcResult[]);
				}
				return this.useLicenseRpcResults;
			}
		}

		public AcquireUseLicensesRpcResults(byte[] data) : base(data)
		{
		}

		public AcquireUseLicensesRpcResults(OverallRpcResult overallRpcResult, UseLicenseResult[] originalResults) : base(overallRpcResult)
		{
			if (overallRpcResult == null)
			{
				throw new ArgumentNullException("overallRpcResult");
			}
			if (originalResults == null)
			{
				throw new ArgumentNullException("originalResults");
			}
			UseLicenseRpcResult[] array = new UseLicenseRpcResult[originalResults.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new UseLicenseRpcResult(originalResults[i]);
			}
			base.SetParameterValue("UseLicenseRpcResult", array);
		}

		private const string UseLicenseRpcResultParameterName = "UseLicenseRpcResult";

		private UseLicenseRpcResult[] useLicenseRpcResults;
	}
}

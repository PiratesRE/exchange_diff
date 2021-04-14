using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetTenantActiveCryptoModeRpcResults : LicensingRpcResults
	{
		public ActiveCryptoModeRpcResult[] ActiveCryptoModeRpcResults
		{
			get
			{
				if (this.activeCryptoModeRpcResult == null)
				{
					this.activeCryptoModeRpcResult = (base.GetParameterValue("GetTenantActiveCryptoModeRpcResult") as ActiveCryptoModeRpcResult[]);
				}
				return this.activeCryptoModeRpcResult;
			}
		}

		public GetTenantActiveCryptoModeRpcResults(byte[] data) : base(data)
		{
		}

		public GetTenantActiveCryptoModeRpcResults(OverallRpcResult overallRpcResult, ActiveCryptoModeResult[] originalResults) : base(overallRpcResult)
		{
			if (overallRpcResult == null)
			{
				throw new ArgumentNullException("overallRpcResult");
			}
			if (originalResults == null)
			{
				throw new ArgumentNullException("originalResults");
			}
			ActiveCryptoModeRpcResult[] array = new ActiveCryptoModeRpcResult[originalResults.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ActiveCryptoModeRpcResult(originalResults[i]);
			}
			base.SetParameterValue("GetTenantActiveCryptoModeRpcResult", array);
		}

		private const string GetTenantActiveCryptoModeResultParameterName = "GetTenantActiveCryptoModeRpcResult";

		private ActiveCryptoModeRpcResult[] activeCryptoModeRpcResult;
	}
}

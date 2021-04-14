using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public abstract class RequestBase : UMVersionedRpcRequest
	{
		public string ProxyAddress
		{
			get
			{
				return this.proxyAddress;
			}
			set
			{
				this.proxyAddress = value;
			}
		}

		public Guid UserObjectGuid
		{
			get
			{
				return this.userObjectGuid;
			}
			set
			{
				this.userObjectGuid = value;
			}
		}

		public Guid TenantGuid { get; set; }

		internal ProcessRequestDelegate ProcessRequest { get; set; }

		internal override UMRpcResponse Execute()
		{
			UMRpcResponse result;
			try
			{
				result = this.ProcessRequest(this);
			}
			catch (LocalizedException exception)
			{
				result = new ErrorResponse(exception);
			}
			return result;
		}

		protected override void LogErrorEvent(Exception exception)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PoPRequestError, null, new object[]
			{
				this.GetFriendlyName(),
				CommonUtil.ToEventLogString(exception)
			});
		}

		private string proxyAddress;

		private Guid userObjectGuid;
	}
}

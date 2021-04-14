using System;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class BaseProvisionCommand<RequestType, ResponseType> : SingleStepServiceCommand<RequestType, ServiceResultNone> where RequestType : BaseRequest where ResponseType : BaseResponseMessage, new()
	{
		private protected string Protocol { protected get; private set; }

		private protected string DeviceType { protected get; private set; }

		private protected string DeviceID { protected get; private set; }

		protected BaseProvisionCommand(CallContext callContext, RequestType request, bool hasPal, string deviceType, string deviceId, bool specifyProtocol, string protocol) : base(callContext, request)
		{
			if (hasPal)
			{
				this.Protocol = "MOWA";
			}
			else if (!specifyProtocol)
			{
				this.Protocol = "DOWA";
			}
			else
			{
				this.Protocol = protocol;
			}
			this.DeviceID = deviceId;
			this.DeviceType = deviceType;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ResponseType responseType = Activator.CreateInstance<ResponseType>();
			responseType.ProcessServiceResult(base.Result);
			return responseType;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			if (!this.IsDeviceIDValid())
			{
				return new ServiceResult<ServiceResultNone>(new ServiceError((CoreResources.IDs)3374101509U, ResponseCodeType.ErrorInvalidArgument, 0, ExchangeVersion.Exchange2012));
			}
			if (!this.IsDeviceTypeValid())
			{
				return new ServiceResult<ServiceResultNone>(new ServiceError(CoreResources.IDs.ErrorInvalidProvisionDeviceType, ResponseCodeType.ErrorInvalidArgument, 0, ExchangeVersion.Exchange2012));
			}
			try
			{
				this.InternalExecute();
			}
			catch (AirSyncPermanentException ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "BaseProvisioningCommand_Execute");
				if (ex.AirSyncStatusCode == StatusCode.MaximumDevicesReached)
				{
					return new ServiceResult<ServiceResultNone>(new ServiceError(CoreResources.IDs.ErrorInternalServerError, ResponseCodeType.ErrorMaximumDevicesReached, 0, ExchangeVersion.Exchange2013));
				}
				return new ServiceResult<ServiceResultNone>(new ServiceError(CoreResources.IDs.ErrorInternalServerError, ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2013));
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		protected abstract void InternalExecute();

		private bool IsDeviceIDValid()
		{
			return !string.IsNullOrEmpty(this.DeviceID) && this.DeviceID.Length <= 32 && this.DeviceID.IndexOf('-') < 0;
		}

		private bool IsDeviceTypeValid()
		{
			return !string.IsNullOrEmpty(this.DeviceType) && this.DeviceType.Length <= 32 && this.DeviceType.IndexOf('-') < 0;
		}
	}
}

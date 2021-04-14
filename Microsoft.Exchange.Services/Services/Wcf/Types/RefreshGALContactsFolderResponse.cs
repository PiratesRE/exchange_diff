using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.People.EntitySets.ResponseTypes;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public sealed class RefreshGALContactsFolderResponse : IExchangeWebMethodResponse
	{
		internal RefreshGALContactsFolderResponse(ServiceResult<RefreshGALFolderResponseEntity> serviceResult)
		{
			this.serviceResult = serviceResult;
		}

		public ResponseType GetResponseType()
		{
			return ResponseType.RefreshGALContactsFolderResponseMessage;
		}

		public ResponseCodeType GetErrorCodeToLog()
		{
			ResponseCodeType result = ResponseCodeType.NoError;
			if (this.serviceResult.Code != ServiceResultCode.Success)
			{
				result = this.serviceResult.Error.MessageKey;
			}
			return result;
		}

		private readonly ServiceResult<RefreshGALFolderResponseEntity> serviceResult;
	}
}

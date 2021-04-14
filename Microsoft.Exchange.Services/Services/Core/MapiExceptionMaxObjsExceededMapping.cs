using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal class MapiExceptionMaxObjsExceededMapping : ExceptionMappingBase
	{
		public MapiExceptionMaxObjsExceededMapping() : base(typeof(MapiExceptionMaxObjsExceeded), ExceptionMappingBase.Attributes.None)
		{
		}

		protected override ResponseCodeType GetResponseCode(LocalizedException exception)
		{
			MapiExceptionMaxObjsExceeded mapiExceptionMaxObjsExceeded = exception as MapiExceptionMaxObjsExceeded;
			if (mapiExceptionMaxObjsExceeded != null && mapiExceptionMaxObjsExceeded.LowLevelError == 1252)
			{
				return ResponseCodeType.ErrorMessagePerFolderCountReceiveQuotaExceeded;
			}
			return ResponseCodeType.ErrorInternalServerError;
		}

		protected override CoreResources.IDs GetResourceId(LocalizedException exception)
		{
			if (this.GetResponseCode(exception) == ResponseCodeType.ErrorMessagePerFolderCountReceiveQuotaExceeded)
			{
				return (CoreResources.IDs)2791864679U;
			}
			return CoreResources.IDs.ErrorInternalServerError;
		}

		protected override ExchangeVersion GetEffectiveVersion(LocalizedException exception)
		{
			if (this.GetResponseCode(exception) == ResponseCodeType.ErrorMessagePerFolderCountReceiveQuotaExceeded)
			{
				return ExchangeVersion.ExchangeV2_14;
			}
			return ExchangeVersion.Exchange2007;
		}
	}
}

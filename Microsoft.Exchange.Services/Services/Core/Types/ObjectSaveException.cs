using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ObjectSaveException : ServicePermanentException
	{
		public ObjectSaveException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorItemSave, messageId, innerException)
		{
		}

		public ObjectSaveException(Exception innerException, bool useItemError) : base(useItemError ? ((CoreResources.IDs)2339310738U) : ((CoreResources.IDs)3867216855U), innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}

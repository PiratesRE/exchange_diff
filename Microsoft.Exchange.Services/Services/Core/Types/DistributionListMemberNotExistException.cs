using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DistributionListMemberNotExistException : ServicePermanentExceptionWithPropertyPath
	{
		public DistributionListMemberNotExistException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorDistributionListMemberNotExist, propertyPath)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}

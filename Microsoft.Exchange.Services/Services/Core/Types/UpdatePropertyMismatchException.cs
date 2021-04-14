using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class UpdatePropertyMismatchException : ServicePermanentExceptionWithPropertyPath
	{
		public UpdatePropertyMismatchException(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorUpdatePropertyMismatch, propertyPath)
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

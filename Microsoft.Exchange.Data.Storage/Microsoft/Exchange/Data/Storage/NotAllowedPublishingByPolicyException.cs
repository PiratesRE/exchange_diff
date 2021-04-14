using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NotAllowedPublishingByPolicyException : StoragePermanentException
	{
		internal int MaxAllowedDetailLevel { get; private set; }

		public NotAllowedPublishingByPolicyException() : base(ServerStrings.NotAllowedAnonymousSharingByPolicy)
		{
			this.MaxAllowedDetailLevel = 0;
		}

		public NotAllowedPublishingByPolicyException(DetailLevelEnumType detailLevel, DetailLevelEnumType maxAllowedDetailLevel) : base(ServerStrings.DetailLevelNotAllowedByPolicy(LocalizedDescriptionAttribute.FromEnum(typeof(DetailLevelEnumType), detailLevel)))
		{
			EnumValidator.ThrowIfInvalid<DetailLevelEnumType>(detailLevel, "detailLevel");
			EnumValidator.ThrowIfInvalid<DetailLevelEnumType>(maxAllowedDetailLevel, "maxAllowedDetailLevel");
			this.MaxAllowedDetailLevel = (int)maxAllowedDetailLevel;
		}

		protected NotAllowedPublishingByPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

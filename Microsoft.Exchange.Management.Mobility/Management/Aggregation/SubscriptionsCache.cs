using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Serializable]
	public class SubscriptionsCache : ConfigurableObject
	{
		public SubscriptionsCache() : base(new SimpleProviderPropertyBag())
		{
		}

		public List<SubscriptionCacheObject> SubscriptionCacheObjects
		{
			get
			{
				return this.cacheObjects;
			}
			internal set
			{
				this.cacheObjects = value;
				if (this.cacheObjects != null)
				{
					foreach (SubscriptionCacheObject subscriptionCacheObject in this.cacheObjects)
					{
						if (subscriptionCacheObject.ObjectState != SubscriptionCacheObjectState.Valid)
						{
							this.invalid = true;
							break;
						}
					}
				}
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public override bool IsValid
		{
			get
			{
				return !this.invalid;
			}
		}

		public ObjectState CacheObjectState
		{
			get
			{
				return base.ObjectState;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SubscriptionsCache.schema;
			}
		}

		internal string FailureReason
		{
			get
			{
				return this.failureReason;
			}
			set
			{
				this.failureReason = value;
			}
		}

		internal void SetIdentity(ADObjectId userIdentity)
		{
			SyncUtilities.ThrowIfArgumentNull("userIdentity", userIdentity);
			this.identity = userIdentity;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (!string.IsNullOrEmpty(this.failureReason))
			{
				ValidationError item = new CacheValidationError(new LocalizedString(this.failureReason));
				errors.Add(item);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!string.IsNullOrEmpty(this.failureReason))
			{
				ValidationError item = new CacheValidationError(new LocalizedString(this.failureReason));
				errors.Add(item);
			}
		}

		private static readonly SimpleProviderObjectSchema schema = ObjectSchema.GetInstance<SimpleProviderObjectSchema>();

		private static readonly ValidationError[] EmptyValidationError = new ValidationError[0];

		private List<SubscriptionCacheObject> cacheObjects;

		private ADObjectId identity;

		private bool invalid;

		private string failureReason;
	}
}

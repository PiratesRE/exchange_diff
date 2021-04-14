using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[Serializable]
	public sealed class ConnectSubscriptionProxy : PimSubscriptionProxy
	{
		public ConnectSubscriptionProxy() : this(new ConnectSubscription())
		{
		}

		internal ConnectSubscriptionProxy(ConnectSubscription subscription) : base(subscription)
		{
		}

		public bool HasAccessToken
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).HasAccessToken;
			}
		}

		public int EncryptedAccessTokenHash
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).EncryptedAccessTokenHash;
			}
		}

		public ConnectState ConnectState
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).ConnectState;
			}
		}

		public string AppId
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).AppId;
			}
			set
			{
				((ConnectSubscription)base.Subscription).AppId = value;
			}
		}

		public string UserId
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).UserId;
			}
			set
			{
				((ConnectSubscription)base.Subscription).UserId = value;
			}
		}

		public bool InitialSyncInRecoveryMode
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).InitialSyncInRecoveryMode;
			}
			internal set
			{
				((ConnectSubscription)base.Subscription).InitialSyncInRecoveryMode = value;
			}
		}

		internal string AppAuthorizationCode
		{
			get
			{
				return this.appAuthorizationCode;
			}
			set
			{
				this.appAuthorizationCode = value;
			}
		}

		internal string MessageClass
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).MessageClass;
			}
			set
			{
				((ConnectSubscription)base.Subscription).MessageClass = value;
			}
		}

		internal Guid ProviderGuid
		{
			get
			{
				return ((ConnectSubscription)base.Subscription).ProviderGuid;
			}
			set
			{
				((ConnectSubscription)base.Subscription).ProviderGuid = value;
			}
		}

		internal string RedirectUri { get; set; }

		internal string RequestToken { get; set; }

		internal string RequestSecret
		{
			get
			{
				return this.requestSecret;
			}
			set
			{
				this.requestSecret = value;
			}
		}

		internal string OAuthVerifier { get; set; }

		public override ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (string.IsNullOrEmpty(this.MessageClass))
			{
				list.Add(new PropertyValidationError(DataStrings.PropertyNotEmptyOrNull, ConnectSubscriptionProxy.ConnectSubscriptionSchema.MessageClass, this.MessageClass));
			}
			if (Guid.Empty.Equals(this.ProviderGuid))
			{
				list.Add(new PropertyValidationError(DataStrings.PropertyNotEmptyOrNull, ConnectSubscriptionProxy.ConnectSubscriptionSchema.ProviderGuid, this.ProviderGuid));
			}
			if (string.IsNullOrEmpty(this.AppId))
			{
				list.Add(new PropertyValidationError(DataStrings.PropertyNotEmptyOrNull, ConnectSubscriptionProxy.ConnectSubscriptionSchema.AppId, this.AppId));
			}
			if (string.IsNullOrEmpty(this.UserId))
			{
				list.Add(new PropertyValidationError(DataStrings.PropertyNotEmptyOrNull, ConnectSubscriptionProxy.ConnectSubscriptionSchema.UserId, this.UserId));
			}
			return base.Validate().Concat(list).ToArray<ValidationError>();
		}

		internal void AssignAccessToken(string accessTokenInClearText)
		{
			((ConnectSubscription)base.Subscription).AccessTokenInClearText = accessTokenInClearText;
		}

		internal void AssignAccessTokenSecret(string accessTokenSecretInClearText)
		{
			((ConnectSubscription)base.Subscription).AccessTokenSecretInClearText = accessTokenSecretInClearText;
		}

		[NonSerialized]
		private string appAuthorizationCode;

		[NonSerialized]
		private string requestSecret;

		private sealed class ConnectSubscriptionSchema : SimpleProviderObjectSchema
		{
			internal static readonly SimpleProviderPropertyDefinition MessageClass = new SimpleProviderPropertyDefinition("MessageClass", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition ProviderGuid = new SimpleProviderPropertyDefinition("ProviderGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition AppId = new SimpleProviderPropertyDefinition("AppId", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition UserId = new SimpleProviderPropertyDefinition("UserId", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}

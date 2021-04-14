using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class ODataPermission
	{
		public static ODataPermission Create(ODataRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			AuthZClientInfo.ApplicationAttachedAuthZClientInfo applicationAttachedAuthZClientInfo = request.ODataContext.CallContext.EffectiveCaller as AuthZClientInfo.ApplicationAttachedAuthZClientInfo;
			if (applicationAttachedAuthZClientInfo != null)
			{
				return new ODataPermission.OAuthRequestPermission(request, applicationAttachedAuthZClientInfo.OAuthIdentity);
			}
			return ODataPermission.FullPermission.Instance;
		}

		public abstract void Check();

		public sealed class FullPermission : ODataPermission
		{
			public static ODataPermission.FullPermission Instance
			{
				get
				{
					return ODataPermission.FullPermission.instance;
				}
			}

			public override void Check()
			{
			}

			private static ODataPermission.FullPermission instance = new ODataPermission.FullPermission();
		}

		public sealed class OAuthRequestPermission : ODataPermission
		{
			public OAuthRequestPermission(ODataRequest request, OAuthIdentity identity)
			{
				this.request = request;
				this.identity = identity;
			}

			public override void Check()
			{
				this.ThrowIfFalse(this.identity.OAuthApplication.ApplicationType == OAuthApplicationType.V1App || this.identity.OAuthApplication.ApplicationType == OAuthApplicationType.V1ExchangeSelfIssuedApp, OAuthErrors.TokenProfileNotApplicable);
				if (!this.identity.IsAppOnly)
				{
					this.ThrowIfFalse(this.identity.ActAsUser.Sid.Equals(this.request.ODataContext.TargetMailbox.Sid), OAuthErrors.AllowAccessOwnMailboxOnly);
				}
				V1ProfileAppInfo v1ProfileApp = this.identity.OAuthApplication.V1ProfileApp;
				string[] array = this.identity.IsAppOnly ? OAuthGrant.ExtractKnownGrantsFromRole(v1ProfileApp.Role) : OAuthGrant.ExtractKnownGrants(v1ProfileApp.Scope);
				this.ThrowIfFalse(array != null && array.Length > 0, OAuthErrors.NoGrantPresented);
				string[] required = ODataPermission.OAuthRequestPermission.dictionary.GetOrAdd(this.request.GetType(), (Type type) => (from AllowedOAuthGrantAttribute x in type.GetCustomAttributes(typeof(AllowedOAuthGrantAttribute), false)
				select x.Grant).ToArray<string>());
				this.ThrowIfFalse(required != null && required.Length > 0, OAuthErrors.NotSupportedWithV1AppToken);
				this.ThrowIfFalse(array.Any((string x) => required.Contains(x)), OAuthErrors.NotEnoughGrantPresented);
				this.request.PerformAdditionalGrantCheck(array);
			}

			private void ThrowIfFalse(bool condition, OAuthErrors inboundError)
			{
				if (!condition)
				{
					throw new ODataAuthorizationException(new InvalidOAuthTokenException(inboundError, null, null));
				}
			}

			private static ConcurrentDictionary<Type, string[]> dictionary = new ConcurrentDictionary<Type, string[]>();

			private readonly ODataRequest request;

			private readonly OAuthIdentity identity;
		}
	}
}

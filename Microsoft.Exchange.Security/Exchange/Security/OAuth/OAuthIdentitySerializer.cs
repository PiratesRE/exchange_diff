using System;
using System.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthIdentitySerializer
	{
		public static string SerializeOAuthIdentity(OAuthIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			SerializedOAuthIdentity serializedOAuthIdentity = new SerializedOAuthIdentity();
			if (!identity.IsAppOnly)
			{
				serializedOAuthIdentity.UserDn = identity.ActAsUser.DistinguishedName;
			}
			if (identity.OAuthApplication.PartnerApplication != null)
			{
				serializedOAuthIdentity.PartnerApplicationDn = identity.OAuthApplication.PartnerApplication.DistinguishedName;
				serializedOAuthIdentity.PartnerApplicationAppId = identity.OAuthApplication.PartnerApplication.ApplicationIdentifier;
				serializedOAuthIdentity.PartnerApplicationRealm = identity.OAuthApplication.PartnerApplication.Realm;
			}
			if (identity.OAuthApplication.OfficeExtension != null)
			{
				serializedOAuthIdentity.OfficeExtensionId = identity.OAuthApplication.OfficeExtension.ExtensionId;
				serializedOAuthIdentity.Scope = identity.OAuthApplication.OfficeExtension.Scope;
			}
			if (identity.OAuthApplication.V1ProfileApp != null)
			{
				serializedOAuthIdentity.V1ProfileAppId = identity.OAuthApplication.V1ProfileApp.AppId;
				serializedOAuthIdentity.Scope = identity.OAuthApplication.V1ProfileApp.Scope;
			}
			if (identity.OAuthApplication.IsFromSameOrgExchange != null)
			{
				serializedOAuthIdentity.IsFromSameOrgExchange = identity.OAuthApplication.IsFromSameOrgExchange.Value.ToString();
			}
			if (identity.OrganizationId.ConfigurationUnit != null)
			{
				serializedOAuthIdentity.OrganizationName = identity.OrganizationId.ConfigurationUnit.Parent.Name;
				serializedOAuthIdentity.OrganizationIdBase64 = CommonAccessTokenAccessor.SerializeOrganizationId(identity.OrganizationId);
			}
			return serializedOAuthIdentity.SerializeToJson();
		}

		public static CommonAccessToken ConvertToCommonAccessToken(OAuthIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			string value = OAuthIdentitySerializer.SerializeOAuthIdentity(identity);
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.OAuth);
			commonAccessToken.ExtensionData["OAuthData"] = value;
			return commonAccessToken;
		}

		public static OAuthIdentity ConvertFromCommonAccessToken(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "[OAuthIdentitySerializer::ConvertFromCommonAccessToken] Token type is {0}.", token.TokenType);
			if (!AccessTokenType.OAuth.ToString().Equals(token.TokenType, StringComparison.OrdinalIgnoreCase))
			{
				string message = string.Format("Unexpect token type {0}.", token.TokenType);
				throw new OAuthIdentityDeserializationException(message);
			}
			string text = token.ExtensionData["OAuthData"];
			ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "[OAuthIdentitySerializer::ConvertFromCommonAccessToken] OAuthData: {0}.", text);
			if (string.IsNullOrEmpty(text))
			{
				throw new OAuthIdentityDeserializationException("The access token does not contain OAuthData.");
			}
			return OAuthIdentitySerializer.DeserializeOAuthIdentity(text);
		}

		public static OAuthIdentity DeserializeOAuthIdentity(string blob)
		{
			if (string.IsNullOrEmpty(blob))
			{
				throw new ArgumentNullException("blob");
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "[OAuthIdentitySerializer::DeserializeOAuthIdentity] Deserializing OAuth identity string blob {0}", blob);
			SerializedOAuthIdentity serializedId = null;
			Exception ex = null;
			try
			{
				serializedId = blob.DeserializeFromJson<SerializedOAuthIdentity>();
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.OAuthTracer.TraceError<Exception>(0L, "[OAuthIdentitySerializer::DeserializeOAuthIdentity] Unable to deserialize the OAuth identity. Error: {0}", ex);
				throw new OAuthIdentityDeserializationException("Unable to deserialize the OAuth identity.", ex);
			}
			OAuthIdentity result = null;
			try
			{
				result = OAuthIdentitySerializer.RehydrateOAuthIdentity(serializedId);
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			catch (ADTransientException ex5)
			{
				ex = ex5;
			}
			catch (DataValidationException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				ExTraceGlobals.OAuthTracer.TraceError<Exception>(0L, "[OAuthIdentitySerializer::DeserializeOAuthIdentity] Error occurred during directory operation. Error: {0}", ex);
				throw new OAuthIdentityDeserializationException("Error occurred during directory operation.", ex);
			}
			return result;
		}

		private static OAuthIdentity RehydrateOAuthIdentity(SerializedOAuthIdentity serializedId)
		{
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			ADSessionSettings adsessionSettings;
			if (!string.IsNullOrEmpty(serializedId.OrganizationIdBase64))
			{
				organizationId = CommonAccessTokenAccessor.DeserializeOrganizationId(serializedId.OrganizationIdBase64);
				try
				{
					adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
					goto IL_82;
				}
				catch (CannotResolveTenantNameException inner)
				{
					throw new OAuthIdentityDeserializationException(string.Format("Cannot resolve tenant {0}.", serializedId.OrganizationName), inner);
				}
			}
			if (!string.IsNullOrEmpty(serializedId.OrganizationName))
			{
				try
				{
					adsessionSettings = ADSessionSettings.FromTenantCUName(serializedId.OrganizationName);
					organizationId = adsessionSettings.CurrentOrganizationId;
					goto IL_82;
				}
				catch (CannotResolveTenantNameException inner2)
				{
					throw new OAuthIdentityDeserializationException(string.Format("Cannot resolve tenant {0}.", serializedId.OrganizationName), inner2);
				}
			}
			adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_82:
			OAuthApplication oauthApplication;
			if (!string.IsNullOrEmpty(serializedId.V1ProfileAppId) && !string.IsNullOrEmpty(serializedId.PartnerApplicationAppId))
			{
				PartnerApplication partnerApplication = OAuthIdentitySerializer.PartnerApplicationCache.Instance.Get(new OAuthIdentitySerializer.PartnerApplicationCacheKey(serializedId));
				oauthApplication = new OAuthApplication(new V1ProfileAppInfo(serializedId.V1ProfileAppId, serializedId.Scope, null), partnerApplication);
			}
			else if (!string.IsNullOrEmpty(serializedId.V1ProfileAppId))
			{
				oauthApplication = new OAuthApplication(new V1ProfileAppInfo(serializedId.V1ProfileAppId, serializedId.Scope, null));
			}
			else if (!string.IsNullOrEmpty(serializedId.OfficeExtensionId))
			{
				oauthApplication = new OAuthApplication(new OfficeExtensionInfo(serializedId.OfficeExtensionId, serializedId.Scope));
			}
			else
			{
				oauthApplication = new OAuthApplication(OAuthIdentitySerializer.PartnerApplicationCache.Instance.Get(new OAuthIdentitySerializer.PartnerApplicationCacheKey(serializedId)));
			}
			if (!string.IsNullOrEmpty(serializedId.IsFromSameOrgExchange))
			{
				oauthApplication.IsFromSameOrgExchange = new bool?(string.Equals(bool.TrueString, serializedId.IsFromSameOrgExchange));
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "[OAuthIdentitySerializer::RehydrateOAuthIdentity] The resolved OAuthApplication object is {0}.", oauthApplication.Id);
			MiniRecipient miniRecipient = null;
			if (!string.IsNullOrEmpty(serializedId.UserDn))
			{
				miniRecipient = OAuthIdentitySerializer.UserCache.Instance.Get(new OAuthIdentitySerializer.UserCacheKey(serializedId));
				ExTraceGlobals.OAuthTracer.TraceDebug<ObjectId>(0L, "[OAuthIdentitySerializer::RehydrateOAuthIdentity] The resolved user object is {0}.", miniRecipient.Identity);
			}
			return OAuthIdentity.Create(organizationId, oauthApplication, OAuthActAsUser.CreateFromMiniRecipient(organizationId, miniRecipient));
		}

		internal sealed class PartnerApplicationCacheKey
		{
			public PartnerApplicationCacheKey(SerializedOAuthIdentity serializedOAuthIdentity) : this(serializedOAuthIdentity.PartnerApplicationDn, serializedOAuthIdentity.PartnerApplicationAppId, serializedOAuthIdentity.PartnerApplicationRealm)
			{
			}

			public PartnerApplicationCacheKey(string dn, string appId, string realm)
			{
				this.partnerApplicationDn = dn;
				this.partnerApplicationAppId = appId;
				this.partnerApplicationRealm = realm;
			}

			public string PartnerApplicationDn
			{
				get
				{
					return this.partnerApplicationDn;
				}
			}

			public string PartnerApplicationAppId
			{
				get
				{
					return this.partnerApplicationAppId;
				}
			}

			public string PartnerApplicationRealm
			{
				get
				{
					return this.partnerApplicationRealm;
				}
			}

			public override bool Equals(object obj)
			{
				OAuthIdentitySerializer.PartnerApplicationCacheKey partnerApplicationCacheKey = obj as OAuthIdentitySerializer.PartnerApplicationCacheKey;
				return partnerApplicationCacheKey != null && (string.Equals(this.partnerApplicationDn, partnerApplicationCacheKey.partnerApplicationDn, StringComparison.OrdinalIgnoreCase) && string.Equals(this.partnerApplicationAppId, partnerApplicationCacheKey.partnerApplicationAppId, StringComparison.OrdinalIgnoreCase)) && string.Equals(this.partnerApplicationRealm, partnerApplicationCacheKey.partnerApplicationRealm, StringComparison.OrdinalIgnoreCase);
			}

			public override int GetHashCode()
			{
				int num = this.partnerApplicationDn.GetHashCodeCaseInsensitive();
				num ^= this.partnerApplicationAppId.GetHashCodeCaseInsensitive();
				if (this.partnerApplicationRealm != null)
				{
					num ^= this.partnerApplicationRealm.GetHashCodeCaseInsensitive();
				}
				return num;
			}

			private readonly string partnerApplicationDn;

			private readonly string partnerApplicationAppId;

			private readonly string partnerApplicationRealm;
		}

		internal sealed class PartnerApplicationCache : LazyLookupTimeoutCache<OAuthIdentitySerializer.PartnerApplicationCacheKey, PartnerApplication>
		{
			private PartnerApplicationCache() : base(2, OAuthIdentitySerializer.PartnerApplicationCache.cacheSize.Value, false, OAuthIdentitySerializer.PartnerApplicationCache.cacheTimeToLive.Value)
			{
			}

			public static OAuthIdentitySerializer.PartnerApplicationCache Instance
			{
				get
				{
					return OAuthIdentitySerializer.PartnerApplicationCache.instance;
				}
			}

			protected override PartnerApplication CreateOnCacheMiss(OAuthIdentitySerializer.PartnerApplicationCacheKey key, ref bool shouldAdd)
			{
				shouldAdd = true;
				ADObjectId adobjectId = new ADObjectId(key.PartnerApplicationDn);
				ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(adobjectId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, adsessionSettings, 454, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\OAuthIdentitySerializer.cs");
				PartnerApplication partnerApplication = tenantOrTopologyConfigurationSession.Read<PartnerApplication>(adobjectId);
				if (partnerApplication == null && adsessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug<ADObjectId>(0L, "[PartnerApplicationCache:CreateOnCacheMiss] Could not find the PartnerApplication object {0}. Trying to search by ApplicationId and Realm.", adobjectId);
					if (DateTime.UtcNow - this.lastRetrieveTime > OAuthIdentitySerializer.PartnerApplicationCache.cacheTimeToLive.Value)
					{
						ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[PartnerApplicationCache:CreateOnCacheMiss] Refreshing datacenter level PAs");
						this.firstOrgPartnerApps = tenantOrTopologyConfigurationSession.Find<PartnerApplication>(null, QueryScope.SubTree, null, null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize);
						this.lastRetrieveTime += OAuthIdentitySerializer.PartnerApplicationCache.cacheTimeToLive.Value;
					}
					if (this.firstOrgPartnerApps != null)
					{
						PartnerApplication[] array = (from pa in this.firstOrgPartnerApps
						where OAuthCommon.IsIdMatch(pa.ApplicationIdentifier, key.PartnerApplicationAppId) && OAuthCommon.IsRealmMatch(pa.Realm, key.PartnerApplicationRealm)
						select pa).ToArray<PartnerApplication>();
						if (array.Length != 1)
						{
							ExTraceGlobals.OAuthTracer.TraceError<int, string, string>(0L, "[PartnerApplicationCache:CreateOnCacheMiss] Found {0} matched Partner Application object with id '{1}', realm '{2}'", array.Length, key.PartnerApplicationAppId, key.PartnerApplicationRealm);
						}
						partnerApplication = array[0];
					}
				}
				if (partnerApplication == null)
				{
					throw new OAuthIdentityDeserializationException(string.Format("Unabled to retrieve Partner Application object '{0}'.", key.PartnerApplicationDn));
				}
				return partnerApplication;
			}

			private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("BackendPartnerApplicationCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromHours(30.0), ExTraceGlobals.OAuthTracer);

			private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("BackendPartnerApplicationCacheMaxItems", 500, ExTraceGlobals.OAuthTracer);

			private static OAuthIdentitySerializer.PartnerApplicationCache instance = new OAuthIdentitySerializer.PartnerApplicationCache();

			private DateTime lastRetrieveTime = DateTime.MinValue;

			private PartnerApplication[] firstOrgPartnerApps;
		}

		internal sealed class UserCacheKey
		{
			public UserCacheKey(SerializedOAuthIdentity serializedOAuthIdentity)
			{
				this.organizationName = serializedOAuthIdentity.OrganizationName;
				this.userDn = serializedOAuthIdentity.UserDn;
			}

			public override bool Equals(object obj)
			{
				OAuthIdentitySerializer.UserCacheKey userCacheKey = obj as OAuthIdentitySerializer.UserCacheKey;
				return userCacheKey != null && string.Equals(this.organizationName, userCacheKey.organizationName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.userDn, userCacheKey.userDn, StringComparison.OrdinalIgnoreCase);
			}

			public override int GetHashCode()
			{
				int num = this.userDn.GetHashCodeCaseInsensitive();
				if (this.organizationName != null)
				{
					num ^= this.userDn.GetHashCodeCaseInsensitive();
				}
				return num;
			}

			public MiniRecipient ResolveMiniRecipient()
			{
				ADSessionSettings sessionSettings = null;
				if (string.IsNullOrEmpty(this.organizationName))
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				else
				{
					try
					{
						sessionSettings = ADSessionSettings.FromTenantCUName(this.organizationName);
					}
					catch (CannotResolveTenantNameException inner)
					{
						throw new OAuthIdentityDeserializationException(string.Format("Cannot resolve tenant {0}.", this.organizationName), inner);
					}
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 582, "ResolveMiniRecipient", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\OAuthIdentitySerializer.cs");
				MiniRecipient miniRecipient = tenantOrRootOrgRecipientSession.ReadMiniRecipient(new ADObjectId(this.userDn), null);
				if (miniRecipient == null)
				{
					throw new OAuthIdentityDeserializationException(string.Format("Unabled to retrieve user '{0}'.", this.userDn));
				}
				return miniRecipient;
			}

			private readonly string organizationName;

			private readonly string userDn;
		}

		internal sealed class UserCache : LazyLookupTimeoutCache<OAuthIdentitySerializer.UserCacheKey, MiniRecipient>
		{
			private UserCache() : base(2, OAuthIdentitySerializer.UserCache.cacheSize.Value, false, OAuthIdentitySerializer.UserCache.cacheTimeToLive.Value)
			{
			}

			public static OAuthIdentitySerializer.UserCache Instance
			{
				get
				{
					return OAuthIdentitySerializer.UserCache.instance;
				}
			}

			protected override MiniRecipient CreateOnCacheMiss(OAuthIdentitySerializer.UserCacheKey key, ref bool shouldAdd)
			{
				shouldAdd = true;
				return key.ResolveMiniRecipient();
			}

			private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("BackendUserCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromHours(30.0), ExTraceGlobals.OAuthTracer);

			private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("BackendUserCacheMaxItems", 2000, ExTraceGlobals.OAuthTracer);

			private static readonly TimeSpan AbsoluteLiveTime = TimeSpan.FromMinutes(30.0);

			private static OAuthIdentitySerializer.UserCache instance = new OAuthIdentitySerializer.UserCache();
		}
	}
}

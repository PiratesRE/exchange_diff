using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal abstract class BackendAuthenticator
	{
		protected abstract string[] RequiredFields { get; }

		public static void GetAuthIdentifier(CommonAccessToken token, ref BackendAuthenticator authenticator, out string authIdentifier)
		{
			authIdentifier = null;
			if (authenticator == null)
			{
				authenticator = BackendAuthenticator.GetAuthenticator(token);
			}
			authenticator.InternalGetAuthIdentifier(token, out authIdentifier);
		}

		public static void Rehydrate(CommonAccessToken token, ref BackendAuthenticator authenticator, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
		{
			IAccountValidationContext accountValidationContext = null;
			BackendAuthenticator.Rehydrate(token, ref authenticator, wantAuthIdentifier, out authIdentifier, out principal, ref accountValidationContext);
		}

		public static void Rehydrate(CommonAccessToken token, ref BackendAuthenticator authenticator, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
		{
			authIdentifier = null;
			principal = null;
			if (authenticator == null)
			{
				authenticator = BackendAuthenticator.GetAuthenticator(token);
			}
			if (authenticator is BackendAuthenticator.LiveIdBasicAuthenticator)
			{
				authenticator.InternalRehydrate(token, wantAuthIdentifier, out authIdentifier, out principal, ref accountValidationContext);
				return;
			}
			authenticator.InternalRehydrate(token, wantAuthIdentifier, out authIdentifier, out principal);
		}

		protected abstract void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier);

		protected abstract void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal);

		protected abstract void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext);

		protected virtual BackendAuthenticator InternalGetAuthenticator(CommonAccessToken token)
		{
			return this;
		}

		protected virtual void ValidateToken(CommonAccessToken token)
		{
			if (this.RequiredFields != null)
			{
				foreach (string key in this.RequiredFields)
				{
					if (!token.ExtensionData.ContainsKey(key))
					{
						throw new BackendRehydrationException(SecurityStrings.MissingExtensionDataKey(key));
					}
				}
			}
		}

		protected static string GetNonEmptyValue(CommonAccessToken token, string key)
		{
			string text;
			if (!token.ExtensionData.TryGetValue(key, out text) || string.IsNullOrEmpty(text))
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey(key));
			}
			return text;
		}

		protected static OrganizationId ExtractOrganizationId(CommonAccessToken token)
		{
			OrganizationId result;
			try
			{
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "OrganizationIdBase64");
				result = CommonAccessTokenAccessor.DeserializeOrganizationId(nonEmptyValue);
			}
			catch (ArgumentException innerException)
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("OrganizationIdBase64"), innerException);
			}
			catch (FormatException innerException2)
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("OrganizationIdBase64"), innerException2);
			}
			return result;
		}

		protected static IEnumerable<string> ExtractGroupMembershipSids(CommonAccessToken token)
		{
			IEnumerable<string> result = new List<string>();
			try
			{
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "GroupMembershipSids");
				result = CommonAccessTokenAccessor.DeserializeGroupMembershipSids(nonEmptyValue);
			}
			catch (BackendRehydrationException)
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.ShouldSkipAdfsGroupReadOnFrontend.Enabled)
				{
					throw;
				}
			}
			return result;
		}

		protected static bool ExtractIsPublicSession(CommonAccessToken token)
		{
			bool result;
			try
			{
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "IsPublicSession");
				result = bool.Parse(nonEmptyValue);
			}
			catch (ArgumentException innerException)
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("OrganizationIdBase64"), innerException);
			}
			catch (FormatException innerException2)
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("OrganizationIdBase64"), innerException2);
			}
			return result;
		}

		protected static bool TryGetAuthIdentifierFromUserSid(CommonAccessToken token, out string authIdentifier)
		{
			authIdentifier = null;
			string text;
			if (token.ExtensionData.TryGetValue("UserSid", out text) && !string.IsNullOrEmpty(text))
			{
				authIdentifier = text.ToUpper();
				return true;
			}
			return false;
		}

		protected static bool TryGetAuthIdentifierFromUserSid(SecurityIdentifier securityIdentifier, out string authIdentifier)
		{
			authIdentifier = null;
			if (securityIdentifier != null)
			{
				authIdentifier = securityIdentifier.Value;
				return true;
			}
			return false;
		}

		protected static bool TryGetAuthIdentifierFromUserSid(string userSid, out string authIdentifier)
		{
			authIdentifier = null;
			if (!string.IsNullOrEmpty(userSid))
			{
				authIdentifier = userSid.ToUpper();
				return true;
			}
			return false;
		}

		private static BackendAuthenticator GetAuthenticator(CommonAccessToken token)
		{
			BackendAuthenticator backendAuthenticator = null;
			AccessTokenType key;
			if (Enum.TryParse<AccessTokenType>(token.TokenType, true, out key) && BackendAuthenticator.Authenticators.TryGetValue(key, out backendAuthenticator))
			{
				backendAuthenticator = backendAuthenticator.InternalGetAuthenticator(token);
				backendAuthenticator.ValidateToken(token);
				return backendAuthenticator;
			}
			throw new BackendRehydrationException(SecurityStrings.AccessTokenTypeNotSupported(token.TokenType));
		}

		protected const int MinAuthIdentifierCachePartitions = 1;

		protected const int MinAuthIdentifierCacheBuckets = 2;

		protected const int MinAuthIdentifierCacheLifetime = 60;

		protected const int MaxAuthIdentifierCachePartitions = 1024;

		protected const int MaxAuthIdentifierCacheBuckets = 100;

		protected const int MaxAuthIdentifierCacheLifetime = 86400;

		protected const int DefaultAuthIdentifierCachePartitions = 32;

		protected const int DefaultAuthIdentifierCacheBuckets = 5;

		protected const int DefaultAuthIdentifierCacheLifetime = 900;

		protected const bool DefaultAuthIdentifierCacheEnabled = true;

		protected static string[] EmptyStringArray = new string[0];

		private static readonly Dictionary<AccessTokenType, BackendAuthenticator> Authenticators = new Dictionary<AccessTokenType, BackendAuthenticator>
		{
			{
				AccessTokenType.Windows,
				new BackendAuthenticator.WindowsAuthenticator()
			},
			{
				AccessTokenType.LiveId,
				new BackendAuthenticator.LiveIdAuthenticator()
			},
			{
				AccessTokenType.LiveIdBasic,
				new BackendAuthenticator.LiveIdBasicAuthenticator()
			},
			{
				AccessTokenType.LiveIdNego2,
				new BackendAuthenticator.LiveIdNego2Authenticator()
			},
			{
				AccessTokenType.OAuth,
				new BackendAuthenticator.OAuthAuthenticator()
			},
			{
				AccessTokenType.Adfs,
				new BackendAuthenticator.AdfsAuthenticator()
			},
			{
				AccessTokenType.CertificateSid,
				new BackendAuthenticator.CertificateSidAuthenticator()
			},
			{
				AccessTokenType.RemotePowerShellDelegated,
				new BackendAuthenticator.RemotePowerShellDelegatedAuthenticator()
			},
			{
				AccessTokenType.Anonymous,
				new BackendAuthenticator.AnonymousAuthenticator()
			},
			{
				AccessTokenType.CompositeIdentity,
				new CompositeIdentityAuthenticator()
			}
		};

		private static BoolAppSettingsEntry RehydrateSidOAuthIdentity = new BoolAppSettingsEntry("OAuthAuthenticator.RehydrateSidOAuthIdentity", false, ExTraceGlobals.BackendRehydrationTracer);

		private sealed class AdfsAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.AdfsAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "UserPrincipalName");
				string nonEmptyValue2 = BackendAuthenticator.GetNonEmptyValue(token, "UserSid");
				OrganizationId organizationId = BackendAuthenticator.ExtractOrganizationId(token);
				IEnumerable<string> groupSidIds = BackendAuthenticator.ExtractGroupMembershipSids(token);
				bool isPublicSession = BackendAuthenticator.ExtractIsPublicSession(token);
				AdfsIdentity identity = new AdfsIdentity(nonEmptyValue, nonEmptyValue2, organizationId, organizationId.PartitionId.ToString(), groupSidIds, isPublicSession);
				if (wantAuthIdentifier && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
				principal = new GenericPrincipal(identity, null);
			}

			private static string[] requiredFields = new string[]
			{
				"UserSid",
				"UserPrincipalName",
				"OrganizationIdBase64",
				"GroupMembershipSids"
			};
		}

		private sealed class AnonymousAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return null;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				authIdentifier = null;
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
			}
		}

		private sealed class CertificateSidAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.CertificateSidAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				authIdentifier = null;
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				string text = token.ExtensionData["UserSid"];
				if (token.ExtensionData.ContainsKey("CertificateSubject"))
				{
					string text2;
					string text3;
					if (BackendAuthenticator.CertificateSidAuthenticator.TryToLookUpUserSidByCertificateSubject(token.ExtensionData["CertificateSubject"], out text2, out text3))
					{
						ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string, string>(0L, "[CertificateSidAuthenticator.InternalRehydrate] Replace user sid from {0} to {1}.", text, text2);
						text = text2;
						token.ExtensionData["UserSid"] = text;
					}
					token.ExtensionData.Remove("CertificateSubject");
					HttpContext.Current.Request.Headers["X-CommonAccessToken"] = token.Serialize();
					if (text3 != null)
					{
						HttpContext.Current.Items["AuthenticatedUser"] = text3;
					}
				}
				GenericIdentity identity = new GenericIdentity(text);
				principal = new GenericPrincipal(identity, BackendAuthenticator.EmptyStringArray);
			}

			private static bool TryToLookUpUserSidByCertificateSubject(string certificateSubject, out string userSidByCertSubject, out string userName)
			{
				userSidByCertSubject = null;
				userName = null;
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string>(0L, "[CertificateSidAuthenticator::TryToLookUpUserSidByCertificateSubject] Certificate Subject = {0}.", certificateSubject);
				X509Identifier identifier;
				if (!X509Identifier.TryParse(certificateSubject, out identifier))
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug(0L, "[CertificateSidAuthenticator::TryToLookUpUserSidByCertificateSubject] Invalid certificate subject.");
					return false;
				}
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 148, "TryToLookUpUserSidByCertificateSubject", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\BackendAuthenticator\\CertificateSidAuthenticator.cs");
				ADRawEntry[] array;
				try
				{
					array = tenantOrRootOrgRecipientSession.FindByCertificate(identifier, new PropertyDefinition[]
					{
						ADMailboxRecipientSchema.Sid,
						ADObjectSchema.Name
					});
				}
				catch (Exception arg)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<Exception>(0L, "[CertificateSidAuthenticator::TryToLookUpUserSidByCertificateSubject] Error encountered: {0}", arg);
					return false;
				}
				if (array.Length != 1)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<int>(0L, "[CertificateSidAuthenticator::TryToLookUpUserSidByCertificateSubject] No/Multiple user matching the certificate. users.Length = {0}", array.Length);
					return false;
				}
				ADRawEntry adrawEntry = array[0];
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)adrawEntry[ADMailboxRecipientSchema.Sid];
				if (securityIdentifier == null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug(0L, "[CertificateSidAuthenticator::TryToLookUpUserSidByCertificateSubject] Sid doesn't exist in the user object.");
					return false;
				}
				userSidByCertSubject = securityIdentifier.ToString();
				userName = ((adrawEntry[ADObjectSchema.Name] == null) ? null : adrawEntry[ADObjectSchema.Name].ToString());
				return true;
			}

			private static string[] requiredFields = new string[]
			{
				"UserSid"
			};
		}

		private sealed class RemotePowerShellDelegatedAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.RemotePowerShellDelegatedAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				authIdentifier = null;
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				GenericIdentity identity = new GenericIdentity(token.ExtensionData["DelegatedData"], DelegatedPrincipal.DelegatedAuthenticationType.ToString());
				principal = new GenericPrincipal(identity, BackendAuthenticator.EmptyStringArray);
			}

			private static string[] requiredFields = new string[]
			{
				"DelegatedData"
			};
		}

		private sealed class WindowsAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return null;
				}
			}

			protected override void ValidateToken(CommonAccessToken token)
			{
				if (token.WindowsAccessToken == null)
				{
					throw new BackendRehydrationException(SecurityStrings.MissingWindowsAccessToken);
				}
				base.ValidateToken(token);
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token.WindowsAccessToken.UserSid, out authIdentifier))
				{
					authIdentifier = null;
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				if (wantAuthIdentifier && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token.WindowsAccessToken.UserSid, out authIdentifier))
				{
					authIdentifier = null;
				}
				WindowsTokenIdentity identity = new WindowsTokenIdentity(token.WindowsAccessToken);
				principal = new GenericPrincipal(identity, null);
			}
		}

		private sealed class LiveIdAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LiveIdAuthenticator.requiredFields;
				}
			}

			protected override BackendAuthenticator InternalGetAuthenticator(CommonAccessToken token)
			{
				if (!token.ExtensionData.ContainsKey("OrganizationIdBase64"))
				{
					return BackendAuthenticator.LiveIdAuthenticator.legacyLiveIdAuthenticator.InternalGetAuthenticator(token);
				}
				return base.InternalGetAuthenticator(token);
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "UserPrincipalName");
				string nonEmptyValue2 = BackendAuthenticator.GetNonEmptyValue(token, "UserSid");
				string nonEmptyValue3 = BackendAuthenticator.GetNonEmptyValue(token, "MemberName");
				OrganizationId organizationId = BackendAuthenticator.ExtractOrganizationId(token);
				LiveIdLoginAttributes loginAttributes = this.ExtractLoginAttributes(token);
				LiveIDIdentity liveIDIdentity = new LiveIDIdentity(nonEmptyValue, nonEmptyValue2, nonEmptyValue3, organizationId.PartitionId.ToString(), loginAttributes, null);
				liveIDIdentity.UserOrganizationId = organizationId;
				liveIDIdentity.HasAcceptedAccruals = true;
				string value;
				if (token.ExtensionData.TryGetValue("LiveIdHasAcceptedAccruals", out value) && !string.IsNullOrEmpty(value))
				{
					try
					{
						liveIDIdentity.HasAcceptedAccruals = bool.Parse(value);
					}
					catch (FormatException innerException)
					{
						throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("LiveIdHasAcceptedAccruals"), innerException);
					}
				}
				if (wantAuthIdentifier && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
				principal = new GenericPrincipal(liveIDIdentity, null);
			}

			private LiveIdLoginAttributes ExtractLoginAttributes(CommonAccessToken token)
			{
				uint num = 0U;
				if (token.ExtensionData.ContainsKey("LoginAttributes"))
				{
					num = Convert.ToUInt32(token.ExtensionData["LoginAttributes"]);
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<uint>((long)this.GetHashCode(), "[LiveIdAuthenticator::ExtractLoginAttributes] Found loginAttributes in the common access token. Value = {0}", num);
				}
				else
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceError((long)this.GetHashCode(), "[LiveIdAuthenticator::ExtractLoginAttributes] loginAttributes NOT Found in the common access token. Defaulting to 0");
				}
				return new LiveIdLoginAttributes(num);
			}

			private static BackendAuthenticator.LegacyLiveIdAuthenticator legacyLiveIdAuthenticator = new BackendAuthenticator.LegacyLiveIdAuthenticator();

			private static string[] requiredFields = new string[]
			{
				"UserSid",
				"UserPrincipalName",
				"MemberName",
				"OrganizationIdBase64"
			};
		}

		private sealed class LegacyLiveIdAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LegacyLiveIdAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				string text = token.ExtensionData["UserPrincipalName"];
				string text2 = token.ExtensionData["UserSid"];
				string text3 = token.ExtensionData["MemberName"];
				string text4 = token.ExtensionData["OrganizationName"];
				string partitionId = null;
				if (token.ExtensionData.ContainsKey("Partition"))
				{
					partitionId = token.ExtensionData["Partition"];
				}
				if (string.IsNullOrEmpty(text))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("UserPrincipalName"));
				}
				if (string.IsNullOrEmpty(text2))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("UserSid"));
				}
				if (string.IsNullOrEmpty(text3))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("MemberName"));
				}
				if (string.IsNullOrEmpty(text4))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("OrganizationName"));
				}
				LiveIDIdentity liveIDIdentity = new LiveIDIdentity(text, text2, text3, partitionId, new LiveIdLoginAttributes(0U), null);
				try
				{
					ADSessionSettings adsessionSettings = ADSessionSettings.FromTenantCUName(text4);
					liveIDIdentity.UserOrganizationId = adsessionSettings.CurrentOrganizationId;
				}
				catch (CannotResolveTenantNameException)
				{
					throw new BackendRehydrationException(SecurityStrings.CannotResolveOrganization(text4));
				}
				liveIDIdentity.HasAcceptedAccruals = bool.Parse(token.ExtensionData["LiveIdHasAcceptedAccruals"]);
				OrganizationProperties userOrganizationProperties;
				OrganizationPropertyCache.TryGetOrganizationProperties(liveIDIdentity.UserOrganizationId, out userOrganizationProperties);
				liveIDIdentity.UserOrganizationProperties = userOrganizationProperties;
				if (wantAuthIdentifier && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
				principal = new GenericPrincipal(liveIDIdentity, null);
			}

			private static string[] requiredFields = new string[]
			{
				"OrganizationName",
				"UserPrincipalName",
				"UserSid",
				"LiveIdHasAcceptedAccruals"
			};
		}

		private sealed class LiveIdBasicAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LiveIdBasicAuthenticator.requiredFields;
				}
			}

			protected override BackendAuthenticator InternalGetAuthenticator(CommonAccessToken token)
			{
				if (token.Version >= 2)
				{
					return this;
				}
				if (!token.ExtensionData.ContainsKey("OrganizationIdBase64"))
				{
					return BackendAuthenticator.LiveIdBasicAuthenticator.legacyLiveIdBasicAuthenticator.InternalGetAuthenticator(token);
				}
				return base.InternalGetAuthenticator(token);
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier) && BackendAuthenticator.LiveIdBasicAuthenticator.AuthIdentifierCacheEnabled.Value)
				{
					authIdentifier = this.authIdentifierCache.Lookup(BackendAuthenticator.LiveIdBasicAuthenticator.BuildCacheKey(token));
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				authIdentifier = null;
				principal = null;
				List<string> list = null;
				bool flag = false;
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "MemberName");
				string nonEmptyValue2 = BackendAuthenticator.GetNonEmptyValue(token, "Puid");
				string text = null;
				if ((!token.ExtensionData.TryGetValue("OrganizationContext", out text) || string.IsNullOrEmpty(text)) && HttpContext.Current != null && HttpContext.Current.Request != null)
				{
					text = HttpContext.Current.Request.Headers[WellKnownHeader.OrganizationContext];
				}
				AccountValidationContextByPUID accountValidationContextByPUID = null;
				if (ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("AccountValidationEnabled"))
				{
					if (accountValidationContext == null || !(accountValidationContext is AccountValidationContextByPUID))
					{
						accountValidationContextByPUID = this.GetAccountValidationContext(nonEmptyValue2, token);
					}
					else
					{
						accountValidationContextByPUID = (AccountValidationContextByPUID)accountValidationContext;
					}
				}
				ADRawEntry adrawEntry = null;
				bool flag2 = false;
				int passwordConfidenceInDays = 5;
				ITenantRecipientSession recipientSession = null;
				if (!BackendAuthenticator.LiveIdBasicAuthenticator.SkipPasswordConfidenceCheck.Value)
				{
					string value;
					token.ExtensionData.TryGetValue("CheckPasswordConfidence", out value);
					if (!string.IsNullOrEmpty(value))
					{
						bool.TryParse(value, out flag2);
					}
					string text2 = null;
					token.ExtensionData.TryGetValue("PasswordConfidenceInDays", out text2);
					if (!string.IsNullOrEmpty(text2))
					{
						int.TryParse(text2, out passwordConfidenceInDays);
					}
				}
				if (!BackendAuthenticator.LiveIdBasicAuthenticator.RehydrateLiveIdIdentity.Value)
				{
					string text3 = null;
					if (!token.ExtensionData.TryGetValue("ImplicitUpn", out text3) || string.IsNullOrEmpty(text3))
					{
						PropertyDefinition[] properties = (!flag2) ? BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGet : BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGetOfflineOrgId;
						adrawEntry = DirectoryHelper.GetADRawEntry(nonEmptyValue2, text, nonEmptyValue, properties, out recipientSession);
						if (adrawEntry == null)
						{
							throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(nonEmptyValue2, nonEmptyValue), new UnauthorizedAccessException());
						}
						OrganizationId organizationId = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
						if (accountValidationContextByPUID != null)
						{
							accountValidationContextByPUID.SetOrgId(organizationId);
						}
						string arg = (string)adrawEntry[ADMailboxRecipientSchema.SamAccountName];
						text3 = string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
					}
					try
					{
						WindowsIdentity windowsIdentity = new WindowsIdentity(text3);
						if (wantAuthIdentifier)
						{
							if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(windowsIdentity.User, out authIdentifier))
							{
								authIdentifier = null;
							}
							else
							{
								flag = true;
							}
						}
						principal = new WindowsPrincipal(windowsIdentity);
						goto IL_469;
					}
					catch (SecurityException innerException)
					{
						throw new BackendRehydrationException(SecurityStrings.FailedToLogon(text3), innerException);
					}
					catch (UnauthorizedAccessException innerException2)
					{
						throw new BackendRehydrationException(SecurityStrings.FailedToLogon(text3), innerException2);
					}
				}
				string userSid = null;
				string userPrincipal;
				OrganizationId organizationId2;
				if (token.Version < 2)
				{
					userPrincipal = BackendAuthenticator.GetNonEmptyValue(token, "UserPrincipalName");
					userSid = BackendAuthenticator.GetNonEmptyValue(token, "UserSid");
					organizationId2 = BackendAuthenticator.ExtractOrganizationId(token);
				}
				else
				{
					string a;
					bool flag3 = (token.ExtensionData.TryGetValue("UserType", out a) && string.Equals(a, UserType.OutlookCom.ToString())) || ConsumerIdentityHelper.IsConsumerMailbox(nonEmptyValue);
					bool flag4 = false;
					if (HttpContext.Current != null && HttpContext.Current.Request != null)
					{
						string value2 = HttpContext.Current.Request.Headers[WellKnownHeader.MissingDirectoryUserObjectHint];
						if (!string.IsNullOrEmpty(value2))
						{
							bool.TryParse(value2, out flag4);
						}
					}
					if (!flag4)
					{
						PropertyDefinition[] properties2 = (!flag2) ? BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGet : BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGetOfflineOrgId;
						adrawEntry = DirectoryHelper.GetADRawEntry(nonEmptyValue2, flag3 ? TemplateTenantConfiguration.DefaultDomain : text, nonEmptyValue, properties2, BackendAuthenticator.LiveIdBasicAuthenticator.RehydrateMSAIdentity.Value, out recipientSession);
					}
					if (adrawEntry == null)
					{
						if (!flag4)
						{
							throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(nonEmptyValue2, nonEmptyValue), new UnauthorizedAccessException());
						}
						MSAIdentity identity = new MSAIdentity(nonEmptyValue2, nonEmptyValue);
						authIdentifier = null;
						principal = new GenericPrincipal(identity, null);
						return;
					}
					else
					{
						userPrincipal = (string)adrawEntry[ADUserSchema.UserPrincipalName];
						userSid = ((SecurityIdentifier)adrawEntry[ADMailboxRecipientSchema.Sid]).ToString();
						organizationId2 = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
						if (accountValidationContextByPUID != null)
						{
							accountValidationContextByPUID.SetOrgId(organizationId2);
						}
						if (BackendAuthenticator.LiveIdBasicAuthenticator.IsRemotePowerShell.Value)
						{
							BackendAuthenticator.LiveIdBasicAuthenticator.UpdateTokenForMissingProperties(token, adrawEntry);
						}
						if (BackendAuthenticator.LiveIdBasicAuthenticator.PrepopulateGroupSids.Value)
						{
							list = DirectoryHelper.GetTokenSids(adrawEntry, nonEmptyValue2, flag3 ? TemplateTenantConfiguration.DefaultDomain : text, nonEmptyValue, BackendAuthenticator.LiveIdBasicAuthenticator.RehydrateMSAIdentity.Value);
						}
					}
				}
				LiveIdLoginAttributes loginAttributes = this.ExtractLoginAttributes(token);
				LiveIDIdentity liveIDIdentity = new LiveIDIdentity(userPrincipal, userSid, nonEmptyValue, organizationId2.PartitionId.ToString(), loginAttributes, nonEmptyValue2);
				if (list != null)
				{
					liveIDIdentity.PrepopulateGroupSidIds(list);
				}
				liveIDIdentity.UserOrganizationId = organizationId2;
				liveIDIdentity.HasAcceptedAccruals = true;
				string value3;
				if (token.ExtensionData.TryGetValue("LiveIdHasAcceptedAccruals", out value3) && !string.IsNullOrEmpty(value3))
				{
					try
					{
						liveIDIdentity.HasAcceptedAccruals = bool.Parse(value3);
					}
					catch (FormatException innerException3)
					{
						throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("LiveIdHasAcceptedAccruals"), innerException3);
					}
				}
				if (wantAuthIdentifier)
				{
					if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(userSid, out authIdentifier))
					{
						authIdentifier = null;
					}
					else
					{
						flag = true;
					}
				}
				principal = new GenericPrincipal(liveIDIdentity, null);
				IL_469:
				string value4 = null;
				token.ExtensionData.TryGetValue("SyncHRD", out value4);
				bool flag5 = false;
				bool.TryParse(value4, out flag5);
				string passwordToSync = null;
				token.ExtensionData.TryGetValue("PasswordToSync", out passwordToSync);
				if ((flag5 || !string.IsNullOrEmpty(passwordToSync)) && !flag2)
				{
					LiveIdBasicAuthentication authentication = new LiveIdBasicAuthentication();
					byte[] bytes = Encoding.Default.GetBytes(nonEmptyValue);
					byte[] passwordBytes = Encoding.Default.GetBytes(passwordToSync);
					authentication.SyncUPN = BackendAuthenticator.LiveIdBasicAuthenticator.SyncUPN.Value;
					authentication.BeginSyncADPassword(nonEmptyValue2, bytes, passwordBytes, null, delegate(IAsyncResult ar)
					{
						Array.Clear(passwordBytes, 0, passwordBytes.Length);
						passwordToSync = null;
						authentication.EndSyncADPassword(ar);
					}, null, Guid.Empty, flag5);
				}
				else if (flag2)
				{
					if (adrawEntry == null)
					{
						adrawEntry = DirectoryHelper.GetADRawEntry(nonEmptyValue2, text, nonEmptyValue, BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGetOfflineOrgId, out recipientSession);
					}
					ADObjectId adobjectId = (ADObjectId)adrawEntry[ADMailboxRecipientSchema.Database];
					if (adrawEntry[ADMailboxRecipientSchema.ExchangeGuid] == null || adobjectId == null || Guid.Empty.Equals((Guid)adrawEntry[ADMailboxRecipientSchema.ExchangeGuid]))
					{
						throw new BackendRehydrationException(SecurityStrings.LowPasswordConfidence(nonEmptyValue), new LowPasswordConfidenceException(SecurityStrings.LowPasswordConfidence(nonEmptyValue)));
					}
					bool flag6 = false;
					try
					{
						flag6 = OfflineOrgIdAuth.CheckPasswordConfidence(nonEmptyValue2, adrawEntry.Id, passwordConfidenceInDays, recipientSession);
					}
					catch (Exception e)
					{
						throw new BackendRehydrationException(SecurityStrings.LowPasswordConfidence(nonEmptyValue), new LowPasswordConfidenceException(SecurityStrings.LowPasswordConfidenceWithException(nonEmptyValue, e)));
					}
					if (!flag6)
					{
						throw new BackendRehydrationException(SecurityStrings.LowPasswordConfidence(nonEmptyValue), new LowPasswordConfidenceException(SecurityStrings.LowPasswordConfidence(nonEmptyValue)));
					}
				}
				if (BackendAuthenticator.LiveIdBasicAuthenticator.VerifyUserHasNoMailbox.Value)
				{
					if (adrawEntry == null)
					{
						adrawEntry = DirectoryHelper.GetADRawEntry(nonEmptyValue2, text, nonEmptyValue, BackendAuthenticator.LiveIdBasicAuthenticator.propertiesToGet);
					}
					if (adrawEntry != null && HttpContext.Current != null && HttpContext.Current.Response != null)
					{
						ADObjectId adobjectId2 = (ADObjectId)adrawEntry[ADMailboxRecipientSchema.Database];
						ADObjectId adobjectId3 = (ADObjectId)adrawEntry[ADUserSchema.ArchiveDatabase];
						if ((adobjectId2 == null || adobjectId2.ObjectGuid == Guid.Empty) && (adobjectId3 == null || adobjectId3.ObjectGuid == Guid.Empty))
						{
							OrganizationId organizationId3 = (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
							if (accountValidationContextByPUID != null && accountValidationContextByPUID.OrgId == null)
							{
								accountValidationContextByPUID.SetOrgId(organizationId3);
							}
							string externalDirectoryOrganizationId = this.GetExternalDirectoryOrganizationId(organizationId3);
							if (GlsDirectorySession.GetTenantOverrideStatus(externalDirectoryOrganizationId).HasFlag(GlsOverrideFlag.GlsRecordMismatch))
							{
								string value5 = string.Format("LiveIdBasic-InternalRehydrate-UserHasNoMailbox: puid={0}, organizationContext={1}, memberName={2}", nonEmptyValue2, text, nonEmptyValue);
								HttpContext.Current.Response.Headers[WellKnownHeader.BEServerRoutingError] = value5;
								throw new BackendRehydrationException(SecurityStrings.FailedToLogon(nonEmptyValue), new UnauthorizedAccessException());
							}
						}
					}
				}
				if (accountValidationContextByPUID != null)
				{
					accountValidationContext = accountValidationContextByPUID;
				}
				if (wantAuthIdentifier && flag && !string.IsNullOrEmpty(authIdentifier) && BackendAuthenticator.LiveIdBasicAuthenticator.AuthIdentifierCacheEnabled.Value)
				{
					this.authIdentifierCache.Add(BackendAuthenticator.LiveIdBasicAuthenticator.BuildCacheKey(token), authIdentifier);
				}
			}

			private AccountValidationContextByPUID GetAccountValidationContext(string puid, CommonAccessToken token)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (token.ExtensionData.ContainsKey("CreateTime"))
				{
					string text = token.ExtensionData["CreateTime"];
					if (!string.IsNullOrEmpty(text))
					{
						ExDateTime.TryParse(text, out utcNow);
					}
				}
				string appName;
				if (token.ExtensionData.ContainsKey("AppName"))
				{
					appName = token.ExtensionData["AppName"];
				}
				else
				{
					appName = string.Empty;
				}
				return new AccountValidationContextByPUID(puid, utcNow, appName);
			}

			private string GetExternalDirectoryOrganizationId(OrganizationId organizationId)
			{
				if (organizationId == null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - organizationId == null");
					return null;
				}
				if (organizationId.ConfigurationUnit == null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - organizationId.ConfigurationUnit == null");
					return null;
				}
				ITenantConfigurationSession tenantConfigurationSession = null;
				try
				{
					tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 724, "GetExternalDirectoryOrganizationId", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\BackendAuthenticator\\LiveIdBasicAuthenticator.cs");
				}
				catch (Exception arg)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceWarning<OrganizationId, Exception>((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - Caught Exception when trying to CreateTenantConfigurationSession for organizationId '{0}'.  Exception details: {1}.", organizationId, arg);
				}
				if (tenantConfigurationSession == null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - Cannot CreateTenantConfigurationSession for organizationId '{0}'.", organizationId);
					return null;
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
				if (exchangeConfigurationUnit == null)
				{
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - Cannot find tenantOrg for organizationId '{0}'.", organizationId);
					return null;
				}
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "GetExternalDirectoryOrganizationId - Found tenantOrg ExternalDirectoryOrganizationId '{0}' for organizationId '{1}'.", exchangeConfigurationUnit.ExternalDirectoryOrganizationId, organizationId);
				return exchangeConfigurationUnit.ExternalDirectoryOrganizationId;
			}

			private static string BuildCacheKey(CommonAccessToken token)
			{
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "Puid");
				return nonEmptyValue.ToUpper();
			}

			private static void UpdateTokenForMissingProperties(CommonAccessToken token, ADRawEntry rawEntry)
			{
				token.ExtensionData["UserSid"] = ((SecurityIdentifier)rawEntry[ADMailboxRecipientSchema.Sid]).ToString();
				token.ExtensionData["UserPrincipalName"] = (string)rawEntry[ADUserSchema.UserPrincipalName];
				OrganizationId organizationId = (OrganizationId)rawEntry[ADObjectSchema.OrganizationId];
				if (organizationId != null)
				{
					if (organizationId.PartitionId != null)
					{
						string arg = (string)rawEntry[ADMailboxRecipientSchema.SamAccountName];
						token.ExtensionData["ImplicitUpn"] = string.Format("{0}@{1}", arg, organizationId.PartitionId.ForestFQDN);
						token.ExtensionData["Partition"] = organizationId.PartitionId.ToString();
					}
					token.ExtensionData["OrganizationIdBase64"] = CommonAccessTokenAccessor.SerializeOrganizationId(organizationId);
				}
				HttpContext.Current.Request.Headers["X-CommonAccessToken"] = token.Serialize();
				HttpContext.Current.Items["Item-CommonAccessToken"] = token;
			}

			private LiveIdLoginAttributes ExtractLoginAttributes(CommonAccessToken token)
			{
				uint num = 0U;
				if (token.ExtensionData.ContainsKey("LoginAttributes"))
				{
					num = Convert.ToUInt32(token.ExtensionData["LoginAttributes"]);
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<uint>((long)this.GetHashCode(), "[LiveIdAuthenticator::ExtractLoginAttributes] Found loginAttributes in the common access token. Value = {0}", num);
				}
				return new LiveIdLoginAttributes(num);
			}

			private static BoolAppSettingsEntry RehydrateLiveIdIdentity = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.RehydrateLiveIdIdentity", false, ExTraceGlobals.BackendRehydrationTracer);

			private static BoolAppSettingsEntry SyncUPN = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.SyncUPN", false, ExTraceGlobals.BackendRehydrationTracer);

			public static BoolAppSettingsEntry RehydrateMSAIdentity = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.RehydrateMSAIdentity", false, ExTraceGlobals.BackendRehydrationTracer);

			public static BoolAppSettingsEntry SkipPasswordConfidenceCheck = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.SkipPasswordConfidenceCheck", false, ExTraceGlobals.BackendRehydrationTracer);

			public static BoolAppSettingsEntry VerifyUserHasNoMailbox = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.VerifyUserHasNoMailbox", false, ExTraceGlobals.BackendRehydrationTracer);

			public static BoolAppSettingsEntry PrepopulateGroupSids = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.PrepopulateGroupSids", false, ExTraceGlobals.BackendRehydrationTracer);

			private static BackendAuthenticator.LegacyLiveIdBasicAuthenticator legacyLiveIdBasicAuthenticator = new BackendAuthenticator.LegacyLiveIdBasicAuthenticator();

			private static string[] requiredFields = new string[]
			{
				"Puid",
				"MemberName"
			};

			private static BoolAppSettingsEntry AuthIdentifierCacheEnabled = new BoolAppSettingsEntry("LiveIdBasicAuthenticator.AuthIdentifierCacheEnabled", true, ExTraceGlobals.BackendRehydrationTracer);

			private static IntAppSettingsEntry AuthIdentifierCachePartitions = new IntAppSettingsEntry("LiveIdBasicAuthenticator.AuthIdentifierCachePartitions", 32, ExTraceGlobals.BackendRehydrationTracer);

			private static IntAppSettingsEntry AuthIdentifierCacheBuckets = new IntAppSettingsEntry("LiveIdBasicAuthenticator.AuthIdentifierCacheBuckets", 5, ExTraceGlobals.BackendRehydrationTracer);

			private static IntAppSettingsEntry AuthIdentifierCacheLifetime = new IntAppSettingsEntry("LiveIdBasicAuthenticator.AuthIdentifierCacheLifetime", 900, ExTraceGlobals.BackendRehydrationTracer);

			private readonly AuthIdentifierCache authIdentifierCache = new AuthIdentifierCache(Math.Min(Math.Max(BackendAuthenticator.LiveIdBasicAuthenticator.AuthIdentifierCachePartitions.Value, 1), 1024), Math.Min(Math.Max(BackendAuthenticator.LiveIdBasicAuthenticator.AuthIdentifierCacheBuckets.Value, 2), 100), TimeSpan.FromSeconds((double)Math.Min(Math.Max(BackendAuthenticator.LiveIdBasicAuthenticator.AuthIdentifierCacheLifetime.Value, 60), 86400)));

			private static readonly Lazy<bool> IsRemotePowerShell = new Lazy<bool>(delegate()
			{
				if (HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request.Url == null)
				{
					return false;
				}
				bool result;
				try
				{
					Uri url = HttpContext.Current.Request.Url;
					if (url.AbsolutePath == null)
					{
						result = false;
					}
					else
					{
						result = url.AbsolutePath.StartsWith("/powershell", StringComparison.OrdinalIgnoreCase);
					}
				}
				catch (InvalidOperationException)
				{
					result = false;
				}
				return result;
			}, true);

			private static PropertyDefinition[] propertiesToGet = new PropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName,
				ADObjectSchema.OrganizationId,
				ADMailboxRecipientSchema.SamAccountName,
				ADMailboxRecipientSchema.Sid,
				ADMailboxRecipientSchema.Database,
				ADUserSchema.ArchiveDatabase,
				ADUserSchema.ArchiveGuid
			};

			private static PropertyDefinition[] propertiesToGetOfflineOrgId = new PropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName,
				ADObjectSchema.OrganizationId,
				ADMailboxRecipientSchema.SamAccountName,
				ADMailboxRecipientSchema.Sid,
				ADMailboxRecipientSchema.ExchangeGuid,
				ADMailboxRecipientSchema.Database,
				IADMailStorageSchema.DatabaseName,
				ADUserSchema.ArchiveDatabase,
				ADUserSchema.ArchiveGuid
			};
		}

		private sealed class LegacyLiveIdBasicAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LegacyLiveIdBasicAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
				{
					authIdentifier = null;
				}
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				PartitionId partitionId;
				if (!PartitionId.TryParse(token.ExtensionData["Partition"], out partitionId))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("Partition"));
				}
				string sddlForm = token.ExtensionData["UserSid"];
				ADRawEntry adrawEntry = BackendAuthenticator.LegacyLiveIdBasicAuthenticator.FindAdUser(partitionId, new SecurityIdentifier(sddlForm));
				string arg = adrawEntry[ADMailboxRecipientSchema.SamAccountName].ToString();
				string text = string.Format("{0}@{1}", arg, partitionId.ForestFQDN);
				try
				{
					WindowsIdentity ntIdentity = new WindowsIdentity(text);
					if (wantAuthIdentifier && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token, out authIdentifier))
					{
						authIdentifier = null;
					}
					principal = new WindowsPrincipal(ntIdentity);
				}
				catch (SecurityException innerException)
				{
					throw new BackendRehydrationException(SecurityStrings.FailedToLogon(text), innerException);
				}
				catch (UnauthorizedAccessException innerException2)
				{
					throw new BackendRehydrationException(SecurityStrings.FailedToLogon(text), innerException2);
				}
			}

			private static ADRawEntry FindAdUser(PartitionId partitionId, SecurityIdentifier userSid)
			{
				PropertyDefinition[] properties = new PropertyDefinition[]
				{
					ADMailboxRecipientSchema.SamAccountName
				};
				Exception ex = null;
				try
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
					ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 950, "FindAdUser", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\BackendAuthenticator\\LiveIdBasicAuthenticator.cs");
					return tenantRecipientSession.FindADRawEntryBySid(userSid, properties);
				}
				catch (NonUniqueRecipientException ex2)
				{
					ex = ex2;
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<NonUniqueRecipientException>(0L, "[LiveIdBasicAuthenticator::FindAdUser] Error encountered: {0}", ex2);
				}
				catch (ADTransientException ex3)
				{
					ex = ex3;
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<ADTransientException>(0L, "[LiveIdBasicAuthenticator::FindAdUser] Error encountered: {0}", ex3);
				}
				catch (DataValidationException ex4)
				{
					ex = ex4;
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<DataValidationException>(0L, "[LiveIdBasicAuthenticator::FindAdUser] Error encountered: {0}", ex4);
				}
				catch (DataSourceOperationException ex5)
				{
					ex = ex5;
					ExTraceGlobals.BackendRehydrationTracer.TraceDebug<DataSourceOperationException>(0L, "[LiveIdBasicAuthenticator::FindAdUser] Error encountered: {0}", ex5);
				}
				if (ex != null)
				{
					throw new BackendRehydrationException(SecurityStrings.CannotLookupUser(partitionId.ToString(), userSid.ToString(), ex.Message));
				}
				return null;
			}

			private static string[] requiredFields = new string[]
			{
				"Partition",
				"UserSid",
				"MemberName"
			};
		}

		private sealed class LiveIdNego2Authenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LiveIdNego2Authenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				authIdentifier = null;
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				string name = token.ExtensionData["UserSid"];
				GenericIdentity identity = new GenericIdentity(name);
				principal = new GenericPrincipal(identity, BackendAuthenticator.EmptyStringArray);
			}

			private static string[] requiredFields = new string[]
			{
				"UserSid"
			};
		}

		private sealed class OAuthAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.OAuthAuthenticator.requiredFields;
				}
			}

			protected override BackendAuthenticator InternalGetAuthenticator(CommonAccessToken token)
			{
				if (token.Version == 2)
				{
					return this;
				}
				if (token.ExtensionData.ContainsKey("OAuthData"))
				{
					return BackendAuthenticator.OAuthAuthenticator.legacyAuthenticator.InternalGetAuthenticator(token);
				}
				return base.InternalGetAuthenticator(token);
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				OrganizationId organizationId = BackendAuthenticator.ExtractOrganizationId(token);
				OAuthActAsUser actAsUser = BackendAuthenticator.OAuthAuthenticator.ExtractActAsUser(organizationId, token);
				authIdentifier = BackendAuthenticator.OAuthAuthenticator.ComputeAuthIdentifier(actAsUser);
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				if (FaultInjection.TraceTest<bool>((FaultInjection.LIDs)4085656893U))
				{
					throw new InvalidOAuthTokenException(OAuthErrors.TestOnlyExceptionDuringRehydration, null, null);
				}
				OrganizationId organizationId = BackendAuthenticator.ExtractOrganizationId(token);
				OAuthApplication application = BackendAuthenticator.OAuthAuthenticator.ExtractOAuthApplication(token);
				OAuthActAsUser actAsUser = BackendAuthenticator.OAuthAuthenticator.ExtractActAsUser(organizationId, token);
				if (wantAuthIdentifier)
				{
					authIdentifier = BackendAuthenticator.OAuthAuthenticator.ComputeAuthIdentifier(actAsUser);
				}
				OAuthIdentity oauthIdentity = OAuthIdentity.Create(organizationId, application, actAsUser);
				try
				{
					IIdentity identity = oauthIdentity.ConvertIdentityIfNeed();
					principal = new GenericPrincipal(identity, null);
				}
				catch (InvalidOAuthLinkedAccountException ex)
				{
					throw new BackendRehydrationException(new LocalizedString(ex.Message), ex);
				}
			}

			private static OAuthApplication ExtractOAuthApplication(CommonAccessToken token)
			{
				string nonEmptyValue = BackendAuthenticator.GetNonEmptyValue(token, "AppType");
				OAuthApplicationType oauthApplicationType;
				if (!Enum.TryParse<OAuthApplicationType>(nonEmptyValue, out oauthApplicationType))
				{
					throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("AppType"));
				}
				OAuthApplication oauthApplication = null;
				switch (oauthApplicationType)
				{
				case OAuthApplicationType.S2SApp:
				{
					string nonEmptyValue2 = BackendAuthenticator.GetNonEmptyValue(token, "AppDn");
					string nonEmptyValue3 = BackendAuthenticator.GetNonEmptyValue(token, "AppId");
					string realm = null;
					token.ExtensionData.TryGetValue("AppRealm", out realm);
					OAuthIdentitySerializer.PartnerApplicationCacheKey key = new OAuthIdentitySerializer.PartnerApplicationCacheKey(nonEmptyValue2, nonEmptyValue3, realm);
					PartnerApplication partnerApplication = OAuthIdentitySerializer.PartnerApplicationCache.Instance.Get(key);
					oauthApplication = new OAuthApplication(partnerApplication);
					string b;
					if (token.ExtensionData.TryGetValue("IsFromSameOrgExchange", out b))
					{
						oauthApplication.IsFromSameOrgExchange = new bool?(string.Equals(bool.TrueString, b, StringComparison.OrdinalIgnoreCase));
					}
					break;
				}
				case OAuthApplicationType.CallbackApp:
				{
					string nonEmptyValue4 = BackendAuthenticator.GetNonEmptyValue(token, "CallbackAppId");
					string scope = null;
					token.ExtensionData.TryGetValue("Scope", out scope);
					oauthApplication = new OAuthApplication(new OfficeExtensionInfo(nonEmptyValue4, scope));
					break;
				}
				case OAuthApplicationType.V1App:
				{
					string nonEmptyValue5 = BackendAuthenticator.GetNonEmptyValue(token, "V1AppId");
					string scp = null;
					string rol = null;
					token.ExtensionData.TryGetValue("Scope", out scp);
					token.ExtensionData.TryGetValue("Role", out rol);
					oauthApplication = new OAuthApplication(new V1ProfileAppInfo(nonEmptyValue5, scp, rol));
					break;
				}
				case OAuthApplicationType.V1ExchangeSelfIssuedApp:
				{
					string nonEmptyValue6 = BackendAuthenticator.GetNonEmptyValue(token, "V1AppId");
					string scp2 = null;
					string rol2 = null;
					token.ExtensionData.TryGetValue("Scope", out scp2);
					token.ExtensionData.TryGetValue("Role", out rol2);
					string nonEmptyValue7 = BackendAuthenticator.GetNonEmptyValue(token, "AppDn");
					string nonEmptyValue8 = BackendAuthenticator.GetNonEmptyValue(token, "AppId");
					string realm2 = null;
					token.ExtensionData.TryGetValue("AppRealm", out realm2);
					OAuthIdentitySerializer.PartnerApplicationCacheKey key2 = new OAuthIdentitySerializer.PartnerApplicationCacheKey(nonEmptyValue7, nonEmptyValue8, realm2);
					PartnerApplication partnerApplication2 = OAuthIdentitySerializer.PartnerApplicationCache.Instance.Get(key2);
					oauthApplication = new OAuthApplication(new V1ProfileAppInfo(nonEmptyValue6, scp2, rol2), partnerApplication2);
					break;
				}
				}
				return oauthApplication;
			}

			public static OAuthActAsUser ExtractActAsUser(OrganizationId organizationId, CommonAccessToken token)
			{
				if (string.Equals(BackendAuthenticator.GetNonEmptyValue(token, "AppOnly"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
				Dictionary<string, string> rawAttributes = null;
				string value;
				if (token.ExtensionData.TryGetValue("RawUserInfo", out value))
				{
					rawAttributes = value.DeserializeFromJson<Dictionary<string, string>>();
				}
				Dictionary<string, string> verifiedAttributes = null;
				string value2;
				if (token.ExtensionData.TryGetValue("VerifiedUserInfo", out value2))
				{
					verifiedAttributes = value2.DeserializeFromJson<Dictionary<string, string>>();
				}
				return OAuthActAsUser.InternalCreateFromAttributes(organizationId, false, rawAttributes, verifiedAttributes);
			}

			internal static string ComputeAuthIdentifier(OAuthActAsUser actAsUser)
			{
				string result = null;
				if (actAsUser != null && actAsUser.Sid != null && !BackendAuthenticator.TryGetAuthIdentifierFromUserSid(actAsUser.GetMasterAccountSidIfAvailable(), out result))
				{
					result = null;
				}
				return result;
			}

			private static BackendAuthenticator.LegacyOAuthAuthenticator legacyAuthenticator = new BackendAuthenticator.LegacyOAuthAuthenticator();

			private static string[] requiredFields = new string[]
			{
				"OrganizationIdBase64",
				"AppType",
				"AppOnly"
			};
		}

		private sealed class LegacyOAuthAuthenticator : BackendAuthenticator
		{
			protected override string[] RequiredFields
			{
				get
				{
					return BackendAuthenticator.LegacyOAuthAuthenticator.requiredFields;
				}
			}

			protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
			{
				OAuthIdentity oauthIdentity = OAuthIdentitySerializer.ConvertFromCommonAccessToken(token);
				authIdentifier = BackendAuthenticator.OAuthAuthenticator.ComputeAuthIdentifier(oauthIdentity.ActAsUser);
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
			{
				throw new NotImplementedException();
			}

			protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
			{
				authIdentifier = null;
				principal = null;
				try
				{
					OAuthIdentity oauthIdentity = OAuthIdentitySerializer.ConvertFromCommonAccessToken(token);
					if (wantAuthIdentifier)
					{
						authIdentifier = BackendAuthenticator.OAuthAuthenticator.ComputeAuthIdentifier(oauthIdentity.ActAsUser);
					}
					IIdentity identity = oauthIdentity;
					if (BackendAuthenticator.RehydrateSidOAuthIdentity.Value)
					{
						ExTraceGlobals.BackendRehydrationTracer.TraceDebug(0L, "[OAuthAuthenticator::InternalRehydrate] Convert OAuthIdentity to SidOAuthIdentity.");
						identity = SidOAuthIdentity.Create(oauthIdentity);
					}
					else if (oauthIdentity.OAuthApplication.V1ProfileApp != null)
					{
						if (!string.Equals(oauthIdentity.OAuthApplication.V1ProfileApp.Scope, Constants.ClaimValues.UserImpersonation, StringComparison.OrdinalIgnoreCase))
						{
							throw new BackendRehydrationException(new LocalizedString("Invalid value in scp claim-type."));
						}
						identity = new SidBasedIdentity(oauthIdentity.ActAsUser.UserPrincipalName, oauthIdentity.ActAsUser.GetMasterAccountSidIfAvailable().Value, oauthIdentity.ActAsUser.UserPrincipalName, "OAuth", oauthIdentity.OrganizationId.PartitionId.ToString())
						{
							UserOrganizationId = oauthIdentity.OrganizationId
						};
					}
					principal = new GenericPrincipal(identity, null);
				}
				catch (OAuthIdentityDeserializationException ex)
				{
					throw new BackendRehydrationException(new LocalizedString(ex.Message), ex);
				}
				catch (InvalidOAuthLinkedAccountException ex2)
				{
					throw new BackendRehydrationException(new LocalizedString(ex2.Message), ex2);
				}
			}

			private static string[] requiredFields = new string[]
			{
				"OAuthData"
			};
		}
	}
}

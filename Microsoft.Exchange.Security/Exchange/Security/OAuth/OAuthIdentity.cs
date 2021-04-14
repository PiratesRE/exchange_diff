using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.PartnerToken;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class OAuthIdentity : GenericIdentity, IOrganizationScopedIdentity, IIdentity
	{
		private OAuthIdentity(OrganizationId organizationId, OAuthApplication application, OAuthActAsUser actAsUser) : base(application.Id, Constants.BearerAuthenticationType)
		{
			this.OrganizationId = organizationId;
			this.OAuthApplication = application;
			this.ActAsUser = actAsUser;
		}

		public override string Name
		{
			get
			{
				if (this.IsAppOnly)
				{
					return this.OAuthApplication.Id;
				}
				if (!this.ActAsUser.IsUserVerified)
				{
					return string.Format("<unverified>{0}", this.ActAsUser);
				}
				if (this.ActAsUser.Sid == null)
				{
					return string.Format("<user w/o sid>{0}", this.ActAsUser);
				}
				return this.ActAsUser.Sid.Value;
			}
		}

		public OrganizationId OrganizationId { get; private set; }

		public OAuthApplication OAuthApplication { get; private set; }

		public OAuthActAsUser ActAsUser { get; internal set; }

		public bool IsAuthenticatedAtBackend { get; internal set; }

		public bool IsOfficeExtension
		{
			get
			{
				return this.OAuthApplication.IsOfficeExtension;
			}
		}

		public OfficeExtensionInfo OfficeExtension
		{
			get
			{
				return this.OAuthApplication.OfficeExtension;
			}
		}

		public bool IsKnownFromSameOrgExchange
		{
			get
			{
				return this.OAuthApplication.IsFromSameOrgExchange != null && this.OAuthApplication.IsFromSameOrgExchange.Value;
			}
		}

		public string ExtraLoggingInfo { get; set; }

		public bool IsAppOnly
		{
			get
			{
				return this.ActAsUser == null;
			}
		}

		public ADRecipient ADRecipient
		{
			get
			{
				if (this.adRecipient == null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 175, "ADRecipient", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\OAuth\\OAuthIdentity.cs");
					this.adRecipient = tenantOrRootOrgRecipientSession.FindBySid(this.ActAsUser.Sid);
				}
				return this.adRecipient;
			}
		}

		OrganizationId IOrganizationScopedIdentity.OrganizationId
		{
			get
			{
				return this.OrganizationId;
			}
		}

		IStandardBudget IOrganizationScopedIdentity.AcquireBudget()
		{
			if (!this.IsAppOnly && !(this.ActAsUser.Sid == null))
			{
				return StandardBudget.Acquire(this.ActAsUser.Sid, BudgetType.Ews, true, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId));
			}
			if (this.IsKnownFromSameOrgExchange)
			{
				return StandardBudget.Acquire(new UnthrottledBudgetKey("cross-premise freebusy", BudgetType.Ews));
			}
			return StandardBudget.Acquire(new TenantBudgetKey(this.OrganizationId, BudgetType.Ews));
		}

		public static OAuthIdentity Create(OrganizationId organizationId, OAuthApplication application, OAuthActAsUser actAsUser)
		{
			OAuthCommon.VerifyNonNullArgument("organizationId", organizationId);
			OAuthCommon.VerifyNonNullArgument("application", application);
			return new OAuthIdentity(organizationId, application, actAsUser);
		}

		public static OAuthIdentity Create(OrganizationId organizationId, OAuthApplication application, MiniRecipient recipient)
		{
			OAuthCommon.VerifyNonNullArgument("organizationId", organizationId);
			OAuthCommon.VerifyNonNullArgument("application", application);
			return new OAuthIdentity(organizationId, application, OAuthActAsUser.CreateFromMiniRecipient(organizationId, recipient));
		}

		public CommonAccessToken ToCommonAccessToken(int targetServerVersion)
		{
			if (FaultInjection.TraceTest<bool>((FaultInjection.LIDs)3011915069U))
			{
				throw new InvalidOAuthTokenException(OAuthErrors.TestOnlyExceptionDuringOAuthCATGeneration, null, null);
			}
			if (OAuthIdentity.EnforceV1Token.Value || targetServerVersion < OAuthTokenAccessor.MinVersion || FaultInjection.TraceTest<bool>((FaultInjection.LIDs)3481677117U))
			{
				return this.ToCommonAccessTokenVersion1();
			}
			return this.ToCommonAccessTokenVersion2();
		}

		public CommonAccessToken ToCommonAccessTokenVersion1()
		{
			return OAuthIdentitySerializer.ConvertToCommonAccessToken(this);
		}

		public CommonAccessToken ToCommonAccessTokenVersion2()
		{
			return OAuthTokenAccessor.Create(this).GetToken();
		}

		public IIdentity ConvertIdentityIfNeed()
		{
			bool flag = this.OAuthApplication.ApplicationType == OAuthApplicationType.V1App && this.OAuthApplication.V1ProfileApp != null && this.OAuthApplication == Constants.IdTokenApplication;
			bool flag2 = this.OAuthApplication.ApplicationType == OAuthApplicationType.V1App && OAuthGrant.ExtractKnownGrants(this.OAuthApplication.V1ProfileApp.Scope).Contains("user_impersonation");
			bool flag3 = this.OAuthApplication.ApplicationType == OAuthApplicationType.CallbackApp && OAuthGrant.ExtractKnownGrants(this.OAuthApplication.OfficeExtension.Scope).Contains("user_impersonation");
			if (flag2 || flag || flag3)
			{
				return new SidBasedIdentity(this.ActAsUser.UserPrincipalName, this.ActAsUser.GetMasterAccountSidIfAvailable().Value, (this.ActAsUser.WindowsLiveID != SmtpAddress.Empty) ? this.ActAsUser.WindowsLiveID.ToString() : this.ActAsUser.UserPrincipalName, Constants.BearerAuthenticationType, this.OrganizationId.PartitionId.ToString())
				{
					UserOrganizationId = this.OrganizationId
				};
			}
			if (OAuthIdentity.RehydrateSidOAuthIdentity.Value)
			{
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug(0L, "[OAuthAuthenticator::InternalRehydrate] Convert OAuthIdentity to SidOAuthIdentity.");
				return SidOAuthIdentity.Create(this);
			}
			return this;
		}

		private static readonly BoolAppSettingsEntry EnforceV1Token = new BoolAppSettingsEntry("OAuthHttpModule.EnforceV1Token", false, ExTraceGlobals.OAuthTracer);

		private static BoolAppSettingsEntry RehydrateSidOAuthIdentity = new BoolAppSettingsEntry("OAuthAuthenticator.RehydrateSidOAuthIdentity", false, ExTraceGlobals.OAuthTracer);

		private ADRecipient adRecipient;
	}
}

using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMRecipient : DisposableBase
	{
		public UMRecipient(ADRecipient adrecipient)
		{
			this.Initialize(adrecipient, true);
		}

		protected UMRecipient()
		{
		}

		public ADRecipient ADRecipient
		{
			get
			{
				return this.recipient;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.ADRecipient.DisplayName ?? this.ADRecipient.Alias;
			}
		}

		public bool RequiresLegacyRedirectForCallAnswering
		{
			get
			{
				return this.IsUMEnabledMailbox && this.InternalIsIncompatibleMailboxUser;
			}
		}

		public bool RequiresLegacyRedirectForSubscriberAccess
		{
			get
			{
				return this.IsUMEnabledMailbox && this.InternalUMMailboxPolicy != null && this.InternalUMMailboxPolicy.AllowSubscriberAccess && this.InternalIsIncompatibleMailboxUser;
			}
		}

		public ADObjectId MailboxServerSite
		{
			get
			{
				if (this.InternalExchangePrincipal != null)
				{
					ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\UMRecipient.cs", "MailboxServerSite", 162);
					Site site = currentServiceTopology.GetSite(this.InternalExchangePrincipal.MailboxInfo.Location.ServerFqdn, "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\UMRecipient.cs", "MailboxServerSite", 163);
					return site.Id;
				}
				return null;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				if (this.recipient == null)
				{
					return null;
				}
				return this.recipient.OrganizationId;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				if (this.tenantGuid == null)
				{
					ExAssert.RetailAssert(this.recipient != null, "recipient is null");
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.recipient);
					this.tenantGuid = new Guid?(iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId());
				}
				return this.tenantGuid.Value;
			}
		}

		public VersionEnum VersionCompatibility
		{
			get
			{
				VersionEnum result = VersionEnum.Compatible;
				if (this.InternalExchangePrincipal != null)
				{
					if (this.InternalExchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E2007MinVersion && this.InternalExchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E14MinVersion)
					{
						result = VersionEnum.E12Legacy;
					}
					else if (this.InternalExchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E14MinVersion && this.InternalExchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion)
					{
						result = VersionEnum.E14Legacy;
					}
				}
				return result;
			}
		}

		public virtual string MailAddress
		{
			get
			{
				SmtpAddress primarySmtpAddress = this.ADRecipient.PrimarySmtpAddress;
				return this.ADRecipient.PrimarySmtpAddress.ToString();
			}
		}

		public virtual DRMProtectionOptions DRMPolicyForCA
		{
			get
			{
				if (this.InternalUMMailboxPolicy != null)
				{
					return this.InternalUMMailboxPolicy.ProtectUnauthenticatedVoiceMail;
				}
				return DRMProtectionOptions.None;
			}
		}

		public virtual DRMProtectionOptions DRMPolicyForInterpersonal
		{
			get
			{
				if (this.InternalUMMailboxPolicy != null)
				{
					return this.InternalUMMailboxPolicy.ProtectAuthenticatedVoiceMail;
				}
				return DRMProtectionOptions.None;
			}
		}

		protected static UMRecipient.FieldCheckDelegate FieldMissingCheck
		{
			get
			{
				return UMRecipient.fieldMissingCheck;
			}
		}

		protected UMMailboxPolicy InternalUMMailboxPolicy
		{
			get
			{
				if (this.policy == null)
				{
					return null;
				}
				return this.policy.Value;
			}
		}

		protected UMMailbox InternalADUMMailboxSettings
		{
			get
			{
				return this.adumMailboxSettings;
			}
		}

		public bool IsUMEnabledMailbox
		{
			get
			{
				return this.InternalADUMMailboxSettings != null && this.InternalADUMMailboxSettings.UMEnabled;
			}
		}

		protected ExchangePrincipal InternalExchangePrincipal
		{
			get
			{
				if (this.exchangePrincipal == null)
				{
					return null;
				}
				return this.exchangePrincipal.Value;
			}
		}

		protected bool InternalIsIncompatibleMailboxUser
		{
			get
			{
				bool result = false;
				if (this.InternalExchangePrincipal != null)
				{
					result = !CommonUtil.IsServerCompatible(this.InternalExchangePrincipal.MailboxInfo.Location.ServerVersion);
				}
				return result;
			}
		}

		public static bool TryCreate(ADRecipient adrecipient, out UMRecipient umrecipient)
		{
			umrecipient = new UMRecipient();
			if (umrecipient.Initialize(adrecipient, false))
			{
				return true;
			}
			umrecipient.Dispose();
			umrecipient = null;
			return false;
		}

		public bool RequiresRedirectForCallAnswering()
		{
			return this.RequiresLegacyRedirectForCallAnswering;
		}

		public bool RequiresRedirectForSubscriberAccess()
		{
			return this.RequiresLegacyRedirectForSubscriberAccess;
		}

		public override string ToString()
		{
			return this.ADRecipient.ToString();
		}

		protected virtual bool Initialize(ADRecipient recipient, bool throwOnFailure)
		{
			bool flag = false;
			try
			{
				this.recipient = recipient;
				if (!this.CheckField(this.recipient, "recipient", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (CommonConstants.UseDataCenterCallRouting)
				{
					if (!this.CheckField(OrganizationId.ForestWideOrgId.Equals(this.recipient.OrganizationId), "ScopeOfTheUser", (object x) => !(bool)x, throwOnFailure))
					{
						return flag;
					}
				}
				ADUser aduser = recipient as ADUser;
				this.adumMailboxSettings = ((aduser != null) ? new UMMailbox(aduser) : null);
				this.policy = new Lazy<UMMailboxPolicy>(new Func<UMMailboxPolicy>(this.InitializeMailboxPolicy));
				this.exchangePrincipal = new Lazy<ExchangePrincipal>(new Func<ExchangePrincipal>(this.InitializeExchangePrincipal));
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
			return flag;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMRecipient>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected bool CheckField(object fieldValue, string fieldName, UMRecipient.FieldCheckDelegate checker, bool throwOnFailure)
		{
			if (checker(fieldValue))
			{
				return true;
			}
			PIIMessage data = PIIMessage.Create(PIIType._User, this.ADRecipient);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "UMRecipient.CheckField failed!  recipient ='_User', field='{0}', throw = '{1}'", new object[]
			{
				fieldName,
				throwOnFailure
			});
			if (throwOnFailure)
			{
				throw new UMRecipientValidationException(this.ToString(), fieldName);
			}
			return false;
		}

		private ExchangePrincipal InitializeExchangePrincipal()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "InitializeExchangePrincipal: lazy initialization", new object[0]);
			ExchangePrincipal result = null;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				ADUser aduser = this.ADRecipient as ADUser;
				if (aduser != null && !string.IsNullOrEmpty(aduser.LegacyExchangeDN) && aduser.RecipientType == RecipientType.UserMailbox)
				{
					ADSessionSettings settings = this.InvokeWithStopwatch<ADSessionSettings>("DirectoryExtensions.ToADSessionSettings", () => aduser.OrganizationId.ToADSessionSettings());
					result = this.InvokeWithStopwatch<ExchangePrincipal>("ExchangePrincipal.FromLegacyDN", () => ExchangePrincipal.FromLegacyDN(settings, aduser.LegacyExchangeDN, RemotingOptions.AllowCrossSite));
					stopwatch.Stop();
					TimeSpan elapsed = stopwatch.Elapsed;
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "InitializeExchangePrincipal: object found ({0}ms)", new object[]
					{
						(int)elapsed.TotalMilliseconds
					});
					if (elapsed > UMRecipient.ExchangePrincipalWarningThreshold)
					{
						string message = string.Format("InitializeExchangePrincipal: Exchange principal {0} took {1} seconds to initialize. This is more than the warning threshold of {2} seconds", this.recipient.LegacyExchangeDN, elapsed.TotalSeconds, UMRecipient.ExchangePrincipalWarningThreshold.TotalSeconds);
						ExceptionHandling.SendWatsonWithoutDumpWithExtraData(new TimeoutException(message));
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "InitializeExchangePrincipal: finished generating watson", new object[0]);
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				stopwatch.Stop();
				PIIMessage data = PIIMessage.Create(PIIType._User, this.ADRecipient);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "UMRecipient.InitializeExchangePrincipal threw an exception: {0}", new object[]
				{
					ex
				});
				this.exchangePrincipal = null;
			}
			return result;
		}

		private UMMailboxPolicy InitializeMailboxPolicy()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "InitializeMailboxPolicy: lazy initialization", new object[0]);
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.ADRecipient);
			return iadsystemConfigurationLookup.GetPolicyFromRecipient(this.ADRecipient);
		}

		protected T InvokeWithStopwatch<T>(string operationName, Func<T> func)
		{
			return this.latencyStopwatch.Invoke<T>(operationName, func);
		}

		private static readonly TimeSpan ExchangePrincipalWarningThreshold = TimeSpan.FromSeconds(10.0);

		private static UMRecipient.FieldCheckDelegate fieldMissingCheck = (object fieldValue) => null != fieldValue;

		private LatencyStopwatch latencyStopwatch = new LatencyStopwatch();

		private Lazy<ExchangePrincipal> exchangePrincipal;

		private UMMailbox adumMailboxSettings;

		private Lazy<UMMailboxPolicy> policy;

		private ADRecipient recipient;

		private Guid? tenantGuid;

		protected delegate bool FieldCheckDelegate(object fieldValue);

		internal class Factory
		{
			public static T FromADRecipient<T>(ADRecipient adrecipient) where T : UMRecipient
			{
				T t = default(T);
				UMRecipient umrecipient = null;
				if (UMSubscriber.TryCreate(adrecipient, out umrecipient))
				{
					t = (umrecipient as T);
				}
				else if (UMMailboxRecipient.TryCreate(adrecipient, out umrecipient))
				{
					t = (umrecipient as T);
				}
				else if (UMRecipient.TryCreate(adrecipient, out umrecipient))
				{
					t = (umrecipient as T);
				}
				if (t == null && umrecipient != null)
				{
					umrecipient.Dispose();
					umrecipient = null;
				}
				return t;
			}

			public static T FromExtension<T>(string phone, UMDialPlan dialPlan, UMRecipient scopingUser) where T : UMRecipient
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateUmProxyAddressLookup(dialPlan);
				ADRecipient adrecipient = iadrecipientLookup.LookupByExtensionAndDialPlan(phone, dialPlan);
				if (adrecipient == null)
				{
					return default(T);
				}
				return UMRecipient.Factory.FromADRecipient<T>(adrecipient);
			}

			public static T FromPrincipal<T>(IExchangePrincipal principal) where T : UMRecipient
			{
				if (principal == null)
				{
					throw new InvalidPrincipalException();
				}
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(principal.MailboxInfo.OrganizationId, null);
				ADRecipient adrecipient = iadrecipientLookup.LookupByExchangePrincipal(principal);
				return UMRecipient.Factory.FromADRecipient<T>(adrecipient);
			}
		}
	}
}

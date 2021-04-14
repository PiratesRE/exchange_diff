using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "FederationTrust", SupportsShouldProcess = true)]
	public sealed class TestFederationTrust : DataAccessTask<FederationTrust>
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter UserIdentity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["UserIdentity"];
			}
			set
			{
				base.Fields["UserIdentity"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestFederationTrust;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.RootOrgGlobalConfigSession;
		}

		protected override void InternalValidate()
		{
			if (this.UserIdentity == null)
			{
				ADSite localSite = base.RootOrgGlobalConfigSession.GetLocalSite();
				string text;
				if (TestConnectivityCredentialsManager.IsExchangeMultiTenant())
				{
					SmtpAddress? multiTenantAutomatedTaskUser = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(this, base.RootOrgGlobalConfigSession, localSite);
					text = ((multiTenantAutomatedTaskUser != null) ? multiTenantAutomatedTaskUser.Value.ToString() : null);
				}
				else
				{
					SmtpAddress? enterpriseAutomatedTaskUser = TestConnectivityCredentialsManager.GetEnterpriseAutomatedTaskUser(localSite, "CasDomain.com");
					text = ((enterpriseAutomatedTaskUser != null) ? enterpriseAutomatedTaskUser.Value.Local : null);
				}
				if (text != null)
				{
					this.UserIdentity = new RecipientIdParameter(text);
					return;
				}
				base.ThrowTerminatingError(new NoDefaultTestAccountException(), (ErrorCategory)1003, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				this.Process();
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, (ErrorCategory)1003, null);
			}
			finally
			{
				base.WriteObject(Environment.NewLine + "Closing Test-FederationTrust...");
				if (this.MonitoringContext)
				{
					MonitoringData monitoringData = new MonitoringData();
					foreach (TestFederationTrust.ResultEvent resultEvent in this.events)
					{
						monitoringData.Events.Add(new MonitoringEvent(TestFederationTrust.EventSource, (int)resultEvent.Id, resultEvent.Type, resultEvent.Message));
					}
					base.WriteObject(monitoringData);
				}
				else
				{
					foreach (TestFederationTrust.ResultEvent sendToPipeline in this.events)
					{
						base.WriteObject(sendToPipeline);
					}
				}
			}
		}

		private void Process()
		{
			base.WriteObject(Environment.NewLine + Environment.NewLine + "Begin process.");
			base.WriteObject(Environment.NewLine + "STEP 1 of 6: Getting ADUser information for " + this.UserIdentity.ToString() + "...");
			ADUser user = this.GetUser();
			if (user == null)
			{
				LocalizedException exception = new LocalizedException(new LocalizedString("Error: tried to initialized user, but this.GetUser() returned null when called in Process()"));
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidData, null);
			}
			base.WriteObject("RESULT: Success.");
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), user.OrganizationId, null, false);
			adsessionSettings.IsSharedConfigChecked = true;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 201, "Process", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestFederationTrust.cs");
			tenantOrTopologyConfigurationSession.GetFederatedOrganizationId(user.OrganizationId);
			base.WriteObject(Environment.NewLine + "STEP 2 of 6: Getting FederationTrust object for " + this.UserIdentity.ToString() + "...");
			FederationTrust federationTrust = this.GetFederationTrust(user);
			if (federationTrust == null)
			{
				LocalizedException exception2 = new LocalizedException(new LocalizedString("Error: tried to initialized federationTrust, but this.GetFederationTrust(user) returned null when called in Process()"));
				base.ThrowTerminatingError(exception2, ErrorCategory.InvalidData, null);
			}
			base.WriteObject("RESULT: Success.");
			base.WriteObject(Environment.NewLine + "STEP 3 of 6: Validating that the FederationTrust has the same STS certificates as the actual certificates published by the STS in the federation metadata.");
			this.ValidateFederationTrustCertificatesWithFederationMetadata(federationTrust);
			base.WriteObject("RESULT: Success.");
			base.WriteObject(Environment.NewLine + "STEP 4 of 6: Getting STS and Organization certificates from the federation trust object...");
			X509Certificate2[] stsCertificates = this.GetStsCertificates(federationTrust);
			X509Certificate2[] organizationCertificates = this.GetOrganizationCertificates(federationTrust);
			if (stsCertificates == null)
			{
				LocalizedException exception3 = new LocalizedException(new LocalizedString("Error: GetStsCertificates(federationTrust) returned null when called in Process()"));
				base.ThrowTerminatingError(exception3, ErrorCategory.InvalidData, null);
			}
			if (organizationCertificates == null)
			{
				LocalizedException exception4 = new LocalizedException(new LocalizedString("Error: GetOrganizationCertificates(federationTrust) returned null when called in Process()"));
				base.ThrowTerminatingError(exception4, ErrorCategory.InvalidData, null);
			}
			base.WriteObject("RESULT: Success." + Environment.NewLine + " ");
			SecurityTokenService securityTokenService = new SecurityTokenService(federationTrust.TokenIssuerEpr, LiveConfiguration.GetWebProxy(new WriteVerboseDelegate(base.WriteVerbose)), organizationCertificates[0], federationTrust.TokenIssuerUri, federationTrust.PolicyReferenceUri, federationTrust.ApplicationUri.OriginalString);
			this.PrintConfiguration(user, federationTrust);
			base.WriteObject(Environment.NewLine + "STEP 5 of 6: Requesting delegation token...");
			RequestedToken delegationToken = this.GetDelegationToken(user, federationTrust.ApplicationUri, securityTokenService);
			base.WriteObject("RESULT: Success. Token retrieved.");
			if (delegationToken != null)
			{
				base.WriteObject(Environment.NewLine + "STEP 6 of 6: Validating delegation token...");
				TokenValidator tokenValidator = new TokenValidator(federationTrust.ApplicationUri, stsCertificates, organizationCertificates);
				this.ValidateToken(delegationToken, tokenValidator);
				base.WriteObject("RESULT: Success.");
			}
			else
			{
				LocalizedException exception5 = new LocalizedException(new LocalizedString("Error. Attempted to get delegation token, but token came back as null."));
				base.ThrowTerminatingError(exception5, ErrorCategory.InvalidData, null);
			}
			base.WriteVerbose(new LocalizedString(Environment.NewLine + "COMPLETE." + Environment.NewLine));
		}

		private void PrintConfiguration(ADUser user, FederationTrust fedTrust)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			string text = enabled ? "Online" : "On-Prem";
			bool flag = false;
			HashSet<string> localFederatedDomains = this.GetLocalFederatedDomains(user);
			int startIndex = user.GetFederatedIdentity().Identity.IndexOf('@') + 1;
			string text2 = user.GetFederatedIdentity().Identity.Substring(startIndex);
			if (localFederatedDomains.Contains(text2))
			{
				flag = true;
			}
			OrganizationId organizationId = user.OrganizationId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			FederatedOrganizationId federatedOrganizationId = organizationIdCacheValue.FederatedOrganizationId;
			string text3 = string.Concat(new string[]
			{
				Environment.NewLine,
				Environment.NewLine,
				"Printing current configuration...",
				Environment.NewLine,
				Environment.NewLine,
				"---------------------------------------------------------------------",
				Environment.NewLine,
				"Online or On-Prem: ",
				text,
				Environment.NewLine,
				Environment.NewLine,
				"Printing configuration information (from FederationTrust): ",
				Environment.NewLine,
				"Name: ",
				fedTrust.Name,
				Environment.NewLine,
				"TokenIssuerUri: ",
				fedTrust.TokenIssuerUri.ToString(),
				Environment.NewLine,
				"TokenIssuerType: ",
				fedTrust.TokenIssuerType.ToString(),
				Environment.NewLine,
				"TokenIssuerEpr: ",
				fedTrust.TokenIssuerEpr.ToString(),
				Environment.NewLine,
				"TokenIssuerMetadataEpr: ",
				fedTrust.TokenIssuerMetadataEpr.ToString(),
				Environment.NewLine,
				"TokenIssuerCertificateThumbprint: ",
				fedTrust.TokenIssuerCertificate.Thumbprint.ToString(),
				Environment.NewLine,
				"TokenIssuerCertReference: ",
				fedTrust.TokenIssuerCertReference,
				Environment.NewLine,
				"ApplicationIdentifier: ",
				fedTrust.ApplicationIdentifier,
				Environment.NewLine,
				"ApplicationUri: ",
				fedTrust.ApplicationUri.ToString(),
				Environment.NewLine,
				"OrgPublicCertificateThumbprint: ",
				fedTrust.OrgCertificate.Thumbprint.ToString(),
				Environment.NewLine,
				"PolicyReferenceUri: ",
				fedTrust.PolicyReferenceUri,
				Environment.NewLine,
				Environment.NewLine,
				"Printing configuration information (from ADUser): ",
				Environment.NewLine
			});
			if (flag)
			{
				text3 = text3 + "Domain Type: Federated" + Environment.NewLine;
			}
			else
			{
				text3 = text3 + "Domain Type: Managed" + Environment.NewLine;
			}
			text3 = string.Concat(new object[]
			{
				text3,
				"Domain Name: ",
				text2,
				Environment.NewLine,
				"Email: ",
				user.GetFederatedSmtpAddress().ToString(),
				Environment.NewLine,
				"RST Identity Type = FederatedIdentity Type: ",
				user.GetFederatedIdentity().Type,
				Environment.NewLine,
				"RST Idenitty Value = FederatedIdentity Value: ",
				user.GetFederatedIdentity().Identity,
				Environment.NewLine,
				"ImmutableId: ",
				user.ImmutableId.ToString(),
				Environment.NewLine,
				"msExchOnPremiseObjectGuid: ",
				user.OnPremisesObjectId.ToString(),
				Environment.NewLine,
				"ObjectGuid: ",
				user.Guid.ToString(),
				Environment.NewLine
			});
			if (enabled)
			{
				text3 = text3 + "WindowsLiveID: " + user.WindowsLiveID.ToString() + Environment.NewLine;
			}
			else
			{
				text3 = text3 + "AccountNamespace: " + federatedOrganizationId.AccountNamespace.ToString() + Environment.NewLine;
			}
			text3 = string.Concat(new string[]
			{
				text3,
				"SID: ",
				user.Sid.ToString(),
				Environment.NewLine,
				"Organization ID: ",
				user.OrganizationId.ToString(),
				Environment.NewLine,
				Environment.NewLine,
				"Offer: ",
				TestFederationTrust.TokenOffer.ToString(),
				Environment.NewLine,
				"---------------------------------------------------------------------",
				Environment.NewLine
			});
			base.WriteVerbose(new LocalizedString(text3));
			this.ValidateConfiguration(enabled, flag, user, text2);
		}

		private void ValidateConfiguration(bool online, bool federated, ADUser user, string adUserDomainName)
		{
			base.WriteObject(string.Concat(new string[]
			{
				Environment.NewLine,
				"Validating current configuration for ",
				adUserDomainName,
				"...",
				Environment.NewLine,
				" "
			}));
			if (online)
			{
				base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the WindowsLiveID is set, since it must be for online..."));
				SmtpAddress windowsLiveID = user.WindowsLiveID;
				if (user.WindowsLiveID.ToString().Length == 0)
				{
					LocalizedException exception = new LocalizedException(new LocalizedString("Error: The WindowsLiveId must be set when online, and it is not set."));
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidData, null);
				}
				base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
				if (federated)
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the RST Identity type is ImmutableId, since it must be because adUser is federated..."));
					if (!user.GetFederatedIdentity().Type.ToString().Equals("ImmutableId"))
					{
						LocalizedException exception2 = new LocalizedException(new LocalizedString("Error: the adUser's domain is federated, but the RST Identity type is UPN. Federated domains must have an RST Identity type of ImmutableId."));
						base.ThrowTerminatingError(exception2, ErrorCategory.InvalidData, null);
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if either msExchImmutableId or msExchOnPremiseObjectGuid is set, since at least one must be because adUser is federated..."));
					if ((user.OnPremisesObjectId == null || user.OnPremisesObjectId.ToString().Length == 0) && (user.ImmutableId == null || user.ImmutableId.ToString().Length == 0))
					{
						LocalizedException exception3 = new LocalizedException(new LocalizedString("Error: the ADUser's domain is federated, but both the ImmutableId and the OnPremiseObjectGuid are not set. One of these two must be set when the domain type is federated."));
						base.ThrowTerminatingError(exception3, ErrorCategory.InvalidData, null);
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
				}
				else
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the RST Identity type is UPN, since it must be because adUser is managed..."));
					if (!user.GetFederatedIdentity().Type.ToString().Equals("UPN"))
					{
						LocalizedException exception4 = new LocalizedException(new LocalizedString("Error: the adUser's domain is managed, but the RST Identity type is ImmutableId. Managed domains must have an RST Identity type of UPN."));
						base.ThrowTerminatingError(exception4, ErrorCategory.InvalidData, null);
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
				}
			}
			base.WriteObject(Environment.NewLine + "Validation successful.");
		}

		private HashSet<string> GetLocalFederatedDomains(ADUser user)
		{
			OrganizationId organizationId = user.OrganizationId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			return new HashSet<string>(organizationIdCacheValue.FederatedDomains);
		}

		private ADUser GetUser()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 433, "GetUser", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestFederationTrust.cs");
			return (ADUser)base.GetDataObject<ADUser>(this.UserIdentity, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.UserIdentity.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.UserIdentity.ToString())));
		}

		private FederationTrust GetFederationTrust(ADUser user)
		{
			base.WriteVerbose(new LocalizedString(string.Concat(new object[]
			{
				Environment.NewLine,
				"using ADUser OrganizationId: ",
				user.OrganizationId,
				" "
			})));
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), user.OrganizationId, null, false);
			adsessionSettings.IsSharedConfigChecked = true;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 463, "GetFederationTrust", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestFederationTrust.cs");
			FederatedOrganizationId federatedOrganizationId = tenantOrTopologyConfigurationSession.GetFederatedOrganizationId(user.OrganizationId);
			if (federatedOrganizationId == null || !federatedOrganizationId.Enabled)
			{
				this.Log(EventTypeEnumeration.Information, TestFederationTrust.TestFederationTrustEventId.NoFederationTrust, Strings.NoFederationTrust);
				return null;
			}
			ADSessionSettings adsessionSettings2 = ADSessionSettings.FromRootOrgScopeSet();
			adsessionSettings2.IsSharedConfigChecked = true;
			IConfigurationSession tenantOrTopologyConfigurationSession2 = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, adsessionSettings2, 486, "GetFederationTrust", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestFederationTrust.cs");
			FederationTrust federationTrust = tenantOrTopologyConfigurationSession2.Read<FederationTrust>(federatedOrganizationId.DelegationTrustLink);
			if (federationTrust == null)
			{
				this.Log(EventTypeEnumeration.Information, TestFederationTrust.TestFederationTrustEventId.NoFederationTrust, Strings.NoFederationTrust);
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			if (string.IsNullOrEmpty(federationTrust.OrgPrivCertificate))
			{
				stringBuilder.Append("OrgPrivCertificate");
				flag = true;
			}
			var array = new <>f__AnonymousType26<object, string>[]
			{
				new
				{
					Value = federationTrust.TokenIssuerUri,
					Name = "TokenIssuerUri"
				},
				new
				{
					Value = federationTrust.TokenIssuerEpr,
					Name = "TokenIssuerEpr"
				},
				new
				{
					Value = federationTrust.ApplicationUri,
					Name = "ApplicationUri"
				},
				new
				{
					Value = federationTrust.TokenIssuerCertificate,
					Name = "TokenIssuerCertificate"
				}
			};
			var array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				var <>f__AnonymousType = array2[i];
				if (<>f__AnonymousType.Value == null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(<>f__AnonymousType.Name);
					flag = true;
				}
			}
			if (flag)
			{
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.FederationTrustConfiguration, Strings.MissingPropertyInFederationTrust(stringBuilder.ToString()));
				return null;
			}
			base.WriteVerbose(new LocalizedString(string.Concat(new object[]
			{
				Environment.NewLine,
				Environment.NewLine,
				"Retrieved FederationTrust object with the following properties: ",
				Environment.NewLine,
				"TokenIssuerUri: ",
				federationTrust.TokenIssuerUri,
				Environment.NewLine,
				"TokenIssuerEpr: ",
				federationTrust.TokenIssuerEpr,
				Environment.NewLine,
				"ApplicationUri: ",
				federationTrust.ApplicationUri,
				Environment.NewLine,
				"TokenIssuerCertificate Thumbprint: ",
				federationTrust.TokenIssuerCertificate.Thumbprint
			})));
			try
			{
				user.GetFederatedIdentity();
			}
			catch (LocalizedException ex)
			{
				base.WriteVerbose(Strings.FailureAndReason(Strings.FederatedOrganizationIdNotSet.ToString(), ex.ToString()));
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.FederationTrustConfiguration, Strings.FederatedOrganizationIdNotSet);
				return null;
			}
			try
			{
				user.GetFederatedSmtpAddress();
			}
			catch (LocalizedException ex2)
			{
				base.WriteVerbose(Strings.FailureAndReason(Strings.UserWithoutFederatedDomain.ToString(), ex2.ToString()));
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.FederationTrustConfiguration, Strings.UserWithoutFederatedDomain);
				return null;
			}
			this.Log(EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.FederationTrustConfiguration, Strings.FederationTrustValid);
			return federationTrust;
		}

		private X509Certificate2[] GetStsCertificates(FederationTrust federationTrust)
		{
			bool flag = federationTrust.TokenIssuerCertificate != null && TestFederationTrust.IsExpiredCertificate(federationTrust.TokenIssuerCertificate);
			bool flag2 = federationTrust.TokenIssuerPrevCertificate != null && TestFederationTrust.IsExpiredCertificate(federationTrust.TokenIssuerPrevCertificate);
			LocalizedString message = flag ? Strings.CertificateExpired("TokenIssuerCertificate") : Strings.CertificateValid("TokenIssuerCertificate");
			LocalizedString message2 = flag2 ? Strings.CertificateExpired("TokenIssuerPrevCertificate") : Strings.CertificateValid("TokenIssuerPrevCertificate");
			if (!flag && !flag2)
			{
				base.WriteVerbose(new LocalizedString(string.Concat(new string[]
				{
					Environment.NewLine,
					"Both STS trust certificates [current <",
					federationTrust.TokenIssuerCertificate.Thumbprint,
					"> and previous <",
					federationTrust.TokenIssuerPrevCertificate.Thumbprint,
					">] are valid."
				})));
			}
			else if (!flag && flag2)
			{
				base.WriteVerbose(new LocalizedString(string.Concat(new string[]
				{
					Environment.NewLine,
					"The current STS trust certificate <",
					federationTrust.TokenIssuerCertificate.Thumbprint,
					"> is valid, but the previous STS trust certificate <",
					federationTrust.TokenIssuerPrevCertificate.Thumbprint,
					"> has expired."
				})));
			}
			else if (flag && !flag2)
			{
				base.WriteVerbose(new LocalizedString(string.Concat(new string[]
				{
					Environment.NewLine,
					"The current STS trust certificate <",
					federationTrust.TokenIssuerCertificate.Thumbprint,
					"> has expired. The previous STS trust certificate <",
					federationTrust.TokenIssuerPrevCertificate.Thumbprint,
					"> is still valid."
				})));
			}
			if (flag && flag2)
			{
				base.WriteVerbose(new LocalizedString(string.Concat(new string[]
				{
					Environment.NewLine,
					"Both STS trust certificates [current <",
					federationTrust.TokenIssuerCertificate.Thumbprint,
					"> and previous <",
					federationTrust.TokenIssuerPrevCertificate.Thumbprint,
					">] have expired."
				})));
				if (federationTrust.TokenIssuerCertificate != null)
				{
					this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.StsCertificate, message);
				}
				if (federationTrust.TokenIssuerPrevCertificate != null)
				{
					this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.StsPreviousCertificate, message2);
				}
				return null;
			}
			if (federationTrust.TokenIssuerCertificate != null)
			{
				this.Log(flag ? EventTypeEnumeration.Warning : EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.StsCertificate, message);
			}
			if (federationTrust.TokenIssuerPrevCertificate != null)
			{
				this.Log(flag2 ? EventTypeEnumeration.Warning : EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.StsPreviousCertificate, message2);
			}
			List<X509Certificate2> list = new List<X509Certificate2>(2);
			if (federationTrust.TokenIssuerCertificate != null && !flag)
			{
				list.Add(federationTrust.TokenIssuerCertificate);
			}
			if (federationTrust.TokenIssuerPrevCertificate != null && !flag2)
			{
				list.Add(federationTrust.TokenIssuerPrevCertificate);
			}
			return list.ToArray();
		}

		private X509Certificate2[] GetOrganizationCertificates(FederationTrust federationTrust)
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.ReadOnly);
			X509Certificate2[] result;
			try
			{
				X509Certificate2 organizationCertificate = this.GetOrganizationCertificate(x509Store, federationTrust.OrgPrivCertificate, TestFederationTrust.TestFederationTrustEventId.OrganizationCertificate, "OrgPrivCertificate");
				if (organizationCertificate == null)
				{
					this.WriteWarning(new LocalizedString("Could not retrieve orgPrivCertificate from GetOrganizationCertificates"));
					result = null;
				}
				else
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + Environment.NewLine + "orgPrivCertificate: " + organizationCertificate.Thumbprint));
					X509Certificate2 x509Certificate = null;
					if (!string.IsNullOrEmpty(federationTrust.OrgPrevPrivCertificate))
					{
						x509Certificate = this.GetOrganizationCertificate(x509Store, federationTrust.OrgPrevPrivCertificate, TestFederationTrust.TestFederationTrustEventId.OrganizationPreviousCertificate, "OrgPrevPrivCertificate");
						base.WriteVerbose(new LocalizedString(Environment.NewLine + "orgPrevPrivCertificate: " + x509Certificate.Thumbprint));
					}
					result = ((x509Certificate != null) ? new X509Certificate2[]
					{
						organizationCertificate,
						x509Certificate
					} : new X509Certificate2[]
					{
						organizationCertificate
					});
				}
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		private X509Certificate2 GetOrganizationCertificate(X509Store store, string thumbprint, TestFederationTrust.TestFederationTrustEventId eventId, string propertyName)
		{
			X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
			if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0 || x509Certificate2Collection[0] == null)
			{
				this.Log(EventTypeEnumeration.Error, eventId, Strings.FederationCertificateNotFound(propertyName));
				return null;
			}
			X509Certificate2 x509Certificate = x509Certificate2Collection[0];
			if (TestFederationTrust.IsExpiredCertificate(x509Certificate))
			{
				this.Log(EventTypeEnumeration.Error, eventId, Strings.FederationCertificateExpired(propertyName));
				return null;
			}
			if (!this.IsValidPrivateKey(x509Certificate, eventId, propertyName))
			{
				this.Log(EventTypeEnumeration.Error, eventId, Strings.FederationCertificateHasNoPrivateKey(propertyName));
				return null;
			}
			this.Log(EventTypeEnumeration.Success, eventId, Strings.CertificateValid(propertyName));
			return x509Certificate;
		}

		private bool IsValidPrivateKey(X509Certificate2 certificate, TestFederationTrust.TestFederationTrustEventId eventId, string propertyName)
		{
			if (!certificate.HasPrivateKey)
			{
				return false;
			}
			bool result;
			try
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = certificate.PrivateKey as RSACryptoServiceProvider;
				result = (rsacryptoServiceProvider != null);
			}
			catch (CryptographicException)
			{
				result = false;
			}
			catch (NotSupportedException)
			{
				result = false;
			}
			return result;
		}

		private static bool IsExpiredCertificate(X509Certificate2 certificate)
		{
			DateTime utcNow = DateTime.UtcNow;
			return utcNow > certificate.NotAfter || utcNow < certificate.NotBefore;
		}

		private RequestedToken GetDelegationToken(ADUser user, Uri target, SecurityTokenService securityTokenService)
		{
			RequestedToken result;
			try
			{
				DelegationTokenRequest request = new DelegationTokenRequest
				{
					FederatedIdentity = user.GetFederatedIdentity(),
					EmailAddress = user.GetFederatedSmtpAddress().ToString(),
					Target = new TokenTarget(target),
					Offer = TestFederationTrust.TokenOffer
				};
				RequestedToken requestedToken = securityTokenService.IssueToken(request);
				this.Log(EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.TokenRequest, Strings.DelegationTokenRequestSuccess);
				result = requestedToken;
			}
			catch (LocalizedException ex)
			{
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.TokenRequest, Strings.TokenRequestFailed);
				base.WriteVerbose(Strings.FailureAndReason(Strings.TokenRequestFailed.ToString(), ex.ToString()));
				result = null;
			}
			return result;
		}

		private void ValidateToken(RequestedToken requestedToken, TokenValidator tokenValidator)
		{
			try
			{
				TokenValidationResults tokenValidationResults = tokenValidator.ValidateToken(requestedToken.SecurityToken, TestFederationTrust.TokenOffer);
				if (tokenValidationResults.Result == TokenValidationResult.Valid)
				{
					this.Log(EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.TokenValidation, Strings.DelegationTokenValidationSuccess);
				}
				else
				{
					this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.TokenValidation, Strings.TokenValidationFailed);
					base.WriteVerbose(Strings.FailureAndReason(Strings.TokenValidationFailed.ToString(), tokenValidationResults.Result.ToString()));
				}
			}
			catch (LocalizedException ex)
			{
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.TokenValidation, Strings.TokenValidationFailed);
				base.WriteVerbose(Strings.FailureAndReason(Strings.TokenValidationFailed.ToString(), ex.ToString()));
			}
		}

		private void ValidateFederationTrustCertificatesWithFederationMetadata(FederationTrust federationTrust)
		{
			if (federationTrust.TokenIssuerMetadataEpr == null)
			{
				this.Log(EventTypeEnumeration.Information, TestFederationTrust.TestFederationTrustEventId.FederationMetadata, Strings.NoFederationMetadataEpr);
				return;
			}
			PartnerFederationMetadata partnerFederationMetadata = null;
			try
			{
				partnerFederationMetadata = LivePartnerFederationMetadata.LoadFrom(federationTrust.TokenIssuerMetadataEpr, new WriteVerboseDelegate(base.WriteVerbose));
			}
			catch (FederationMetadataException ex)
			{
				this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.FederationMetadata, Strings.RetrieveFederationMetadataFailed);
				base.WriteVerbose(Strings.FailureAndReason(Strings.RetrieveFederationMetadataFailed.ToString(), ex.ToString()));
				return;
			}
			HashSet<string> nonExpiredCertificateThumbprint = this.GetNonExpiredCertificateThumbprint(federationTrust.TokenIssuerMetadataEpr.ToString(), new X509Certificate2[]
			{
				partnerFederationMetadata.TokenIssuerCertificate,
				partnerFederationMetadata.TokenIssuerPrevCertificate
			});
			HashSet<string> nonExpiredCertificateThumbprint2 = this.GetNonExpiredCertificateThumbprint("FederationTrust", new X509Certificate2[]
			{
				federationTrust.TokenIssuerCertificate,
				federationTrust.TokenIssuerPrevCertificate
			});
			base.WriteVerbose(new LocalizedString(string.Concat(new string[]
			{
				Environment.NewLine,
				Environment.NewLine,
				"Federation Trust Certificates: ",
				Environment.NewLine,
				"TokenIssuerCertificate: ",
				federationTrust.TokenIssuerCertificate.Thumbprint,
				Environment.NewLine,
				"TokenIssuerPrevCertificate: ",
				federationTrust.TokenIssuerPrevCertificate.Thumbprint,
				Environment.NewLine,
				Environment.NewLine,
				"Federation Metadata Certificates: ",
				Environment.NewLine,
				"TokenIssuerCertificate: ",
				partnerFederationMetadata.TokenIssuerCertificate.Thumbprint,
				Environment.NewLine,
				"TokenIssuerPrevCertificate: ",
				partnerFederationMetadata.TokenIssuerPrevCertificate.Thumbprint
			})));
			if (nonExpiredCertificateThumbprint.SetEquals(nonExpiredCertificateThumbprint2))
			{
				this.Log(EventTypeEnumeration.Success, TestFederationTrust.TestFederationTrustEventId.FederationMetadata, Strings.FederationTrustHasAllStsCertificates);
				return;
			}
			this.Log(EventTypeEnumeration.Error, TestFederationTrust.TestFederationTrustEventId.FederationMetadata, Strings.FederationTrustHasOutdatedCertificates);
		}

		private HashSet<string> GetNonExpiredCertificateThumbprint(string source, params X509Certificate2[] certificatesToCheck)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (X509Certificate2 x509Certificate in certificatesToCheck)
			{
				if (x509Certificate != null)
				{
					if (TestFederationTrust.IsExpiredCertificate(x509Certificate))
					{
						base.WriteVerbose(Strings.IgnoringExpiredCertificate(x509Certificate.Thumbprint, source));
					}
					else
					{
						hashSet.Add(x509Certificate.Thumbprint);
					}
				}
			}
			return hashSet;
		}

		private void Log(EventTypeEnumeration type, TestFederationTrust.TestFederationTrustEventId id, LocalizedString message)
		{
			this.events.Add(new TestFederationTrust.ResultEvent
			{
				Id = id,
				Type = type,
				Message = message
			});
		}

		private List<TestFederationTrust.ResultEvent> events = new List<TestFederationTrust.ResultEvent>(5);

		private static readonly Offer TokenOffer = Offer.SharingCalendarFreeBusy;

		private static readonly string EventSource = "MSExchange Monitoring Test-FederationTrust";

		internal sealed class ResultEvent
		{
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"Id=",
					this.Id,
					",Type=",
					this.Type,
					",Message=",
					this.Message
				});
			}

			public TestFederationTrust.TestFederationTrustEventId Id;

			public EventTypeEnumeration Type;

			public LocalizedString Message;
		}

		internal enum TestFederationTrustEventId
		{
			NoFederationTrust = 1100,
			FederationTrustConfiguration,
			StsCertificate,
			StsPreviousCertificate,
			OrganizationCertificate,
			OrganizationPreviousCertificate,
			TokenRequest,
			TokenValidation,
			FederationMetadata
		}
	}
}

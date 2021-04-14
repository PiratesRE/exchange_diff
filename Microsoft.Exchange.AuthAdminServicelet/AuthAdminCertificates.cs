using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Servicelets.AuthAdmin.Messages;

namespace Microsoft.Exchange.Servicelets.AuthAdmin
{
	internal class AuthAdminCertificates
	{
		internal AuthAdminCertificates(AuthAdminContext context, WaitHandle stopEvent)
		{
			this.Context = context;
			this.StopEvent = stopEvent;
		}

		private protected AuthAdminContext Context { protected get; private set; }

		private protected WaitHandle StopEvent { protected get; private set; }

		protected bool TrustAnySSLCertificate { get; set; }

		internal void DoScheduledWork(ITopologyConfigurationSession session)
		{
			if (this.StopEvent.WaitOne(0, false))
			{
				return;
			}
			this.session = session;
			this.Context.Logger.Log(MigrationEventType.Information, "Starting automatic certificate administration for Trusted Issuers", new object[0]);
			this.TrustAnySSLCertificate = this.Context.Config.GetConfig<bool>("TrustAnySSLCertificate");
			this.Context.Logger.Log(MigrationEventType.Information, "Processing Authentication Servers", new object[0]);
			this.ProcessAllAuthServers();
			if (this.StopEvent.WaitOne(0, false))
			{
				return;
			}
			this.Context.Logger.Log(MigrationEventType.Information, "Processing Delegated AuthN Federation Servers", new object[0]);
			this.ProcessAllFederationTrusts();
			if (this.StopEvent.WaitOne(0, false))
			{
				return;
			}
			this.Context.Logger.Log(MigrationEventType.Information, "Processing Partner Applications", new object[0]);
			this.ProcessAllPartnerApplications();
			this.Context.Logger.Log(MigrationEventType.Information, "Certificate administration complete for trusted issuers", new object[0]);
		}

		private void ProcessAllAuthServers()
		{
			AuthServer[] authServers = null;
			this.Context.Logger.Log(MigrationEventType.Information, "Retrieving Authentication Server Configuration", new object[0]);
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				authServers = this.session.Find<AuthServer>(null, QueryScope.SubTree, new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.MicrosoftACS),
					new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.AzureAD),
					new ComparisonFilter(ComparisonOperator.Equal, AuthServerSchema.Type, AuthServerType.ADFS)
				}), null, ADGenericPagedReader<AuthServer>.DefaultPageSize);
			});
			if (adoperationResult != ADOperationResult.Success)
			{
				this.Context.Logger.Log(MigrationEventType.Warning, "Unable to read Authentication Servers, result = {0}", new object[]
				{
					adoperationResult.ErrorCode.ToString()
				});
				if (adoperationResult.Exception != null && adoperationResult.Exception is TransientException)
				{
					throw adoperationResult.Exception;
				}
				return;
			}
			else
			{
				if (authServers == null)
				{
					this.Context.Logger.Log(MigrationEventType.Information, "No Authentication Servers found", new object[0]);
					return;
				}
				foreach (AuthServer authServer in authServers)
				{
					if (this.StopEvent.WaitOne(0, false))
					{
						return;
					}
					this.Context.Logger.Log(MigrationEventType.Information, "Processing Authentication Server {0}", new object[]
					{
						authServer.Name
					});
					if (!authServer.Enabled)
					{
						this.Context.Logger.Log(MigrationEventType.Information, "Authentication Server {0} is not enabled", new object[]
						{
							authServer.Name
						});
					}
					else if (string.IsNullOrEmpty(authServer.AuthMetadataUrl))
					{
						this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerConfiguration, new string[]
						{
							authServer.Name
						});
					}
					else if (this.ProcessAuthServer(authServer))
					{
						this.session.Save(authServer);
						this.Context.Logger.LogTerseEvent(MigrationEventType.Information, MSExchangeAuthAdminEventLogConstants.Tuple_TrustedIssuerUpdated, new string[]
						{
							authServer.Name
						});
					}
				}
				return;
			}
		}

		private bool ProcessAuthServer(AuthServer authServer)
		{
			bool result = false;
			AuthMetadata authMetadata = this.FetchMetadata(authServer.Name, authServer.AuthMetadataUrl, true);
			if (authMetadata == null)
			{
				return false;
			}
			AuthMetadataParser.SetEndpointsIfWSFed(authMetadata, authServer.Type, authServer.AuthMetadataUrl);
			if (!OAuthCommon.IsIdMatch(authServer.IssuerIdentifier, authMetadata.ServiceName) || !OAuthCommon.IsRealmMatchIncludingEmpty(authServer.Realm, authMetadata.Realm) || string.IsNullOrEmpty(authMetadata.IssuingEndpoint) || ((authServer.Type == AuthServerType.AzureAD || authServer.Type == AuthServerType.ADFS) && string.IsNullOrEmpty(authMetadata.AuthorizationEndpoint)))
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerChanges, new string[]
				{
					authServer.Name,
					authServer.AuthMetadataUrl
				});
				return false;
			}
			if (string.Compare(authServer.TokenIssuingEndpoint, authMetadata.IssuingEndpoint, StringComparison.OrdinalIgnoreCase) != 0)
			{
				result = true;
				authServer.TokenIssuingEndpoint = authMetadata.IssuingEndpoint;
			}
			if ((authServer.Type == AuthServerType.AzureAD || authServer.Type == AuthServerType.ADFS) && string.Compare(authServer.AuthorizationEndpoint, authMetadata.AuthorizationEndpoint, StringComparison.OrdinalIgnoreCase) != 0)
			{
				result = true;
				authServer.AuthorizationEndpoint = authMetadata.AuthorizationEndpoint;
			}
			MultiValuedProperty<byte[]> multiValuedProperty = null;
			if (this.ProcessCertificates(authServer.Name, authServer.CertificateBytes, authMetadata.CertificateStrings, out multiValuedProperty) && multiValuedProperty != null)
			{
				result = true;
				authServer.CertificateBytes = multiValuedProperty;
			}
			return result;
		}

		private void ProcessAllPartnerApplications()
		{
			PartnerApplication[] partnerApps = null;
			this.Context.Logger.Log(MigrationEventType.Information, "Retrieving Partner Application Configuration", new object[0]);
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				partnerApps = this.session.Find<PartnerApplication>(null, QueryScope.SubTree, null, null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize);
			});
			if (adoperationResult != ADOperationResult.Success)
			{
				this.Context.Logger.Log(MigrationEventType.Warning, "Unable to read Partner Applications, result = {0}", new object[]
				{
					adoperationResult.ErrorCode.ToString()
				});
				if (adoperationResult.Exception != null && adoperationResult.Exception is TransientException)
				{
					throw adoperationResult.Exception;
				}
				return;
			}
			else
			{
				if (partnerApps == null)
				{
					this.Context.Logger.Log(MigrationEventType.Information, "No Partner Applications found", new object[0]);
					return;
				}
				foreach (PartnerApplication partnerApplication in partnerApps)
				{
					if (this.StopEvent.WaitOne(0, false))
					{
						return;
					}
					this.Context.Logger.Log(MigrationEventType.Information, "Processing Partner Application {0}", new object[]
					{
						partnerApplication.Name
					});
					if (!partnerApplication.Enabled)
					{
						this.Context.Logger.Log(MigrationEventType.Information, "Partner Application {0} is not enabled", new object[]
						{
							partnerApplication.Name
						});
					}
					else if (partnerApplication.UseAuthServer)
					{
						this.Context.Logger.Log(MigrationEventType.Information, "Partner Application {0} uses Authentication Server, skipping", new object[]
						{
							partnerApplication.Name
						});
					}
					else if (string.IsNullOrEmpty(partnerApplication.AuthMetadataUrl))
					{
						this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerConfiguration, new string[]
						{
							partnerApplication.Name
						});
					}
					else if (this.ProcessPartnerApplication(partnerApplication))
					{
						this.session.Save(partnerApplication);
						this.Context.Logger.LogTerseEvent(MigrationEventType.Information, MSExchangeAuthAdminEventLogConstants.Tuple_TrustedIssuerUpdated, new string[]
						{
							partnerApplication.Name
						});
					}
				}
				return;
			}
		}

		private bool ProcessPartnerApplication(PartnerApplication partnerApplication)
		{
			bool result = false;
			AuthMetadata authMetadata = this.FetchMetadata(partnerApplication.Name, partnerApplication.AuthMetadataUrl, false);
			if (authMetadata == null)
			{
				return false;
			}
			if (!OAuthCommon.IsIdMatch(partnerApplication.ApplicationIdentifier, authMetadata.ServiceName) || !OAuthCommon.IsRealmMatchIncludingEmpty(partnerApplication.Realm, authMetadata.Realm) || !string.Equals(partnerApplication.IssuerIdentifier, authMetadata.Issuer))
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerChanges, new string[]
				{
					partnerApplication.Name,
					partnerApplication.AuthMetadataUrl
				});
				return false;
			}
			MultiValuedProperty<byte[]> multiValuedProperty = null;
			if (this.ProcessCertificates(partnerApplication.Name, partnerApplication.CertificateBytes, authMetadata.CertificateStrings, out multiValuedProperty) && multiValuedProperty != null)
			{
				result = true;
				partnerApplication.CertificateBytes = multiValuedProperty;
			}
			return result;
		}

		private void ProcessAllFederationTrusts()
		{
			FederationTrust[] federationTrusts = null;
			this.Context.Logger.Log(MigrationEventType.Information, "Retrieving Federation Trust Configuration", new object[0]);
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId orgContainerId = this.session.GetOrgContainerId();
				ADObjectId descendantId = orgContainerId.GetDescendantId(FederationTrust.FederationTrustsContainer);
				federationTrusts = this.session.Find<FederationTrust>(descendantId, QueryScope.SubTree, null, null, ADGenericPagedReader<FederationTrust>.DefaultPageSize);
			});
			if (adoperationResult != ADOperationResult.Success)
			{
				this.Context.Logger.Log(MigrationEventType.Warning, "Unable to read Federation Trusts, result = {0}", new object[]
				{
					adoperationResult.ErrorCode.ToString()
				});
				if (adoperationResult.Exception != null && adoperationResult.Exception is TransientException)
				{
					throw adoperationResult.Exception;
				}
				return;
			}
			else
			{
				if (federationTrusts == null)
				{
					this.Context.Logger.Log(MigrationEventType.Information, "No Federation Trusts found", new object[0]);
					return;
				}
				foreach (FederationTrust federationTrust in federationTrusts)
				{
					if (this.StopEvent.WaitOne(0, false))
					{
						return;
					}
					this.Context.Logger.Log(MigrationEventType.Information, "Processing Federation Trust {0}", new object[]
					{
						federationTrust.Name
					});
					if (federationTrust.TokenIssuerMetadataEpr == null)
					{
						this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerConfiguration, new string[]
						{
							federationTrust.Name
						});
					}
					else if (this.ProcessFederationTrust(federationTrust))
					{
						this.session.Save(federationTrust);
						this.Context.Logger.LogTerseEvent(MigrationEventType.Information, MSExchangeAuthAdminEventLogConstants.Tuple_TrustedIssuerUpdated, new string[]
						{
							federationTrust.Name
						});
					}
				}
				return;
			}
		}

		private bool ProcessFederationTrust(FederationTrust federationTrust)
		{
			PartnerFederationMetadata partnerFederationMetadata = null;
			try
			{
				partnerFederationMetadata = LivePartnerFederationMetadata.LoadFrom(federationTrust.TokenIssuerMetadataEpr, null);
			}
			catch (FederationMetadataException ex)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_UnableToAccessMetadata, new string[]
				{
					ex.Message,
					federationTrust.TokenIssuerMetadataEpr.OriginalString,
					federationTrust.Name
				});
			}
			catch (Exception ex2)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_UnableToAccessMetadata, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex2, null),
					federationTrust.TokenIssuerMetadataEpr.OriginalString,
					federationTrust.Name
				});
			}
			if (partnerFederationMetadata == null)
			{
				return false;
			}
			List<LocalizedString> warningMessages = new List<LocalizedString>();
			try
			{
				LivePartnerFederationMetadata.InitializeDataObjectFromMetadata(federationTrust, partnerFederationMetadata, delegate(LocalizedString localizedString)
				{
					warningMessages.Add(localizedString);
				});
			}
			catch (FederationMetadataException ex3)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_CorruptMetadata, new string[]
				{
					ex3.Message,
					federationTrust.TokenIssuerMetadataEpr.OriginalString,
					federationTrust.Name
				});
			}
			catch (Exception ex4)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_UnableToAccessMetadata, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex4, null),
					federationTrust.TokenIssuerMetadataEpr.OriginalString,
					federationTrust.Name
				});
			}
			if (warningMessages.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (LocalizedString localizedString2 in warningMessages)
				{
					stringBuilder.AppendFormat("{0};", localizedString2.ToString());
				}
				this.Context.Logger.LogTerseEvent(MigrationEventType.Warning, MSExchangeAuthAdminEventLogConstants.Tuple_Warning, new string[]
				{
					federationTrust.Name,
					stringBuilder.ToString()
				});
			}
			return federationTrust.ObjectState == ObjectState.Changed;
		}

		private AuthMetadata FetchMetadata(string name, string metadataUrl, bool requireIssuingEndpoint)
		{
			AuthMetadata result = null;
			Uri uri = null;
			if (!Uri.TryCreate(metadataUrl, UriKind.Absolute, out uri))
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidMetadataUrl, new string[]
				{
					metadataUrl,
					name
				});
				return null;
			}
			try
			{
				result = AuthMetadataClient.AcquireMetadata(metadataUrl, requireIssuingEndpoint, this.TrustAnySSLCertificate, true);
			}
			catch (AuthMetadataClientException ex)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_TransientException, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex, null)
				});
			}
			catch (AuthMetadataParserException ex2)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_CorruptMetadata, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex2, null),
					metadataUrl,
					name
				});
			}
			catch (Exception ex3)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_UnableToAccessMetadata, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex3, null),
					metadataUrl,
					name
				});
			}
			return result;
		}

		private bool ProcessCertificates(string name, MultiValuedProperty<byte[]> adCertBytes, string[] certificateStrings, out MultiValuedProperty<byte[]> newList)
		{
			newList = null;
			if (certificateStrings == null || certificateStrings.Length == 0 || certificateStrings.Length > AuthAdminCertificates.MaxCertCount)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidCertificateList, new string[]
				{
					name
				});
				return false;
			}
			X509Certificate2Collection x509Certificate2Collection = null;
			X509Certificate2Collection x509Certificate2Collection2 = null;
			try
			{
				x509Certificate2Collection = this.GetCertificateCollectionFromStrings(certificateStrings);
			}
			catch (Exception ex)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidCertificates, new string[]
				{
					AnchorLogger.GetDiagnosticInfo(ex, null),
					name
				});
				return false;
			}
			if (x509Certificate2Collection.Count != certificateStrings.Length)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidCertificateList, new string[]
				{
					name
				});
				return false;
			}
			try
			{
				x509Certificate2Collection2 = this.GetCertficateCollectionFromMVProperty(adCertBytes);
			}
			catch (Exception)
			{
				this.Context.Logger.LogTerseEvent(MigrationEventType.Error, MSExchangeAuthAdminEventLogConstants.Tuple_InvalidTrustedIssuerConfiguration, new string[]
				{
					name
				});
				return false;
			}
			if (x509Certificate2Collection2.Count != x509Certificate2Collection.Count)
			{
				newList = this.GetMVPropertyFromCertificateCollection(x509Certificate2Collection);
				return true;
			}
			foreach (X509Certificate2 certificate in x509Certificate2Collection)
			{
				if (!x509Certificate2Collection2.Contains(certificate))
				{
					newList = this.GetMVPropertyFromCertificateCollection(x509Certificate2Collection);
					break;
				}
			}
			return newList != null;
		}

		private X509Certificate2Collection GetCertificateCollectionFromStrings(string[] rawStringArray)
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			foreach (string text in rawStringArray)
			{
				if (!string.IsNullOrEmpty(text))
				{
					x509Certificate2Collection.Add(new X509Certificate2(Convert.FromBase64String(text)));
				}
			}
			return x509Certificate2Collection;
		}

		private X509Certificate2Collection GetCertficateCollectionFromMVProperty(MultiValuedProperty<byte[]> adCertBytes)
		{
			if (adCertBytes == null || adCertBytes.Count == 0)
			{
				return new X509Certificate2Collection();
			}
			return new X509Certificate2Collection((from certByte in adCertBytes
			select new X509Certificate2(certByte)).ToArray<X509Certificate2>());
		}

		private MultiValuedProperty<byte[]> GetMVPropertyFromCertificateCollection(X509Certificate2Collection certificateList)
		{
			byte[][] array = new byte[certificateList.Count][];
			int num = 0;
			foreach (X509Certificate2 x509Certificate in certificateList)
			{
				array[num++] = x509Certificate.GetRawCertData();
			}
			return new MultiValuedProperty<byte[]>(array);
		}

		private static readonly int MaxCertCount = 10;

		private ITopologyConfigurationSession session;
	}
}

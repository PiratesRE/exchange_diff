using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal static class FederationCertificate
	{
		internal static IEnumerable<CertificateRecord> FederationCertificates(IConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADPagedReader<FederationTrust> dataObjects = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId orgContainerId = session.GetOrgContainerId();
				ADObjectId descendantId = orgContainerId.GetDescendantId(FederationTrust.FederationTrustsContainer);
				dataObjects = session.FindPaged<FederationTrust>(descendantId, QueryScope.SubTree, null, null, 0);
			});
			if (!adoperationResult.Succeeded)
			{
				throw adoperationResult.Exception;
			}
			List<CertificateRecord> list = new List<CertificateRecord>();
			foreach (FederationTrust federationTrust in dataObjects)
			{
				if (!string.IsNullOrEmpty(federationTrust.OrgPrevPrivCertificate))
				{
					CertificateRecord certificateRecord = new CertificateRecord
					{
						Type = FederationCertificateType.PreviousCertificate,
						Thumbprint = federationTrust.OrgPrevPrivCertificate
					};
					CertificateRecord certificateRecord2 = list.Find(new Predicate<CertificateRecord>(certificateRecord.Equals));
					if (certificateRecord2 != null)
					{
						certificateRecord2.Type |= FederationCertificateType.PreviousCertificate;
					}
					else
					{
						list.Add(certificateRecord);
					}
				}
				if (!string.IsNullOrEmpty(federationTrust.OrgPrivCertificate))
				{
					CertificateRecord certificateRecord3 = new CertificateRecord
					{
						Type = FederationCertificateType.CurrentCertificate,
						Thumbprint = federationTrust.OrgPrivCertificate
					};
					CertificateRecord certificateRecord4 = list.Find(new Predicate<CertificateRecord>(certificateRecord3.Equals));
					if (certificateRecord4 != null)
					{
						certificateRecord4.Type |= FederationCertificateType.CurrentCertificate;
					}
					else
					{
						list.Add(certificateRecord3);
					}
				}
				if (!string.IsNullOrEmpty(federationTrust.OrgNextPrivCertificate))
				{
					CertificateRecord certificateRecord5 = new CertificateRecord
					{
						Type = FederationCertificateType.NextCertificate,
						Thumbprint = federationTrust.OrgNextPrivCertificate
					};
					CertificateRecord certificateRecord6 = list.Find(new Predicate<CertificateRecord>(certificateRecord5.Equals));
					if (certificateRecord6 != null)
					{
						certificateRecord6.Type |= FederationCertificateType.NextCertificate;
					}
					else
					{
						list.Add(certificateRecord5);
					}
				}
			}
			return list;
		}

		internal static void DiscoverServers(ITopologyConfigurationSession session, bool limitedSearch, out Dictionary<TopologySite, List<TopologyServer>> siteDictionary, out TopologySite localSite)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ExchangeTopology exchangeTopology = ExchangeTopology.Discover(session, ExchangeTopologyScope.ServerAndSiteTopology);
			localSite = exchangeTopology.LocalSite;
			siteDictionary = new Dictionary<TopologySite, List<TopologyServer>>();
			if (localSite == null)
			{
				return;
			}
			List<ITopologySite> list = new List<ITopologySite>();
			if (limitedSearch)
			{
				foreach (ITopologySiteLink topologySiteLink in localSite.TopologySiteLinks)
				{
					foreach (ITopologySite item in topologySiteLink.TopologySites)
					{
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			foreach (TopologyServer topologyServer in exchangeTopology.AllTopologyServers)
			{
				if (topologyServer.TopologySite != null && FederationCertificate.IsServerQualifiedForFederationTrust(topologyServer) && (!limitedSearch || list.Contains(topologyServer.TopologySite)))
				{
					List<TopologyServer> list2;
					if (!siteDictionary.TryGetValue(topologyServer.TopologySite, out list2))
					{
						list2 = new List<TopologyServer>();
						siteDictionary.Add(topologyServer.TopologySite, list2);
					}
					list2.Add(topologyServer);
				}
			}
		}

		internal static FederationTrustCertificateState TestForCertificate(string serverName, string thumbprint)
		{
			ExchangeCertificate exchangeCertificate;
			return FederationCertificate.TestForCertificate(serverName, thumbprint, out exchangeCertificate);
		}

		internal static FederationTrustCertificateState TestForCertificate(string serverName, string thumbprint, out ExchangeCertificate cert)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			if (string.IsNullOrEmpty(thumbprint))
			{
				throw new ArgumentNullException("thumbprint");
			}
			cert = null;
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			exchangeCertificateRpc.GetByThumbprint = thumbprint;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			FederationTrustCertificateState federationTrustCertificateState = FederationTrustCertificateState.NotInstalled;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(serverName);
				outputBlob = exchangeCertificateRpcClient.GetCertificate2(0, inBlob);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(serverName);
					outputBlob = exchangeCertificateRpcClient2.GetCertificate(0, inBlob2);
				}
				catch (RpcException)
				{
					federationTrustCertificateState = FederationTrustCertificateState.ServerUnreachable;
				}
			}
			if (federationTrustCertificateState != FederationTrustCertificateState.ServerUnreachable)
			{
				ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
				if (exchangeCertificateRpc2.ReturnCertList != null && exchangeCertificateRpc2.ReturnCertList.Count == 1)
				{
					federationTrustCertificateState = FederationTrustCertificateState.Installed;
					cert = exchangeCertificateRpc2.ReturnCertList[0];
				}
			}
			return federationTrustCertificateState;
		}

		internal static void PushCertificate(Task.TaskProgressLoggingDelegate writeProgress, Task.TaskWarningLoggingDelegate writeWarning, string thumbprint)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 324, "PushCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FederationProvisioning\\FederationCertificate.cs");
			Server sourceServer = null;
			try
			{
				sourceServer = topologyConfigurationSession.FindLocalServer();
			}
			catch (LocalServerNotFoundException)
			{
				writeWarning(Strings.WarningPushFailed(thumbprint));
				return;
			}
			FederationCertificate.PushCertificate(topologyConfigurationSession, sourceServer, writeProgress, writeWarning, thumbprint);
		}

		internal static void PushCertificate(Server sourceServer, Task.TaskProgressLoggingDelegate writeProgress, Task.TaskWarningLoggingDelegate writeWarning, string thumbprint)
		{
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 354, "PushCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FederationProvisioning\\FederationCertificate.cs");
			FederationCertificate.PushCertificate(session, sourceServer, writeProgress, writeWarning, thumbprint);
		}

		private static void PushCertificate(ITopologyConfigurationSession session, Server sourceServer, Task.TaskProgressLoggingDelegate writeProgress, Task.TaskWarningLoggingDelegate writeWarning, string thumbprint)
		{
			SecureString securePassword = FederationCertificate.GeneratePassword();
			FederationCertificate.EnableCertificateForNetworkService(sourceServer.Name, thumbprint);
			string base64cert = null;
			try
			{
				base64cert = FederationCertificate.ExportCertificate(sourceServer.Name, securePassword, thumbprint);
			}
			catch (InvalidOperationException)
			{
				writeWarning(Strings.WarningPushFailed(thumbprint));
				return;
			}
			catch (LocalizedException)
			{
				writeWarning(Strings.WarningPushFailed(thumbprint));
				return;
			}
			Dictionary<TopologySite, List<TopologyServer>> dictionary = null;
			TopologySite topologySite = null;
			FederationCertificate.DiscoverServers(session, true, out dictionary, out topologySite);
			if (topologySite != null)
			{
				List<TopologyServer> list;
				if (dictionary.TryGetValue(topologySite, out list))
				{
					int count = list.Count;
					int num = 0;
					foreach (TopologyServer topologyServer in list)
					{
						int percent = (int)((double)(++num) / (double)count * 100.0);
						writeProgress(Strings.ProgressActivityPushFederationCertificate(thumbprint), Strings.ProgressActivityPushFederationServer(topologyServer.Name), percent);
						if (!topologyServer.Id.Equals(sourceServer.Id))
						{
							try
							{
								FederationTrustCertificateState federationTrustCertificateState = FederationCertificate.TestForCertificate(topologyServer.Name, thumbprint);
								if (federationTrustCertificateState == FederationTrustCertificateState.NotInstalled)
								{
									FederationCertificate.ImportCertificate(topologyServer.Name, securePassword, base64cert);
								}
								if (federationTrustCertificateState != FederationTrustCertificateState.ServerUnreachable)
								{
									FederationCertificate.EnableCertificateForNetworkService(topologyServer.Name, thumbprint);
								}
							}
							catch (InvalidOperationException)
							{
								writeWarning(Strings.WarningPushCertificate(thumbprint, topologyServer.Name));
							}
							catch (LocalizedException)
							{
								writeWarning(Strings.WarningPushCertificate(thumbprint, topologyServer.Name));
							}
						}
					}
				}
				return;
			}
			writeWarning(Strings.WarningCannotGetLocalSite(thumbprint));
		}

		internal static string ExportCertificate(string source, SecureString securePassword, string thumbprint)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentNullException("source");
			}
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			if (string.IsNullOrEmpty(thumbprint))
			{
				throw new ArgumentNullException("thumbprint");
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			exchangeCertificateRpc.ExportByThumbprint = thumbprint;
			exchangeCertificateRpc.ExportBinary = true;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(source);
				outputBlob = exchangeCertificateRpcClient.ExportCertificate2(0, inBlob, securePassword);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(source);
					outputBlob = exchangeCertificateRpcClient2.ExportCertificate(0, inBlob2, securePassword);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.ThrowLocalizedException(e, source);
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			if (!string.IsNullOrEmpty(exchangeCertificateRpc2.ReturnTaskErrorString))
			{
				throw new InvalidOperationException(exchangeCertificateRpc2.ReturnTaskErrorString);
			}
			return Convert.ToBase64String(exchangeCertificateRpc2.ReturnExportFileData);
		}

		internal static void ImportCertificate(string destination, SecureString securePassword, string base64cert)
		{
			if (string.IsNullOrEmpty(destination))
			{
				throw new ArgumentNullException("destination");
			}
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			if (string.IsNullOrEmpty(base64cert))
			{
				throw new ArgumentNullException("base64cert");
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			exchangeCertificateRpc.ImportCert = base64cert;
			exchangeCertificateRpc.ImportExportable = true;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(destination);
				outputBlob = exchangeCertificateRpcClient.ImportCertificate2(0, inBlob, securePassword);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(destination);
					outputBlob = exchangeCertificateRpcClient2.ImportCertificate(0, inBlob2, securePassword);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.ThrowLocalizedException(e, destination);
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			if (!string.IsNullOrEmpty(exchangeCertificateRpc2.ReturnTaskErrorString))
			{
				throw new InvalidOperationException(exchangeCertificateRpc2.ReturnTaskErrorString);
			}
		}

		internal static void EnableCertificateForNetworkService(string destination, string thumbprint)
		{
			if (string.IsNullOrEmpty(destination))
			{
				throw new ArgumentNullException("destination");
			}
			if (string.IsNullOrEmpty(thumbprint))
			{
				throw new ArgumentNullException("thumbprint");
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			exchangeCertificateRpc.EnableByThumbprint = thumbprint;
			exchangeCertificateRpc.EnableNetworkService = true;
			exchangeCertificateRpc.EnableServices = AllowedServices.None;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(destination);
				outputBlob = exchangeCertificateRpcClient.EnableCertificate2(0, inBlob);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(destination);
					outputBlob = exchangeCertificateRpcClient2.EnableCertificate(0, inBlob2);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.ThrowLocalizedException(e, destination);
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			if (!string.IsNullOrEmpty(exchangeCertificateRpc2.ReturnTaskErrorString))
			{
				throw new InvalidOperationException(exchangeCertificateRpc2.ReturnTaskErrorString);
			}
		}

		internal static SecureString GeneratePassword()
		{
			string text = Guid.NewGuid().ToString();
			SecureString securePassword = new SecureString();
			Array.ForEach<char>(text.ToCharArray(), delegate(char c)
			{
				securePassword.AppendChar(c);
			});
			return securePassword;
		}

		internal static string UnifyThumbprintFormat(string thumbprint)
		{
			if (string.IsNullOrEmpty(thumbprint))
			{
				return null;
			}
			if (thumbprint.Contains(" "))
			{
				return thumbprint.Replace(" ", null);
			}
			return thumbprint;
		}

		internal static void ValidateCertificate(ExchangeCertificate certificate, bool skipAutomatedDeploymentChecks)
		{
			ExchangeCertificateValidity exchangeCertificateValidity = ManageExchangeCertificate.ValidateExchangeCertificate(certificate, true);
			if (exchangeCertificateValidity != ExchangeCertificateValidity.Valid)
			{
				throw new FederationCertificateInvalidException(Strings.CertificateNotValidForExchange(certificate.Thumbprint, exchangeCertificateValidity.ToString()));
			}
			if (string.IsNullOrEmpty(certificate.SubjectKeyIdentifier))
			{
				throw new FederationCertificateInvalidException(Strings.ErrorCertificateNoSKI(certificate.Thumbprint));
			}
			if (!skipAutomatedDeploymentChecks && !certificate.PrivateKeyExportable)
			{
				throw new FederationCertificateInvalidException(Strings.ErrorCertificateNotExportable(certificate.Thumbprint));
			}
			if (!string.Equals(certificate.GetKeyAlgorithm(), WellKnownOid.RsaRsa.Value, StringComparison.OrdinalIgnoreCase))
			{
				throw new FederationCertificateInvalidException(Strings.ErrorCertificateNotRSA(certificate.Thumbprint));
			}
			if (TlsCertificateInfo.IsCNGProvider(certificate))
			{
				throw new FederationCertificateInvalidException(Strings.ErrorCertificateNotCAPI(certificate.Thumbprint));
			}
			if ((ExDateTime)certificate.NotAfter < ExDateTime.UtcNow && (ExDateTime)certificate.NotBefore > ExDateTime.UtcNow)
			{
				throw new FederationCertificateInvalidException(Strings.ErrorCertificateHasExpired(certificate.Thumbprint));
			}
		}

		internal static X509Certificate2 LoadCertificateWithPrivateKey(string thumbprint, WriteVerboseDelegate writeVerbose)
		{
			X509Certificate2 exchangeFederationCertByThumbprint = FederationCertificate.GetExchangeFederationCertByThumbprint(thumbprint, writeVerbose);
			if (!exchangeFederationCertByThumbprint.HasPrivateKey)
			{
				writeVerbose(Strings.ErrorCertificateHasNoPrivateKey(thumbprint));
				return null;
			}
			try
			{
				if (!(exchangeFederationCertByThumbprint.PrivateKey is RSACryptoServiceProvider))
				{
					writeVerbose(Strings.ErrorCertificateHasNoPrivateKey(thumbprint));
					return null;
				}
			}
			catch (CryptographicException)
			{
				writeVerbose(Strings.ErrorCertificateHasNoPrivateKey(thumbprint));
				return null;
			}
			catch (NotSupportedException)
			{
				writeVerbose(Strings.ErrorCertificateHasNoPrivateKey(thumbprint));
				return null;
			}
			return exchangeFederationCertByThumbprint;
		}

		public static X509Certificate2 GetExchangeFederationCertByThumbprint(string thumbprint, WriteVerboseDelegate writeVerbose)
		{
			writeVerbose(Strings.SearchingForCertificate(thumbprint));
			if (string.IsNullOrEmpty(thumbprint))
			{
				throw new ArgumentNullException("thumbprint");
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			Exception ex = null;
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new FederationCertificateInvalidException(Strings.ErrorOpeningCertificateStore(x509Store.Name), ex);
			}
			try
			{
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection != null && x509Certificate2Collection.Count > 0)
				{
					return x509Certificate2Collection[0];
				}
			}
			finally
			{
				x509Store.Close();
			}
			throw new FederationCertificateInvalidException(Strings.ErrorCertificateNotFound(thumbprint));
		}

		private static bool IsServerQualifiedForFederationTrust(TopologyServer server)
		{
			return server.IsE14OrLater && (server.IsClientAccessServer || server.IsHubTransportServer || server.IsMailboxServer || server.IsCafeServer);
		}
	}
}

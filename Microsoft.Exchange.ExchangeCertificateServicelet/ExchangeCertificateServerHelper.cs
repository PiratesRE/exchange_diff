using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate
{
	internal class ExchangeCertificateServerHelper
	{
		public static byte[] CreateCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob)
		{
			bool flag = false;
			new List<ExchangeCertificate>();
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 345, "CreateCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(topologyConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			TimeSpan validFor = DateTime.UtcNow.AddMonths(60) - DateTime.UtcNow;
			exchangeCertificateRpc.ReturnTaskWarningList = new List<LocalizedString>();
			List<string> list = null;
			try
			{
				list = ExchangeCertificateServerHelper.EnsureCreateDefaults(server, topologyConfigurationSession, exchangeCertificateRpc);
			}
			catch (ArgumentException ex)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex.Message, ErrorCategory.InvalidArgument);
			}
			if (exchangeCertificateRpc.CreateWhatIf)
			{
				string domainNames = string.Empty;
				if (list != null && list.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(list.Count * 25);
					foreach (string value in list)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(value);
					}
					domainNames = stringBuilder.ToString();
				}
				if (exchangeCertificateRpc.CreateRequest)
				{
					exchangeCertificateRpc.ReturnConfirmation = Strings.ConfirmGenerateExchangeCertificateRequest(exchangeCertificateRpc.CreateFriendlyName ?? "Microsoft Exchange", exchangeCertificateRpc.CreateSubjectName ?? string.Empty, domainNames, exchangeCertificateRpc.CreateServices, exchangeCertificateRpc.CreateKeySize);
				}
				else
				{
					exchangeCertificateRpc.ReturnConfirmation = Strings.ConfirmGenerateExchangeCertificate(exchangeCertificateRpc.CreateFriendlyName ?? "Microsoft Exchange", exchangeCertificateRpc.CreateSubjectName ?? string.Empty, domainNames, exchangeCertificateRpc.CreateServices, exchangeCertificateRpc.CreateKeySize);
				}
				return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
			}
			if (exchangeCertificateRpc.CreateRequest)
			{
				CertificateRequestInfo certificateRequestInfo = new CertificateRequestInfo();
				if (list.Count > 0)
				{
					certificateRequestInfo.AlternativeDomainNames = list;
				}
				certificateRequestInfo.FriendlyName = exchangeCertificateRpc.CreateFriendlyName;
				certificateRequestInfo.IsExportable = exchangeCertificateRpc.CreateExportable;
				certificateRequestInfo.KeySize = exchangeCertificateRpc.CreateKeySize;
				certificateRequestInfo.SourceProvider = CertificateCreationOption.RSAProvider;
				certificateRequestInfo.Subject = new X500DistinguishedName(exchangeCertificateRpc.CreateSubjectName, X500DistinguishedNameFlags.UseUTF8Encoding);
				string text = null;
				string returnCertRequest = null;
				try
				{
					returnCertRequest = CertificateEnroller.GeneratePkcs10Request(certificateRequestInfo, out text);
				}
				catch (ArgumentException ex2)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, ex2.Message, ErrorCategory.InvalidArgument);
				}
				ExchangeCertificate returnCert = null;
				if (!string.IsNullOrEmpty(text))
				{
					returnCert = ExchangeCertificate.GetCertificateFromStore("REQUEST", text);
				}
				exchangeCertificateRpc.ReturnCert = returnCert;
				exchangeCertificateRpc.ReturnCertRequest = returnCertRequest;
				return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
			}
			byte[] result;
			try
			{
				X509Certificate2 cert = TlsCertificateInfo.CreateSelfSignCertificate(new X500DistinguishedName(exchangeCertificateRpc.CreateSubjectName, X500DistinguishedNameFlags.UseUTF8Encoding), list, validFor, exchangeCertificateRpc.CreateExportable ? CertificateCreationOption.Exportable : CertificateCreationOption.None, exchangeCertificateRpc.CreateKeySize, exchangeCertificateRpc.CreateFriendlyName, false, exchangeCertificateRpc.CreateSubjectKeyIdentifier);
				exchangeCertificateRpc.ReturnConfirmationList = ManageExchangeCertificate.EnableForServices(cert, exchangeCertificateRpc.CreateServices, exchangeCertificateRpc.RequireSsl, topologyConfigurationSession, server, exchangeCertificateRpc.ReturnTaskWarningList, exchangeCertificateRpc.CreateAllowConfirmation, false);
				List<ServiceData> installed = new List<ServiceData>();
				ExchangeCertificateServerHelper.GetInstalledRoles(topologyConfigurationSession, server, installed);
				ExchangeCertificate exchangeCertificate = new ExchangeCertificate(cert);
				ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate, server, installed);
				exchangeCertificateRpc.ReturnCert = exchangeCertificate;
				result = exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
			}
			catch (IISNotInstalledException)
			{
				result = ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.IISNotInstalled, ErrorCategory.InvalidArgument);
			}
			catch (InvalidOperationException ex3)
			{
				result = ExchangeCertificateRpc.SerializeError(rpcVersion, ex3.Message, ErrorCategory.InvalidArgument);
			}
			catch (LocalizedException ex4)
			{
				result = ExchangeCertificateRpc.SerializeError(rpcVersion, ex4.Message, ErrorCategory.NotSpecified);
			}
			catch (CryptographicException ex5)
			{
				result = ExchangeCertificateRpc.SerializeError(rpcVersion, ex5.Message, ErrorCategory.InvalidOperation);
			}
			return result;
		}

		public static byte[] GetCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob)
		{
			bool flag = false;
			List<ServiceData> installed = new List<ServiceData>();
			List<ExchangeCertificate> list = new List<ExchangeCertificate>();
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 536, "GetCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(topologyConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			ExchangeCertificateServerHelper.GetInstalledRoles(topologyConfigurationSession, server, installed);
			if (exchangeCertificateRpc.GetByCertificate != null)
			{
				try
				{
					X509Certificate2 cert = new X509Certificate2(exchangeCertificateRpc.GetByCertificate);
					ManageExchangeCertificate.EnsureValidExchangeCertificate(cert, true);
					ExchangeCertificate exchangeCertificate = new ExchangeCertificate(cert);
					ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate, server, installed);
					list.Add(exchangeCertificate);
					exchangeCertificateRpc.ReturnCertList = list;
					return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
				}
				catch (LocalizedException ex)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, ex.Message, ErrorCategory.NotSpecified);
				}
				catch (CryptographicException ex2)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, ex2.Message, ErrorCategory.NotSpecified);
				}
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			X509Store x509Store2 = new X509Store("REQUEST", StoreLocation.LocalMachine);
			try
			{
				x509Store2.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store2 = null;
			}
			try
			{
				if (exchangeCertificateRpc.GetByDomains != null && exchangeCertificateRpc.GetByDomains.Count > 0)
				{
					if (x509Store != null)
					{
						IEnumerable<string> names = ManageExchangeCertificate.DomainsToList(exchangeCertificateRpc.GetByDomains);
						CertificateSelectionOption options = CertificateSelectionOption.WildcardAllowed | CertificateSelectionOption.PreferedNonSelfSigned;
						ChainEngine engine = new ChainEngine();
						X509Certificate2 x509Certificate;
						IEnumerable<X509Certificate2> enumerable = TlsCertificateInfo.FindAll(x509Store, names, options, engine, out x509Certificate);
						if (string.IsNullOrEmpty(exchangeCertificateRpc.GetByThumbprint))
						{
							if (x509Certificate != null)
							{
								ExchangeCertificate exchangeCertificate2 = new ExchangeCertificate(x509Certificate);
								ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate2, server, installed);
								list.Add(exchangeCertificate2);
							}
							using (IEnumerator<X509Certificate2> enumerator = enumerable.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									X509Certificate2 x509Certificate2 = enumerator.Current;
									if (x509Certificate != x509Certificate2)
									{
										ExchangeCertificate exchangeCertificate3 = new ExchangeCertificate(x509Certificate2);
										ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate3, server, installed);
										list.Add(exchangeCertificate3);
									}
								}
								goto IL_410;
							}
						}
						foreach (X509Certificate2 x509Certificate3 in enumerable)
						{
							if (string.Equals(x509Certificate3.Thumbprint, exchangeCertificateRpc.GetByThumbprint, StringComparison.OrdinalIgnoreCase))
							{
								ExchangeCertificate exchangeCertificate4 = new ExchangeCertificate(x509Certificate3);
								ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate4, server, installed);
								list.Add(exchangeCertificate4);
								break;
							}
						}
						if (list.Count == 0)
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateNotFound(exchangeCertificateRpc.GetByThumbprint), ErrorCategory.NotSpecified);
						}
					}
				}
				else if (exchangeCertificateRpc.GetByThumbprint != null)
				{
					if (x509Store != null)
					{
						X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.GetByThumbprint, false);
						X509Certificate2Enumerator enumerator3 = x509Certificate2Collection.GetEnumerator();
						if (enumerator3.MoveNext())
						{
							X509Certificate2 cert2 = enumerator3.Current;
							ManageExchangeCertificate.EnsureValidExchangeCertificate(cert2, true);
							ExchangeCertificate exchangeCertificate5 = new ExchangeCertificate(cert2);
							ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate5, server, installed);
							list.Add(exchangeCertificate5);
						}
					}
					if (x509Store2 != null && list.Count == 0)
					{
						X509Certificate2Collection x509Certificate2Collection2 = x509Store2.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.GetByThumbprint, false);
						foreach (X509Certificate2 cert3 in x509Certificate2Collection2)
						{
							ManageExchangeCertificate.EnsureValidExchangeCertificate(cert3, true);
							string value = CertificateEnroller.ReadPkcs10Request(cert3);
							if (!string.IsNullOrEmpty(value))
							{
								ExchangeCertificate item = new ExchangeCertificate(cert3);
								list.Add(item);
							}
						}
					}
					if (list.Count == 0)
					{
						return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateNotFound(exchangeCertificateRpc.GetByThumbprint), ErrorCategory.NotSpecified);
					}
				}
				else
				{
					if (x509Store != null)
					{
						foreach (X509Certificate2 cert4 in x509Store.Certificates)
						{
							ExchangeCertificate exchangeCertificate6 = new ExchangeCertificate(cert4);
							if (ManageExchangeCertificate.ValidateExchangeCertificate(cert4, true) == ExchangeCertificateValidity.Valid)
							{
								ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate6, server, installed);
								list.Add(exchangeCertificate6);
							}
						}
					}
					if (x509Store2 != null)
					{
						foreach (X509Certificate2 cert5 in x509Store2.Certificates)
						{
							string value2 = CertificateEnroller.ReadPkcs10Request(cert5);
							if (!string.IsNullOrEmpty(value2))
							{
								ExchangeCertificate item2 = new ExchangeCertificate(cert5);
								list.Add(item2);
							}
						}
					}
					list.Sort(new Comparison<ExchangeCertificate>(ManageExchangeCertificate.CompareByNotBefore));
				}
				IL_410:;
			}
			catch (LocalizedException ex3)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex3.Message, ErrorCategory.NotSpecified);
			}
			catch (CryptographicException ex4)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex4.Message, ErrorCategory.NotSpecified);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
				if (x509Store2 != null)
				{
					x509Store2.Close();
				}
			}
			exchangeCertificateRpc.ReturnCertList = list;
			return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
		}

		public static byte[] RemoveCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob)
		{
			bool flag = false;
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 792, "RemoveCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(topologyConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			X509Store x509Store2 = new X509Store("REQUEST", StoreLocation.LocalMachine);
			try
			{
				x509Store2.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store2 = null;
			}
			try
			{
				X509Certificate2 x509Certificate = null;
				bool flag2 = false;
				if (exchangeCertificateRpc.RemoveByThumbprint != null)
				{
					if (x509Store != null)
					{
						X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.RemoveByThumbprint, false);
						if (x509Certificate2Collection.Count > 0)
						{
							ManageExchangeCertificate.EnsureValidExchangeCertificate(x509Certificate2Collection[0], true);
							x509Certificate = x509Certificate2Collection[0];
						}
					}
					if (x509Store2 != null && x509Certificate == null)
					{
						X509Certificate2Collection x509Certificate2Collection2 = x509Store2.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.RemoveByThumbprint, false);
						if (x509Certificate2Collection2.Count > 0)
						{
							ManageExchangeCertificate.EnsureValidExchangeCertificate(x509Certificate2Collection2[0], true);
							x509Certificate = x509Certificate2Collection2[0];
							flag2 = true;
						}
					}
				}
				if (x509Certificate == null)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateNotFound(exchangeCertificateRpc.RemoveByThumbprint), ErrorCategory.ObjectNotFound);
				}
				if (!flag2)
				{
					if (server.IsHubTransportServer || server.IsEdgeServer)
					{
						ExchangeCertificate internalTransportCertificate = ExchangeCertificate.GetInternalTransportCertificate(server);
						if (internalTransportCertificate != null && exchangeCertificateRpc.RemoveByThumbprint.Equals(internalTransportCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CannotRemoveInternalTransportCertificate, ErrorCategory.InvalidArgument);
						}
					}
					if (server.IsUnifiedMessagingServer)
					{
						UMServer umserver = new UMServer(server);
						if (!string.IsNullOrEmpty(umserver.UMCertificateThumbprint) && exchangeCertificateRpc.RemoveByThumbprint.Equals(umserver.UMCertificateThumbprint, StringComparison.OrdinalIgnoreCase))
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CannotRemoveUMCertificate, ErrorCategory.InvalidArgument);
						}
					}
					if (server.IsCafeServer)
					{
						SIPFEServerConfiguration sipfeserverConfiguration = SIPFEServerConfiguration.Find(server, topologyConfigurationSession);
						if (sipfeserverConfiguration != null && !string.IsNullOrEmpty(sipfeserverConfiguration.UMCertificateThumbprint) && exchangeCertificateRpc.RemoveByThumbprint.Equals(sipfeserverConfiguration.UMCertificateThumbprint, StringComparison.OrdinalIgnoreCase))
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CannotRemoveUMCRCertificate, ErrorCategory.InvalidArgument);
						}
					}
					if (server.IsClientAccessServer || server.IsHubTransportServer || server.IsMailboxServer)
					{
						foreach (CertificateRecord certificateRecord in FederationCertificate.FederationCertificates(topologyConfigurationSession))
						{
							if (exchangeCertificateRpc.RemoveByThumbprint.Equals(certificateRecord.Thumbprint, StringComparison.OrdinalIgnoreCase))
							{
								return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RemoveCertificateFederation, ErrorCategory.InvalidArgument);
							}
						}
					}
					string text = ExchangeCertificateServerHelper.CheckSendConnectorCerts(topologyConfigurationSession, x509Certificate);
					if (!string.IsNullOrEmpty(text))
					{
						return ExchangeCertificateRpc.SerializeError(rpcVersion, string.Format(Strings.CannotRemoveSendConnectorCertificate, text), ErrorCategory.InvalidArgument);
					}
					x509Store.Remove(x509Certificate);
				}
				else
				{
					x509Store2.Remove(x509Certificate);
				}
			}
			catch (LocalizedException ex)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex.Message, ErrorCategory.NotSpecified);
			}
			catch (CryptographicException ex2)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex2.Message, ErrorCategory.NotSpecified);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
				if (x509Store2 != null)
				{
					x509Store2.Close();
				}
			}
			return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
		}

		public static byte[] ExportCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob, SecureString password)
		{
			bool flag = false;
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			Server server = null;
			ITopologyConfigurationSession systemConfigDataSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 983, "ExportCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(systemConfigDataSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly | OpenFlags.IncludeArchived);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			X509Store x509Store2 = new X509Store("REQUEST", StoreLocation.LocalMachine);
			try
			{
				x509Store2.Open(OpenFlags.OpenExistingOnly | OpenFlags.IncludeArchived);
			}
			catch (CryptographicException)
			{
				x509Store2 = null;
			}
			try
			{
				X509Certificate2 x509Certificate = null;
				bool flag2 = false;
				if (x509Store != null)
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.ExportByThumbprint, false);
					if (x509Certificate2Collection.Count > 0)
					{
						x509Certificate = x509Certificate2Collection[0];
						if (!x509Certificate.HasPrivateKey)
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ExportCertificateAs12WithoutKey, ErrorCategory.InvalidArgument);
						}
						if (password == null || password.Length == 0)
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ExportWithPrivateKeyRequiresPassword, ErrorCategory.InvalidArgument);
						}
						if (!TlsCertificateInfo.IsCertificateExportable(x509Certificate))
						{
							return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ExportCertificateAs12KeyNotExportable, ErrorCategory.InvalidArgument);
						}
						flag2 = false;
					}
				}
				if (x509Store2 != null && x509Certificate == null)
				{
					X509Certificate2Collection x509Certificate2Collection2 = x509Store2.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.ExportByThumbprint, false);
					if (x509Certificate2Collection2.Count > 0)
					{
						x509Certificate = x509Certificate2Collection2[0];
						flag2 = true;
					}
				}
				if (x509Certificate == null)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateNotFound(exchangeCertificateRpc.ExportByThumbprint), ErrorCategory.ObjectNotFound);
				}
				if (!flag2)
				{
					byte[] array = x509Certificate.Export(X509ContentType.Pfx, password);
					exchangeCertificateRpc.ReturnExportFileData = array;
					exchangeCertificateRpc.ReturnExportBase64 = Convert.ToBase64String(array);
					exchangeCertificateRpc.ReturnExportBinary = true;
					exchangeCertificateRpc.ReturnExportPKCS10 = false;
				}
				else
				{
					string text = CertificateEnroller.ReadPkcs10Request(x509Certificate);
					if (string.IsNullOrEmpty(text))
					{
						return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateDoesNotHavePKCS10, ErrorCategory.ObjectNotFound);
					}
					if (exchangeCertificateRpc.ExportBinary)
					{
						exchangeCertificateRpc.ReturnExportFileData = Convert.FromBase64String(text);
						exchangeCertificateRpc.ReturnExportBinary = true;
					}
					else
					{
						exchangeCertificateRpc.ReturnExportFileData = Encoding.ASCII.GetBytes(ManageExchangeCertificate.WrapCertificateRequestWithPemTags(text));
						exchangeCertificateRpc.ReturnExportBinary = false;
					}
					exchangeCertificateRpc.ReturnExportBase64 = ManageExchangeCertificate.WrapCertificateRequestWithPemTags(text);
					exchangeCertificateRpc.ReturnExportPKCS10 = true;
				}
			}
			catch (LocalizedException ex)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex.Message, ErrorCategory.NotSpecified);
			}
			catch (CryptographicException ex2)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex2.Message, ErrorCategory.NotSpecified);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
				if (x509Store2 != null)
				{
					x509Store2.Close();
				}
			}
			return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
		}

		public static byte[] ImportCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob, SecureString password)
		{
			bool flag = false;
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			if (string.IsNullOrEmpty(exchangeCertificateRpc.ImportCert))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateDataInvalid, ErrorCategory.ReadError);
			}
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1161, "ImportCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(topologyConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			List<ServiceData> installed = new List<ServiceData>();
			ExchangeCertificateServerHelper.GetInstalledRoles(topologyConfigurationSession, server, installed);
			try
			{
				byte[] rawData = null;
				string findValue;
				bool flag2;
				if (CertificateEnroller.TryAcceptPkcs7(exchangeCertificateRpc.ImportCert, out findValue, out flag2))
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false);
					if (x509Certificate2Collection.Count > 0)
					{
						if (!string.IsNullOrEmpty(exchangeCertificateRpc.ImportDescription))
						{
							x509Certificate2Collection[0].FriendlyName = exchangeCertificateRpc.ImportDescription;
						}
						ExchangeCertificate exchangeCertificate = new ExchangeCertificate(x509Certificate2Collection[0]);
						ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate, server, installed);
						exchangeCertificateRpc.ReturnCert = exchangeCertificate;
					}
					return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
				}
				if (flag2)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateUntrustedRoot, ErrorCategory.ReadError);
				}
				try
				{
					rawData = Convert.FromBase64String(exchangeCertificateRpc.ImportCert);
				}
				catch (FormatException)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateBase64DataInvalid, ErrorCategory.ReadError);
				}
				X509Certificate2 x509Certificate = null;
				try
				{
					X509KeyStorageFlags x509KeyStorageFlags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet;
					bool flag3 = password == null || password.Length == 0;
					X509Certificate2Collection x509Certificate2Collection2 = new X509Certificate2Collection();
					if (exchangeCertificateRpc.ImportExportable)
					{
						x509KeyStorageFlags |= X509KeyStorageFlags.Exportable;
					}
					x509Certificate2Collection2.Import(rawData, flag3 ? null : password.AsUnsecureString(), x509KeyStorageFlags);
					x509Certificate = ManageExchangeCertificate.FindImportedCertificate(x509Certificate2Collection2);
				}
				catch (CryptographicException)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateDataInvalid, ErrorCategory.ReadError);
				}
				if (x509Certificate == null)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateDataInvalid, ErrorCategory.ReadError);
				}
				if (!string.IsNullOrEmpty(exchangeCertificateRpc.ImportDescription))
				{
					x509Certificate.FriendlyName = exchangeCertificateRpc.ImportDescription;
				}
				X509Certificate2Collection x509Certificate2Collection3 = x509Store.Certificates.Find(X509FindType.FindByThumbprint, x509Certificate.Thumbprint, false);
				if (x509Certificate2Collection3.Count > 0)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.ImportCertificateAlreadyExists(x509Certificate.Thumbprint), ErrorCategory.WriteError);
				}
				x509Store.Add(x509Certificate);
				ExchangeCertificate exchangeCertificate2 = new ExchangeCertificate(x509Certificate);
				ExchangeCertificateServerHelper.UpdateServices(exchangeCertificate2, server, installed);
				exchangeCertificateRpc.ReturnCert = exchangeCertificate2;
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
		}

		public static byte[] EnableCertificate(ExchangeCertificateRpcVersion rpcVersion, byte[] inputBlob)
		{
			bool flag = false;
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(rpcVersion, inputBlob, null);
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1315, "EnableCertificate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\ExchangeCertificate\\Program\\ExchangeCertificateServer.cs");
			try
			{
				server = ManageExchangeCertificate.FindLocalServer(topologyConfigurationSession);
			}
			catch (LocalServerNotFoundException)
			{
				flag = true;
			}
			if (flag || !ManageExchangeCertificate.IsServerRoleSupported(server))
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.RoleDoesNotSupportExchangeCertificateTasksException, ErrorCategory.InvalidOperation);
			}
			if ((exchangeCertificateRpc.EnableServices & AllowedServices.SMTP) == AllowedServices.SMTP && !server.IsHubTransportServer && !server.IsEdgeServer && !server.IsCafeServer)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.EnableCertificateServiceNotInstalled, ErrorCategory.InvalidArgument);
			}
			if ((exchangeCertificateRpc.EnableServices & (AllowedServices.IMAP | AllowedServices.POP)) != AllowedServices.None && !server.IsClientAccessServer && !server.IsCafeServer)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.EnableCertificateServiceNotInstalled, ErrorCategory.InvalidArgument);
			}
			if ((exchangeCertificateRpc.EnableServices & AllowedServices.Federation) == AllowedServices.Federation)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.EnableCertificateFederation, ErrorCategory.InvalidArgument);
			}
			if ((exchangeCertificateRpc.EnableServices & AllowedServices.UM) == AllowedServices.UM && !server.IsUnifiedMessagingServer)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.EnableCertificateServiceNotInstalled, ErrorCategory.InvalidArgument);
			}
			if ((exchangeCertificateRpc.EnableServices & AllowedServices.UMCallRouter) == AllowedServices.UMCallRouter && !server.IsCafeServer)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.EnableCertificateServiceNotInstalled, ErrorCategory.InvalidArgument);
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				x509Store = null;
			}
			try
			{
				X509Certificate2 x509Certificate = null;
				if (x509Store != null)
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, exchangeCertificateRpc.EnableByThumbprint, false);
					if (x509Certificate2Collection.Count > 0)
					{
						if (!exchangeCertificateRpc.EnableNetworkService)
						{
							ManageExchangeCertificate.EnsureValidExchangeCertificate(x509Certificate2Collection[0], true);
						}
						x509Certificate = x509Certificate2Collection[0];
					}
				}
				if (x509Certificate == null)
				{
					return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.CertificateNotFound(exchangeCertificateRpc.EnableByThumbprint), ErrorCategory.ObjectNotFound);
				}
				exchangeCertificateRpc.ReturnTaskWarningList = new List<LocalizedString>();
				if (exchangeCertificateRpc.EnableUpdateAD)
				{
					ManageExchangeCertificate.UpdateActiveDirectory(x509Certificate, topologyConfigurationSession, server, exchangeCertificateRpc.ReturnTaskWarningList, false);
				}
				else
				{
					exchangeCertificateRpc.ReturnConfirmationList = ManageExchangeCertificate.EnableForServices(x509Certificate, exchangeCertificateRpc.EnableServices, exchangeCertificateRpc.RequireSsl, topologyConfigurationSession, server, exchangeCertificateRpc.ReturnTaskWarningList, exchangeCertificateRpc.EnableAllowConfirmation, exchangeCertificateRpc.EnableNetworkService);
				}
			}
			catch (IISNotInstalledException)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, Strings.IISNotInstalled, ErrorCategory.InvalidArgument);
			}
			catch (InvalidOperationException ex)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex.Message, ErrorCategory.ObjectNotFound);
			}
			catch (LocalizedException ex2)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex2.Message, ErrorCategory.NotSpecified);
			}
			catch (CryptographicException ex3)
			{
				return ExchangeCertificateRpc.SerializeError(rpcVersion, ex3.Message, ErrorCategory.NotSpecified);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return exchangeCertificateRpc.SerializeOutputParameters(rpcVersion);
		}

		private static List<string> EnsureCreateDefaults(Server server, IConfigurationSession configurationSession, ExchangeCertificateRpc rpcParams)
		{
			List<string> list = new List<string>();
			X509Certificate2 x509Certificate = null;
			if (rpcParams.CreateCloneCert != null)
			{
				x509Certificate = new X509Certificate2(rpcParams.CreateCloneCert);
				foreach (string name in TlsCertificateInfo.GetFQDNs(x509Certificate))
				{
					ManageExchangeCertificate.AddUniqueDomain(list, name);
				}
				rpcParams.CreateFriendlyName = x509Certificate.FriendlyName;
			}
			if (!string.IsNullOrEmpty(rpcParams.CreateFriendlyName) && rpcParams.CreateFriendlyName.Length > 63)
			{
				throw new ArgumentException(Strings.FriendlyNameTooLong);
			}
			if (rpcParams.CreateDomains != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in rpcParams.CreateDomains)
				{
					ManageExchangeCertificate.AddUniqueDomain(list, smtpDomainWithSubdomains.ToString());
				}
			}
			if (rpcParams.CreateIncAccepted)
			{
				foreach (AcceptedDomain acceptedDomain in configurationSession.FindAllPaged<AcceptedDomain>())
				{
					if (acceptedDomain.DomainName != null)
					{
						ManageExchangeCertificate.AddUniqueDomainIfValid(list, acceptedDomain.DomainName.ToString());
					}
				}
			}
			if (rpcParams.CreateIncAutoDisc)
			{
				if (server.IsClientAccessServer)
				{
					using (IEnumerator<AcceptedDomain> enumerator4 = configurationSession.FindAllPaged<AcceptedDomain>().GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							AcceptedDomain acceptedDomain2 = enumerator4.Current;
							if (acceptedDomain2.DomainName != null && acceptedDomain2.DomainType == AcceptedDomainType.Authoritative)
							{
								ManageExchangeCertificate.AddUniqueDomainIfValid(list, "autodiscover." + acceptedDomain2.DomainName.Domain);
							}
						}
						goto IL_19C;
					}
				}
				rpcParams.ReturnTaskWarningList.Add(Strings.IncludeAutoDiscoverOnlyApplicableToCAS);
			}
			IL_19C:
			if (rpcParams.CreateDomains == null && list.Count == 0)
			{
				if (string.IsNullOrEmpty(rpcParams.CreateSubjectName))
				{
					ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.DnsHostName);
					ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.DnsPhysicalHostName);
					ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.DnsFullyQualifiedDomainName);
					ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.DnsPhysicalFullyQualifiedDomainName);
				}
				else if (!ManageExchangeCertificate.CheckCnIsFQDN(rpcParams.CreateSubjectName))
				{
					throw new ArgumentException(Strings.SubjectCnNotValidFQDN);
				}
			}
			else
			{
				foreach (string text in list)
				{
					if (!ManageExchangeCertificate.IsDomainValidForCertificate(text))
					{
						throw new ArgumentException(Strings.DomainNameIsNotValidForCertificate(text.ToString()));
					}
				}
			}
			if (rpcParams.CreateIncFqdn)
			{
				ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.DnsFullyQualifiedDomainName);
			}
			if (rpcParams.CreateIncNetBios)
			{
				ManageExchangeCertificate.AddUniqueDomainIfValid(list, ComputerInformation.NetbiosName);
			}
			if (rpcParams.CreateKeySize == 0)
			{
				if (x509Certificate != null)
				{
					rpcParams.CreateKeySize = x509Certificate.PublicKey.Key.KeySize;
				}
				else
				{
					rpcParams.CreateKeySize = 2048;
				}
			}
			if (!TlsCertificateInfo.IsValidKeySize(rpcParams.CreateKeySize))
			{
				throw new ArgumentException(Strings.InvalidKeySize(rpcParams.CreateKeySize));
			}
			if (string.IsNullOrEmpty(rpcParams.CreateSubjectName))
			{
				if (x509Certificate != null)
				{
					rpcParams.CreateSubjectName = x509Certificate.SubjectName.Name;
				}
				else
				{
					X500DistinguishedName defaultSubjectName = TlsCertificateInfo.GetDefaultSubjectName(list);
					if (defaultSubjectName != null)
					{
						rpcParams.CreateSubjectName = defaultSubjectName.Name;
					}
				}
			}
			return list;
		}

		private static void LoadIisInformation(Server server, IConfigurationSession session, List<ServiceData> installed)
		{
			IEnumerable<string> enumerable = null;
			try
			{
				enumerable = IisUtility.FindAllWebSitePaths(ComputerInformation.DnsPhysicalHostName);
			}
			catch (IISNotInstalledException)
			{
				return;
			}
			Dictionary<string, ServiceData> dictionary = new Dictionary<string, ServiceData>(StringComparer.OrdinalIgnoreCase);
			string key = null;
			foreach (string text in enumerable)
			{
				if (text.EndsWith("/1", StringComparison.OrdinalIgnoreCase))
				{
					key = text;
				}
				string webSiteSslCertificate = IisUtility.GetWebSiteSslCertificate(text);
				if (!string.IsNullOrEmpty(webSiteSslCertificate))
				{
					ServiceData serviceData = new ServiceData(string.Empty, webSiteSslCertificate, AllowedServices.IIS)
					{
						IisServices = 
						{
							new IisService(text)
						}
					};
					installed.Add(serviceData);
					dictionary.Add(text, serviceData);
				}
			}
			ADPagedReader<ADVirtualDirectory> adpagedReader = session.FindPaged<ADVirtualDirectory>(server.Id.GetChildId("Protocols").GetChildId("HTTP"), QueryScope.SubTree, null, null, 0);
			foreach (ADVirtualDirectory advirtualDirectory in adpagedReader)
			{
				string text2 = advirtualDirectory[ExchangeVirtualDirectorySchema.MetabasePath] as string;
				if (!string.IsNullOrEmpty(text2))
				{
					string text3 = null;
					string key2 = null;
					string text4 = null;
					IisUtility.ParseApplicationRootPath(text2, ref text3, ref key2, ref text4);
					if (dictionary.ContainsKey(key2))
					{
						dictionary[key2].IisServices.Add(new IisService(advirtualDirectory));
					}
				}
				else if (dictionary.ContainsKey(key))
				{
					dictionary[key].IisServices.Add(new IisService(advirtualDirectory));
				}
			}
		}

		private static void GetInstalledRoles(ITopologyConfigurationSession configurationSession, Server server, List<ServiceData> installed)
		{
			if (server.IsUnifiedMessagingServer)
			{
				UMServer umserver = new UMServer(server);
				installed.Add(new ServiceData(string.Empty, umserver.UMCertificateThumbprint, AllowedServices.UM));
			}
			if (server.IsCafeServer)
			{
				SIPFEServerConfiguration sipfeserverConfiguration = SIPFEServerConfiguration.Find(server, configurationSession);
				if (sipfeserverConfiguration != null)
				{
					installed.Add(new ServiceData(string.Empty, sipfeserverConfiguration.UMCertificateThumbprint, AllowedServices.UMCallRouter));
				}
			}
			if (server.IsClientAccessServer || server.IsCafeServer)
			{
				string text = ManageExchangeCertificate.GetPopFQDN(configurationSession);
				if (!string.IsNullOrEmpty(text))
				{
					installed.Add(new ServiceData(text, AllowedServices.POP));
				}
				text = ManageExchangeCertificate.GetIMapFQDN(configurationSession);
				if (!string.IsNullOrEmpty(text))
				{
					installed.Add(new ServiceData(text, AllowedServices.IMAP));
				}
				ExchangeCertificateServerHelper.LoadIisInformation(server, configurationSession, installed);
			}
			if (server.IsClientAccessServer || server.IsHubTransportServer || server.IsMailboxServer)
			{
				foreach (CertificateRecord certificateRecord in FederationCertificate.FederationCertificates(configurationSession))
				{
					installed.Add(new ServiceData(string.Empty, certificateRecord.Thumbprint, AllowedServices.Federation));
				}
			}
		}

		private static void UpdateServices(ExchangeCertificate cert, Server server, List<ServiceData> installed)
		{
			if ((server.IsHubTransportServer || server.IsEdgeServer || server.IsCafeServer) && ManageExchangeCertificate.IsCertEnabledForNetworkService(cert))
			{
				cert.Services |= AllowedServices.SMTP;
			}
			foreach (ServiceData serviceData in installed)
			{
				if (!string.IsNullOrEmpty(serviceData.Thumbprint))
				{
					if (!string.Equals(cert.Thumbprint, serviceData.Thumbprint, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					cert.Services |= serviceData.Flag;
					using (List<IisService>.Enumerator enumerator2 = serviceData.IisServices.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							IisService item = enumerator2.Current;
							cert.IisServices.Add(new IisService(item, cert.CertificateDomains));
						}
						continue;
					}
				}
				foreach (string a in TlsCertificateInfo.GetFQDNs(cert))
				{
					if (string.Equals(a, serviceData.Domain, StringComparison.OrdinalIgnoreCase))
					{
						cert.Services |= serviceData.Flag;
					}
				}
			}
		}

		private static string CheckSendConnectorCerts(ITopologyConfigurationSession configurationSession, X509Certificate2 certificate)
		{
			ADObjectId childId = configurationSession.GetOrgContainerId().GetChildId("Administrative Groups");
			SmtpSendConnectorConfig[] array = configurationSession.Find<SmtpSendConnectorConfig>(childId, QueryScope.SubTree, null, null, int.MaxValue);
			StringBuilder stringBuilder = new StringBuilder();
			if (array != null)
			{
				string text = ", ";
				foreach (SmtpSendConnectorConfig smtpSendConnectorConfig in array)
				{
					if (ExchangeCertificateServerHelper.IsSendConnectorUsingSMTPCertificate(smtpSendConnectorConfig, certificate))
					{
						stringBuilder.Append(smtpSendConnectorConfig.Id.Name);
						stringBuilder.Append(text);
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - text.Length, text.Length);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsSendConnectorUsingSMTPCertificate(SmtpSendConnectorConfig smtpSendConnectorConfig, X509Certificate2 certificate)
		{
			bool result = false;
			SmtpX509Identifier tlsCertificateName = smtpSendConnectorConfig.TlsCertificateName;
			if (tlsCertificateName != null)
			{
				result = (certificate.Issuer.Equals(tlsCertificateName.CertificateIssuer) && certificate.Subject.Equals(tlsCertificateName.CertificateSubject));
			}
			return result;
		}
	}
}

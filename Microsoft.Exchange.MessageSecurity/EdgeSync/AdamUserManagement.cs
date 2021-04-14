using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.EdgeSync;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Win32;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	public static class AdamUserManagement
	{
		internal static string GetPasswordHash(string password)
		{
			string result;
			using (HashAlgorithm hashAlgorithm = new SHA256Cng())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(password);
				hashAlgorithm.TransformFinalBlock(bytes, 0, bytes.Length);
				result = Convert.ToBase64String(hashAlgorithm.Hash);
			}
			return result;
		}

		internal static void UpdateEdgeSyncCredentialsOnEdge(ITopologyConfigurationSession configurationSession, Server localServer)
		{
			X509Certificate2 edgeCert = null;
			if (!ADNotificationAdapter.TryReadConfigurationPaged<Server>(() => configurationSession.FindAllServersWithVersionNumber(Server.E2007MinVersion), delegate(Server server)
			{
				if (server.IsHubTransportServer && server.EdgeSyncCredentials != null && server.EdgeSyncCredentials.Count != 0)
				{
					foreach (byte[] data in server.EdgeSyncCredentials)
					{
						EdgeSyncCredential edgeSyncCredential = EdgeSyncCredential.DeserializeEdgeSyncCredential(data);
						if (!edgeSyncCredential.IsBootStrapAccount && edgeSyncCredential.EdgeServerFQDN.Equals(localServer.Fqdn, StringComparison.InvariantCultureIgnoreCase) && edgeSyncCredential.EdgeEncryptedESRAPassword != null)
						{
							if (edgeCert == null)
							{
								edgeCert = AdamUserManagement.GetInternalTransportCertificate(localServer);
								if (edgeCert == null)
								{
									Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_TlsCertificateNotFound, null, new object[0]);
									break;
								}
							}
							string password = null;
							try
							{
								password = AdamUserManagement.DecryptPassword(edgeCert, edgeSyncCredential.EdgeEncryptedESRAPassword);
							}
							catch (CryptographicException ex)
							{
								Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_CouldNotDecryptPassword, null, new object[]
								{
									edgeSyncCredential.ESRAUsername,
									edgeCert.Thumbprint,
									ex.Message
								});
								continue;
							}
							string passwordHash = AdamUserManagement.GetPasswordHash(password);
							if (AdamUserManagement.credentialCache.ContainsKey(edgeSyncCredential.ESRAUsername))
							{
								if (passwordHash.Equals(AdamUserManagement.credentialCache[edgeSyncCredential.ESRAUsername], StringComparison.InvariantCulture))
								{
									continue;
								}
							}
							try
							{
								AdamUserManagement.CreateOrUpdateADAMPrincipal(edgeSyncCredential.ESRAUsername, password);
							}
							catch (DirectoryException ex2)
							{
								Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_CredentialsRenewalFailure, edgeSyncCredential.ESRAUsername, new object[]
								{
									edgeSyncCredential.ESRAUsername,
									ex2.Message
								});
								continue;
							}
							AdamUserManagement.credentialCache[edgeSyncCredential.ESRAUsername] = passwordHash;
							Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_CredentialsRenewalSuccess, null, new object[]
							{
								edgeSyncCredential.ESRAUsername,
								passwordHash,
								new DateTime(edgeSyncCredential.EffectiveDate).ToLocalTime()
							});
						}
					}
				}
			}))
			{
				Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_ReadServerConfigFailed, null, null);
			}
		}

		internal static byte[] EncryptPassword(X509Certificate2 cert, string password)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(password);
			byte[] result;
			try
			{
				using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
				{
					RSA rsa = (RSA)cert.PublicKey.Key;
					rsacryptoServiceProvider.ImportParameters(rsa.ExportParameters(false));
					result = rsacryptoServiceProvider.Encrypt(bytes, false);
				}
			}
			catch (CryptographicException ex)
			{
				AdamUserManagement.TraceError("Encryption failure: {0}", new object[]
				{
					ex
				});
				result = null;
			}
			finally
			{
				Array.Clear(bytes, 0, bytes.Length);
			}
			return result;
		}

		internal static string GeneratePassword()
		{
			byte[] randomBytes = AdamUserManagement.GetRandomBytes(16);
			int num = 0;
			char[] array = new char[8];
			for (int i = 0; i < 3; i++)
			{
				array[i] = (char)(Convert.ToInt32(randomBytes[num++]) % 26 + 97);
			}
			for (int j = 3; j < 6; j++)
			{
				array[j] = (char)(Convert.ToInt32(randomBytes[num++]) % 26 + 65);
			}
			array[6] = "!@#$%^&*()<>?"[Convert.ToInt32(randomBytes[num++]) % "!@#$%^&*()<>?".Length];
			array[7] = (char)(Convert.ToInt32(randomBytes[num++]) % 10 + 48);
			for (int k = 0; k < array.Length; k++)
			{
				int num2 = k + Convert.ToInt32(randomBytes[num++]) % (array.Length - k);
				char c = array[k];
				array[k] = array[num2];
				array[num2] = c;
			}
			string value = Convert.ToBase64String(randomBytes, 8, 8);
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Append(array);
			return stringBuilder.ToString();
		}

		internal static void RemoveSubscriptionCredentialsOnAllBHs(string edgeFqdn)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 294, "RemoveSubscriptionCredentialsOnAllBHs", "f:\\15.00.1497\\sources\\dev\\MessageSecurity\\src\\Common\\EdgeSync\\AdamUserManagement.cs");
			ADPagedReader<Server> adpagedReader = topologyConfigurationSession.FindAllServersWithVersionNumber(Server.E2007MinVersion);
			foreach (Server server in adpagedReader)
			{
				if (server.IsHubTransportServer && server.EdgeSyncCredentials.Count > 0)
				{
					List<EdgeSyncCredential> list = new List<EdgeSyncCredential>();
					bool flag = false;
					foreach (byte[] data in server.EdgeSyncCredentials)
					{
						EdgeSyncCredential edgeSyncCredential = EdgeSyncCredential.DeserializeEdgeSyncCredential(data);
						if (edgeSyncCredential.EdgeServerFQDN.Equals(edgeFqdn))
						{
							flag = true;
						}
						else
						{
							list.Add(edgeSyncCredential);
						}
					}
					if (flag)
					{
						MultiValuedProperty<byte[]> multiValuedProperty = new MultiValuedProperty<byte[]>();
						foreach (EdgeSyncCredential credential in list)
						{
							multiValuedProperty.Add(EdgeSyncCredential.SerializeEdgeSyncCredential(credential));
						}
						server.EdgeSyncCredentials = multiValuedProperty;
						topologyConfigurationSession.Save(server);
					}
				}
			}
		}

		internal static void RemoveAllADAMPrincipals()
		{
			using (LdapConnection ldapConnection = new LdapConnection(string.Format(CultureInfo.InvariantCulture, "localhost:{0}", new object[]
			{
				AdamUserManagement.GetAdamSslPort()
			})))
			{
				ldapConnection.AuthType = AuthType.Negotiate;
				ldapConnection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(AdamUserManagement.VerifyServerCertificate);
				ldapConnection.SessionOptions.SecureSocketLayer = true;
				ldapConnection.Bind();
				ADObjectId configContainer = AdamUserManagement.GetConfigContainer(ldapConnection);
				SearchResponse searchResponse = (SearchResponse)ldapConnection.SendRequest(new SearchRequest(configContainer.GetDescendantId(AdamUserManagement.ServicesContainer).DistinguishedName, "(objectClass=user)", SearchScope.OneLevel, null));
				if (searchResponse.Entries.Count != 0)
				{
					for (int i = 0; i < searchResponse.Entries.Count; i++)
					{
						SearchResultEntry searchResultEntry = searchResponse.Entries[i];
						if (searchResultEntry.DistinguishedName.Contains("ESRA"))
						{
							DeleteResponse deleteResponse = (DeleteResponse)ldapConnection.SendRequest(new DeleteRequest(searchResultEntry.DistinguishedName));
							if (deleteResponse.ResultCode == ResultCode.Success)
							{
								ExTraceGlobals.SubscriptionTracer.TraceDebug<string>(0L, "Successfully deleted the subscription account {0}", searchResultEntry.DistinguishedName);
							}
							else
							{
								ExTraceGlobals.SubscriptionTracer.TraceDebug<string, ResultCode>(0L, "Deleting subscription account {0} failed with {1}", searchResultEntry.DistinguishedName, deleteResponse.ResultCode);
							}
						}
					}
				}
			}
		}

		internal static int ReadSslPortFromRegistry()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange");
			int num = 50636;
			if (registryKey != null)
			{
				try
				{
					num = (int)registryKey.GetValue("SslPort", num);
				}
				finally
				{
					registryKey.Close();
				}
			}
			return num;
		}

		internal static void VerifyADAMPrincipal(string user, string password)
		{
			try
			{
				using (LdapConnection ldapConnection = new LdapConnection(string.Format(CultureInfo.InvariantCulture, "localhost:{0}", new object[]
				{
					AdamUserManagement.GetAdamSslPort()
				})))
				{
					ldapConnection.AuthType = AuthType.Basic;
					ldapConnection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(AdamUserManagement.VerifyServerCertificate);
					ldapConnection.SessionOptions.SecureSocketLayer = true;
					ldapConnection.Bind(new NetworkCredential(user, password));
				}
			}
			catch (Exception ex)
			{
				AdamUserManagement.TraceError("VerifyADAMPrincipal ADAM account {0} failed with {1}", new object[]
				{
					user,
					ex.Message
				});
				throw;
			}
		}

		internal static string CreateOrUpdateADAMPrincipal(string user, string password)
		{
			return AdamUserManagement.CreateOrUpdateADAMPrincipal(user, password, false, TimeSpan.MaxValue);
		}

		internal static string CreateOrUpdateADAMPrincipal(string user, string password, bool bootStrapAccount, TimeSpan expiry)
		{
			string result;
			using (LdapConnection ldapConnection = new LdapConnection(string.Format(CultureInfo.InvariantCulture, "localhost:{0}", new object[]
			{
				AdamUserManagement.GetAdamSslPort()
			})))
			{
				string text = null;
				ldapConnection.AuthType = AuthType.Negotiate;
				ldapConnection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback(AdamUserManagement.VerifyServerCertificate);
				ldapConnection.SessionOptions.SecureSocketLayer = true;
				ldapConnection.Bind();
				ADObjectId configContainer = AdamUserManagement.GetConfigContainer(ldapConnection);
				ADObjectId descendantId = configContainer.GetDescendantId(AdamUserManagement.DirectoryServiceContainer);
				ADObjectId descendantId2 = configContainer.GetDescendantId(AdamUserManagement.ServicesContainer);
				ADObjectId descendantId3 = configContainer.GetDescendantId(AdamUserManagement.AdministrativeRolesContainer);
				AdamUserManagement.TraceDebug("user {0}", new object[]
				{
					user
				});
				if (ExTraceGlobals.SubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					using (HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider())
					{
						byte[] bytes = Encoding.ASCII.GetBytes(password);
						hashAlgorithm.TransformFinalBlock(bytes, 0, bytes.Length);
						byte[] hash = hashAlgorithm.Hash;
						AdamUserManagement.TraceDebug("passwordHash {0}", new object[]
						{
							Convert.ToBase64String(hash)
						});
					}
				}
				AdamUserManagement.EnableUserCreationInConfigContainer(ldapConnection, descendantId.DistinguishedName);
				string text2;
				if (ADObjectId.IsValidDistinguishedName(user))
				{
					text2 = user;
				}
				else
				{
					text2 = descendantId2.GetChildId(user).DistinguishedName;
				}
				if (bootStrapAccount)
				{
					DateTime d = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					DateTime d2 = DateTime.UtcNow.AddTicks(expiry.Ticks);
					d2.AddMinutes(1.0);
					text = ((ulong)(d2 - d).TotalSeconds * 10000000UL).ToString(CultureInfo.InvariantCulture);
				}
				SearchResponse searchResponse = (SearchResponse)ldapConnection.SendRequest(new SearchRequest(descendantId2.DistinguishedName, string.Format(CultureInfo.InvariantCulture, "(&(objectClass=user)(distinguishedName={0}))", new object[]
				{
					text2
				}), SearchScope.OneLevel, null));
				if (searchResponse.Entries.Count == 0)
				{
					AdamUserManagement.TraceDebug("Can't find existing ADAM account for {0}.  Creating a new one", new object[]
					{
						text2
					});
					DirectoryAttribute[] array;
					if (bootStrapAccount)
					{
						array = new DirectoryAttribute[]
						{
							null,
							null,
							new DirectoryAttribute("accountExpires", text)
						};
					}
					else
					{
						array = new DirectoryAttribute[2];
					}
					array[0] = new DirectoryAttribute("objectClass", "user");
					array[1] = new DirectoryAttribute("userPassword", password);
					ldapConnection.SendRequest(new AddRequest(text2, array));
					AdamUserManagement.GrantEdgeSyncUserAdministratorRole(ldapConnection, text2, descendantId3.DistinguishedName);
				}
				else
				{
					AdamUserManagement.TraceDebug("Updating existing ADAM account {0}", new object[]
					{
						text2
					});
					if (bootStrapAccount)
					{
						ldapConnection.SendRequest(new ModifyRequest(text2, DirectoryAttributeOperation.Replace, "accountExpires", new object[]
						{
							text
						}));
					}
					ldapConnection.SendRequest(new ModifyRequest(text2, DirectoryAttributeOperation.Replace, "userPassword", new object[]
					{
						password
					}));
				}
				result = text2;
			}
			return result;
		}

		private static int GetAdamSslPort()
		{
			int result = 50636;
			try
			{
				result = AdamUserManagement.ReadSslPortFromRegistry();
			}
			catch (Exception ex)
			{
				AdamUserManagement.TraceError("Registry query for HKLM\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange failed, exception {0}", new object[]
				{
					ex.Message
				});
				Common.EventLogger.LogEvent(MessageSecurityEventLogConstants.Tuple_FailedReadingAdamSslPort, ex.Message, new object[]
				{
					ex.Message
				});
			}
			return result;
		}

		private static string DecryptPassword(X509Certificate2 cert, byte[] encryptedPassword)
		{
			byte[] array = null;
			string @string;
			try
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)cert.PrivateKey;
				array = rsacryptoServiceProvider.Decrypt(encryptedPassword, false);
				@string = Encoding.ASCII.GetString(array);
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, array.Length);
				}
			}
			return @string;
		}

		private static X509Certificate2 GetInternalTransportCertificate(Server server)
		{
			if (server.InternalTransportCertificate == null)
			{
				AdamUserManagement.TraceError("ADAMUserManagement.GetInternalTransportCertificate failed because local server's Internal Transport Certificate in ADAM is null.", new object[0]);
				return null;
			}
			X509Certificate2 result;
			try
			{
				result = new X509Certificate2(server.InternalTransportCertificate);
			}
			catch (CryptographicException)
			{
				AdamUserManagement.TraceError("ADAMUserManagement.GetInternalTransportCertificate failed because local server's Internal Transport Certificate in ADAM is corrupted.", new object[0]);
				result = null;
			}
			return result;
		}

		private static bool VerifyServerCertificate(LdapConnection conn, X509Certificate cert)
		{
			return true;
		}

		private static byte[] GetRandomBytes(int length)
		{
			byte[] array = new byte[length];
			byte[] result;
			using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rngcryptoServiceProvider.GetBytes(array);
				result = array;
			}
			return result;
		}

		private static void GrantEdgeSyncUserAdministratorRole(LdapConnection connection, string userDN, string configContainerAdministratorRoleDN)
		{
			SearchResponse searchResponse = (SearchResponse)connection.SendRequest(new SearchRequest(configContainerAdministratorRoleDN, "(objectClass=*)", SearchScope.Base, null));
			if (searchResponse.Entries == null || searchResponse.Entries.Count == 0)
			{
				throw new MessageSecurityException(Strings.NoConfigAdminRoleObjectFound);
			}
			string text = (string)searchResponse.Entries[0].Attributes["member"][0];
			if (!string.IsNullOrEmpty(text) && text.Contains(userDN))
			{
				return;
			}
			connection.SendRequest(new ModifyRequest(configContainerAdministratorRoleDN, DirectoryAttributeOperation.Add, "member", new object[]
			{
				userDN
			}));
		}

		private static void EnableUserCreationInConfigContainer(LdapConnection connection, string directoryServiceContainerDN)
		{
			SearchResponse searchResponse = (SearchResponse)connection.SendRequest(new SearchRequest(directoryServiceContainerDN, "(objectClass=*)", SearchScope.Base, null));
			if (searchResponse.Entries == null || searchResponse.Entries.Count == 0)
			{
				throw new MessageSecurityException(Strings.NoDirectoryServiceObjectsFound(directoryServiceContainerDN));
			}
			DirectoryAttribute directoryAttribute = searchResponse.Entries[0].Attributes["msDS-Other-Settings"];
			for (int i = 0; i < directoryAttribute.Count; i++)
			{
				string text = (string)directoryAttribute[i];
				if (!string.IsNullOrEmpty(text) && text.Contains("ADAMAllowADAMSecurityPrincipalsInConfigPartition=1"))
				{
					return;
				}
				if (!string.IsNullOrEmpty(text) && text.Contains("ADAMAllowADAMSecurityPrincipalsInConfigPartition=0"))
				{
					connection.SendRequest(new ModifyRequest(directoryServiceContainerDN, DirectoryAttributeOperation.Delete, "msDS-Other-Settings", new object[]
					{
						"ADAMAllowADAMSecurityPrincipalsInConfigPartition=0"
					}));
				}
			}
			connection.SendRequest(new ModifyRequest(directoryServiceContainerDN, DirectoryAttributeOperation.Add, "msDS-Other-Settings", new object[]
			{
				"ADAMAllowADAMSecurityPrincipalsInConfigPartition=1"
			}));
		}

		private static ADObjectId GetConfigContainer(LdapConnection connection)
		{
			SearchResponse searchResponse = (SearchResponse)connection.SendRequest(new SearchRequest(string.Empty, "(objectClass=*)", SearchScope.Base, null));
			if (searchResponse.Entries.Count == 0)
			{
				ExTraceGlobals.SubscriptionTracer.TraceError(0L, "No root was found");
				throw new MessageSecurityException(Strings.NoRootFound);
			}
			if (searchResponse.Entries.Count > 1)
			{
				ExTraceGlobals.SubscriptionTracer.TraceError(0L, "Can't have more than one Root");
				throw new MessageSecurityException(Strings.MoreThanOneRootFound);
			}
			SearchResultAttributeCollection attributes = searchResponse.Entries[0].Attributes;
			return new ADObjectId((string)attributes["configurationNamingContext"][0]);
		}

		private static void TraceError(string message, params object[] args)
		{
			ExTraceGlobals.SubscriptionTracer.TraceError(0L, message, args);
		}

		private static void TraceDebug(string message, params object[] args)
		{
			ExTraceGlobals.SubscriptionTracer.TraceDebug(0L, message, args);
		}

		private const string AllowADAMSecurityPrincipalsInConfigPartition = "ADAMAllowADAMSecurityPrincipalsInConfigPartition=1";

		private const string DisallowADAMSecurityPrincipalsInConfigPartition = "ADAMAllowADAMSecurityPrincipalsInConfigPartition=0";

		private static readonly ADObjectId ServicesContainer = new ADObjectId("CN=Services");

		private static readonly ADObjectId DirectoryServiceContainer = new ADObjectId("CN=Directory Service,CN=Windows NT,CN=Services");

		private static readonly ADObjectId AdministrativeRolesContainer = new ADObjectId("CN=Administrators,CN=Roles");

		private static Dictionary<string, string> credentialCache = new Dictionary<string, string>();
	}
}

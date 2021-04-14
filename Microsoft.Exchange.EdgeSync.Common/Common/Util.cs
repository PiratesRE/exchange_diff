using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Logging;
using Microsoft.Exchange.MessageSecurity.EdgeSync;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.EdgeSync.Common
{
	internal static class Util
	{
		public static NetworkCredential ExtractNetworkCredential(Server hubServer, string edgeServerFqdn, EdgeSyncLogSession logSession)
		{
			EdgeSyncCredential edgeSyncCredential = new EdgeSyncCredential();
			edgeSyncCredential.EffectiveDate = 0L;
			edgeSyncCredential.ESRAUsername = string.Empty;
			edgeSyncCredential.EncryptedESRAPassword = null;
			bool flag = false;
			foreach (byte[] data in hubServer.EdgeSyncCredentials)
			{
				EdgeSyncCredential edgeSyncCredential2 = EdgeSyncCredential.DeserializeEdgeSyncCredential(data);
				if (edgeServerFqdn.Equals(edgeSyncCredential2.EdgeServerFQDN, StringComparison.OrdinalIgnoreCase) && edgeSyncCredential2.EffectiveDate < DateTime.UtcNow.Ticks && edgeSyncCredential2.EffectiveDate > edgeSyncCredential.EffectiveDate)
				{
					edgeSyncCredential = edgeSyncCredential2;
					flag = true;
				}
			}
			if (!flag)
			{
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, hubServer.Name, new object[]
				{
					hubServer.Fqdn,
					edgeServerFqdn
				});
				return null;
			}
			return Util.DecryptEdgeSyncCredential(hubServer, edgeSyncCredential, logSession);
		}

		public static NetworkCredential DecryptEdgeSyncCredential(Server localServer, EdgeSyncCredential cred, EdgeSyncLogSession logSession)
		{
			X509Certificate2 x509Certificate = null;
			X509Store x509Store = null;
			try
			{
				if (localServer.InternalTransportCertificate == null)
				{
					logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve the default certificate.  Unable to decrypt credentials.");
					EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, null, new object[]
					{
						cred.EdgeServerFQDN
					});
					return null;
				}
				X509Certificate2 x509Certificate2 = new X509Certificate2(localServer.InternalTransportCertificate);
				x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, x509Certificate2.Thumbprint, false);
				if (x509Certificate2Collection.Count <= 0)
				{
					logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve the certificate from the local store.");
					EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, null, new object[]
					{
						cred.EdgeServerFQDN
					});
					return null;
				}
				x509Certificate = x509Certificate2Collection[0];
			}
			catch (ArgumentException)
			{
				logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve certificate.");
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, null, new object[]
				{
					cred.EdgeServerFQDN
				});
				return null;
			}
			catch (CryptographicException)
			{
				logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve certificate because of Corrupt or mismatched keys.");
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, null, new object[]
				{
					cred.EdgeServerFQDN
				});
				return null;
			}
			catch (SecurityException)
			{
				logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve certificate because we don't have permission.");
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_NoCredentialsFound, null, new object[]
				{
					cred.EdgeServerFQDN
				});
				return null;
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			byte[] bytes;
			try
			{
				AsymmetricAlgorithm privateKey = x509Certificate.PrivateKey;
				if (privateKey == null)
				{
					logSession.LogCredential(cred.EdgeServerFQDN, "Failed to retrieve certificate because it doesn't have private key.");
					logSession.LogCertificate(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, x509Certificate);
					EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_CredentialDecryptionException, null, new object[]
					{
						cred.EdgeServerFQDN,
						SystemStrings.NoPrivateKey,
						x509Certificate.Thumbprint,
						x509Certificate.Subject
					});
					return null;
				}
				RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)privateKey;
				bytes = rsacryptoServiceProvider.Decrypt(cred.EncryptedESRAPassword, false);
			}
			catch (CryptographicException ex)
			{
				logSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, ex, "Failed to decrypt the credential.");
				logSession.LogCertificate(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, x509Certificate);
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_CredentialDecryptionException, null, new object[]
				{
					cred.EdgeServerFQDN,
					ex.Message,
					x509Certificate.Thumbprint,
					x509Certificate.Subject
				});
				return null;
			}
			logSession.LogCredentialDetail(cred.ESRAUsername, new DateTime(cred.EffectiveDate));
			return new NetworkCredential(cred.ESRAUsername, Encoding.ASCII.GetString(bytes));
		}

		public static string MakeSiteConnectorName(Server server)
		{
			return string.Format("EdgeSync - {0} to Internet", server.ServerSite.Name);
		}

		public static string MakeInboundConnectorName(Server server)
		{
			return string.Format("EdgeSync - Inbound to {0}", server.ServerSite.Name);
		}

		public static SmtpSendConnectorConfig TryFindConnector(ITopologyConfigurationSession session, string cn)
		{
			SmtpSendConnectorConfig[] array = session.Find<SmtpSendConnectorConfig>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, cn), null, 1);
			if (array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		public static string GetServerName(LdapDirectoryIdentifier directoryId)
		{
			string text = string.Empty;
			if (directoryId != null && directoryId.Servers != null && directoryId.Servers.Length > 0 && directoryId.Servers[0] != null)
			{
				text = directoryId.Servers[0];
				int num = text.IndexOf(':');
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
			}
			return text;
		}

		public static QueryFilter BuildServerFilterForSite(ADObjectId siteId)
		{
			return new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 64UL)
				}),
				new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId)
			});
		}

		public static byte[] ComputeHash(SortedList<string, DirectoryAttribute> attributesToCopy)
		{
			byte[] result = null;
			SHA256Cng sha256Cng = new SHA256Cng();
			using (sha256Cng)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, sha256Cng, CryptoStreamMode.Write))
					{
						foreach (KeyValuePair<string, DirectoryAttribute> keyValuePair in attributesToCopy)
						{
							byte[] array = Encoding.UTF8.GetBytes(keyValuePair.Key);
							cryptoStream.Write(array, 0, array.Length);
							array = Encoding.Unicode.GetBytes(keyValuePair.Value.Name);
							cryptoStream.Write(array, 0, array.Length);
							foreach (object obj in keyValuePair.Value)
							{
								if (obj is string)
								{
									array = Encoding.UTF8.GetBytes((string)obj);
								}
								else if (obj is Uri)
								{
									array = Encoding.UTF8.GetBytes(((Uri)obj).ToString());
								}
								else
								{
									array = (byte[])obj;
								}
								cryptoStream.Write(array, 0, array.Length);
							}
						}
						cryptoStream.FlushFinalBlock();
						result = sha256Cng.Hash;
					}
				}
			}
			return result;
		}

		public static string FindAssembly(DirectoryInfo dir, string fileName)
		{
			string result;
			try
			{
				if (File.Exists(Path.Combine(dir.FullName, fileName)))
				{
					result = Path.Combine(dir.FullName, fileName);
				}
				else
				{
					DirectoryInfo[] directories = dir.GetDirectories();
					if (directories.Length > 0)
					{
						foreach (DirectoryInfo dir2 in directories)
						{
							string result2;
							if ((result2 = Util.FindAssembly(dir2, fileName)) != null)
							{
								return result2;
							}
						}
						result = null;
					}
					else
					{
						result = null;
					}
				}
			}
			catch (DirectoryNotFoundException)
			{
				result = null;
			}
			return result;
		}

		public static string GetCookieInformationToLog(Dictionary<string, Cookie> cookies)
		{
			if (cookies == null)
			{
				return "<null>";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Cookie cookie in cookies.Values)
			{
				stringBuilder.AppendFormat("{0}, ", (cookie == null) ? "<null>" : cookie.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal class CertificateCache : ICertificateCache
	{
		public CertificateCache(ChainEnginePool pool)
		{
			if (pool == null)
			{
				throw new ArgumentNullException("pool");
			}
			this.pool = pool;
		}

		public X509Certificate2 EphemeralInternalTransportCertificate
		{
			get
			{
				if (!this.ephemeralInternalTransportCertificateInitialized)
				{
					lock (this.ephemeralInternalTransportCertificateLock)
					{
						if (!this.ephemeralInternalTransportCertificateInitialized)
						{
							this.ephemeralInternalTransportCertificateInitialized = true;
							try
							{
								this.ephemeralInternalTransportCertificate = CertificateCache.CreateEphemeralInternalTransportCertificate();
							}
							catch (CryptographicException arg)
							{
								ExTraceGlobals.CertificateTracer.TraceError<CryptographicException>(0L, "Ephemeral Internal Transport Certificate could not be created: {0}", arg);
							}
						}
					}
				}
				return this.ephemeralInternalTransportCertificate;
			}
		}

		public void Open(OpenFlags flags)
		{
			this.rootStore.Open(flags);
			this.certStore.Open(flags);
		}

		public void Close()
		{
			if (this.certStore != null)
			{
				this.certStore.Close();
				this.certStore = null;
				this.certStoreBookmark = -1;
			}
			if (this.rootStore != null)
			{
				this.rootStore.Close();
				this.rootStoreBookmark = -1;
			}
		}

		public void Reset()
		{
			this.cache.Clear();
			this.certStoreBookmark--;
			this.rootStoreBookmark--;
		}

		public X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed)
		{
			return this.Find(names, wildcardAllowed, WildcardMatchType.MultiLevel);
		}

		public X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType)
		{
			IX509Certificate2 ix509Certificate;
			if (this.TryFind(names, wildcardAllowed, wildcardMatchType, out ix509Certificate))
			{
				return ix509Certificate.Certificate;
			}
			return null;
		}

		public bool TryFind(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType, out IX509Certificate2 certificate)
		{
			certificate = null;
			if (this.certStore == null)
			{
				return false;
			}
			X509Certificate2 x509Certificate = this.FindHelper(names, wildcardAllowed, wildcardMatchType, new TlsCertificateInfo.FilterCert(TlsCertificateInfo.EmptyFilterCert), null);
			if (x509Certificate == null)
			{
				return false;
			}
			certificate = new X509Certificate2Wrapper(x509Certificate);
			return true;
		}

		public IX509Certificate2 FindByThumbprint(string thumbprint)
		{
			X509Certificate2 x509Certificate = this.Find(thumbprint);
			if (x509Certificate != null)
			{
				return new X509Certificate2Wrapper(x509Certificate);
			}
			return null;
		}

		public X509Certificate2 Find(string thumbprint)
		{
			if (this.certStore == null)
			{
				return null;
			}
			X509Certificate2 x509Certificate;
			lock (this)
			{
				this.CheckStores();
				if (!this.thumbprintBasedCache.TryGetValue(thumbprint, out x509Certificate))
				{
					x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, X509FindType.FindByThumbprint, thumbprint);
					this.thumbprintBasedCache.Add(thumbprint, x509Certificate);
				}
			}
			if (x509Certificate != null)
			{
				ExTraceGlobals.CertificateTracer.Information<string>(0L, "A certificate with thumbprint [{0}] has been found.", thumbprint);
			}
			else
			{
				ExTraceGlobals.CertificateTracer.TraceError<string>(0L, "A certificate with thumbprint [{0}] has not been found.", thumbprint);
			}
			return x509Certificate;
		}

		public X509Certificate2 Find(SmtpX509Identifier x509Identifier)
		{
			IX509Certificate2 ix509Certificate;
			if (this.TryFind(x509Identifier, out ix509Certificate))
			{
				return ix509Certificate.Certificate;
			}
			return null;
		}

		public bool TryFind(SmtpX509Identifier x509Identifier, out IX509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("x509Identifier", x509Identifier);
			certificate = null;
			if (this.certStore == null)
			{
				return false;
			}
			string text = x509Identifier.ToString();
			X509Certificate2 x509Certificate;
			lock (this)
			{
				this.CheckStores();
				if (!this.cache.TryGetValue(text, out x509Certificate))
				{
					string certificateIssuer = (!string.IsNullOrEmpty(x509Identifier.CertificateIssuer)) ? x509Identifier.CertificateIssuer : string.Empty;
					if (TlsCertificateInfo.TryFindFirstCertWithSubjectAndIssuerDistinguishedName(x509Identifier.CertificateSubject, certificateIssuer, out x509Certificate))
					{
						this.cache.Add(text, x509Certificate);
					}
				}
			}
			if (x509Certificate == null)
			{
				ExTraceGlobals.CertificateTracer.Information<string>((long)text.GetHashCode(), "No certificate search found for the X509Identifier {0} ", text);
				return false;
			}
			ExTraceGlobals.CertificateTracer.Information<string, string>((long)text.GetHashCode(), "Certificate found with Thumbprint = [{0}] and X509Identifier = {1} ", x509Certificate.Thumbprint, text);
			certificate = new X509Certificate2Wrapper(x509Certificate);
			return true;
		}

		public X509Certificate2 GetInternalTransportCertificate(string thumbprint, ExEventLog logger)
		{
			IX509Certificate2 internalTransportCertificate = this.GetInternalTransportCertificate(thumbprint, new ExEventLogWrapper(logger));
			if (internalTransportCertificate != null)
			{
				return internalTransportCertificate.Certificate;
			}
			return null;
		}

		public IX509Certificate2 GetInternalTransportCertificate(string thumbprint, IExEventLog logger)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			X509Certificate2 x509Certificate = null;
			if (!string.IsNullOrEmpty(thumbprint))
			{
				x509Certificate = this.Find(thumbprint);
			}
			if (x509Certificate == null)
			{
				string dnsPhysicalFullyQualifiedDomainName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
				ExTraceGlobals.CertificateTracer.TraceError<string>(0L, "Cannot load Internal Transport Certificate indicated by configuration. Falling back to a certificate with server FQDN '{0}'.", dnsPhysicalFullyQualifiedDomainName);
				x509Certificate = this.Find(new string[]
				{
					dnsPhysicalFullyQualifiedDomainName
				}, true);
				if (x509Certificate != null)
				{
					logger.LogEvent(TransportEventLogConstants.Tuple_CannotLoadInternalTransportCertificateFallbackServerFQDN, thumbprint, new object[]
					{
						thumbprint,
						x509Certificate.Thumbprint
					});
				}
				else
				{
					ExTraceGlobals.CertificateTracer.TraceDebug<string>(0L, "Fall-back to certificate with server FQDN of '{0}' failed. Falling back to ephemeral, self-signed certificate.", dnsPhysicalFullyQualifiedDomainName);
					x509Certificate = this.EphemeralInternalTransportCertificate;
					if (x509Certificate != null)
					{
						logger.LogEvent(TransportEventLogConstants.Tuple_CannotLoadIntTransportCertificateFallbackEphemeralCertificate, thumbprint, new object[]
						{
							thumbprint,
							x509Certificate.Thumbprint
						});
					}
					else
					{
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "CannotLoadIntTransportCertificateFallbackEphemeralCertificate", null, "Fall-back to ephemeral certificate failed. No valid certificate could be found for Internal Transport Certificate.", ResultSeverityLevel.Error, false);
						ExTraceGlobals.CertificateTracer.TraceDebug(0L, "Fall-back to ephemeral certificate failed. No valid certificate could be found for Internal Transport Certificate.");
						logger.LogEvent(TransportEventLogConstants.Tuple_CannotLoadInternalTransportCertificateFromStore, thumbprint, new object[]
						{
							thumbprint
						});
					}
				}
			}
			if (x509Certificate != null)
			{
				ExTraceGlobals.CertificateTracer.TraceDebug<string>(0L, "Certificate with thumbprint '{0}' is being returned for Internal Transport Certificate.", x509Certificate.Thumbprint);
			}
			if (x509Certificate != null)
			{
				return new X509Certificate2Wrapper(x509Certificate);
			}
			return null;
		}

		private static X509Certificate2 CreateEphemeralInternalTransportCertificate()
		{
			ICollection<string> internalTransportCertificateDomains = CertificateCache.GetInternalTransportCertificateDomains();
			if (internalTransportCertificateDomains == null || !internalTransportCertificateDomains.Any<string>())
			{
				return null;
			}
			return TlsCertificateInfo.CreateSelfSignCertificate(TlsCertificateInfo.GetDefaultSubjectName(internalTransportCertificateDomains), internalTransportCertificateDomains, DateTime.UtcNow.AddMonths(12) - DateTime.UtcNow, CertificateCreationOption.None, 2048, null, true);
		}

		private static ICollection<string> GetInternalTransportCertificateDomains()
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] array = new string[]
			{
				ComputerInformation.DnsHostName,
				ComputerInformation.DnsPhysicalHostName,
				ComputerInformation.DnsFullyQualifiedDomainName,
				ComputerInformation.DnsPhysicalFullyQualifiedDomainName
			};
			foreach (string text in array)
			{
				if (Dns.IsValidName(text) || Dns.IsValidWildcardName(text))
				{
					hashSet.TryAdd(text);
				}
			}
			return hashSet;
		}

		private void Clear()
		{
			this.cache.Clear();
			this.thumbprintBasedCache.Clear();
			this.certStoreBookmark = -1;
		}

		private void CheckStores()
		{
			int bookmark = this.certStore.Bookmark;
			int bookmark2 = this.rootStore.Bookmark;
			if (bookmark != this.certStoreBookmark || bookmark2 != this.rootStoreBookmark)
			{
				ExTraceGlobals.CertificateTracer.Information(0L, "Certificate store My | Root has changed - flushing certificate cache");
				this.Clear();
				this.certStoreBookmark = bookmark;
				this.rootStoreBookmark = bookmark2;
			}
		}

		private X509Certificate2 FindHelper(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType, TlsCertificateInfo.FilterCert filterCert, SmtpX509Identifier x509Identifier)
		{
			if (this.certStore == null)
			{
				return null;
			}
			List<string> list = names.ToList<string>();
			StringBuilder stringBuilder = new StringBuilder(256);
			int num = 0;
			foreach (string value in list)
			{
				if (num++ != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(value);
			}
			stringBuilder.Append(wildcardAllowed.ToString());
			stringBuilder.Append(wildcardMatchType);
			if (x509Identifier != null)
			{
				stringBuilder.Append(x509Identifier.CertificateSubject);
				stringBuilder.Append(x509Identifier.CertificateIssuer);
			}
			string key = stringBuilder.ToString();
			X509Certificate2 x509Certificate;
			lock (this)
			{
				this.CheckStores();
				if (!this.cache.TryGetValue(key, out x509Certificate))
				{
					CertificateSelectionOption certificateSelectionOption = wildcardAllowed ? CertificateSelectionOption.WildcardAllowed : CertificateSelectionOption.None;
					certificateSelectionOption |= CertificateSelectionOption.PreferedNonSelfSigned;
					try
					{
						using (ChainEngine engine = this.pool.GetEngine())
						{
							if (x509Identifier == null)
							{
								x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, list, certificateSelectionOption, wildcardMatchType, engine);
							}
							else
							{
								x509Certificate = TlsCertificateInfo.FindCertificate(this.certStore.BaseStore, list, certificateSelectionOption, wildcardMatchType, engine, filterCert, x509Identifier.CertificateSubject, x509Identifier.CertificateIssuer);
							}
						}
						this.cache.Add(key, x509Certificate);
					}
					catch (ArgumentException)
					{
						x509Certificate = null;
					}
				}
			}
			stringBuilder.Clear();
			if (x509Certificate != null)
			{
				stringBuilder.AppendFormat("Certificate search found [{0}] which has one of the following FQDN's ({1}), wildcard allowed ({2}), wildcard match type ({3}) ", new object[]
				{
					x509Certificate.Thumbprint,
					new EnumerableTracer<string>(list),
					wildcardAllowed.ToString(),
					wildcardMatchType.ToString()
				});
			}
			else
			{
				stringBuilder.AppendFormat("No certificate search found which has one of the following FQDN's : {0}, wildcard allowed ({1}), wildcard match type ({2}) ", new object[]
				{
					new EnumerableTracer<string>(list),
					wildcardAllowed.ToString(),
					wildcardMatchType.ToString()
				});
			}
			if (x509Identifier != null)
			{
				stringBuilder.AppendFormat("subject ({0}), issuer ({1})  ", new object[]
				{
					x509Identifier.CertificateSubject,
					x509Identifier.CertificateIssuer
				});
			}
			ExTraceGlobals.CertificateTracer.Information((long)list.GetHashCode(), stringBuilder.ToString());
			return x509Certificate;
		}

		private const int EphemeralInternalTransportCertificateKeySize = 2048;

		private CertificateStore certStore = new CertificateStore(StoreName.My, StoreLocation.LocalMachine);

		private int certStoreBookmark = -1;

		private readonly CertificateStore rootStore = new CertificateStore(StoreName.Root, StoreLocation.LocalMachine);

		private int rootStoreBookmark = -1;

		private readonly Dictionary<string, X509Certificate2> cache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, X509Certificate2> thumbprintBasedCache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

		private X509Certificate2 ephemeralInternalTransportCertificate;

		private bool ephemeralInternalTransportCertificateInitialized;

		private readonly object ephemeralInternalTransportCertificateLock = new object();

		private readonly ChainEnginePool pool;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class ExchangeCertificate : X509Certificate2, ISerializable, IFormattable
	{
		private ExchangeCertificate(SerializationInfo info, StreamingContext context)
		{
			byte[] rawData = (byte[])info.GetValue("CertData", typeof(byte[]));
			this.Import(rawData);
			this.selfSigned = (bool)info.GetValue("SelfSigned", typeof(bool));
			this.status = (CertificateStatus)info.GetValue("Status", typeof(CertificateStatus));
			this.rootCAType = (CertificateAuthorityType)info.GetValue("RootCAType", typeof(CertificateAuthorityType));
			this.services = (AllowedServices)info.GetValue("Services", typeof(AllowedServices));
			this.iisServices = (List<IisService>)info.GetValue("IISServices", typeof(List<IisService>));
			this.privateKeyExportable = (bool)info.GetValue("PrivateKeyExportable", typeof(bool));
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "Identity")
				{
					this.Identity = info.GetString("Identity");
				}
			}
		}

		internal ExchangeCertificate(object[] data)
		{
			if (data == null && data.Length < 8)
			{
				throw new ArgumentException("data");
			}
			this.Import((byte[])data[0]);
			this.selfSigned = (bool)data[1];
			this.status = (CertificateStatus)data[2];
			this.rootCAType = (CertificateAuthorityType)data[3];
			this.services = (AllowedServices)data[4];
			List<IisService> list = new List<IisService>();
			list.AddRange((IisService[])data[5]);
			this.iisServices = list;
			this.privateKeyExportable = (bool)data[6];
			if (data[7] != null)
			{
				this.Identity = (string)data[7];
			}
		}

		public ExchangeCertificate(X509Certificate2 cert) : base(cert)
		{
			this.services = AllowedServices.None;
		}

		internal static ExchangeCertificate GetInternalTransportCertificate(Server server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (server.InternalTransportCertificate == null)
			{
				return null;
			}
			ExchangeCertificate result;
			try
			{
				X509Certificate2 cert = new X509Certificate2(server.InternalTransportCertificate);
				result = new ExchangeCertificate(cert);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		internal static ExchangeCertificate GetCertificateFromStore(string storeName, string thumbprint)
		{
			X509Store store = new X509Store(storeName, StoreLocation.LocalMachine);
			return ExchangeCertificate.GetCertificateFromStore(store, thumbprint);
		}

		internal static ExchangeCertificate GetCertificateFromStore(StoreName storeName, string thumbprint)
		{
			X509Store store = new X509Store(storeName, StoreLocation.LocalMachine);
			return ExchangeCertificate.GetCertificateFromStore(store, thumbprint);
		}

		private static ExchangeCertificate GetCertificateFromStore(X509Store store, string thumbprint)
		{
			if (thumbprint == null)
			{
				throw new ArgumentNullException("thumbprint");
			}
			try
			{
				store.Open(OpenFlags.OpenExistingOnly);
			}
			catch (CryptographicException)
			{
				return null;
			}
			ExchangeCertificate result;
			try
			{
				X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection.Count == 0)
				{
					result = null;
				}
				else
				{
					result = new ExchangeCertificate(x509Certificate2Collection[0]);
				}
			}
			finally
			{
				store.Close();
			}
			return result;
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return ExFormatProvider.FormatX509Certificate(this, format, formatProvider);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.CheckCertificateChainAndCacheProps();
			info.AddValue("CertData", this.Export(X509ContentType.SerializedCert));
			info.AddValue("SelfSigned", this.selfSigned);
			info.AddValue("Status", this.status);
			info.AddValue("RootCAType", this.rootCAType);
			info.AddValue("Services", this.services);
			info.AddValue("IISServices", this.iisServices);
			info.AddValue("PrivateKeyExportable", this.privateKeyExportable);
			if (this.Identity != null)
			{
				info.AddValue("Identity", this.Identity);
			}
		}

		public IList<AccessRule> AccessRules
		{
			get
			{
				return TlsCertificateInfo.GetAccessRules(this);
			}
		}

		public IList<SmtpDomainWithSubdomains> CertificateDomains
		{
			get
			{
				IList<string> fqdns = TlsCertificateInfo.GetFQDNs(this);
				List<SmtpDomainWithSubdomains> list = new List<SmtpDomainWithSubdomains>(fqdns.Count);
				foreach (string s in fqdns)
				{
					SmtpDomainWithSubdomains item;
					if (SmtpDomainWithSubdomains.TryParse(s, out item))
					{
						list.Add(item);
					}
				}
				return list.AsReadOnly();
			}
		}

		public string CertificateRequest
		{
			get
			{
				return CertificateEnroller.ReadPkcs10Request(this);
			}
		}

		public List<IisService> IisServices
		{
			get
			{
				return this.iisServices;
			}
		}

		public bool IsSelfSigned
		{
			get
			{
				this.CheckCertificateChainAndCacheProps();
				return this.selfSigned;
			}
		}

		public string KeyIdentifier
		{
			get
			{
				if (base.PublicKey == null)
				{
					return string.Empty;
				}
				X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = new X509SubjectKeyIdentifierExtension(base.PublicKey, false);
				return x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier;
			}
		}

		public CertificateAuthorityType RootCAType
		{
			get
			{
				this.CheckCertificateChainAndCacheProps();
				return this.rootCAType;
			}
		}

		public AllowedServices Services
		{
			get
			{
				return this.services;
			}
			internal set
			{
				this.services = value;
			}
		}

		public CertificateStatus Status
		{
			get
			{
				this.CheckCertificateChainAndCacheProps();
				return this.status;
			}
		}

		public string SubjectKeyIdentifier
		{
			get
			{
				foreach (X509Extension x509Extension in base.Extensions)
				{
					if (x509Extension is X509SubjectKeyIdentifierExtension)
					{
						X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = (X509SubjectKeyIdentifierExtension)x509Extension;
						return x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier;
					}
				}
				return string.Empty;
			}
		}

		public bool PrivateKeyExportable
		{
			get
			{
				this.CheckCertificateChainAndCacheProps();
				return this.privateKeyExportable;
			}
		}

		public int PublicKeySize
		{
			get
			{
				if (base.PublicKey != null && base.PublicKey.Key != null)
				{
					return base.PublicKey.Key.KeySize;
				}
				return 0;
			}
		}

		public string Identity { get; set; }

		public string ServicesStringForm
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if ((this.Services & AllowedServices.IMAP) != AllowedServices.None)
				{
					stringBuilder.Append('I');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.POP) != AllowedServices.None)
				{
					stringBuilder.Append('P');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.UM) != AllowedServices.None)
				{
					stringBuilder.Append('U');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.IIS) != AllowedServices.None)
				{
					stringBuilder.Append('W');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.SMTP) != AllowedServices.None)
				{
					stringBuilder.Append('S');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.Federation) != AllowedServices.None)
				{
					stringBuilder.Append('F');
				}
				else
				{
					stringBuilder.Append('.');
				}
				if ((this.Services & AllowedServices.UMCallRouter) != AllowedServices.None)
				{
					stringBuilder.Append('C');
				}
				else
				{
					stringBuilder.Append('.');
				}
				return stringBuilder.ToString();
			}
		}

		private static Dictionary<string, CertificateAuthorityType> PhysicalStoreList()
		{
			return new Dictionary<string, CertificateAuthorityType>(4)
			{
				{
					"Root\\.AuthRoot",
					CertificateAuthorityType.ThirdParty
				},
				{
					"Root\\.Default",
					CertificateAuthorityType.Registry
				},
				{
					"Root\\.Enterprise",
					CertificateAuthorityType.Enterprise
				},
				{
					"Root\\.GroupPolicy",
					CertificateAuthorityType.GroupPolicy
				}
			};
		}

		private static CertificateAuthorityType RootSource(string thumbprint)
		{
			X509Store x509Store = null;
			foreach (string text in ExchangeCertificate.physicalStores.Keys)
			{
				try
				{
					x509Store = CertificateStore.Open(StoreType.Physical, text, OpenFlags.OpenExistingOnly);
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
					if (x509Certificate2Collection != null && x509Certificate2Collection.Count > 0)
					{
						return ExchangeCertificate.physicalStores[text];
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				finally
				{
					if (x509Store != null)
					{
						x509Store.Close();
					}
					x509Store = null;
				}
			}
			return CertificateAuthorityType.Unknown;
		}

		internal object[] ExchangeCertificateAsArray()
		{
			object[] array = new object[8];
			this.CheckCertificateChainAndCacheProps();
			array[0] = this.Export(X509ContentType.SerializedCert);
			array[1] = this.selfSigned;
			array[2] = this.status;
			array[3] = this.rootCAType;
			array[4] = this.services;
			array[5] = this.IisServices.ToArray();
			array[6] = this.privateKeyExportable;
			array[7] = this.Identity;
			return array;
		}

		private void CheckCertificateChainAndCacheProps()
		{
			if (this.status != CertificateStatus.Unknown)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.CertificateRequest))
			{
				this.status = CertificateStatus.PendingRequest;
				this.selfSigned = false;
				this.rootCAType = CertificateAuthorityType.Unknown;
				return;
			}
			this.privateKeyExportable = TlsCertificateInfo.IsCertificateExportable(this);
			ChainPolicyParameters options = new BaseChainPolicyParameters(ChainPolicyOptions.None);
			ChainMatchIssuer pkixKpServerAuth = AndChainMatchIssuer.PkixKpServerAuth;
			ChainBuildParameter parameter = new ChainBuildParameter(pkixKpServerAuth, TimeSpan.FromSeconds(30.0), false, TimeSpan.Zero);
			using (ChainEngine chainEngine = new ChainEngine())
			{
				using (ChainContext chainContext = chainEngine.Build(this, ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout, parameter))
				{
					if (chainContext == null)
					{
						this.status = CertificateStatus.Unknown;
						this.selfSigned = false;
						this.rootCAType = CertificateAuthorityType.Unknown;
					}
					else
					{
						this.selfSigned = chainContext.IsSelfSigned;
						if (chainContext.Status == TrustStatus.IsUntrustedRoot)
						{
							if (chainContext.IsSelfSigned)
							{
								this.status = CertificateStatus.Valid;
								this.rootCAType = CertificateAuthorityType.None;
							}
							else
							{
								this.status = CertificateStatus.Untrusted;
								this.rootCAType = CertificateAuthorityType.Unknown;
							}
						}
						else
						{
							ChainSummary chainSummary = chainContext.Validate(options);
							ChainValidityStatus chainValidityStatus = chainSummary.Status;
							if (chainValidityStatus <= (ChainValidityStatus)2148081683U)
							{
								if (chainValidityStatus == ChainValidityStatus.Valid)
								{
									this.status = CertificateStatus.Valid;
									goto IL_168;
								}
								switch (chainValidityStatus)
								{
								case (ChainValidityStatus)2148081682U:
								case (ChainValidityStatus)2148081683U:
									break;
								default:
									goto IL_15A;
								}
							}
							else
							{
								if (chainValidityStatus == (ChainValidityStatus)2148204801U)
								{
									this.status = CertificateStatus.DateInvalid;
									goto IL_168;
								}
								switch (chainValidityStatus)
								{
								case (ChainValidityStatus)2148204812U:
									this.status = CertificateStatus.Revoked;
									goto IL_168;
								case (ChainValidityStatus)2148204813U:
									goto IL_15A;
								case (ChainValidityStatus)2148204814U:
									break;
								default:
									goto IL_15A;
								}
							}
							this.status = CertificateStatus.RevocationCheckFailure;
							goto IL_168;
							IL_15A:
							this.status = CertificateStatus.Invalid;
							this.rootCAType = CertificateAuthorityType.Unknown;
							IL_168:
							if (this.status != CertificateStatus.Invalid)
							{
								X509Certificate2 rootCertificate = chainContext.RootCertificate;
								if (rootCertificate == null)
								{
									throw new InvalidOperationException("Root certificate was null!");
								}
								this.rootCAType = ExchangeCertificate.RootSource(rootCertificate.Thumbprint);
							}
						}
					}
				}
			}
		}

		private CspKeyContainerInfo GetKeyContainer()
		{
			if (!base.HasPrivateKey)
			{
				return null;
			}
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			try
			{
				asymmetricAlgorithm = base.PrivateKey;
			}
			catch (CryptographicException)
			{
				return null;
			}
			if (asymmetricAlgorithm == null)
			{
				return null;
			}
			ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = asymmetricAlgorithm as ICspAsymmetricAlgorithm;
			if (cspAsymmetricAlgorithm == null)
			{
				return null;
			}
			return cspAsymmetricAlgorithm.CspKeyContainerInfo;
		}

		internal const string Noun = "ExchangeCertificate";

		private static readonly Dictionary<string, CertificateAuthorityType> physicalStores = ExchangeCertificate.PhysicalStoreList();

		private bool selfSigned;

		private bool privateKeyExportable;

		private CertificateStatus status;

		private CertificateAuthorityType rootCAType;

		private AllowedServices services;

		private List<IisService> iisServices = new List<IisService>();

		private static class SerializationMemberNames
		{
			public const string CertData = "CertData";

			public const string SelfSigned = "SelfSigned";

			public const string Status = "Status";

			public const string RootCAType = "RootCAType";

			public const string Services = "Services";

			public const string IISServices = "IISServices";

			public const string PrivateKeyExportable = "PrivateKeyExportable";

			public const string Identity = "Identity";
		}
	}
}

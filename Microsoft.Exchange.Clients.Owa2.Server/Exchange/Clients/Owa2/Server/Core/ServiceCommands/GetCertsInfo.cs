using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetCertsInfo : ServiceCommand<GetCertsInfoResponse>
	{
		public GetCertsInfo(CallContext callContext, string certRawData, bool isSend) : base(callContext)
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			OrganizationId organizationId = mailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
			this.smimeAdminOptions = new SmimeAdminSettingsType(organizationId);
			this.certRawData = certRawData;
			this.isSend = isSend;
			this.response = new GetCertsInfoResponse();
		}

		public ChainValidityStatus ValidateCertificate(X509Certificate2 certificate, bool isSend)
		{
			this.response.PolicyFlag = 0U;
			this.response.ChainData = null;
			ChainContext chainContext = null;
			ChainValidityStatus chainValidityStatus;
			try
			{
				X509KeyUsageFlags expectedUsage = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature;
				bool checkCRLOnSend = this.smimeAdminOptions.CheckCRLOnSend;
				bool disableCRLCheck = this.smimeAdminOptions.DisableCRLCheck;
				uint crlconnectionTimeout = this.smimeAdminOptions.CRLConnectionTimeout;
				uint crlretrievalTimeout = this.smimeAdminOptions.CRLRetrievalTimeout;
				bool flag = disableCRLCheck || (isSend && !checkCRLOnSend);
				if (string.IsNullOrEmpty(this.smimeAdminOptions.SMIMECertificateIssuingCAFull))
				{
					bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
					if (enabled)
					{
						chainValidityStatus = (ChainValidityStatus)2148204809U;
						this.response.PolicyFlag = 65536U;
					}
					else
					{
						chainValidityStatus = X509CertificateCollection.ValidateCertificate(certificate, null, expectedUsage, !flag, null, null, TimeSpan.FromMilliseconds(crlconnectionTimeout), TimeSpan.FromMilliseconds(crlretrievalTimeout), ref chainContext, false, null);
						this.response.PolicyFlag = (uint)this.MapChainStatusToChainFlag(chainValidityStatus);
					}
				}
				else
				{
					X509Store x509Store = CertificateStore.Open(StoreType.Memory, null, OpenFlags.ReadWrite);
					X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
					x509Certificate2Collection.Import(Convert.FromBase64String(this.smimeAdminOptions.SMIMECertificateIssuingCAFull));
					x509Store.AddRange(x509Certificate2Collection);
					chainValidityStatus = X509CertificateCollection.ValidateCertificate(certificate, null, expectedUsage, !flag, x509Store, null, TimeSpan.FromMilliseconds(crlconnectionTimeout), TimeSpan.FromMilliseconds(crlretrievalTimeout), ref chainContext, true, base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId.ToString());
					this.response.PolicyFlag = (uint)this.MapChainStatusToChainFlag(chainValidityStatus);
				}
				if (!isSend)
				{
					this.response.DisplayedId = this.GetIdFromCertificate(certificate);
					if (this.response.DisplayedId == null)
					{
						chainValidityStatus = ChainValidityStatus.SubjectMismatch;
					}
					this.response.DisplayName = X509PartialCertificate.GetDisplayName(certificate);
					this.response.Issuer = this.GetIssuerDisplayNameFromCertificate(certificate);
				}
			}
			finally
			{
				if (chainContext != null)
				{
					chainContext.Dispose();
				}
			}
			this.response.ChainValidityStatus = (uint)chainValidityStatus;
			return chainValidityStatus;
		}

		private X509ChainStatusFlags MapChainStatusToChainFlag(ChainValidityStatus status)
		{
			if (status <= (ChainValidityStatus)2148081683U)
			{
				switch (status)
				{
				case ChainValidityStatus.Valid:
					return X509ChainStatusFlags.NoError;
				case ChainValidityStatus.ValidSelfSigned:
					return X509ChainStatusFlags.NoError;
				case ChainValidityStatus.EmptyCertificate:
					return X509ChainStatusFlags.PartialChain;
				case ChainValidityStatus.SubjectMismatch:
					return X509ChainStatusFlags.InvalidNameConstraints;
				default:
					switch (status)
					{
					case (ChainValidityStatus)2148081680U:
						return X509ChainStatusFlags.Revoked;
					case (ChainValidityStatus)2148081682U:
						return X509ChainStatusFlags.RevocationStatusUnknown;
					case (ChainValidityStatus)2148081683U:
						return X509ChainStatusFlags.OfflineRevocation;
					}
					break;
				}
			}
			else
			{
				if (status == (ChainValidityStatus)2148098052U)
				{
					return X509ChainStatusFlags.NotSignatureValid;
				}
				if (status == (ChainValidityStatus)2148098073U)
				{
					return X509ChainStatusFlags.InvalidBasicConstraints;
				}
				switch (status)
				{
				case (ChainValidityStatus)2148204801U:
					return X509ChainStatusFlags.NotTimeValid;
				case (ChainValidityStatus)2148204802U:
					return X509ChainStatusFlags.NotTimeNested;
				case (ChainValidityStatus)2148204803U:
					return X509ChainStatusFlags.InvalidBasicConstraints;
				case (ChainValidityStatus)2148204806U:
					return X509ChainStatusFlags.NotValidForUsage;
				case (ChainValidityStatus)2148204809U:
					return X509ChainStatusFlags.UntrustedRoot;
				case (ChainValidityStatus)2148204810U:
					return X509ChainStatusFlags.InvalidBasicConstraints;
				case (ChainValidityStatus)2148204812U:
					return X509ChainStatusFlags.Revoked;
				case (ChainValidityStatus)2148204813U:
					return X509ChainStatusFlags.UntrustedRoot;
				case (ChainValidityStatus)2148204814U:
					return X509ChainStatusFlags.RevocationStatusUnknown;
				case (ChainValidityStatus)2148204815U:
					return X509ChainStatusFlags.InvalidNameConstraints;
				case (ChainValidityStatus)2148204816U:
					return X509ChainStatusFlags.NotValidForUsage;
				}
			}
			return X509ChainStatusFlags.RevocationStatusUnknown;
		}

		private bool IsValidUsage(X509Certificate2 cert, X509KeyUsageFlags expectedUsage)
		{
			for (int i = 0; i < cert.Extensions.Count; i++)
			{
				if (cert.Extensions[i].Oid.Value == WellKnownOid.KeyUsage.Value)
				{
					X509KeyUsageExtension x509KeyUsageExtension = (X509KeyUsageExtension)cert.Extensions[i];
					if ((expectedUsage & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature && x509KeyUsageExtension.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature))
					{
						return true;
					}
					if ((expectedUsage & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation && x509KeyUsageExtension.KeyUsages.HasFlag(X509KeyUsageFlags.NonRepudiation))
					{
						return true;
					}
					if ((expectedUsage & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment && x509KeyUsageExtension.KeyUsages.HasFlag(X509KeyUsageFlags.KeyEncipherment))
					{
						return true;
					}
					if ((expectedUsage & X509KeyUsageFlags.DataEncipherment) == X509KeyUsageFlags.DataEncipherment && x509KeyUsageExtension.KeyUsages.HasFlag(X509KeyUsageFlags.DataEncipherment))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal string GetIdFromCertificate(X509Certificate2 certificate)
		{
			string emailAdress = X509PartialCertificate.GetEmailAdress(certificate);
			if (!string.IsNullOrEmpty(emailAdress))
			{
				return emailAdress;
			}
			string senderCertificateAttributesToDisplay = this.smimeAdminOptions.SenderCertificateAttributesToDisplay;
			if (string.IsNullOrEmpty(senderCertificateAttributesToDisplay))
			{
				return null;
			}
			IList<KeyValuePair<Oid, string>> list = X500DistinguishedNameDecoder.Decode(certificate.SubjectName);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = null;
			string[] array = this.smimeAdminOptions.SenderCertificateAttributesToDisplay.Split(GetCertsInfo.comma, StringSplitOptions.RemoveEmptyEntries);
			bool flag = true;
			bool flag2 = false;
			foreach (string text in array)
			{
				string text2 = text.Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					Oid oid = new Oid(text2);
					flag2 = false;
					foreach (KeyValuePair<Oid, string> keyValuePair in list)
					{
						if (string.Equals(keyValuePair.Key.Value, oid.Value, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(keyValuePair.Value))
						{
							if (stringBuilder == null)
							{
								stringBuilder = new StringBuilder();
							}
							if (!flag)
							{
								stringBuilder.Append(", ");
							}
							else
							{
								flag = false;
							}
							stringBuilder.Append(keyValuePair.Value);
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						return null;
					}
				}
			}
			if (stringBuilder == null)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		internal string EncodeCertChain(ChainContext chainContext, bool includeRootCert, X509Certificate2 signersCertificate)
		{
			uint count = (uint)chainContext.GetChains().Count;
			List<byte[]>[] array = new List<byte[]>[count];
			uint[] array2 = new uint[count];
			uint[] array3 = new uint[count];
			uint num = 16U;
			int num2 = 0;
			foreach (CertificateChain certificateChain in chainContext.GetChains())
			{
				array3[num2] = 16U;
				array2[num2] = 0U;
				array[num2] = new List<byte[]>(certificateChain.Elements.Count);
				foreach (ChainElement chainElement in certificateChain.Elements)
				{
					if (!signersCertificate.Equals(chainElement.Certificate) && (includeRootCert || (chainElement.TrustInformation & TrustInformation.IsSelfSigned) == TrustInformation.None))
					{
						array3[num2] += 12U;
						byte[] rawData = chainElement.Certificate.RawData;
						array[num2].Add(rawData);
						array3[num2] += (uint)rawData.Length;
						array2[num2] += 1U;
					}
				}
				num += array3[num2];
				num2++;
			}
			byte[] array4 = new byte[num];
			int num3 = 0;
			num3 += ExBitConverter.Write(num, array4, num3);
			num3 += ExBitConverter.Write((uint)chainContext.Status, array4, num3);
			num3 += ExBitConverter.Write((uint)chainContext.TrustInformation, array4, num3);
			num3 += ExBitConverter.Write(count, array4, num3);
			num2 = 0;
			foreach (CertificateChain certificateChain2 in chainContext.GetChains())
			{
				num3 += ExBitConverter.Write(array3[num2], array4, num3);
				num3 += ExBitConverter.Write((uint)certificateChain2.Status, array4, num3);
				num3 += ExBitConverter.Write((uint)certificateChain2.TrustInformation, array4, num3);
				num3 += ExBitConverter.Write(array2[num2], array4, num3);
				int num4 = 0;
				foreach (ChainElement chainElement2 in certificateChain2.Elements)
				{
					if (!signersCertificate.Equals(chainElement2.Certificate) && (includeRootCert || (chainElement2.TrustInformation & TrustInformation.IsSelfSigned) == TrustInformation.None))
					{
						num3 += ExBitConverter.Write((uint)(array[num2][num4].Length + 12), array4, num3);
						num3 += ExBitConverter.Write((uint)chainElement2.Status, array4, num3);
						num3 += ExBitConverter.Write((uint)chainElement2.TrustInformation, array4, num3);
						Array.Copy(array[num2][num4], 0, array4, num3, array[num2][num4].Length);
						num3 += array[num2][num4].Length;
						num4++;
					}
				}
				num2++;
			}
			return Convert.ToBase64String(array4);
		}

		internal string GetIssuerDisplayNameFromCertificate(X509Certificate2 certificate)
		{
			IList<KeyValuePair<Oid, string>> list = X500DistinguishedNameDecoder.Decode(certificate.IssuerName);
			if (list == null || list.Count == 0)
			{
				return string.Empty;
			}
			foreach (KeyValuePair<Oid, string> keyValuePair in list)
			{
				if (string.Equals(keyValuePair.Key.Value, GetCertsInfo.commonNameOid.Value, StringComparison.OrdinalIgnoreCase))
				{
					return keyValuePair.Value;
				}
			}
			return string.Empty;
		}

		protected override GetCertsInfoResponse InternalExecute()
		{
			byte[] rawData = Convert.FromBase64String(this.certRawData);
			X509Certificate2 x509Certificate = new X509Certificate2(rawData);
			this.response.IsValid = (this.ValidateCertificate(x509Certificate, this.isSend) == ChainValidityStatus.Valid);
			this.response.Subject = x509Certificate.Subject;
			this.response.SubjectKeyIdentifier = null;
			foreach (X509Extension x509Extension in x509Certificate.Extensions)
			{
				if (x509Extension is X509SubjectKeyIdentifierExtension)
				{
					this.response.SubjectKeyIdentifier = ((X509SubjectKeyIdentifierExtension)x509Extension).SubjectKeyIdentifier;
					break;
				}
			}
			return this.response;
		}

		private static char[] comma = new char[]
		{
			','
		};

		private static Oid commonNameOid = new Oid("2.5.4.3");

		private readonly string certRawData;

		private readonly bool isSend;

		private readonly GetCertsInfoResponse response;

		private readonly SmimeAdminSettingsType smimeAdminOptions;
	}
}

using System;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class SignedXMLVerifier
	{
		public static bool VerifySignedXml(XmlDocument xmlDoc)
		{
			XmlElement value;
			if (SignedXMLVerifier.TryGetSignatureNode(xmlDoc, out value))
			{
				SignedXml signedXml = new SignedXml(xmlDoc);
				signedXml.LoadXml(value);
				using (XmlNodeList elementsByTagName = signedXml.KeyInfo.GetXml().GetElementsByTagName("X509Certificate"))
				{
					SignedXMLVerifier.<>c__DisplayClass2 CS$<>8__locals1 = new SignedXMLVerifier.<>c__DisplayClass2();
					int num = 0;
					X509Certificate2 x509Certificate = null;
					CS$<>8__locals1.certificate = null;
					using (IEnumerator enumerator = elementsByTagName.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SignedXMLVerifier.<>c__DisplayClass5 CS$<>8__locals2 = new SignedXMLVerifier.<>c__DisplayClass5();
							CS$<>8__locals2.CS$<>8__locals3 = CS$<>8__locals1;
							CS$<>8__locals2.node = (XmlNode)enumerator.Current;
							if (!SignedXMLVerifier.TryRunAction<CryptographicException>(delegate()
							{
								CS$<>8__locals2.CS$<>8__locals3.certificate = new X509Certificate2(Encoding.ASCII.GetBytes(CS$<>8__locals2.node.InnerText));
							}))
							{
								return false;
							}
							X509Chain chain = new X509Chain();
							if (!SignedXMLVerifier.TryRunAction<ArgumentException, CryptographicException>(delegate()
							{
								chain.Build(CS$<>8__locals1.certificate);
							}))
							{
								return false;
							}
							if (chain.ChainElements.Count > num)
							{
								x509Certificate = CS$<>8__locals1.certificate;
								num = chain.ChainElements.Count;
							}
						}
					}
					if (x509Certificate != null && !SignedXMLVerifier.ValidateCertificateChain(x509Certificate, SignedXMLVerifier.GetSigningTime(xmlDoc)))
					{
						return false;
					}
					return signedXml.CheckSignature();
				}
				return false;
			}
			return false;
		}

		public static void RemoveSignature(XmlDocument xmlDoc)
		{
			XmlElement signatureElement;
			if (SignedXMLVerifier.TryGetSignatureNode(xmlDoc, out signatureElement))
			{
				SignedXMLVerifier.TryRunAction<ArgumentException>(delegate()
				{
					signatureElement.ParentNode.RemoveChild(signatureElement);
				});
			}
		}

		public static bool TryGetSignatureNode(XmlDocument xmlDoc, out XmlElement signatureElement)
		{
			signatureElement = null;
			using (XmlNodeList elementsByTagName = xmlDoc.GetElementsByTagName("Signature"))
			{
				if (elementsByTagName.Count == 0)
				{
					SignedXMLVerifier.Tracer.TraceError(0L, "Manifest Signature Verification failed: No Signature was found in the manifest.");
					return false;
				}
				if (elementsByTagName.Count >= 2)
				{
					SignedXMLVerifier.Tracer.TraceError(0L, "Manifest Signature Verification failed: More that one signature was found in the manifest.");
					return false;
				}
				signatureElement = (XmlElement)elementsByTagName[0];
			}
			return true;
		}

		private static DateTime GetSigningTime(XmlDocument xmlDoc)
		{
			XmlNodeList elements = xmlDoc.GetElementsByTagName("CounterSignature");
			if (elements == null || elements.Count == 0)
			{
				SignedXMLVerifier.Tracer.TraceWarning(0, 0L, "No Counter Signature found in xml.");
				return DateTime.UtcNow;
			}
			byte[] publicKeyBytes = null;
			if (!SignedXMLVerifier.TryRunAction<ArgumentException, FormatException>(delegate()
			{
				publicKeyBytes = Convert.FromBase64String(elements[0].InnerXml);
			}))
			{
				return DateTime.UtcNow;
			}
			SignedCms signedCms = new SignedCms();
			if (!SignedXMLVerifier.TryRunAction<ArgumentException, CryptographicException>(delegate()
			{
				signedCms.Decode(publicKeyBytes);
			}))
			{
				return DateTime.UtcNow;
			}
			if (signedCms.SignerInfos == null)
			{
				SignedXMLVerifier.Tracer.TraceWarning(0, 0L, "signedCms.SignerInfos is null");
				return DateTime.UtcNow;
			}
			foreach (SignerInfo signerInfo in signedCms.SignerInfos)
			{
				if (signerInfo.SignedAttributes != null)
				{
					foreach (CryptographicAttributeObject cryptographicAttributeObject in signerInfo.SignedAttributes)
					{
						if (cryptographicAttributeObject.Values != null)
						{
							foreach (AsnEncodedData asnEncodedData in cryptographicAttributeObject.Values)
							{
								if (asnEncodedData.Oid != null && asnEncodedData.Oid.Value != null && asnEncodedData.Oid.Value.Equals("1.2.840.113549.1.9.5", StringComparison.OrdinalIgnoreCase) && asnEncodedData.RawData != null)
								{
									Pkcs9SigningTime pkcs9SigningTime = new Pkcs9SigningTime(asnEncodedData.RawData);
									SignedXMLVerifier.Tracer.TraceInformation(0, 0L, string.Format("The Signing time is {0}.", pkcs9SigningTime.SigningTime));
									return pkcs9SigningTime.SigningTime;
								}
							}
						}
					}
				}
			}
			SignedXMLVerifier.Tracer.TraceWarning(0, 0L, "Did not find signing time in manifest, use current time.");
			return DateTime.UtcNow;
		}

		private static bool ValidateCertificateChain(X509Certificate2 certificate, DateTime verificationTime)
		{
			X509Chain chain = new X509Chain
			{
				ChainPolicy = 
				{
					RevocationFlag = X509RevocationFlag.EntireChain,
					RevocationMode = X509RevocationMode.Online,
					UrlRetrievalTimeout = TimeSpan.FromSeconds(30.0),
					VerificationTime = verificationTime
				}
			};
			bool valid = false;
			if (!SignedXMLVerifier.TryRunAction<ArgumentException, CryptographicException>(delegate()
			{
				valid = chain.Build(certificate);
			}))
			{
				return false;
			}
			if (chain.ChainStatus.Length > 1)
			{
				foreach (X509ChainElement x509ChainElement in chain.ChainElements)
				{
					foreach (X509ChainStatus x509ChainStatus in x509ChainElement.ChainElementStatus)
					{
						if (x509ChainStatus.Status != X509ChainStatusFlags.NoError)
						{
							SignedXMLVerifier.Tracer.TraceError<string, X509ChainStatusFlags, string>(0L, "Certificate Chain Validation Failed. Cert Name:{0}, Status:{1}, Info:{2}", certificate.SubjectName.Name, x509ChainStatus.Status, x509ChainStatus.StatusInformation);
							valid = false;
						}
					}
				}
			}
			return valid;
		}

		private static bool TryRunAction<T>(Action action) where T : Exception
		{
			try
			{
				action();
			}
			catch (T t)
			{
				T t2 = (T)((object)t);
				SignedXMLVerifier.Tracer.TraceError(0L, t2.Message);
				return false;
			}
			return true;
		}

		private static bool TryRunAction<T1, T2>(Action action) where T1 : Exception where T2 : Exception
		{
			try
			{
				action();
			}
			catch (T1 t)
			{
				T1 t2 = (T1)((object)t);
				SignedXMLVerifier.Tracer.TraceError(0L, t2.Message);
				return false;
			}
			catch (T2 t3)
			{
				T2 t4 = (T2)((object)t3);
				SignedXMLVerifier.Tracer.TraceError(0L, t4.Message);
				return false;
			}
			return true;
		}

		internal const string XMLSignatureTagName = "Signature";

		internal const string XMLCounterSignatureTagName = "CounterSignature";

		internal const string X509CertificateTagName = "X509Certificate";

		internal const string SigningTimeOid = "1.2.840.113549.1.9.5";

		internal static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;
	}
}

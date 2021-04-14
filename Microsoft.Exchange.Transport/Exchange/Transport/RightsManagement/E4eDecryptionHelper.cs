using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eDecryptionHelper
	{
		internal static E4eDecryptionHelper Instance
		{
			get
			{
				if (E4eDecryptionHelper.instance == null)
				{
					E4eDecryptionHelper.instance = new E4eDecryptionHelper();
				}
				return E4eDecryptionHelper.instance;
			}
		}

		internal static bool VerifyMailItemProperties(EmailMessage emailMessage, EnvelopeRecipientCollection recipients, out string htmlString)
		{
			htmlString = string.Empty;
			Attachment htmlAttachment = E4eDecryptionHelper.GetHtmlAttachment(emailMessage);
			if (htmlAttachment == null)
			{
				return false;
			}
			if (E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eMessageDecrypted"))
			{
				E4eLog.Instance.LogInfo(emailMessage.MessageId, "Skipping decryption--mail is already decrypted once in the pipeline.", new object[0]);
				return false;
			}
			if (recipients.Count != 1)
			{
				E4eLog.Instance.LogError(emailMessage.MessageId, "Will be delivered encrypted. Recipient count not equal one. Count: {0}", new object[]
				{
					recipients.Count
				});
				return false;
			}
			using (Stream contentReadStream = htmlAttachment.GetContentReadStream())
			{
				byte[] bytes = E4eHelper.ReadStreamToEnd(contentReadStream);
				htmlString = Encoding.UTF8.GetString(bytes);
			}
			return true;
		}

		internal static Attachment GetHtmlAttachment(EmailMessage emailMessage)
		{
			foreach (Attachment attachment in emailMessage.Attachments)
			{
				if (string.Equals(attachment.FileName, "message.html", StringComparison.OrdinalIgnoreCase))
				{
					return attachment;
				}
			}
			return null;
		}

		internal static string GetValueOf(string html, string tagName, string attributeName, string attributeValue, string attributeNameOfValue = "value")
		{
			if (string.IsNullOrWhiteSpace(html))
			{
				throw new E4eException("Html is null or empty.");
			}
			string element = E4eHtmlParser.GetElement(html, tagName, attributeName, attributeValue);
			string result = null;
			if (!string.IsNullOrWhiteSpace(element))
			{
				string text;
				E4eHtmlParser.GetElement(element, tagName, attributeNameOfValue, null, out text, out result);
			}
			return result;
		}

		internal static string GetVersion(ref string htmlString)
		{
			return E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "version", "value");
		}

		internal bool VerifyCertificate(X509Certificate2 cert)
		{
			bool result;
			try
			{
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
				x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid;
				bool flag = x509Chain.Build(cert);
				X509ChainStatusFlags x509ChainStatusFlags = X509ChainStatusFlags.NoError;
				foreach (X509ChainStatus x509ChainStatus in x509Chain.ChainStatus)
				{
					x509ChainStatusFlags |= x509ChainStatus.Status;
				}
				E4eLog.Instance.LogInfo(string.Empty, "Certificate {0} valid. Validation result is: {1}", new object[]
				{
					flag ? "is" : "is NOT",
					x509ChainStatusFlags.ToString()
				});
				result = flag;
			}
			catch (ArgumentException ex)
			{
				E4eLog.Instance.LogError(string.Empty, "Error when verify the certificate: {0}", new object[]
				{
					ex.ToString()
				});
				result = false;
			}
			catch (CryptographicException ex2)
			{
				E4eLog.Instance.LogError(string.Empty, "Error when verify the certificate: {0}", new object[]
				{
					ex2.ToString()
				});
				result = false;
			}
			return result;
		}

		internal bool VerifySignature(string text, string signature, string exportedCertificate)
		{
			if (string.IsNullOrEmpty(signature))
			{
				throw new E4eException("Signature is null or empty.");
			}
			if (string.IsNullOrEmpty(exportedCertificate))
			{
				throw new E4eException("Exported certificate is null or empty.");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new E4eException("Text to sign is null or empty");
			}
			byte[] rawData = Convert.FromBase64String(exportedCertificate);
			byte[] signatureByteArray = Convert.FromBase64String(signature);
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(rawData);
			if (!string.Equals(x509Certificate.Subject, E4eHelper.GetCertificateName(), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (this.VerifyCertificate(x509Certificate))
			{
				RSACryptoServiceProvider csp = (RSACryptoServiceProvider)x509Certificate.PublicKey.Key;
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				byte[] bytes = unicodeEncoding.GetBytes(text);
				return this.VerifySignature(signatureByteArray, csp, bytes);
			}
			return false;
		}

		internal RmsDecryptor CreateRmsDecryptor(OrganizationId originalSenderOrgId, string messageId, string recipient, Breadcrumbs<string> breadcrumbs, EmailMessage rmsMessage, OutboundConversionOptions outboundConversionOptions, HttpContext httpContext, IE4eLogger logger, out string publishingLicense, bool copyHeaders = true)
		{
			if (rmsMessage.Attachments == null || rmsMessage.Attachments.Count != 1 || !string.Equals(rmsMessage.Attachments[0].FileName, "message.rpmsg", StringComparison.OrdinalIgnoreCase))
			{
				throw new E4eException(string.Format("Attachments is null or AttachmentCount not equal 1 or Attachment name not equal: {0}", "message.rpmsg"));
			}
			RmsDecryptor result;
			using (Stream contentReadStream = rmsMessage.Attachments[0].GetContentReadStream())
			{
				DrmEmailMessageContainer drmEmailMessageContainer = new DrmEmailMessageContainer();
				drmEmailMessageContainer.Load(contentReadStream, (object param0) => Streams.CreateTemporaryStorageStream());
				publishingLicense = drmEmailMessageContainer.PublishLicense;
				RmsClientManagerContext context = new RmsClientManagerContext(originalSenderOrgId, RmsClientManagerContext.ContextId.MessageId, messageId, null);
				IList<RightsManagementException> list = new List<RightsManagementException>();
				UseLicenseAndUsageRights useLicenseAndUsageRights;
				RightsManagementException ex;
				if (!this.AcquireUseLicenseAndUsageRights(context, publishingLicense, recipient, messageId, out useLicenseAndUsageRights, out ex))
				{
					list.Add(ex);
					logger.LogInfo(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "Unable to AcquireUseLicenseAndUsageRights using recipient: {0}", new object[]
					{
						recipient
					});
					ProxyAddressCollection proxyAddresses = this.GetProxyAddresses(recipient, messageId);
					if (proxyAddresses.Count > 50)
					{
						logger.LogError(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "proxyAddressCollection.Count({0}) > E4eDecryptionHelper.MaxProxyAddressToTry({1})", new object[]
						{
							proxyAddresses.Count,
							50
						});
					}
					bool flag = false;
					int num = Math.Min(proxyAddresses.Count, 50);
					for (int i = 0; i < num; i++)
					{
						string addressString = proxyAddresses[i].AddressString;
						if (!recipient.Equals(addressString, StringComparison.OrdinalIgnoreCase))
						{
							if (this.AcquireUseLicenseAndUsageRights(context, publishingLicense, addressString, messageId, out useLicenseAndUsageRights, out ex))
							{
								flag = true;
								logger.LogInfo(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "Successfully AcquireUseLicenseAndUsageRights using proxy address: {0}", new object[]
								{
									addressString
								});
								break;
							}
							list.Add(ex);
							logger.LogInfo(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "Unable to AcquireUseLicenseAndUsageRights using proxy address: {0}", new object[]
							{
								addressString
							});
						}
					}
					if (!flag)
					{
						AggregateException ex2 = new AggregateException(list);
						logger.LogError(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "Unable to AcquireUseLicenseAndUsageRights. AggregateException: {0}", new object[]
						{
							ex2
						});
						throw ex;
					}
				}
				else
				{
					logger.LogInfo(httpContext, "E4eDecryptionHelper::CreateRmsDecryptor", messageId, "Successfully AcquireUseLicenseAndUsageRights using recipient: {0}", new object[]
					{
						recipient
					});
				}
				RmsDecryptor rmsDecryptor = new RmsDecryptor(context, rmsMessage, drmEmailMessageContainer, recipient, useLicenseAndUsageRights.UseLicense, outboundConversionOptions, breadcrumbs, false, false, copyHeaders, true);
				result = rmsDecryptor;
			}
			return result;
		}

		internal void DecryptMessage(RmsDecryptor rmsDecryptor, AsyncCallback asyncCallback, object state)
		{
			rmsDecryptor.BeginDecrypt(asyncCallback, state);
		}

		internal bool VerifyEncryptedAttachment(ref string htmlString, OrganizationId recipientOrgId, EnvelopeRecipientCollection recipients, OutboundConversionOptions outboundOptions, string messageId, string recipientAddressForDecryption, out string originalSender, out OrganizationId originalSenderOrgId, out string messageIdInMetadata, out string recipientInMetadata, out EmailMessage rmsMessage)
		{
			originalSender = null;
			originalSenderOrgId = null;
			messageIdInMetadata = null;
			recipientInMetadata = null;
			rmsMessage = null;
			string valueOf = E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "metadata", "value");
			string valueOf2 = E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "signature", "value");
			string valueOf3 = E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "certificate", "value");
			if (!this.VerifySignature(valueOf, valueOf2, valueOf3))
			{
				E4eLog.Instance.LogError(messageId, "Decryption will be skipped. Signature verification failed.", new object[0]);
				return false;
			}
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData.Length: {0}", new object[]
			{
				valueOf.Length
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].signature.Length: {0}", new object[]
			{
				valueOf2.Length
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].exportedCertificate.Length: {0}", new object[]
			{
				valueOf3.Length
			});
			string valueOf4 = E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "version", "value");
			E4eLog.Instance.LogInfo(messageId, "[D][form].Version: {0}", new object[]
			{
				string.IsNullOrWhiteSpace(valueOf4) ? "<blank>" : valueOf4
			});
			string[] array = valueOf.Split(new char[]
			{
				'|'
			});
			if (!this.VerifyMetadata(array, messageId))
			{
				return false;
			}
			originalSender = Encoding.UTF8.GetString(Convert.FromBase64String(array[0]));
			originalSenderOrgId = E4eHelper.FromBase64String(array[2]);
			recipientInMetadata = Encoding.UTF8.GetString(Convert.FromBase64String(array[1]));
			messageIdInMetadata = Encoding.UTF8.GetString(Convert.FromBase64String(array[4]));
			string text = array[2];
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(array[3]));
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData[OriginalSender]: {0}", new object[]
			{
				originalSender
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData[Recipient]: {0}", new object[]
			{
				recipientInMetadata
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData[OriginalSenderOrgId]: {0}", new object[]
			{
				text
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData[SentTime]: {0}", new object[]
			{
				@string
			});
			E4eLog.Instance.LogInfo(messageId, "[D][form].metaData[MessageId]: {0}", new object[]
			{
				messageId
			});
			if (!this.VerifyRecipient(recipients, recipientAddressForDecryption, recipientInMetadata, messageId))
			{
				return false;
			}
			string valueOf5 = E4eDecryptionHelper.GetValueOf(htmlString, "input", "name", "rpmsg", "value");
			if (string.IsNullOrEmpty(valueOf5))
			{
				E4eLog.Instance.LogError(messageId, "Decryption will be skipped. RpmsgMsg is null or empty.", new object[0]);
				return false;
			}
			E4eLog.Instance.LogInfo(messageId, "[D][form].rpmsgString.Length: {0}", new object[]
			{
				valueOf5.Length
			});
			rmsMessage = this.StringToEmailMessage(valueOf5, recipientOrgId, outboundOptions, true);
			if (rmsMessage == null || rmsMessage.Attachments == null || rmsMessage.Attachments.Count != 1 || !string.Equals(rmsMessage.Attachments[0].FileName, "message.rpmsg", StringComparison.OrdinalIgnoreCase))
			{
				E4eLog.Instance.LogError(messageId, "Decryption will be skipped. RmsMessage is null or it doesn't contain the message.rpmsg attachment.", new object[0]);
				return false;
			}
			return true;
		}

		internal EmailMessage StringToEmailMessage(string message, OrganizationId orgId, OutboundConversionOptions outboundOptions, bool needHtmlDecode = true)
		{
			string s = needHtmlDecode ? HttpUtility.HtmlDecode(message) : message;
			InboundConversionOptions inboundConversionOptions = E4eHelper.GetInboundConversionOptions(orgId);
			EmailMessage result;
			using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
			{
				using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
				{
					ItemConversion.ConvertAnyMimeToItem(messageItem, stream, inboundConversionOptions);
					result = Utils.ConvertMessageItemToEmailMessage(messageItem, outboundOptions, true);
				}
			}
			return result;
		}

		internal bool AcquireUseLicenseAndUsageRights(RmsClientManagerContext context, string publishingLicense, string recipient, string messageId, out UseLicenseAndUsageRights useLicenseAndUsageRights, out RightsManagementException exception)
		{
			bool result;
			try
			{
				useLicenseAndUsageRights = RmsClientManager.AcquireUseLicenseAndUsageRights(context, publishingLicense, recipient, new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), RecipientTypeDetails.User);
				exception = null;
				result = true;
			}
			catch (RightsManagementException ex)
			{
				useLicenseAndUsageRights = null;
				exception = ex;
				result = false;
			}
			return result;
		}

		internal ProxyAddressCollection GetProxyAddresses(string recipientEmailAddress, string messageId)
		{
			ProxyAddressCollection proxyAddressCollection = ProxyAddressCollection.Empty;
			if (E4eDecryptionHelper.proxyAddressCollectionCache.TryGetValue(recipientEmailAddress, out proxyAddressCollection))
			{
				return proxyAddressCollection;
			}
			ProxyAddressCollection result;
			try
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(recipientEmailAddress);
				SmtpAddress recipientSmtpAddress = new SmtpAddress(proxyAddress.AddressString);
				IRecipientSession recipientSession = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(recipientSmtpAddress.Domain), 632, "GetProxyAddresses", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\RightsManagement\\E4eDecryptionHelper.cs");
				});
				if (!adoperationResult.Succeeded)
				{
					E4eLog.Instance.LogInfo(messageId, "Result.Succeeded is false when getting recipient session for recipient: {0}. Exception: {1}", new object[]
					{
						recipientEmailAddress,
						(adoperationResult.Exception != null) ? adoperationResult.Exception.ToString() : "[No Exception Recorded]"
					});
					result = ProxyAddressCollection.Empty;
				}
				else if (recipientSession == null)
				{
					E4eLog.Instance.LogInfo(messageId, "RecipientSession is null while getting proxy addresses for recipient: {0}.", new object[]
					{
						recipientEmailAddress
					});
					result = ProxyAddressCollection.Empty;
				}
				else
				{
					MiniRecipient miniRecipient = recipientSession.FindByProxyAddress<MiniRecipient>(proxyAddress);
					if (miniRecipient == null)
					{
						E4eLog.Instance.LogInfo(messageId, "MiniRecipient is null while getting proxy addresses for recipient: {0}.", new object[]
						{
							recipientEmailAddress
						});
						result = ProxyAddressCollection.Empty;
					}
					else if (miniRecipient.EmailAddresses == null)
					{
						E4eLog.Instance.LogInfo(messageId, "MiniRecipient.EmailAddresses is null while getting proxy addresses for recipient: {0}.", new object[]
						{
							recipientEmailAddress
						});
						result = ProxyAddressCollection.Empty;
					}
					else
					{
						proxyAddressCollection = miniRecipient.EmailAddresses;
						E4eDecryptionHelper.proxyAddressCollectionCache.Add(recipientEmailAddress, proxyAddressCollection);
						result = proxyAddressCollection;
					}
				}
			}
			catch (Exception ex)
			{
				E4eLog.Instance.LogError(messageId, "Exception thrown while getting proxy addresses for recipient: {0}. Exception: {1}", new object[]
				{
					recipientEmailAddress,
					ex.ToString()
				});
				result = ProxyAddressCollection.Empty;
			}
			return result;
		}

		internal bool IsE4eCrossTenantDecryptionEnabled()
		{
			if (string.IsNullOrWhiteSpace(E4eDecryptionHelper.cachedEnableE4eCrossTenantDecryption))
			{
				object obj = E4eHelper.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "IsE4eCrossTenantDecryptionEnabled");
				if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
				{
					E4eDecryptionHelper.cachedEnableE4eCrossTenantDecryption = "false";
				}
				else
				{
					E4eDecryptionHelper.cachedEnableE4eCrossTenantDecryption = obj.ToString();
				}
			}
			return E4eDecryptionHelper.cachedEnableE4eCrossTenantDecryption.Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		private bool VerifyMetadata(string[] metadataArray, string messageId)
		{
			if (metadataArray.Length != 5)
			{
				E4eLog.Instance.LogError(messageId, "Decryption will be skipped. Metadata length mismatch.", new object[0]);
				return false;
			}
			for (int i = 0; i < 5; i++)
			{
				if (string.IsNullOrEmpty(metadataArray[i]))
				{
					E4eLog.Instance.LogError(messageId, "Decryption will be skipped. Metadata index: {0} is null or empty.", new object[]
					{
						i
					});
					return false;
				}
			}
			return true;
		}

		private bool VerifyRecipient(EnvelopeRecipientCollection recipients, string recipientAddressForDecryption, string recipientInMetadata, string messageId)
		{
			if (string.IsNullOrWhiteSpace(recipientAddressForDecryption))
			{
				if (!recipients[0].Address.ToString().Equals(recipientInMetadata, StringComparison.OrdinalIgnoreCase))
				{
					bool flag = false;
					ProxyAddressCollection proxyAddresses = this.GetProxyAddresses(recipients[0].Address.ToString(), messageId);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ProxyAddress proxyAddress in proxyAddresses)
					{
						if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
						{
							stringBuilder.AppendFormat("{0},", proxyAddress.AddressString);
							if (proxyAddress.AddressString.Equals(recipientInMetadata, StringComparison.OrdinalIgnoreCase))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						E4eLog.Instance.LogError(messageId, "Decryption will be skipped. Recipient in MailItem: '{0}' nor it's proxy addresses: '{1}' matches the recipient in metadata: '{2}'.", new object[]
						{
							recipients[0].Address.ToString(),
							stringBuilder.ToString(),
							recipientInMetadata
						});
						return false;
					}
				}
			}
			else if (!recipientAddressForDecryption.Equals(recipientInMetadata, StringComparison.OrdinalIgnoreCase))
			{
				E4eLog.Instance.LogError(messageId, "Decryption will be skipped. recipientAddressForDecryption: {0} doesn't match recipient in metadata: {1}.", new object[]
				{
					recipientAddressForDecryption,
					recipientInMetadata
				});
				return false;
			}
			return true;
		}

		protected virtual bool VerifySignature(byte[] signatureByteArray, RSACryptoServiceProvider csp, byte[] data)
		{
			bool result;
			using (SHA1Cng sha1Cng = new SHA1Cng())
			{
				byte[] rgbHash = sha1Cng.ComputeHash(data);
				result = csp.VerifyHash(rgbHash, CryptoConfig.MapNameToOID("SHA1"), signatureByteArray);
			}
			return result;
		}

		internal const string DeferralCountPropertyName = "Microsoft.Exchange.E4eDecryptionAgent.DeferralCount";

		private const int MaxProxyAddressToTry = 50;

		private static MruDictionaryCache<string, ProxyAddressCollection> proxyAddressCollectionCache = new MruDictionaryCache<string, ProxyAddressCollection>(1000, 15);

		private static string cachedEnableE4eCrossTenantDecryption = string.Empty;

		private static E4eDecryptionHelper instance;
	}
}

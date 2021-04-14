using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.SMime)]
	[OwaEventNamespace("SMime")]
	internal sealed class SMimeEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(SMimeEventHandler));
		}

		[OwaEvent("GetCerts")]
		[OwaEventParameter("addrs", typeof(RecipientInfo), true)]
		public void GetCerts()
		{
			this.getCertificateResponseWriter = new SMimeEventHandler.GetCertificateListResponseWriter(base.UserContext, this.Writer);
			this.GetCertificates();
		}

		[OwaEventParameter("addrs", typeof(RecipientInfo), true)]
		[OwaEvent("GetCertsInfo")]
		public void GetCertsInfo()
		{
			this.getCertificateResponseWriter = new SMimeEventHandler.GetCertificateInformationResponseWriter(base.UserContext, this.Writer);
			this.GetCertificates();
		}

		[OwaEventParameter("IsVCard", typeof(int))]
		[OwaEvent("UploadEmbeddedItem")]
		[OwaEventParameter("MimeBlob", typeof(string))]
		[OwaEventParameter("VCardCharset", typeof(string), false, true)]
		public void UploadEmbeddedItem()
		{
			bool flag = (int)base.GetParameter("IsVCard") == 1;
			string text = (string)base.GetParameter("VCardCharset");
			Encoding encoding = null;
			if (flag)
			{
				if (string.IsNullOrEmpty(text))
				{
					throw new OwaInvalidRequestException("VCardCharset must be set when IsVCard parameter is true.");
				}
				try
				{
					encoding = Encoding.GetEncoding(text);
				}
				catch (ArgumentException)
				{
					throw new OwaInvalidRequestException("VCardCharset is invalid.");
				}
			}
			using (MemoryStream memoryStream = new MemoryStream(Encoding.GetEncoding("windows-1252").GetBytes((string)base.GetParameter("MimeBlob"))))
			{
				using (Item item = this.CreateItemFromMime(memoryStream, base.UserContext.GetDeletedItemsFolderId(base.UserContext.MailboxSession).StoreObjectId, flag, encoding))
				{
					string className = item.ClassName;
					StoreObjectId objectId = item.Id.ObjectId;
					bool flag2 = false;
					if (ObjectClass.IsTask(className))
					{
						item.Load(new PropertyDefinition[]
						{
							TaskSchema.TaskType
						});
						flag2 = TaskUtilities.IsAssignedTaskType(TaskUtilities.GetTaskType(item));
					}
					if (ObjectClass.IsContact(className) || ObjectClass.IsDistributionList(className) || (ObjectClass.IsTask(className) && !flag2))
					{
						this.Writer.Write("?ae=PreFormAction&a=OpenEmbedded&t=");
					}
					else
					{
						this.Writer.Write("?ae=Item&t=");
					}
					this.Writer.Write(Utilities.UrlEncode(className));
					this.Writer.Write("&id=");
					this.Writer.Write(Utilities.UrlEncode(objectId.ToBase64String()));
					this.Writer.Write("&smemb=1&exdltdrft=1");
				}
			}
		}

		private static string GetIdFromCertificate(X509Certificate2 certificate)
		{
			string emailAdress = X509PartialCertificate.GetEmailAdress(certificate);
			if (!string.IsNullOrEmpty(emailAdress))
			{
				return emailAdress;
			}
			string senderCertificateAttributesToDisplay = OwaRegistryKeys.SenderCertificateAttributesToDisplay;
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
			string[] array = OwaRegistryKeys.SenderCertificateAttributesToDisplay.Split(SMimeEventHandler.comma, StringSplitOptions.RemoveEmptyEntries);
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

		private static string GetIssuerDisplayNameFromCertificate(X509Certificate2 certificate)
		{
			IList<KeyValuePair<Oid, string>> list = X500DistinguishedNameDecoder.Decode(certificate.IssuerName);
			if (list == null || list.Count == 0)
			{
				return string.Empty;
			}
			foreach (KeyValuePair<Oid, string> keyValuePair in list)
			{
				if (string.Equals(keyValuePair.Key.Value, SMimeEventHandler.commonNameOid.Value, StringComparison.OrdinalIgnoreCase))
				{
					return keyValuePair.Value;
				}
			}
			return string.Empty;
		}

		private static void SafeAddADRecipient(HashSet<Participant> list, Participant participant)
		{
			bool flag = false;
			foreach (Participant participant2 in list)
			{
				if (string.Equals(participant.EmailAddress, participant2.EmailAddress, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.TryAdd(participant);
			}
		}

		private Item CreateItemFromMime(MemoryStream mimeStream, StoreObjectId folderId, bool createContactFromVcard, Encoding encoding)
		{
			Item item;
			if (createContactFromVcard)
			{
				item = Contact.Create(base.UserContext.MailboxSession, folderId);
			}
			else
			{
				item = MessageItem.Create(base.UserContext.MailboxSession, folderId);
			}
			item[MessageItemSchema.IsDraft] = false;
			item.Load(new PropertyDefinition[]
			{
				MessageItemSchema.ExpiryTime
			});
			InboundConversionOptions options = Utilities.CreateInboundConversionOptions(base.UserContext);
			if (createContactFromVcard)
			{
				Contact.ImportVCard(item as Contact, mimeStream, encoding, options);
			}
			else
			{
				ItemConversion.ConvertAnyMimeToItem(item, mimeStream, options);
			}
			Utilities.SaveItem(item, false);
			return item;
		}

		private void GetCertificates()
		{
			this.expansionTimeoutTime = new ExDateTime?(ExDateTime.UtcNow.AddMilliseconds((double)OwaRegistryKeys.DLExpansionTimeout));
			try
			{
				this.GetRecipientsCerts();
			}
			catch (SMimeEventHandler.SMimeEventHandlerException e)
			{
				this.getCertificateResponseWriter.RenderError(e);
				return;
			}
			this.RenderPdlCertMissingInfomation();
			this.getCertificateResponseWriter.RenderCertificates(this.certificateDictionary);
		}

		private void RenderPdlCertMissingInfomation()
		{
			if (this.pdlExpansionResults.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<Participant, HashSet<Participant>> keyValuePair in this.pdlExpansionResults)
			{
				int num = 0;
				int num2 = 0;
				foreach (Participant participant in keyValuePair.Value)
				{
					SMimeEventHandler.CertificatsInformation certificatsInformation;
					if (this.certificateDictionary.TryGetValue(participant, out certificatsInformation) || (!(participant.Origin is StoreParticipantOrigin) && string.Equals(participant.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) && this.FindCertificateByRoutingAddress(participant.EmailAddress, out certificatsInformation)))
					{
						num += certificatsInformation.Total;
						num2 += certificatsInformation.Valid;
					}
					else
					{
						num++;
					}
				}
				if (num != num2)
				{
					this.getCertificateResponseWriter.RenderMissing(keyValuePair.Key.DisplayName, "IPM.DistList", ((StoreParticipantOrigin)keyValuePair.Key.Origin).OriginItemId.ToBase64String(), null, num, num2);
				}
			}
		}

		private void GetRecipientsCerts()
		{
			this.allRecipients = this.GroupRecipients();
			HashSet<Participant> hashSet = null;
			HashSet<Participant> hashSet2 = null;
			if (this.allRecipients[SMimeEventHandler.RecipientGroup.PDLs].Count > 0)
			{
				this.ExpandPdlIntoMembers(ref hashSet, ref hashSet2);
			}
			if (hashSet2 != null || this.allRecipients[SMimeEventHandler.RecipientGroup.Contacts].Count > 0)
			{
				this.GetStoreCerts(hashSet2 ?? this.allRecipients[SMimeEventHandler.RecipientGroup.Contacts]);
			}
			if (hashSet != null || this.allRecipients[SMimeEventHandler.RecipientGroup.AdRecipients].Count > 0)
			{
				this.GetDirectoryCerts(hashSet ?? this.allRecipients[SMimeEventHandler.RecipientGroup.AdRecipients]);
			}
			if (this.allRecipients[SMimeEventHandler.RecipientGroup.Others].Count > 0)
			{
				foreach (Participant participant in this.allRecipients[SMimeEventHandler.RecipientGroup.Others])
				{
					this.getCertificateResponseWriter.RenderMissing(participant.DisplayName);
				}
			}
		}

		private void ExpandPdlIntoMembers(ref HashSet<Participant> adRecipientsAfterPdlExpansion, ref HashSet<Participant> contactsAfterPdlExpansion)
		{
			this.Writer.Write("<div id=pdl>");
			foreach (Participant participant in this.allRecipients[SMimeEventHandler.RecipientGroup.PDLs])
			{
				StoreObjectId originItemId = (participant.Origin as StoreParticipantOrigin).OriginItemId;
				Participant[] array = DistributionList.ExpandDeep(base.UserContext.MailboxSession, originItemId, false);
				if (array != null && array.Length != 0)
				{
					HashSet<Participant> hashSet = new HashSet<Participant>(array.Length);
					foreach (Participant item in array)
					{
						hashSet.TryAdd(item);
					}
					this.pdlExpansionResults.Add(participant, hashSet);
					this.Writer.Write("<div _id=\"");
					Utilities.HtmlEncode(originItemId.ToBase64String(), this.Writer);
					this.Writer.Write("\">");
					foreach (Participant participant2 in hashSet)
					{
						string text = participant2.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
						if (string.IsNullOrEmpty(text))
						{
							text = ImceaAddress.Encode(participant2.RoutingType, participant2.EmailAddress, OwaConfigurationManager.Configuration.DefaultAcceptedDomain.DomainName.ToString());
						}
						this.Writer.Write("<p _sa=\"");
						Utilities.HtmlEncode(text, this.Writer);
						this.Writer.Write("\" _dn=\"");
						Utilities.HtmlEncode(participant2.DisplayName, this.Writer);
						StoreParticipantOrigin storeParticipantOrigin = participant2.Origin as StoreParticipantOrigin;
						if (storeParticipantOrigin != null)
						{
							if (contactsAfterPdlExpansion == null)
							{
								contactsAfterPdlExpansion = new HashSet<Participant>(this.allRecipients[SMimeEventHandler.RecipientGroup.Contacts]);
							}
							contactsAfterPdlExpansion.TryAdd(participant2);
							this.Writer.Write("\" _id=\"");
							Utilities.HtmlEncode(storeParticipantOrigin.OriginItemId.ToBase64String(), this.Writer);
						}
						else if (string.Equals(participant2.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
						{
							if (adRecipientsAfterPdlExpansion == null)
							{
								adRecipientsAfterPdlExpansion = new HashSet<Participant>(this.allRecipients[SMimeEventHandler.RecipientGroup.AdRecipients]);
							}
							SMimeEventHandler.SafeAddADRecipient(adRecipientsAfterPdlExpansion, participant2);
							this.Writer.Write("\" _em=\"");
							Utilities.HtmlEncode(participant2.EmailAddress, this.Writer);
						}
						this.Writer.Write("\"></p>");
					}
					this.Writer.Write("</div>");
				}
			}
			this.Writer.Write("</div>");
		}

		private Dictionary<SMimeEventHandler.RecipientGroup, HashSet<Participant>> GroupRecipients()
		{
			RecipientInfo[] array = (RecipientInfo[])base.GetParameter("addrs");
			Dictionary<SMimeEventHandler.RecipientGroup, HashSet<Participant>> dictionary = new Dictionary<SMimeEventHandler.RecipientGroup, HashSet<Participant>>(4);
			SMimeEventHandler.RecipientGroup[] array2 = new SMimeEventHandler.RecipientGroup[]
			{
				SMimeEventHandler.RecipientGroup.PDLs,
				SMimeEventHandler.RecipientGroup.Contacts,
				SMimeEventHandler.RecipientGroup.AdRecipients,
				SMimeEventHandler.RecipientGroup.Others
			};
			foreach (SMimeEventHandler.RecipientGroup key in array2)
			{
				dictionary[key] = new HashSet<Participant>();
			}
			for (int j = 0; j < array.Length; j++)
			{
				bool flag = false;
				for (int k = 0; k < j; k++)
				{
					if (array[k] == array[j] || (array[k].AddressOrigin == AddressOrigin.Directory && array[j].AddressOrigin == AddressOrigin.Directory && string.Equals(array[k].RoutingAddress, array[j].RoutingAddress, StringComparison.OrdinalIgnoreCase)))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Participant participant;
					array[j].ToParticipant(out participant);
					if (Utilities.IsMapiPDL(participant.RoutingType) && participant.Origin is StoreParticipantOrigin)
					{
						dictionary[SMimeEventHandler.RecipientGroup.PDLs].TryAdd(participant);
					}
					else if (participant.Origin is StoreParticipantOrigin)
					{
						dictionary[SMimeEventHandler.RecipientGroup.Contacts].TryAdd(participant);
					}
					else if (participant.Origin is DirectoryParticipantOrigin)
					{
						dictionary[SMimeEventHandler.RecipientGroup.AdRecipients].TryAdd(participant.ChangeOrigin(new OneOffParticipantOrigin()));
					}
					else
					{
						dictionary[SMimeEventHandler.RecipientGroup.Others].TryAdd(participant);
					}
				}
			}
			return dictionary;
		}

		private void AddCerts(Participant participant, List<X509Certificate2> certificates, int total, int valid)
		{
			this.certificateDictionary[participant] = new SMimeEventHandler.CertificatsInformation(certificates, total, valid);
		}

		private bool AddCert(Participant participant, X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				return false;
			}
			this.AddCerts(participant, new List<X509Certificate2>(1)
			{
				certificate
			}, 1, 1);
			return true;
		}

		private X509Certificate2 GetContactCertificate(StoreObjectId id)
		{
			try
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, OwaStoreObjectId.CreateFromStoreObjectId(id, null), new PropertyDefinition[]
				{
					ContactSchema.UserX509Certificates
				}))
				{
					return Utilities.FindBestCertificate(ItemUtility.GetProperty<byte[][]>(item, ContactSchema.UserX509Certificates, null), null, true, false);
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			return null;
		}

		private void GetStoreCerts(HashSet<Participant> contacts)
		{
			foreach (Participant participant in contacts)
			{
				if (!this.AddCert(participant, this.GetContactCertificate(((StoreParticipantOrigin)participant.Origin).OriginItemId)) && this.allRecipients[SMimeEventHandler.RecipientGroup.Contacts].Contains(participant))
				{
					this.getCertificateResponseWriter.RenderMissing(ThemeFileId.Contact, participant.DisplayName, "IPM.Contact", ((StoreParticipantOrigin)participant.Origin).OriginItemId.ToBase64String());
				}
			}
		}

		private void GetDirectoryCerts(HashSet<Participant> recipients)
		{
			HashSet<Participant> hashSet = new HashSet<Participant>(recipients.Count);
			List<string> list = new List<string>(recipients.Count);
			foreach (Participant participant in recipients)
			{
				list.Add(participant.EmailAddress);
			}
			Result<ADRawEntry>[] array = this.GetADRecipientSession().FindByLegacyExchangeDNs(list.ToArray(), SMimeEventHandler.adRecipientProperties);
			foreach (Result<ADRawEntry> result in array)
			{
				if (result.Data != null)
				{
					string text = result.Data[ADRecipientSchema.LegacyExchangeDN] as string;
					string legacyExchangeDN = base.UserContext.MailboxIdentity.GetOWAMiniRecipient().LegacyExchangeDN;
					bool checkRevocation = OwaRegistryKeys.CheckCRLOnSend && string.Equals(text, legacyExchangeDN, StringComparison.OrdinalIgnoreCase);
					foreach (Participant participant2 in recipients)
					{
						if (string.Equals(participant2.EmailAddress, text, StringComparison.OrdinalIgnoreCase))
						{
							hashSet.TryAdd(participant2);
							if (Utilities.IsADDistributionList((RecipientType)result.Data[ADRecipientSchema.RecipientType]))
							{
								if (this.adRecipientExpansion == null)
								{
									this.adRecipientExpansion = new ADRecipientExpansion(SMimeEventHandler.adRecipientProperties);
								}
								SMimeEventHandler.ADDistributionListExpansion addistributionListExpansion = new SMimeEventHandler.ADDistributionListExpansion(this.adRecipientExpansion, result.Data, this.expansionTimeoutTime);
								this.AddCerts(participant2, addistributionListExpansion.Certificats, addistributionListExpansion.Size, addistributionListExpansion.Certificats.Count);
								if (addistributionListExpansion.Size > addistributionListExpansion.Certificats.Count && this.allRecipients[SMimeEventHandler.RecipientGroup.AdRecipients].Contains(participant2))
								{
									this.getCertificateResponseWriter.RenderMissing(participant2.DisplayName, "ADDistList", Utilities.GetBase64StringFromADObjectId(result.Data[ADObjectSchema.Id] as ADObjectId), participant2.EmailAddress, addistributionListExpansion.Size, addistributionListExpansion.Certificats.Count);
								}
							}
							else if (!this.AddCert(participant2, Utilities.GetADRecipientCertificate(result.Data, checkRevocation)) && this.allRecipients[SMimeEventHandler.RecipientGroup.AdRecipients].Contains(participant2))
							{
								this.getCertificateResponseWriter.RenderMissing(ThemeFileId.DistributionListUser, participant2.DisplayName, "AD.RecipientType.User", Utilities.GetBase64StringFromADObjectId(result.Data[ADObjectSchema.Id] as ADObjectId));
							}
						}
					}
				}
			}
			if (hashSet.Count < recipients.Count)
			{
				foreach (Participant participant3 in recipients)
				{
					if (!hashSet.Contains(participant3))
					{
						this.getCertificateResponseWriter.RenderMissing(participant3.DisplayName);
					}
				}
			}
		}

		private IRecipientSession GetADRecipientSession()
		{
			if (this.adRecipientSession == null)
			{
				this.adRecipientSession = Utilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.FullyConsistent, true, base.UserContext);
			}
			return this.adRecipientSession;
		}

		[OwaEventParameter("isSend", typeof(bool))]
		[OwaEvent("ValidateCerts")]
		[OwaEventParameter("certs", typeof(string), true)]
		[OwaEventParameter("chains", typeof(string), true, true)]
		public void ValidateCerts()
		{
			string[] array = (string[])base.GetParameter("certs");
			string[] array2 = (string[])base.GetParameter("chains");
			bool flag = (bool)base.GetParameter("isSend");
			X509Store trustedStore = null;
			if (array2 != null)
			{
				trustedStore = this.CreateStore(array2);
			}
			bool flag2 = true;
			string text = null;
			this.Writer.Write("<p id=valRes>");
			foreach (string s in array)
			{
				ChainContext chainContext = null;
				try
				{
					X509Certificate2 x509Certificate = new X509Certificate2(Convert.FromBase64String(s));
					X509KeyUsageFlags expectedUsage;
					if (!flag || flag2)
					{
						expectedUsage = (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature);
					}
					else
					{
						expectedUsage = X509KeyUsageFlags.KeyEncipherment;
					}
					ChainValidityStatus value = X509CertificateCollection.ValidateCertificate(x509Certificate, null, expectedUsage, flag ? OwaRegistryKeys.CheckCRLOnSend : (!OwaRegistryKeys.DisableCRLCheck), trustedStore, null, TimeSpan.FromMilliseconds((double)OwaRegistryKeys.CRLConnectionTimeout), TimeSpan.FromMilliseconds((double)OwaRegistryKeys.CRLRetrievalTimeout), ref chainContext, false, null);
					if (flag)
					{
						this.Writer.Write((uint)value);
						this.Writer.Write(" ");
					}
					else
					{
						string idFromCertificate = SMimeEventHandler.GetIdFromCertificate(x509Certificate);
						if (idFromCertificate == null)
						{
							value = ChainValidityStatus.SubjectMismatch;
						}
						this.Writer.Write("<p id=res>");
						this.Writer.Write((uint)value);
						this.Writer.Write("</p>");
						this.Writer.Write("<p id=eml>");
						this.Writer.Write(Utilities.HtmlEncode(idFromCertificate));
						this.Writer.Write("</p>");
						string displayName = X509PartialCertificate.GetDisplayName(x509Certificate);
						this.Writer.Write("<p id=cn>");
						this.Writer.Write(Utilities.HtmlEncode(string.IsNullOrEmpty(displayName) ? idFromCertificate : displayName));
						this.Writer.Write("</p>");
						this.Writer.Write("<p id=issuer>");
						this.Writer.Write(Utilities.HtmlEncode(SMimeEventHandler.GetIssuerDisplayNameFromCertificate(x509Certificate)));
						this.Writer.Write("</p>");
					}
					if (flag && flag2 && chainContext != null && OwaRegistryKeys.IncludeCertificateChainWithoutRootCertificate)
					{
						text = SMimeEventHandler.EncodeCertChain(chainContext, OwaRegistryKeys.IncludeCertificateChainAndRootCertificate, x509Certificate);
					}
				}
				finally
				{
					if (chainContext != null)
					{
						chainContext.Dispose();
					}
					flag2 = false;
				}
			}
			this.Writer.Write("</p>");
			if (text != null)
			{
				this.Writer.Write("<p id=chain>");
				this.Writer.Write(text);
				this.Writer.Write("</p>");
			}
		}

		private X509Store CreateStore(string[] base64Certificates)
		{
			X509Store x509Store = CertificateStore.Open(StoreType.Memory, null, OpenFlags.ReadWrite);
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			foreach (string text in base64Certificates)
			{
				if (text != null)
				{
					X509Certificate2Collection x509Certificate2Collection2 = new X509Certificate2Collection();
					x509Certificate2Collection2.Import(Convert.FromBase64String(text));
					x509Certificate2Collection.AddRange(x509Certificate2Collection2);
				}
			}
			try
			{
				x509Store.AddRange(x509Certificate2Collection);
			}
			catch (SecurityException)
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Failed to add certificates to temporay memory store.");
			}
			return x509Store;
		}

		private bool FindCertificateByRoutingAddress(string routingAddress, out SMimeEventHandler.CertificatsInformation info)
		{
			foreach (KeyValuePair<Participant, SMimeEventHandler.CertificatsInformation> keyValuePair in this.certificateDictionary)
			{
				if (!(keyValuePair.Key.Origin is StoreParticipantOrigin) && string.Equals(keyValuePair.Key.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) && string.Equals(keyValuePair.Key.EmailAddress, routingAddress, StringComparison.OrdinalIgnoreCase))
				{
					info = keyValuePair.Value;
					return true;
				}
			}
			info = null;
			return false;
		}

		internal static string EncodeCertChain(ChainContext chainContext, bool includeRootCert, X509Certificate2 signersCertificate)
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

		internal static string EncodeCertificates(ICollection<X509Certificate2> certificates, bool useKeyIdentifier)
		{
			List<byte[]> list = new List<byte[]>(certificates.Count);
			uint num = 12U;
			uint count = (uint)certificates.Count;
			foreach (X509Certificate2 certificate in certificates)
			{
				byte[] array = X509PartialCertificate.Encode(certificate, !useKeyIdentifier);
				list.Add(array);
				num += (uint)array.Length;
			}
			byte[] array2 = new byte[num];
			int num2 = 0;
			ExBitConverter.Write(num, array2, num2);
			num2 += 4;
			ExBitConverter.Write(count, array2, num2);
			num2 += 4;
			ExBitConverter.Write(count, array2, num2);
			num2 += 4;
			foreach (byte[] array3 in list)
			{
				Array.Copy(array3, 0, array2, num2, array3.Length);
				num2 += array3.Length;
			}
			return Convert.ToBase64String(array2);
		}

		public const string EventNamespace = "SMime";

		public const string MethodGetCerts = "GetCerts";

		public const string MethodGetCertsInfo = "GetCertsInfo";

		public const string MethodValidateCerts = "ValidateCerts";

		public const string MethodUploadEmbeddedItem = "UploadEmbeddedItem";

		public const string MimeBlob = "MimeBlob";

		public const string IsVCard = "IsVCard";

		public const string VCardCharset = "VCardCharset";

		public const string AddressList = "addrs";

		public const string Base64EncodedCerts = "certs";

		public const string IsSend = "isSend";

		public const string Base64EncodedChains = "chains";

		private static PropertyDefinition[] adRecipientProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.LegacyExchangeDN,
			ADObjectSchema.Id,
			ADRecipientSchema.Certificate,
			ADRecipientSchema.SMimeCertificate,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.EmailAddresses,
			ADGroupSchema.HiddenGroupMembershipEnabled
		};

		private static char[] comma = new char[]
		{
			','
		};

		private static Oid commonNameOid = new Oid("2.5.4.3");

		private IRecipientSession adRecipientSession;

		private Dictionary<Participant, SMimeEventHandler.CertificatsInformation> certificateDictionary = new Dictionary<Participant, SMimeEventHandler.CertificatsInformation>();

		private Dictionary<Participant, HashSet<Participant>> pdlExpansionResults = new Dictionary<Participant, HashSet<Participant>>();

		private Dictionary<SMimeEventHandler.RecipientGroup, HashSet<Participant>> allRecipients;

		private ADRecipientExpansion adRecipientExpansion;

		private ExDateTime? expansionTimeoutTime;

		private SMimeEventHandler.GetCertificateResponseWriter getCertificateResponseWriter;

		private enum RecipientGroup
		{
			PDLs,
			Contacts,
			AdRecipients,
			Others
		}

		private class CertificatsInformation
		{
			public CertificatsInformation(List<X509Certificate2> certificates, int total, int valid)
			{
				this.Certificates = certificates;
				this.Total = total;
				this.Valid = valid;
			}

			public readonly List<X509Certificate2> Certificates;

			public readonly int Total;

			public readonly int Valid;
		}

		private class ADDistributionListExpansion
		{
			public ADDistributionListExpansion(ADRecipientExpansion adRecipientExpansion, ADRawEntry recipient, ExDateTime? timeoutTime)
			{
				this.timeoutTime = timeoutTime;
				if (Utilities.IsADDistributionList((RecipientType)recipient[ADRecipientSchema.RecipientType]) && (bool)recipient[ADGroupSchema.HiddenGroupMembershipEnabled])
				{
					throw new SMimeEventHandler.HiddenMembershipException();
				}
				adRecipientExpansion.Expand(recipient, new ADRecipientExpansion.HandleRecipientDelegate(this.OnRecipient), new ADRecipientExpansion.HandleFailureDelegate(this.OnFailure));
			}

			public int Size
			{
				get
				{
					return this.size;
				}
			}

			public List<X509Certificate2> Certificats
			{
				get
				{
					return this.certificats;
				}
			}

			private ExpansionControl OnRecipient(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				this.CheckTimeout();
				bool flag = Utilities.IsADDistributionList((RecipientType)recipient[ADRecipientSchema.RecipientType]);
				if (flag && (bool)recipient[ADGroupSchema.HiddenGroupMembershipEnabled])
				{
					throw new SMimeEventHandler.HiddenMembershipException();
				}
				if (!flag)
				{
					string text = recipient[ADRecipientSchema.LegacyExchangeDN] as string;
					if (text != null && !this.foundMembersDNs.Contains(text))
					{
						this.size++;
						X509Certificate2 adrecipientCertificate = Utilities.GetADRecipientCertificate(recipient, false);
						if (adrecipientCertificate != null)
						{
							this.certificats.Add(adrecipientCertificate);
						}
						this.foundMembersDNs.Add(text);
					}
				}
				return ExpansionControl.Continue;
			}

			private ExpansionControl OnFailure(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
			{
				this.CheckTimeout();
				return ExpansionControl.Continue;
			}

			private void CheckTimeout()
			{
				if (this.timeoutTime == null)
				{
					return;
				}
				if (this.timeoutTime.Value < ExDateTime.UtcNow)
				{
					throw new SMimeEventHandler.ExpansionTimeoutException();
				}
			}

			private List<X509Certificate2> certificats = new List<X509Certificate2>();

			private int size;

			private ExDateTime? timeoutTime;

			private HashSet<string> foundMembersDNs = new HashSet<string>();
		}

		private class SMimeEventHandlerException : Exception
		{
		}

		private class HiddenMembershipException : SMimeEventHandler.SMimeEventHandlerException
		{
		}

		private class ExpansionTimeoutException : SMimeEventHandler.SMimeEventHandlerException
		{
		}

		private abstract class GetCertificateResponseWriter
		{
			public GetCertificateResponseWriter(UserContext userContext, TextWriter writer)
			{
				this.userContext = userContext;
				this.writer = writer;
			}

			protected TextWriter Writer
			{
				get
				{
					return this.writer;
				}
			}

			protected UserContext UserContext
			{
				get
				{
					return this.userContext;
				}
			}

			public abstract void RenderMissing(ThemeFileId icon, string name, string type, string id);

			public abstract void RenderMissing(string name);

			public abstract void RenderMissing(string name, string type, string id, string legacyDN, int total, int valid);

			public abstract void RenderCertificates(Dictionary<Participant, SMimeEventHandler.CertificatsInformation> certificateDictionary);

			public abstract void RenderError(SMimeEventHandler.SMimeEventHandlerException e);

			protected string GetDLMissingMessage(int total, int valid)
			{
				if (valid == 0)
				{
					return LocalizedStrings.GetNonEncoded(-969194198);
				}
				return string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(-669360998), new object[]
				{
					total - valid,
					total
				});
			}

			private readonly TextWriter writer;

			private readonly UserContext userContext;
		}

		private sealed class GetCertificateInformationResponseWriter : SMimeEventHandler.GetCertificateResponseWriter
		{
			public GetCertificateInformationResponseWriter(UserContext userContext, TextWriter writer) : base(userContext, writer)
			{
			}

			private void RenderMissingInformationStart()
			{
				this.missingInformationStarted = true;
				base.Writer.Write("<div id=mis>");
			}

			private void RenderMissingInformationEnd()
			{
				this.missingInformationStarted = false;
				base.Writer.Write("</div>");
			}

			public override void RenderMissing(ThemeFileId icon, string name, string type, string id)
			{
				if (!this.missingInformationStarted)
				{
					this.RenderMissingInformationStart();
				}
				base.Writer.Write("<p _id=\"");
				Utilities.HtmlEncode(id, base.Writer);
				base.Writer.Write("\" _dl=0></p>");
			}

			public override void RenderMissing(string name)
			{
			}

			public override void RenderMissing(string name, string type, string id, string legacyDN, int total, int valid)
			{
				if (!this.missingInformationStarted)
				{
					this.RenderMissingInformationStart();
				}
				base.Writer.Write("<p");
				if (legacyDN != null)
				{
					base.Writer.Write(" _em=\"");
					Utilities.HtmlEncode(legacyDN, base.Writer);
				}
				else
				{
					base.Writer.Write(" _id=\"");
					Utilities.HtmlEncode(id, base.Writer);
				}
				base.Writer.Write("\" _dl=1 _tn=");
				base.Writer.Write(total);
				base.Writer.Write(" _vn=");
				base.Writer.Write(valid);
				base.Writer.Write(">");
				Utilities.HtmlEncode(base.GetDLMissingMessage(total, valid), base.Writer);
				base.Writer.Write("</p>");
			}

			public override void RenderCertificates(Dictionary<Participant, SMimeEventHandler.CertificatsInformation> certificateDictionary)
			{
			}

			public override void RenderError(SMimeEventHandler.SMimeEventHandlerException e)
			{
				if (this.missingInformationStarted)
				{
					this.RenderMissingInformationEnd();
				}
				base.Writer.Write("<p id=err _msg=\"");
				if (e is SMimeEventHandler.HiddenMembershipException)
				{
					base.Writer.Write(LocalizedStrings.GetHtmlEncoded(2141668304));
				}
				else
				{
					if (!(e is SMimeEventHandler.ExpansionTimeoutException))
					{
						throw new ArgumentOutOfRangeException("e", "Only support HiddenMembershipException and ExpansionTimeoutException");
					}
					base.Writer.Write(LocalizedStrings.GetHtmlEncoded(298218506));
				}
				base.Writer.Write("\"/>");
			}

			private bool missingInformationStarted;
		}

		private sealed class GetCertificateListResponseWriter : SMimeEventHandler.GetCertificateResponseWriter
		{
			public GetCertificateListResponseWriter(UserContext userContext, TextWriter writer) : base(userContext, writer)
			{
			}

			private void RenderMissingInformationStart()
			{
				this.missingInformationStarted = true;
				base.Writer.Write("<div id=mis><p>");
				base.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1760660333));
				base.Writer.Write("</p><p>");
				base.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1760660334));
				base.Writer.Write("</p><div id=dvMsCrts><table id=tblMsCrts cellpadding=0 cellspacing=0><tr class=hd><td class=\"ic ");
				base.Writer.Write(base.UserContext.IsRtl ? 'r' : 'l');
				base.Writer.Write("\">");
				base.UserContext.RenderThemeImage(base.Writer, ThemeFileId.Document);
				base.Writer.Write("</td><td>");
				base.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1966747349));
				base.Writer.Write("</td></tr>");
			}

			private void RenderMissingInformationEnd()
			{
				this.missingInformationStarted = false;
				base.Writer.Write("<tr id=trMsCrtsEL style=\"height:1;\"><td class=\"i ");
				base.Writer.Write(base.UserContext.IsRtl ? 'r' : 'l');
				base.Writer.Write("\"><img src=\"");
				base.UserContext.RenderThemeFileUrl(base.Writer, ThemeFileId.Clear1x1);
				base.Writer.Write("\"></td><td></td></tr></table></div></div>");
			}

			private void RenderIcon(ThemeFileId themeFileId)
			{
				base.Writer.Write("<td class=\"i ");
				base.Writer.Write(base.UserContext.IsRtl ? 'r' : 'l');
				base.Writer.Write("\"><img src=\"");
				base.UserContext.RenderThemeFileUrl(base.Writer, themeFileId);
				base.Writer.Write("\"></td>");
			}

			private void RenderName(string type, string id, string name, string missingInfo)
			{
				base.Writer.Write("<td nowrap>");
				if (type != null && id != null)
				{
					string handlerCode = string.Format("openItmRdFm(\"{0}\",\"{1}\");", Utilities.JavascriptEncode(type), Utilities.JavascriptEncode(id));
					base.Writer.Write("<a class=\"lnk");
					if (!string.IsNullOrEmpty(missingInfo))
					{
						base.Writer.Write(" bld");
					}
					base.Writer.Write("\" ");
					Utilities.RenderScriptHandler(base.Writer, "onclick", handlerCode);
					base.Writer.Write(" title=\"");
					Utilities.HtmlEncode(name, base.Writer);
					base.Writer.Write("\">");
					Utilities.HtmlEncode(name, base.Writer);
					base.Writer.Write("</a>");
					if (!string.IsNullOrEmpty(missingInfo))
					{
						base.Writer.Write(" <span class=spnFnd>");
						base.Writer.Write(base.UserContext.DirectionMark);
						Utilities.HtmlEncode(missingInfo, base.Writer);
						base.Writer.Write(base.UserContext.DirectionMark);
						base.Writer.Write("</span>");
					}
				}
				else
				{
					Utilities.HtmlEncode(name, base.Writer);
				}
				base.Writer.Write("</td>");
			}

			public override void RenderMissing(ThemeFileId icon, string name, string type, string id)
			{
				if (!this.missingInformationStarted)
				{
					this.RenderMissingInformationStart();
				}
				base.Writer.Write("<tr>");
				this.RenderIcon(icon);
				this.RenderName(type, id, name, null);
				base.Writer.Write("</tr>");
			}

			public override void RenderMissing(string name)
			{
				if (string.IsNullOrEmpty(name))
				{
					return;
				}
				this.RenderMissing(ThemeFileId.DistributionListUser, name, null, null);
			}

			public override void RenderMissing(string name, string type, string id, string legacyDN, int total, int valid)
			{
				if (!this.missingInformationStarted)
				{
					this.RenderMissingInformationStart();
				}
				base.Writer.Write("<tr>");
				this.RenderIcon(ThemeFileId.DistributionListOther);
				this.RenderName(type, id, name, base.GetDLMissingMessage(total, valid));
				base.Writer.Write("</tr>");
			}

			public override void RenderError(SMimeEventHandler.SMimeEventHandlerException e)
			{
				if (this.missingInformationStarted)
				{
					this.RenderMissingInformationEnd();
				}
				base.Writer.Write("<p id=err _msg=\"");
				if (e is SMimeEventHandler.HiddenMembershipException)
				{
					base.Writer.Write(LocalizedStrings.GetHtmlEncoded(1161561076));
				}
				else
				{
					if (!(e is SMimeEventHandler.ExpansionTimeoutException))
					{
						throw new ArgumentOutOfRangeException("e", "Only support HiddenMembershipException and ExpansionTimeoutException");
					}
					base.Writer.Write(LocalizedStrings.GetHtmlEncoded(1073923836));
				}
				base.Writer.Write("\"/>");
			}

			public override void RenderCertificates(Dictionary<Participant, SMimeEventHandler.CertificatsInformation> certificateDictionary)
			{
				if (this.missingInformationStarted)
				{
					this.RenderMissingInformationEnd();
				}
				if (certificateDictionary.Count == 0)
				{
					return;
				}
				bool flag = false;
				foreach (KeyValuePair<Participant, SMimeEventHandler.CertificatsInformation> keyValuePair in certificateDictionary)
				{
					Participant key = keyValuePair.Key;
					SMimeEventHandler.CertificatsInformation value = keyValuePair.Value;
					if (value.Certificates.Count != 0)
					{
						if (!flag)
						{
							flag = true;
							base.Writer.WriteLine("<div id=\"certs\">");
						}
						string s;
						try
						{
							s = SMimeEventHandler.EncodeCertificates(value.Certificates, OwaRegistryKeys.UseKeyIdentifier);
						}
						catch (CryptographicException)
						{
							continue;
						}
						StoreParticipantOrigin storeParticipantOrigin = key.Origin as StoreParticipantOrigin;
						if (storeParticipantOrigin != null)
						{
							base.Writer.Write("<p _id=\"");
							Utilities.HtmlEncode((key.Origin as StoreParticipantOrigin).OriginItemId.ToBase64String(), base.Writer);
							base.Writer.Write("\"");
						}
						else
						{
							base.Writer.Write("<p _em=\"");
							Utilities.HtmlEncode(key.EmailAddress, base.Writer);
							base.Writer.Write("\"");
						}
						base.Writer.Write(" _tn=");
						base.Writer.Write(value.Total);
						base.Writer.Write(" _vn=");
						base.Writer.Write(value.Valid);
						base.Writer.Write(">");
						Utilities.HtmlEncode(s, base.Writer);
						base.Writer.WriteLine("</p>");
					}
				}
				if (flag)
				{
					base.Writer.WriteLine("</div>");
				}
			}

			private bool missingInformationStarted;
		}
	}
}

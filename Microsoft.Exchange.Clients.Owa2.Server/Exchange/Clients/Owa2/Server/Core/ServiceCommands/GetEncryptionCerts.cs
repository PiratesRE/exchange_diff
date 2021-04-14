using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetEncryptionCerts : ServiceCommand<GetCertsResponse>
	{
		public GetEncryptionCerts(CallContext callContext, GetCertsRequest request) : base(callContext)
		{
			this.errors = new List<string>();
			this.errorStatus = GetCertsErrorStatus.NoError;
			this.request = request;
			this.storeSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.recipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			OrganizationId organizationId = this.storeSession.MailboxOwner.MailboxInfo.OrganizationId;
			this.adRecipientExpansion = new ADRecipientExpansion(GetEncryptionCerts.adRecipientProperties, organizationId);
			this.smimeAdminOptions = new SmimeAdminSettingsType(organizationId);
		}

		protected override GetCertsResponse InternalExecute()
		{
			ExDateTime now = ExDateTime.Now;
			GetCertsResponse getCertsResponse = new GetCertsResponse();
			try
			{
				if (this.request.Recipients == null || this.request.Recipients.Length == 0)
				{
					GetCertsResponse getCertsResponse2 = new GetCertsResponse();
					getCertsResponse2.ErrorStatus = GetCertsErrorStatus.InvalidRequest;
					ExTraceGlobals.RequestTracer.TraceDebug<string>((long)this.GetHashCode(), "The GetCertsRequest is invalid: {0}", JsonConverter.ToJSON(this.request));
					return getCertsResponse2;
				}
				ExTraceGlobals.RequestTracer.TraceDebug<string>((long)this.GetHashCode(), "Recipients in GetCertsRequest: {0}", this.EmailAddressToString(this.request.Recipients));
				Participant[] allParticipants = this.GetAllParticipants();
				EmailAddressWrapper[][] array = new EmailAddressWrapper[allParticipants.Length][];
				this.allCerts = new List<byte[]>[this.request.Recipients.Length];
				this.current = 0;
				while (this.current < allParticipants.Length)
				{
					Participant participant = allParticipants[this.current];
					if (participant.Origin is StoreParticipantOrigin)
					{
						if (string.IsNullOrEmpty(this.GetPaticipantAddress(participant)))
						{
							array[this.current] = this.GetCertsFromPrivateDL(participant, true);
						}
						else
						{
							array[this.current] = this.GetCertsFromContact(participant, true);
						}
					}
					else if (participant.Origin is DirectoryParticipantOrigin)
					{
						array[this.current] = this.GetCertsFromDirectory(participant, true);
					}
					else
					{
						array[this.current] = new EmailAddressWrapper[]
						{
							this.request.Recipients[this.current]
						};
					}
					this.current++;
				}
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && array[i].Length > 0)
					{
						flag = true;
						break;
					}
				}
				List<string[]> list = new List<string[]>(this.allCerts.Length);
				foreach (List<byte[]> list2 in this.allCerts)
				{
					if (list2 != null && list2.Count > 0)
					{
						List<string> list3 = new List<string>(list2.Count);
						foreach (byte[] array3 in list2)
						{
							if (array3 != null && array3.Length > 0)
							{
								list3.Add(Convert.ToBase64String(array3));
							}
						}
						list.Add(list3.ToArray());
					}
					else
					{
						list.Add(null);
					}
				}
				getCertsResponse.ErrorStatus = this.errorStatus;
				getCertsResponse.InvalidRecipients = (flag ? array : null);
				getCertsResponse.ValidRecipients = list.ToArray();
			}
			catch (Exception ex)
			{
				this.LogException(ex, "Unknown error occurred", new object[0]);
				getCertsResponse.ErrorStatus = GetCertsErrorStatus.UnknownError;
				getCertsResponse.InvalidRecipients = null;
				getCertsResponse.ValidRecipients = null;
			}
			finally
			{
				getCertsResponse.ErrorDetails = this.errors.ToArray();
				ExTraceGlobals.RequestTracer.TracePerformance<TimeSpan>((long)this.GetHashCode(), "Elapsed time in GetCerts call: ", ExDateTime.Now - now);
			}
			return getCertsResponse;
		}

		private ADRecipient GetAdRecipient(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				return null;
			}
			if (emailAddress[0] != '/')
			{
				return this.recipientSession.FindByProxyAddress(new SmtpProxyAddress(emailAddress, true));
			}
			return this.recipientSession.FindByLegacyExchangeDN(emailAddress);
		}

		private string GetPaticipantAddress(Participant p)
		{
			return p.SmtpEmailAddress ?? p.EmailAddress;
		}

		private EmailAddressWrapper GetEmailAddressWrapper(ADRawEntry adRawEntry)
		{
			return new EmailAddressWrapper
			{
				Name = (adRawEntry[ADRecipientSchema.DisplayName] as string),
				EmailAddress = (adRawEntry[ADRecipientSchema.PrimarySmtpAddress] as string),
				MailboxType = "Mailbox",
				RoutingType = "SMTP"
			};
		}

		private EmailAddressWrapper GetEmailAddressWrapper(Participant p, string mailBoxType)
		{
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
			string paticipantAddress;
			ADRecipient adRecipient;
			if (!string.IsNullOrEmpty(paticipantAddress = this.GetPaticipantAddress(p)) && (adRecipient = this.GetAdRecipient(paticipantAddress)) != null)
			{
				emailAddressWrapper.Name = adRecipient.DisplayName;
				emailAddressWrapper.EmailAddress = adRecipient.PrimarySmtpAddress.ToString();
				emailAddressWrapper.MailboxType = "Mailbox";
				emailAddressWrapper.RoutingType = "SMTP";
			}
			else
			{
				emailAddressWrapper.Name = p.DisplayName;
				emailAddressWrapper.OriginalDisplayName = p.OriginalDisplayName;
				emailAddressWrapper.EmailAddress = this.GetPaticipantAddress(p);
				emailAddressWrapper.MailboxType = mailBoxType;
				emailAddressWrapper.RoutingType = p.RoutingType;
			}
			return emailAddressWrapper;
		}

		private EmailAddressWrapper[] GetCurrent(Participant p, bool isCurrent, string mailBoxType)
		{
			return new EmailAddressWrapper[]
			{
				isCurrent ? this.request.Recipients[this.current] : this.GetEmailAddressWrapper(p, mailBoxType)
			};
		}

		private ParticipantOrigin GetOriginFromStore(EmailAddressWrapper emailAddressWrapper)
		{
			StoreId storeId = null;
			try
			{
				storeId = IdConverter.ConvertItemIdToStoreId(emailAddressWrapper.ItemId, BasicTypes.Item);
			}
			catch (Exception ex)
			{
				this.LogException(ex, "Error occurred when getting store id from item id: {0}", new object[]
				{
					emailAddressWrapper.ItemId
				});
			}
			if (storeId == null)
			{
				return new OneOffParticipantOrigin();
			}
			int emailAddressIndex;
			if (!string.IsNullOrEmpty(emailAddressWrapper.EmailAddressIndex) && int.TryParse(emailAddressWrapper.EmailAddressIndex, out emailAddressIndex))
			{
				return new StoreParticipantOrigin(storeId, (EmailAddressIndex)emailAddressIndex);
			}
			return new StoreParticipantOrigin(storeId);
		}

		private ParticipantOrigin GetOriginFromDirectory(EmailAddressWrapper emailAddressWrapper)
		{
			ADRecipient adRecipient = this.GetAdRecipient(emailAddressWrapper.EmailAddress);
			if (adRecipient != null)
			{
				return new DirectoryParticipantOrigin(adRecipient);
			}
			return new OneOffParticipantOrigin();
		}

		private Participant[] GetAllParticipants()
		{
			Participant[] array = new Participant[this.request.Recipients.Length];
			for (int i = 0; i < this.request.Recipients.Length; i++)
			{
				EmailAddressWrapper emailAddressWrapper = this.request.Recipients[i];
				try
				{
					if (emailAddressWrapper == null)
					{
						ExTraceGlobals.RequestTracer.TraceDebug<string>((long)this.GetHashCode(), "Recipients in GetCertsRequest should not contain any null values: {0}", JsonConverter.ToJSON(this.request));
					}
					else
					{
						string mailboxType;
						ParticipantOrigin origin;
						switch (mailboxType = emailAddressWrapper.MailboxType)
						{
						case "OneOff":
							origin = new OneOffParticipantOrigin();
							goto IL_146;
						case "Contact":
						case "PrivateDL":
							if (emailAddressWrapper.ItemId != null)
							{
								origin = this.GetOriginFromStore(emailAddressWrapper);
								goto IL_146;
							}
							if (!string.IsNullOrEmpty(emailAddressWrapper.EmailAddress))
							{
								origin = this.GetOriginFromDirectory(emailAddressWrapper);
								goto IL_146;
							}
							origin = new OneOffParticipantOrigin();
							goto IL_146;
						}
						if (!string.IsNullOrEmpty(emailAddressWrapper.EmailAddress))
						{
							origin = this.GetOriginFromDirectory(emailAddressWrapper);
						}
						else
						{
							origin = new OneOffParticipantOrigin();
						}
						IL_146:
						array[i] = new Participant(emailAddressWrapper.Name, emailAddressWrapper.EmailAddress, emailAddressWrapper.RoutingType, emailAddressWrapper.OriginalDisplayName, origin, new KeyValuePair<PropertyDefinition, object>[0]);
					}
				}
				catch (Exception ex)
				{
					this.LogException(ex, "Error occurred when getting participant from EmailAddress: {0}", new object[]
					{
						this.EmailAddressToString(emailAddressWrapper)
					});
				}
			}
			return array;
		}

		private EmailAddressWrapper[] TryGetCertsFromDirectory(Participant p, bool isCurrent, string mailBoxType)
		{
			string paticipantAddress = this.GetPaticipantAddress(p);
			if (string.IsNullOrEmpty(paticipantAddress))
			{
				return this.GetCurrent(p, isCurrent, mailBoxType);
			}
			ADRecipient adRecipient = this.GetAdRecipient(paticipantAddress);
			if (adRecipient != null)
			{
				DirectoryParticipantOrigin origin = new DirectoryParticipantOrigin(adRecipient);
				return this.GetCertsFromDirectory(new Participant(p.DisplayName, paticipantAddress, p.RoutingType, p.OriginalDisplayName, origin, new KeyValuePair<PropertyDefinition, object>[0]), isCurrent);
			}
			return this.GetCurrent(p, isCurrent, mailBoxType);
		}

		private EmailAddressWrapper[] GetCertsFromContact(Participant p, bool isCurrent)
		{
			EmailAddressWrapper[] result;
			try
			{
				StoreObjectId originItemId = ((StoreParticipantOrigin)p.Origin).OriginItemId;
				using (Contact contact = Contact.Bind(this.storeSession, originItemId, new PropertyDefinition[]
				{
					ContactSchema.UserX509Certificates
				}))
				{
					byte[][] array = contact.TryGetValueOrDefault(ContactSchema.UserX509Certificates, null);
					if (array == null || array.Length == 0)
					{
						result = this.TryGetCertsFromDirectory(p, true, "Contact");
					}
					else
					{
						X509Certificate2 x509Certificate = this.FindBestCert(null, true, new byte[][][]
						{
							array
						});
						if (x509Certificate != null)
						{
							this.AddCertToCurrentParticipant(x509Certificate.RawData);
							result = null;
						}
						else
						{
							result = this.GetCurrent(p, isCurrent, "Contact");
						}
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				result = this.TryGetCertsFromDirectory(p, true, "Contact");
			}
			catch (Exception ex)
			{
				this.LogException(ex, "Error occurred when getting cert from Contact: {0}", new object[]
				{
					this.ParticipantToString(p)
				});
				result = this.TryGetCertsFromDirectory(p, true, "Contact");
			}
			return result;
		}

		private EmailAddressWrapper[] GetCertsFromPrivateDL(Participant p, bool isCurrent)
		{
			Participant[] array = null;
			try
			{
				StoreObjectId originItemId = ((StoreParticipantOrigin)p.Origin).OriginItemId;
				array = DistributionList.ExpandDeep(this.storeSession, originItemId, false);
				if (array == null || array.Length == 0)
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				this.LogException(ex, "Error occurred when expanding PrivateDL: {0}", new object[]
				{
					this.ParticipantToString(p)
				});
				return this.GetCurrent(p, isCurrent, "PrivateDL");
			}
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
			foreach (Participant participant in array)
			{
				string paticipantAddress = this.GetPaticipantAddress(participant);
				if (participant.Origin is StoreParticipantOrigin)
				{
					EmailAddressWrapper[] collection;
					if (string.IsNullOrEmpty(paticipantAddress))
					{
						if ((collection = this.GetCertsFromPrivateDL(participant, false)) != null)
						{
							list.AddRange(collection);
						}
					}
					else if ((collection = this.GetCertsFromContact(participant, false)) != null)
					{
						list.AddRange(collection);
					}
				}
				else if (participant.Origin is DirectoryParticipantOrigin)
				{
					EmailAddressWrapper[] certsFromDirectory;
					if ((certsFromDirectory = this.GetCertsFromDirectory(participant, false)) != null)
					{
						list.AddRange(certsFromDirectory);
					}
				}
				else if (!string.IsNullOrEmpty(paticipantAddress))
				{
					ADRecipient adRecipient = this.GetAdRecipient(paticipantAddress);
					EmailAddressWrapper[] certsFromDirectory;
					if (adRecipient == null)
					{
						EmailAddressWrapper emailAddressWrapper = this.GetEmailAddressWrapper(participant, "OneOff");
						list.Add(emailAddressWrapper);
					}
					else if ((certsFromDirectory = this.GetCertsFromDirectory(new Participant(adRecipient), false)) != null)
					{
						list.AddRange(certsFromDirectory);
					}
				}
				else
				{
					EmailAddressWrapper emailAddressWrapper2 = this.GetEmailAddressWrapper(participant, "Unknown");
					list.Add(emailAddressWrapper2);
				}
			}
			return list.ToArray();
		}

		private EmailAddressWrapper[] GetCertsFromDirectory(Participant p, bool isCurrent)
		{
			bool flag = false;
			EmailAddressWrapper[] result;
			try
			{
				this.timeoutTimeForDLExpansion = new ExDateTime?(ExDateTime.UtcNow.AddMilliseconds(this.smimeAdminOptions.DLExpansionTimeout));
				ADRawEntry adentry = ((DirectoryParticipantOrigin)p.Origin).ADEntry;
				if (adentry == null)
				{
					result = this.GetCurrent(p, isCurrent, "Unknown");
				}
				else if (this.IsHiddenMembership(adentry))
				{
					result = this.GetCurrent(p, isCurrent, "PublicDL");
				}
				else
				{
					Dictionary<string, EmailAddressWrapper> invalidRecipients = new Dictionary<string, EmailAddressWrapper>();
					Action<ADRawEntry> addInvalidRecipient = delegate(ADRawEntry recipient)
					{
						invalidRecipients[recipient[ADObjectSchema.Id].ToString()] = this.GetEmailAddressWrapper(recipient);
					};
					Action<ADRawEntry> addCertsOrInvalidRecipients = delegate(ADRawEntry recipient)
					{
						try
						{
							string text = recipient[ADRecipientSchema.LegacyExchangeDN] as string;
							if (text == null)
							{
								addInvalidRecipient(recipient);
							}
							else
							{
								text = text.ToLower();
								byte[][] array = this.MultiValuePropertyToByteArray(recipient[ADRecipientSchema.Certificate] as MultiValuedProperty<byte[]>);
								byte[][] array2 = this.MultiValuePropertyToByteArray(recipient[ADRecipientSchema.SMimeCertificate] as MultiValuedProperty<byte[]>);
								if (array.Length == 0 && array2.Length == 0)
								{
									addInvalidRecipient(recipient);
								}
								else
								{
									string[] array3;
									if (this.smimeAdminOptions.UseSecondaryProxiesWhenFindingCertificates)
									{
										ProxyAddressCollection proxyAddressCollection = recipient[ADRecipientSchema.EmailAddresses] as ProxyAddressCollection;
										if (proxyAddressCollection != null && proxyAddressCollection.Count > 0)
										{
											array3 = new string[proxyAddressCollection.Count];
											for (int i = 0; i < proxyAddressCollection.Count; i++)
											{
												array3[i] = proxyAddressCollection[i].AddressString;
											}
										}
										else
										{
											array3 = new string[]
											{
												recipient[ADRecipientSchema.PrimarySmtpAddress].ToString()
											};
										}
									}
									else
									{
										array3 = new string[]
										{
											recipient[ADRecipientSchema.PrimarySmtpAddress].ToString()
										};
									}
									X509Certificate2 x509Certificate = this.FindBestCert(array3, false, new byte[][][]
									{
										array,
										array2
									});
									if (x509Certificate != null)
									{
										this.AddCertToCurrentParticipant(x509Certificate.RawData);
									}
									else
									{
										addInvalidRecipient(recipient);
									}
								}
							}
						}
						catch (Exception ex3)
						{
							this.LogException(ex3, "Error occurred when getting cert from Directory User: {0}", new object[]
							{
								recipient.GetDistinguishedNameOrName()
							});
							addInvalidRecipient(recipient);
						}
					};
					if (flag = this.IsDistributionList(adentry))
					{
						ADRecipientExpansion.HandleRecipientDelegate handleRecipient = delegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
						{
							if (this.IsDLExpansionTimedOut())
							{
								throw new TimeoutException("The DL expansion is timeout.");
							}
							if (this.IsHiddenMembership(recipient))
							{
								addInvalidRecipient(recipient);
								return ExpansionControl.Skip;
							}
							if (!this.IsDistributionList(recipient))
							{
								addCertsOrInvalidRecipients(recipient);
							}
							return ExpansionControl.Continue;
						};
						ADRecipientExpansion.HandleFailureDelegate handleFailure = delegate(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
						{
							if (this.IsDLExpansionTimedOut())
							{
								throw new TimeoutException("The DL expansion is timeout.");
							}
							ExTraceGlobals.RequestTracer.TraceDebug<string, string, ExpansionFailure>((long)this.GetHashCode(), "Error occured when expanding DL: {0}: {1} {2}", recipient.GetDistinguishedNameOrName(), failure.ToString(), failure);
							addInvalidRecipient(recipient);
							return ExpansionControl.Continue;
						};
						try
						{
							this.adRecipientExpansion.Expand(adentry, handleRecipient, handleFailure);
							goto IL_13C;
						}
						catch (Exception ex)
						{
							this.LogException(ex, "Error occurred when expanding PublicDL: {0}", new object[]
							{
								adentry.GetDistinguishedNameOrName()
							});
							addInvalidRecipient(adentry);
							goto IL_13C;
						}
					}
					addCertsOrInvalidRecipients(adentry);
					IL_13C:
					if (invalidRecipients.Count > 0)
					{
						Queue<EmailAddressWrapper> queue = new Queue<EmailAddressWrapper>(invalidRecipients.Count);
						foreach (EmailAddressWrapper item in invalidRecipients.Values)
						{
							queue.Enqueue(item);
						}
						result = queue.ToArray();
					}
					else
					{
						result = null;
					}
				}
			}
			catch (Exception ex2)
			{
				this.LogException(ex2, "Error occurred when getting cert from Directory Object: {0}", new object[]
				{
					this.ParticipantToString(p)
				});
				result = this.GetCurrent(p, isCurrent, flag ? "PublicDL" : "MailBox");
			}
			return result;
		}

		private void AddCertToCurrentParticipant(byte[] certRawData)
		{
			if (this.allCerts[this.current] == null)
			{
				this.allCerts[this.current] = new List<byte[]>();
			}
			this.allCerts[this.current].Add(certRawData);
		}

		private bool IsDLExpansionTimedOut()
		{
			return this.timeoutTimeForDLExpansion != null && this.timeoutTimeForDLExpansion.Value < ExDateTime.UtcNow;
		}

		private X509Certificate2 FindBestCert(IEnumerable<string> emails, bool isContact, params byte[][][] paramsCertsRawData)
		{
			X509Certificate2 result;
			try
			{
				X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
				foreach (byte[][] array in paramsCertsRawData)
				{
					if (array != null)
					{
						foreach (byte[] array3 in array)
						{
							try
							{
								if (isContact)
								{
									x509CertificateCollection.ImportFromContact(array3);
								}
								else
								{
									x509CertificateCollection.Import(array3);
								}
							}
							catch (Exception ex)
							{
								this.LogException(ex, "Error occurred when parsing cert raw data {0}", new object[]
								{
									Convert.ToBase64String(array3)
								});
							}
						}
					}
				}
				X509Store x509Store = null;
				if (!string.IsNullOrEmpty(this.smimeAdminOptions.SMIMECertificateIssuingCAFull))
				{
					x509Store = CertificateStore.Open(StoreType.Memory, null, OpenFlags.ReadWrite);
					X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
					x509Certificate2Collection.Import(Convert.FromBase64String(this.smimeAdminOptions.SMIMECertificateIssuingCAFull));
					x509Store.AddRange(x509Certificate2Collection);
				}
				result = x509CertificateCollection.FindSMimeCertificate(emails, X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment, false, TimeSpan.FromMilliseconds(this.smimeAdminOptions.CRLConnectionTimeout), TimeSpan.FromMilliseconds(this.smimeAdminOptions.CRLRetrievalTimeout), x509Store, base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId.ToString());
			}
			catch (Exception ex2)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte[][] array4 in paramsCertsRawData)
				{
					if (array4 != null)
					{
						foreach (byte[] array6 in array4)
						{
							if (array6 != null)
							{
								stringBuilder.AppendLine(Convert.ToBase64String(array6));
							}
						}
					}
				}
				this.LogException(ex2, "Error occurred when finding best cert from: {0}", new object[]
				{
					stringBuilder
				});
				result = null;
			}
			return result;
		}

		private bool IsDistributionList(ADRawEntry entry)
		{
			object obj = entry[ADRecipientSchema.RecipientType];
			if (obj == null)
			{
				return false;
			}
			RecipientType recipientType = (RecipientType)obj;
			return recipientType == RecipientType.Group || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.DynamicDistributionGroup;
		}

		private bool IsHiddenMembership(ADRawEntry entry)
		{
			object obj = entry[ADGroupSchema.HiddenGroupMembershipEnabled];
			return obj != null && (bool)obj;
		}

		private byte[][] MultiValuePropertyToByteArray(MultiValuedProperty<byte[]> property)
		{
			if (property != null)
			{
				byte[][] array = new byte[property.Count][];
				property.CopyTo(array, 0);
				return array;
			}
			return new byte[0][];
		}

		private string EmailAddressToString(EmailAddressWrapper[] addresses)
		{
			if (addresses == null || addresses.Length == 0)
			{
				return string.Empty;
			}
			string[] array = new string[addresses.Length];
			for (int i = 0; i < addresses.Length; i++)
			{
				array[i] = this.EmailAddressToString(addresses[i]);
			}
			return string.Join("; ", array);
		}

		private string EmailAddressToString(EmailAddressWrapper address)
		{
			if (address == null)
			{
				return "_";
			}
			string text;
			if ((text = address.Name) == null)
			{
				text = (address.OriginalDisplayName ?? "_");
			}
			string str = text;
			string text2;
			if ((text2 = address.EmailAddress) == null && (text2 = address.MailboxType) == null)
			{
				text2 = (address.RoutingType ?? "_");
			}
			string str2 = text2;
			return str + " <" + str2 + ">";
		}

		private string ParticipantToString(Participant p)
		{
			if (p == null)
			{
				return "_";
			}
			string text;
			if ((text = p.DisplayName) == null)
			{
				text = (p.OriginalDisplayName ?? "_");
			}
			string str = text;
			string text2;
			if ((text2 = p.EmailAddress) == null)
			{
				text2 = (p.RoutingType ?? "_");
			}
			string str2 = text2;
			return str + " <" + str2 + ">";
		}

		private void LogException(Exception ex, string messageFormat, params object[] args)
		{
			string text = string.Format(messageFormat, args) + " > " + ex.ToString();
			this.errors.Add(text);
			ExTraceGlobals.RequestTracer.TraceError((long)this.GetHashCode(), text);
			if (ex.InnerException != null)
			{
				this.LogException(ex.InnerException, "[InnerException]", new object[0]);
			}
		}

		private static readonly PropertyDefinition[] adRecipientProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.LegacyExchangeDN,
			ADObjectSchema.Id,
			ADRecipientSchema.Certificate,
			ADRecipientSchema.SMimeCertificate,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.EmailAddresses,
			ADGroupSchema.HiddenGroupMembershipEnabled
		};

		private readonly GetCertsRequest request;

		private readonly ADRecipientExpansion adRecipientExpansion;

		private readonly StoreSession storeSession;

		private readonly IRecipientSession recipientSession;

		private int current;

		private List<byte[]>[] allCerts;

		private ExDateTime? timeoutTimeForDLExpansion;

		private GetCertsErrorStatus errorStatus;

		private readonly List<string> errors;

		private readonly SmimeAdminSettingsType smimeAdminOptions;
	}
}

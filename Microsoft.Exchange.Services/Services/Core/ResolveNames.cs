using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ResolveNames : UtilCommandBase<ResolveNamesRequest, ResolutionSet>
	{
		public ResolveNames(CallContext callContext, ResolveNamesRequest request) : base(callContext, request)
		{
			this.nameToResolve = base.Request.UnresolvedEntry;
			this.shouldReturnFullContactData = base.Request.ReturnFullContactData;
			this.searchScope = base.Request.SearchScope;
			this.contactDataShape = base.Request.ContactDataShape;
			if (base.Request.ParentFolderIds == null)
			{
				BaseFolderId[] array = new BaseFolderId[1];
				this.parentFolders = array;
				return;
			}
			this.parentFolders = base.Request.ParentFolderIds;
			ServiceCommandBase.ThrowIfEmpty<BaseFolderId>(this.parentFolders, "parentFolders", "ResolveNames::PreExecuteCommand");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ResolveNamesResponse resolveNamesResponse = new ResolveNamesResponse();
			resolveNamesResponse.ProcessServiceResult<ResolutionSet>(base.Result);
			return resolveNamesResponse;
		}

		public static EmailAddressWrapper EmailAddressWrapperFromRecipient(ADRecipient recipient)
		{
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
			emailAddressWrapper.Name = recipient.DisplayName;
			if (!string.IsNullOrEmpty(recipient.PrimarySmtpAddress.ToString()))
			{
				emailAddressWrapper.EmailAddress = recipient.PrimarySmtpAddress.ToString();
				emailAddressWrapper.RoutingType = "SMTP";
			}
			MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType(recipient.RecipientType, recipient.RecipientTypeDetails);
			emailAddressWrapper.MailboxType = mailboxTypeType.ToString();
			return emailAddressWrapper;
		}

		private static ContactItemType DirectoryRecipientToContact(ADRecipient recipient, ShapeEnum contactDataShape, IRecipientSession recipientSession)
		{
			ContactItemType contactItemType = new ContactItemType();
			contactItemType.ContactSource = ContactSourceType.ActiveDirectory;
			contactItemType.ContactSourceSpecified = true;
			if (contactDataShape == ShapeEnum.IdOnly)
			{
				contactItemType.DirectoryId = recipient.Id.ToGuidOrDNString();
			}
			else
			{
				contactItemType.DisplayName = recipient.DisplayName;
				int num = (recipient.EmailAddresses.Count > 3) ? 3 : recipient.EmailAddresses.Count;
				if (num > 0)
				{
					EmailAddressDictionaryEntryType[] array = new EmailAddressDictionaryEntryType[num];
					EmailAddressKeyType[] array2 = new EmailAddressKeyType[]
					{
						EmailAddressKeyType.EmailAddress1,
						EmailAddressKeyType.EmailAddress2,
						EmailAddressKeyType.EmailAddress3
					};
					for (int i = 0; i < num; i++)
					{
						array[i] = new EmailAddressDictionaryEntryType(array2[i], recipient.EmailAddresses[i].ProxyAddressString);
					}
					contactItemType.EmailAddresses = array;
				}
				contactItemType.AssistantName = recipient.AssistantName;
				IADOrgPerson iadorgPerson = recipient as IADOrgPerson;
				bool flag = iadorgPerson != null;
				if (flag)
				{
					if (iadorgPerson.Languages != null && iadorgPerson.Languages.Count > 0)
					{
						CultureInfo cultureInfo = iadorgPerson.Languages[0];
						if (cultureInfo != null)
						{
							contactItemType.Culture = cultureInfo.Name;
						}
					}
					contactItemType.GivenName = iadorgPerson.FirstName;
					contactItemType.Initials = iadorgPerson.Initials;
					contactItemType.CompanyName = iadorgPerson.Company;
					contactItemType.PhysicalAddresses = new PhysicalAddressDictionaryEntryType[]
					{
						new PhysicalAddressDictionaryEntryType
						{
							Key = PhysicalAddressKeyType.Business,
							Street = iadorgPerson.StreetAddress,
							City = iadorgPerson.City,
							State = iadorgPerson.StateOrProvince,
							CountryOrRegion = (string)iadorgPerson.CountryOrRegion,
							PostalCode = iadorgPerson.PostalCode
						}
					};
					ResolveNames.PhoneNumbersToContact(iadorgPerson, contactItemType);
					contactItemType.Department = iadorgPerson.Department;
					contactItemType.JobTitle = iadorgPerson.Title;
					if (iadorgPerson.Manager != null)
					{
						contactItemType.Manager = iadorgPerson.Manager.Name;
					}
					contactItemType.OfficeLocation = iadorgPerson.Office;
					contactItemType.Surname = iadorgPerson.LastName;
				}
				if (contactDataShape == ShapeEnum.AllProperties)
				{
					contactItemType.DirectoryId = recipient.Id.ToGuidOrDNString();
					contactItemType.Photo = recipient.ThumbnailPhoto;
					if (contactItemType.Photo != null)
					{
						contactItemType.HasPicture = true;
						contactItemType.HasPictureSpecified = true;
					}
					contactItemType.UserSMIMECertificate = recipient.SMimeCertificate.ToArray();
					contactItemType.MSExchangeCertificate = recipient.Certificate.ToArray();
					contactItemType.Notes = recipient.Notes;
					contactItemType.Alias = recipient.Alias;
					if (flag)
					{
						if (iadorgPerson.Manager != null)
						{
							ADRecipient adrecipient = recipientSession.Read(iadorgPerson.Manager);
							if (adrecipient != null)
							{
								contactItemType.ManagerMailbox = new SingleRecipientType
								{
									Mailbox = ResolveNames.EmailAddressWrapperFromRecipient(adrecipient)
								};
							}
						}
						if (iadorgPerson.DirectReports != null && iadorgPerson.DirectReports.Count > 0)
						{
							Result<ADRecipient>[] array3 = recipientSession.ReadMultiple(iadorgPerson.DirectReports.ToArray());
							if (array3.Length > 0)
							{
								List<EmailAddressWrapper> list = new List<EmailAddressWrapper>(array3.Length);
								foreach (Result<ADRecipient> result in array3)
								{
									if (result.Data != null)
									{
										list.Add(ResolveNames.EmailAddressWrapperFromRecipient(result.Data));
									}
								}
								contactItemType.DirectReports = list.ToArray();
							}
						}
					}
				}
			}
			return contactItemType;
		}

		private static void PhoneNumbersToContact(IADOrgPerson orgPerson, ContactItemType contact)
		{
			List<PhoneNumberDictionaryEntryType> list = new List<PhoneNumberDictionaryEntryType>();
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.AssistantPhone, orgPerson.TelephoneAssistant));
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.BusinessFax, orgPerson.Fax));
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.BusinessPhone, orgPerson.Phone));
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.HomePhone, orgPerson.HomePhone));
			if (orgPerson.OtherHomePhone.Count > 0)
			{
				list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.HomePhone2, orgPerson.OtherHomePhone[0]));
			}
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.MobilePhone, orgPerson.MobilePhone));
			if (orgPerson.OtherFax.Count > 0)
			{
				list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.OtherFax, orgPerson.OtherFax[0]));
			}
			list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.Pager, orgPerson.Pager));
			if (orgPerson.OtherTelephone.Count > 0)
			{
				list.Add(new PhoneNumberDictionaryEntryType(PhoneNumberKeyType.OtherTelephone, orgPerson.OtherTelephone[0]));
			}
			if (list.Count > 0)
			{
				contact.PhoneNumbers = list.ToArray();
			}
		}

		internal override ServiceResult<ResolutionSet> Execute()
		{
			int num;
			ResolutionSet value = this.ResolveName(out num);
			if (num == 0)
			{
				throw new NameResolutionNoResultsException();
			}
			ServiceResult<ResolutionSet> result;
			if (num == 1)
			{
				result = new ServiceResult<ResolutionSet>(value);
			}
			else
			{
				ServiceError error = new ServiceError(CoreResources.IDs.ErrorNameResolutionMultipleResults, ResponseCodeType.ErrorNameResolutionMultipleResults, 0, ExchangeVersion.Exchange2007);
				result = new ServiceResult<ResolutionSet>(value, error);
			}
			return result;
		}

		private ResolveNames.ResolveNamesStepType[] GetResolveNamesSteps()
		{
			switch (this.searchScope)
			{
			case ResolveNamesSearchScopeType.ActiveDirectory:
				return new ResolveNames.ResolveNamesStepType[1];
			case ResolveNamesSearchScopeType.ActiveDirectoryContacts:
				return new ResolveNames.ResolveNamesStepType[]
				{
					ResolveNames.ResolveNamesStepType.ActiveDirectory,
					ResolveNames.ResolveNamesStepType.Contacts
				};
			case ResolveNamesSearchScopeType.Contacts:
				return new ResolveNames.ResolveNamesStepType[]
				{
					ResolveNames.ResolveNamesStepType.Contacts
				};
			case ResolveNamesSearchScopeType.ContactsActiveDirectory:
			{
				ResolveNames.ResolveNamesStepType[] array = new ResolveNames.ResolveNamesStepType[2];
				array[0] = ResolveNames.ResolveNamesStepType.Contacts;
				return array;
			}
			default:
				return null;
			}
		}

		private ResolutionSet ResolveName(out int count)
		{
			ResolutionSet resolutionSet = new ResolutionSet();
			this.includesLastItemInRange = true;
			ResolveNames.ResolveNamesStepType[] resolveNamesSteps = this.GetResolveNamesSteps();
			int num = 100;
			base.InitActiveDirectoryNameResolutionContext();
			foreach (ResolveNames.ResolveNamesStepType resolveNamesStepType in resolveNamesSteps)
			{
				if (num <= 0)
				{
					break;
				}
				if (resolveNamesStepType == ResolveNames.ResolveNamesStepType.ActiveDirectory)
				{
					num -= this.ResolveNameFromDirectory(num, resolutionSet);
				}
				else
				{
					if (this.parentFolders.Count != 1)
					{
						throw new ResolveNamesExceptionOnlyOneContactsFolderAllowed();
					}
					foreach (BaseFolderId folderId in this.parentFolders)
					{
						using (ContactsFolder contactsFolder = base.InitStoreNameResolutionContext(folderId))
						{
							num -= this.ResolveNameFromStore(num, contactsFolder, resolutionSet);
						}
					}
				}
			}
			count = 100 - num;
			resolutionSet.TotalItemsInView = count;
			resolutionSet.IncludesLastItemInRange = this.includesLastItemInRange;
			return resolutionSet;
		}

		private int ResolveNameFromDirectory(int maxAmbigiousResults, ResolutionSet resolutionSet)
		{
			int num = 0;
			ADRecipient[] array = this.directoryRecipientSession.FindByANR(this.nameToResolve, maxAmbigiousResults + 1, UtilCommandBase<ResolveNamesRequest, ResolutionSet>.DirectorySortByName);
			for (int i = 0; i < array.Length; i++)
			{
				MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType(array[i].RecipientType, array[i].RecipientTypeDetails);
				if (mailboxTypeType != MailboxHelper.MailboxTypeType.Unknown)
				{
					num++;
					if (num > maxAmbigiousResults)
					{
						this.includesLastItemInRange = false;
						num = maxAmbigiousResults;
						break;
					}
					ResolutionType resolutionType = new ResolutionType();
					EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
					emailAddressWrapper.Name = array[i].Name;
					if (SmtpAddress.Empty == array[i].PrimarySmtpAddress)
					{
						emailAddressWrapper.EmailAddress = array[i].LegacyExchangeDN;
						emailAddressWrapper.RoutingType = "EX";
					}
					else
					{
						emailAddressWrapper.EmailAddress = array[i].PrimarySmtpAddress.ToString();
						emailAddressWrapper.RoutingType = "SMTP";
					}
					emailAddressWrapper.MailboxType = mailboxTypeType.ToString();
					resolutionType.Mailbox = emailAddressWrapper;
					if (this.shouldReturnFullContactData)
					{
						ContactItemType contact = ResolveNames.DirectoryRecipientToContact(array[i], this.contactDataShape, this.directoryRecipientSession);
						resolutionType.Contact = contact;
					}
					resolutionSet.Add(resolutionType);
				}
			}
			return num;
		}

		private int ResolveNameFromStore(int maxAmbigiousResults, ContactsFolder storeContactsFolder, ResolutionSet resolutionSet)
		{
			ExTraceGlobals.UtilAlgorithmTracer.TraceDebug<int, string>((long)this.GetHashCode(), "ResolveNameFromStore called, maxAmbigiousResults={0}, nameToResolve='{1}'", maxAmbigiousResults, this.nameToResolve);
			if (!storeContactsFolder.IsValidAmbiguousName(this.nameToResolve))
			{
				ExTraceGlobals.UtilAlgorithmTracer.TraceError<string>((long)this.GetHashCode(), "ResolveNameFromStore:The name '{0}' is invalid for ContactFolder.ResolveAmbiguousNameView", this.nameToResolve);
				throw new InvalidNameForNameResolutionException();
			}
			int num = 0;
			object[][] array;
			try
			{
				array = storeContactsFolder.ResolveAmbiguousNameView(this.nameToResolve, maxAmbigiousResults + 1, null, ResolveNames.StoreDefaultContactPropDefs);
			}
			catch (AccessDeniedException innerException)
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2007_SP1))
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageResolveNamesNotSufficientPermissionsToContactsFolder, innerException);
				}
				throw;
			}
			if (array != null)
			{
				ExTraceGlobals.UtilAlgorithmTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "ResolveNameFromStore:ContactFolder.ResolveAmbiguousNameView('{0}',{1} + 1) returned {2} elements.", this.nameToResolve, maxAmbigiousResults, array.GetLength(0));
				List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
				for (int i = 0; i < array.GetLength(0); i++)
				{
					object[] array2 = array[i];
					string emailAddress = array2[2] as string;
					string text = array2[3] as string;
					StoreId storeId = (StoreId)array2[0];
					StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
					bool flag = false;
					bool flag2 = false;
					if (asStoreObjectId != null)
					{
						flag = (asStoreObjectId.ObjectType == StoreObjectType.Contact);
						flag2 = (asStoreObjectId.ObjectType == StoreObjectType.DistributionList);
					}
					if (!flag && !flag2)
					{
						if (storeId == null)
						{
							ExTraceGlobals.UtilAlgorithmTracer.TraceError<int>((long)this.GetHashCode(), "ResolveNameFromStore: Element [{0}] has StoreId==NULL.", i);
						}
						else
						{
							ExTraceGlobals.UtilAlgorithmTracer.TraceError<int, string, Type>((long)this.GetHashCode(), "ResolveNameFromStore: Element [{0}], StoreId='{1}', has invalid type '{2}'.", i, storeId.ToBase64String(), storeId.GetType());
						}
						throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidIdReturnedByResolveNames);
					}
					num++;
					if (num > maxAmbigiousResults)
					{
						num = maxAmbigiousResults;
						this.includesLastItemInRange = false;
						break;
					}
					ResolutionType resolutionType = new ResolutionType();
					EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
					emailAddressWrapper.Name = (array2[1] as string);
					emailAddressWrapper.MailboxType = (flag ? "Contact" : "PrivateDL");
					emailAddressWrapper.EmailAddress = emailAddress;
					emailAddressWrapper.RoutingType = text;
					if (string.CompareOrdinal(text, "EX") == 0)
					{
						list.Add(emailAddressWrapper);
					}
					else if (!flag)
					{
						emailAddressWrapper.EmailAddress = null;
						emailAddressWrapper.RoutingType = "MAPIPDL";
					}
					emailAddressWrapper.ItemId = base.GetServiceItemIdFromStoreId(storeId, IdAndSession.CreateFromFolder(storeContactsFolder));
					ExTraceGlobals.UtilAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "ResolveNameFromStore: element[{0}]: DisplayName='{1}', EmailAddress='{2}',RoutineType='{3}',MailboxType='{4}', storeId='{5}'", new object[]
					{
						i,
						array2[1] as string,
						emailAddressWrapper.EmailAddress,
						emailAddressWrapper.RoutingType,
						emailAddressWrapper.MailboxType,
						storeId.ToBase64String()
					});
					resolutionType.Mailbox = emailAddressWrapper;
					resolutionSet.Add(resolutionType);
				}
				if (list.Count > 0)
				{
					this.CorrectEXAddresses(list);
				}
			}
			return num;
		}

		private void CorrectEXAddresses(List<EmailAddressWrapper> mailboxes)
		{
			string[] array = new string[mailboxes.Count];
			for (int i = 0; i < mailboxes.Count; i++)
			{
				array[i] = mailboxes[i].EmailAddress;
			}
			Result<ADRawEntry>[] array2 = null;
			try
			{
				array2 = this.directoryRecipientSession.FindByLegacyExchangeDNs(array, new PropertyDefinition[]
				{
					ADRecipientSchema.PrimarySmtpAddress,
					ADRecipientSchema.LegacyExchangeDN
				});
			}
			catch (NonUniqueRecipientException arg)
			{
				ExTraceGlobals.UtilAlgorithmTracer.TraceDebug<NonUniqueRecipientException>((long)this.GetHashCode(), "[ResolveNames::CorrectEXAddresses] NonUniqueRecipientException was thrown by FindByLegacyExchangeDNs: {0}", arg);
				throw new NameResolutionMultipleResultsException();
			}
			for (int j = 0; j < array2.Length; j++)
			{
				Result<ADRawEntry> result = array2[j];
				if (result.Error == null && result.Data != null)
				{
					string emailAddress = ((SmtpAddress)result.Data[ADRecipientSchema.PrimarySmtpAddress]).ToString();
					string strB = result.Data[ADRecipientSchema.LegacyExchangeDN] as string;
					EmailAddressWrapper emailAddressWrapper = mailboxes[j];
					if (string.Compare(emailAddressWrapper.EmailAddress, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						emailAddressWrapper.EmailAddress = emailAddress;
						emailAddressWrapper.RoutingType = "SMTP";
					}
				}
			}
		}

		private static readonly PropertyDefinition[] StoreDefaultContactPropDefs = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ParticipantSchema.DisplayName,
			ParticipantSchema.EmailAddress,
			ParticipantSchema.RoutingType
		};

		private string nameToResolve;

		private bool shouldReturnFullContactData;

		private ResolveNamesSearchScopeType searchScope;

		private ShapeEnum contactDataShape;

		private IList<BaseFolderId> parentFolders;

		private enum StoreDefaultContactPropDefsIndex
		{
			ItemId,
			ParticipantlDisplayName,
			ParticipantEmailAddress,
			ParticipantRoutineType
		}

		private enum ResolveNamesStepType
		{
			ActiveDirectory,
			Contacts
		}
	}
}

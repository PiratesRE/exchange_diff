using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ImportContactBase
	{
		public ImportContactBase(MailboxSession mbxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("mbxSession", mbxSession);
			this.mbxSession = mbxSession;
		}

		protected ImportContactBase()
		{
		}

		protected int SaveContactsToMailbox(List<ImportContactObject> contactList, CultureInfo culture)
		{
			int num = 0;
			using (BulkAutomaticLink bulkAutomaticLink = new BulkAutomaticLink(this.mbxSession))
			{
				foreach (ImportContactObject importContactObject in contactList)
				{
					int num2 = 0;
					for (;;)
					{
						try
						{
							using (Contact contact = Contact.Create(this.mbxSession, this.mbxSession.GetDefaultFolderId(DefaultFolderType.Contacts)))
							{
								this.PopulateContactProperties(importContactObject, contact, culture);
								bulkAutomaticLink.Link(contact);
								contact.Save(SaveMode.NoConflictResolution);
								contact.Load();
								bulkAutomaticLink.NotifyContactSaved(contact);
								num++;
							}
						}
						catch (StorageTransientException ex)
						{
							num2++;
							if (num2 >= 5)
							{
								CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)13UL, "Caught storage transient exceptions too many times while processing contact {0}, quitting. Exception: {1}", new object[]
								{
									importContactObject.Index,
									ex
								});
								throw new MailboxTransientExceptionSavingContactException(importContactObject.Index, num, ex);
							}
							CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)14UL, "Caught storage transient exception while processing contact {0}. Save attempt: {1}. Retrying.", new object[]
							{
								importContactObject.Index,
								num2
							});
							continue;
						}
						catch (CorruptDataException ex2)
						{
							CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)15UL, "Caught corrupt data exception while processing contact {0}: {1}.", new object[]
							{
								importContactObject.Index,
								ex2
							});
							throw new InternalErrorSavingContactException(importContactObject.Index, num, ex2);
						}
						catch (QuotaExceededException ex3)
						{
							CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)16UL, "Caught storage quota exceeded exception while processing contact {0}: {1}.", new object[]
							{
								importContactObject.Index,
								ex3
							});
							throw new QuotaExceededSavingContactException(importContactObject.Index, num, ex3);
						}
						catch (StoragePermanentException ex4)
						{
							CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)17UL, "Caught storage permanent exception while processing contact {0}: {1}.", new object[]
							{
								importContactObject.Index,
								ex4
							});
							throw new MailboxPermanentErrorSavingContactException(importContactObject.Index, num, ex4);
						}
						break;
					}
				}
			}
			return num;
		}

		protected virtual void PopulateContactProperties(ImportContactObject contact, Contact xsoContact, CultureInfo culture)
		{
			xsoContact[ContactSchema.FileAsId] = ContactUtils.GetDefaultFileAs(culture.LCID);
			foreach (KeyValuePair<ImportContactProperties, object> keyValuePair in contact.PropertyBag)
			{
				ImportContactProperties key = keyValuePair.Key;
				switch (key)
				{
				case ImportContactProperties.BusinessStreet:
				case ImportContactProperties.BusinessStreet2:
				case ImportContactProperties.BusinessStreet3:
				case ImportContactProperties.HomeStreet:
				case ImportContactProperties.HomeStreet2:
				case ImportContactProperties.HomeStreet3:
					continue;
				case ImportContactProperties.BusinessCity:
				case ImportContactProperties.BusinessState:
				case ImportContactProperties.BusinessPostalCode:
				case ImportContactProperties.BusinessCountryOrRegion:
				case ImportContactProperties.BusinessPOBox:
					break;
				default:
					switch (key)
					{
					case ImportContactProperties.OtherStreet:
					case ImportContactProperties.OtherStreet2:
					case ImportContactProperties.OtherStreet3:
						continue;
					default:
						switch (key)
						{
						case ImportContactProperties.EmailAddress:
						case ImportContactProperties.EmailType:
						case ImportContactProperties.EmailDisplayName:
						case ImportContactProperties.Email2Address:
						case ImportContactProperties.Email2Type:
						case ImportContactProperties.Email2DisplayName:
						case ImportContactProperties.Email3Address:
						case ImportContactProperties.Email3Type:
						case ImportContactProperties.Email3DisplayName:
							continue;
						case ImportContactProperties.Notes:
						{
							BodyFormat bodyFormat = BodyFormat.TextPlain;
							using (TextWriter textWriter = xsoContact.Body.OpenTextWriter(bodyFormat))
							{
								textWriter.Write(keyValuePair.Value);
								continue;
							}
							break;
						}
						}
						break;
					}
					break;
				}
				StorePropertyDefinition property = ImportContactXsoMapper.GetProperty(keyValuePair.Key);
				xsoContact[property] = keyValuePair.Value;
			}
			ImportContactBase.PopulateStreetAddress(contact, xsoContact);
			ImportContactBase.PopulateEmailAddress(contact, xsoContact);
		}

		private static void PopulateEmailAddress(ImportContactObject contact, Contact xsoContact)
		{
			ImportContactBase.PopulateEmail(contact, ImportContactProperties.EmailAddress, ImportContactProperties.EmailType, ImportContactProperties.EmailDisplayName, xsoContact);
			ImportContactBase.PopulateEmail(contact, ImportContactProperties.Email2Address, ImportContactProperties.Email2Type, ImportContactProperties.Email2DisplayName, xsoContact);
			ImportContactBase.PopulateEmail(contact, ImportContactProperties.Email3Address, ImportContactProperties.Email3Type, ImportContactProperties.Email3DisplayName, xsoContact);
		}

		private static void PopulateStreetAddress(ImportContactObject contact, Contact xsoContact)
		{
			ImportContactBase.PopulateStreet(contact, ImportContactProperties.BusinessStreet, ImportContactProperties.BusinessStreet2, ImportContactProperties.BusinessStreet3, xsoContact);
			ImportContactBase.PopulateStreet(contact, ImportContactProperties.HomeStreet, ImportContactProperties.HomeStreet2, ImportContactProperties.HomeStreet3, xsoContact);
			ImportContactBase.PopulateStreet(contact, ImportContactProperties.OtherStreet, ImportContactProperties.OtherStreet2, ImportContactProperties.OtherStreet3, xsoContact);
		}

		private static void PopulateEmail(ImportContactObject contact, ImportContactProperties emailAdress, ImportContactProperties emailType, ImportContactProperties emailDisplayName, Contact xsoContact)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			object obj;
			if (contact.PropertyBag.TryGetValue(emailAdress, out obj))
			{
				text = (string)obj;
			}
			object obj2;
			if (contact.PropertyBag.TryGetValue(emailType, out obj2))
			{
				text2 = (string)obj2;
			}
			object obj3;
			if (contact.PropertyBag.TryGetValue(emailDisplayName, out obj3))
			{
				text3 = (string)obj3;
			}
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3))
			{
				return;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "SMTP";
			}
			StorePropertyDefinition property = ImportContactXsoMapper.GetProperty(emailAdress);
			xsoContact[property] = new Participant(text3, text, text2);
		}

		private static void PopulateStreet(ImportContactObject contact, ImportContactProperties street1, ImportContactProperties street2, ImportContactProperties street3, Contact xsoContact)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			object obj;
			if (contact.PropertyBag.TryGetValue(street1, out obj))
			{
				stringBuilder.Append((string)obj);
			}
			object obj2;
			if (contact.PropertyBag.TryGetValue(street2, out obj2))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append((string)obj2);
			}
			object obj3;
			if (contact.PropertyBag.TryGetValue(street3, out obj3))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append((string)obj3);
			}
			if (stringBuilder.Length == 0)
			{
				return;
			}
			StorePropertyDefinition property = ImportContactXsoMapper.GetProperty(street1);
			xsoContact[property] = stringBuilder.ToString();
		}

		private const int MaxSaveRetries = 5;

		private MailboxSession mbxSession;
	}
}

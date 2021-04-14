using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class UserDataValidator : DisposableBase, IDataValidator, IDisposeTrackable, IDisposable
	{
		public UserDataValidator(UMSubscriber user)
		{
			this.subscriber = user;
			this.permChecker = new DialingPermissionsCheck(this.subscriber.ADRecipient as ADUser, this.subscriber.DialPlan);
			this.cache = new Dictionary<string, UserDataValidator.CacheItem>();
		}

		public bool ValidateADContactForOutdialing(string legacyExchangeDN, out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			PIIMessage piimessage = PIIMessage.Create(PIIType._PII, legacyExchangeDN);
			base.CheckDisposed();
			UserDataValidator.CacheItem cacheItem = null;
			ADRecipient adrecipient;
			if (this.cache.TryGetValue(legacyExchangeDN, out cacheItem))
			{
				adrecipient = cacheItem.Recipient;
				if (cacheItem.TransferToPhoneValidationResult != null)
				{
					dataValidationResult.PAAValidationResult = cacheItem.TransferToPhoneValidationResult.Value;
					dataValidationResult.PhoneNumber = cacheItem.NumberToDial;
					dataValidationResult.ADRecipient = adrecipient;
					PIIMessage piimessage2 = PIIMessage.Create(PIIType._PhoneNumber, (dataValidationResult.PhoneNumber != null) ? dataValidationResult.PhoneNumber.ToString() : "<null>");
					PIIMessage[] data = new PIIMessage[]
					{
						piimessage,
						piimessage2
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidateADContactForOutdialing(_PII) [FROM CACHE] returns status {0} NumberToDial [_PhoneNumber]", new object[]
					{
						result
					});
					return dataValidationResult.PAAValidationResult == PAAValidationResult.Valid;
				}
			}
			else
			{
				adrecipient = this.Resolve(legacyExchangeDN);
				cacheItem = new UserDataValidator.CacheItem(adrecipient);
				this.cache[legacyExchangeDN] = cacheItem;
			}
			if (adrecipient == null)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentDirectoryUser;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "UserDataValidator::ValidateADContactForOutdialing(_PII) returns status {0}", new object[]
				{
					result
				});
				cacheItem.TransferToPhoneValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				cacheItem.TransferToMailboxValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				return false;
			}
			DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = this.permChecker.CheckDirectoryUser(adrecipient, null);
			if (!dialingPermissionsCheckResult.HaveValidPhone)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.NoValidPhones;
				cacheItem.TransferToPhoneValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "UserDataValidator::ValidateADContactForOutdialing(_PII) returns status {0}", new object[]
				{
					result
				});
				return false;
			}
			if (!dialingPermissionsCheckResult.AllowCall)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.PermissionCheckFailure;
				cacheItem.TransferToPhoneValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "UserDataValidator::ValidateADContactForOutdialing(_PII) returns status {0}", new object[]
				{
					result
				});
				return false;
			}
			dataValidationResult.PhoneNumber = dialingPermissionsCheckResult.NumberToDial;
			dataValidationResult.ADRecipient = adrecipient;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "UserDataValidator::ValidateADContactForOutdialing(_PII) returns status {0} NumberToDial {1}[{2}]", new object[]
			{
				result,
				dataValidationResult.PhoneNumber.Number,
				dataValidationResult.PhoneNumber.ToDial
			});
			cacheItem.TransferToPhoneValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
			cacheItem.NumberToDial = dataValidationResult.PhoneNumber;
			return true;
		}

		public bool ValidateADContactForTransferToMailbox(string legacyExchangeDN, out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			bool result2 = true;
			PIIMessage data = PIIMessage.Create(PIIType._PII, legacyExchangeDN);
			base.CheckDisposed();
			UserDataValidator.CacheItem cacheItem = null;
			ADRecipient adrecipient;
			if (this.cache.TryGetValue(legacyExchangeDN, out cacheItem))
			{
				adrecipient = cacheItem.Recipient;
				if (cacheItem.TransferToMailboxValidationResult != null)
				{
					dataValidationResult.PAAValidationResult = cacheItem.TransferToMailboxValidationResult.Value;
					dataValidationResult.ADRecipient = adrecipient;
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidateADContactForOutdialing(_PII) [FROM CACHE] returns status {0}", new object[]
					{
						result
					});
					return dataValidationResult.PAAValidationResult == PAAValidationResult.Valid;
				}
			}
			else
			{
				adrecipient = this.Resolve(legacyExchangeDN);
				cacheItem = new UserDataValidator.CacheItem(adrecipient);
				this.cache[legacyExchangeDN] = cacheItem;
			}
			if (adrecipient == null)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentDirectoryUser;
				cacheItem.TransferToMailboxValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				cacheItem.TransferToPhoneValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				result2 = false;
			}
			else if (adrecipient.RecipientType != RecipientType.UserMailbox && adrecipient.RecipientType != RecipientType.MailContact && adrecipient.RecipientType != RecipientType.MailUser)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonMailboxDirectoryUser;
				cacheItem.TransferToMailboxValidationResult = new PAAValidationResult?(dataValidationResult.PAAValidationResult);
				dataValidationResult.ADRecipient = adrecipient;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidateADContactForTransferToMailbox(_PII) Recipient {0} is of invalid t ype {1}", new object[]
				{
					adrecipient.DisplayName,
					adrecipient.RecipientType.ToString()
				});
				result2 = false;
			}
			dataValidationResult.ADRecipient = adrecipient;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidateADContactForTransferToMailbox(_PII) returns status {0}", new object[]
			{
				result
			});
			return result2;
		}

		public bool ValidatePhoneNumberForOutdialing(string phoneNumber, out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			base.CheckDisposed();
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			PhoneNumber phoneNumber2 = null;
			if (!PhoneNumber.TryParse(this.subscriber.DialPlan, phoneNumber, out phoneNumber2))
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.ParseError;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidatePhoneNumberForOutdialing(_PhoneNumber) returns status {0}", new object[]
				{
					result
				});
				return false;
			}
			if (phoneNumber2.UriType == UMUriType.SipName && this.subscriber.DialPlan.URIType != UMUriType.SipName)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.SipUriInNonSipDialPlan;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidatePhoneNumberForOutdialing(_PhoneNumber) returns status {0}", new object[]
				{
					result
				});
				return false;
			}
			DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = this.permChecker.CheckPhoneNumber(phoneNumber2);
			if (!dialingPermissionsCheckResult.AllowCall)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.PermissionCheckFailure;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidatePhoneNumberForOutdialing(_PhoneNumber) returns status {0}", new object[]
				{
					result
				});
				return false;
			}
			dataValidationResult.PhoneNumber = dialingPermissionsCheckResult.NumberToDial;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidatePhoneNumberForOutdialing(_PhoneNumber) returns status {0} NumberToDial {1}[{2}]", new object[]
			{
				result,
				dataValidationResult.PhoneNumber,
				dataValidationResult.PhoneNumber.ToDial
			});
			return true;
		}

		public bool ValidateContactItemCallerId(StoreObjectId storeId, out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			bool result2 = true;
			base.CheckDisposed();
			if (storeId.ObjectType != StoreObjectType.Contact)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentContact;
				return result2;
			}
			try
			{
				this.BuildContactCache();
				if (!this.contactItemCache.IsContactValid(storeId))
				{
					result2 = false;
					dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentContact;
				}
				else
				{
					dataValidationResult.PersonalContactInfo = this.contactItemCache.GetContact(storeId);
				}
			}
			catch (ObjectNotFoundException)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentContact;
			}
			catch (ArgumentException)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentContact;
			}
			catch (CorruptDataException)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentContact;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::ValidateContactItemCallerId({0}) returns status {1}", new object[]
			{
				storeId.ToString(),
				dataValidationResult.PAAValidationResult
			});
			return result2;
		}

		public bool ValidateADContactCallerId(string legacyExchangeDN, out IDataValidationResult result)
		{
			bool result2 = true;
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(this.subscriber);
			ADRecipient adrecipient = iadrecipientLookup.LookupByLegacyExchangeDN(legacyExchangeDN);
			if (adrecipient == null)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentDirectoryUser;
			}
			dataValidationResult.ADRecipient = adrecipient;
			PIIMessage data = PIIMessage.Create(PIIType._PII, legacyExchangeDN);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidateADContactCallerId(_PII) returns status {0}", new object[]
			{
				result
			});
			return result2;
		}

		public bool ValidatePersonaContactCallerId(string emailAddress, out IDataValidationResult result)
		{
			bool result2 = true;
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			PersonaType personaType = null;
			using (IUMUserMailboxStorage umuserMailboxAccessor = InterServerMailboxAccessor.GetUMUserMailboxAccessor(this.subscriber.ADUser, false))
			{
				personaType = umuserMailboxAccessor.GetPersonaFromEmail(emailAddress);
			}
			if (personaType == null)
			{
				result2 = false;
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentPersona;
			}
			dataValidationResult.PersonaContactInfo = personaType;
			PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, emailAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "UserDataValidator::ValidatePersonaContactCallerId(_EmailAddress) returns status {0}", new object[]
			{
				result
			});
			return result2;
		}

		public bool ValidatePhoneNumberCallerId(string number, out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			bool result2 = true;
			base.CheckDisposed();
			PhoneNumber phoneNumber = null;
			if (!PhoneNumber.TryParse(this.subscriber.DialPlan, number, out phoneNumber))
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.ParseError;
				result2 = false;
			}
			UMUriType umuriType = Utils.DetermineNumberType(number);
			if (umuriType == UMUriType.SipName && this.subscriber.DialPlan.URIType != UMUriType.SipName)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.SipUriInNonSipDialPlan;
				result2 = false;
			}
			dataValidationResult.PhoneNumber = phoneNumber;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::ValidatePhoneNumberCallerId({0}) returns status {1}", new object[]
			{
				number,
				result
			});
			return result2;
		}

		public bool ValidateContactFolderCallerId(out IDataValidationResult result)
		{
			DataValidationResult dataValidationResult = new DataValidationResult();
			result = dataValidationResult;
			bool result2 = true;
			if (!this.subscriber.HasContactsFolder)
			{
				dataValidationResult.PAAValidationResult = PAAValidationResult.NonExistentDefaultContactsFolder;
				result2 = false;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::ValidateContactFolderCallerId() returns status {0}", new object[]
			{
				result
			});
			return result2;
		}

		public bool ValidateExtensions(IList<string> extensions, out PAAValidationResult result, out string extensionInError)
		{
			IList<string> extensionsInPrimaryDialPlan = this.subscriber.GetExtensionsInPrimaryDialPlan();
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			extensionInError = null;
			result = PAAValidationResult.Valid;
			if (extensions.Count == 0)
			{
				return true;
			}
			bool flag = false;
			foreach (string text in extensionsInPrimaryDialPlan)
			{
				if (dictionary.TryGetValue(text, out flag))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::ValidateExtensions() Extension '{0}' has been specified twice for the user", new object[]
					{
						text
					});
					result = PAAValidationResult.InvalidExtension;
					extensionInError = text;
					return false;
				}
				dictionary.Add(text, true);
			}
			flag = false;
			foreach (string text2 in extensions)
			{
				if (!dictionary.TryGetValue(text2, out flag))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::ValidateExtensions() Extension '{0}' is not on the list of extensions for user", new object[]
					{
						text2
					});
					extensionInError = text2;
					result = PAAValidationResult.InvalidExtension;
					return false;
				}
			}
			result = PAAValidationResult.Valid;
			return true;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserDataValidator>(this);
		}

		private ADRecipient Resolve(string legacyExchangeDN)
		{
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(this.subscriber);
			return iadrecipientLookup.LookupByLegacyExchangeDN(legacyExchangeDN);
		}

		private void BuildContactCache()
		{
			if (this.contactItemCache != null)
			{
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "UserDataValidator::BuildContactCache()", new object[0]);
			this.contactItemCache = PersonalContactCache.Create(this.subscriber);
			this.contactItemCache.BuildCache();
		}

		private UMSubscriber subscriber;

		private DialingPermissionsCheck permChecker;

		private Dictionary<string, UserDataValidator.CacheItem> cache;

		private IPersonalContactCache contactItemCache;

		private class CacheItem
		{
			internal CacheItem(ADRecipient recipient)
			{
				this.recipient = recipient;
			}

			internal ADRecipient Recipient
			{
				get
				{
					return this.recipient;
				}
			}

			internal PAAValidationResult? TransferToMailboxValidationResult
			{
				get
				{
					return this.txfrMailboxValidationResult;
				}
				set
				{
					this.txfrMailboxValidationResult = value;
				}
			}

			internal PAAValidationResult? TransferToPhoneValidationResult
			{
				get
				{
					return this.txfrPhoneValidationResult;
				}
				set
				{
					this.txfrPhoneValidationResult = value;
				}
			}

			internal PhoneNumber NumberToDial
			{
				get
				{
					return this.numberToDial;
				}
				set
				{
					this.numberToDial = value;
				}
			}

			private ADRecipient recipient;

			private PAAValidationResult? txfrPhoneValidationResult;

			private PAAValidationResult? txfrMailboxValidationResult;

			private PhoneNumber numberToDial;
		}
	}
}

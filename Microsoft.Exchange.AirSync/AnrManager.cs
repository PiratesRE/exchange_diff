using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AnrManager : DisposeTrackableBase
	{
		public AnrManager(IAirSyncUser user, MailboxSession mailboxSession, int lcid, int nameLimit)
		{
			this.user = user;
			this.mailboxSession = mailboxSession;
			this.lcid = lcid;
			this.nameLimit = nameLimit;
		}

		public void ResolveOneRecipient(string name, bool searchADFirst, AmbiguousRecipientToResolve recipient)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			List<ResolvedRecipient> list = new List<ResolvedRecipient>();
			List<ResolvedRecipient> list2 = new List<ResolvedRecipient>();
			string text;
			string text2;
			bool flag = AnrManager.TryParseNameBeforeAnr(name, out text, out text2);
			if (searchADFirst)
			{
				switch (this.GetNamesByAnrFromAD(flag ? string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					text,
					text2
				}) : name, list2))
				{
				case AnrManager.LookupState.FoundNone:
				{
					AnrManager.LookupState namesByAnrFromContacts = this.GetNamesByAnrFromContacts(flag ? text2 : name, list);
					recipient.ExactMatchFound = (namesByAnrFromContacts == AnrManager.LookupState.FoundExact);
					break;
				}
				case AnrManager.LookupState.FoundExact:
					recipient.ExactMatchFound = true;
					break;
				case AnrManager.LookupState.FoundMany:
					this.GetNamesByAnrFromContacts(flag ? text2 : name, list);
					recipient.ExactMatchFound = false;
					break;
				}
			}
			else
			{
				switch (this.GetNamesByAnrFromContacts(flag ? text2 : name, list))
				{
				case AnrManager.LookupState.FoundNone:
				{
					AnrManager.LookupState namesByAnrFromAD = this.GetNamesByAnrFromAD(flag ? string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
					{
						text,
						text2
					}) : name, list2);
					recipient.ExactMatchFound = (namesByAnrFromAD == AnrManager.LookupState.FoundExact);
					break;
				}
				case AnrManager.LookupState.FoundExact:
					recipient.ExactMatchFound = true;
					break;
				case AnrManager.LookupState.FoundMany:
					this.GetNamesByAnrFromAD(flag ? string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
					{
						text,
						text2
					}) : name, list2);
					recipient.ExactMatchFound = false;
					break;
				}
			}
			list.Sort();
			list2.Sort();
			list.AddRange(list2);
			int num = recipient.ExactMatchFound ? list.Count : Math.Min(this.nameLimit, list.Count);
			recipient.ResolvedNamesCount = list.Count;
			recipient.CompleteList = (num == list.Count);
			list.RemoveRange(num, list.Count - num);
			recipient.ResolvedTo = list;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.addressBookSession != null)
			{
				this.addressBookSession.Dispose();
				this.addressBookSession = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AnrManager>(this);
		}

		private static bool TryParseNameBeforeAnr(string name, out string routingType, out string routingAddress)
		{
			routingType = string.Empty;
			routingAddress = string.Empty;
			Participant participant;
			if (!Participant.TryParse(name, out participant) || participant.RoutingType == null)
			{
				return false;
			}
			routingType = participant.RoutingType;
			routingAddress = participant.EmailAddress;
			return true;
		}

		private ABSession GetAddressBookSession()
		{
			if (this.addressBookSession == null)
			{
				IABSessionSettings sessionSettings = ABDiscoveryManager.GetSessionSettings(this.user.ExchangePrincipal, new int?(this.lcid), new ConsistencyMode?(ConsistencyMode.IgnoreInvalid), GlobalSettings.SyncLog, this.user.ClientSecurityContextWrapper.ClientSecurityContext);
				this.addressBookSession = ADABSession.Create(sessionSettings);
			}
			return this.addressBookSession;
		}

		private RecipientAddress ConstructStoreRecipientAddress(Participant participant, string displayName, StoreObjectId storeId)
		{
			RecipientAddress recipientAddress = new RecipientAddress();
			recipientAddress.DisplayName = displayName;
			recipientAddress.AddressOrigin = AddressOrigin.Store;
			recipientAddress.RoutingType = participant.RoutingType;
			if (!string.IsNullOrEmpty(participant.EmailAddress))
			{
				recipientAddress.RoutingAddress = participant.EmailAddress;
				if (string.Equals(recipientAddress.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
				{
					string text = participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
					if (string.IsNullOrEmpty(text))
					{
						ABObject recipient = null;
						OperationRetryManagerResult operationRetryManagerResult = AnrManager.retryManager.TryRun(delegate
						{
							recipient = this.GetAddressBookSession().FindByLegacyExchangeDN(recipientAddress.RoutingAddress);
						});
						if (!operationRetryManagerResult.Succeeded)
						{
							if (operationRetryManagerResult.Exception is ABProviderLoadException)
							{
								throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, EASServerStrings.UnableToLoadAddressBookProvider, operationRetryManagerResult.Exception, true)
								{
									ErrorStringForProtocolLogger = "ABProviderNotFound"
								};
							}
							if (operationRetryManagerResult.Exception is ABSubscriptionDisabledException)
							{
								throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, operationRetryManagerResult.Exception, false)
								{
									ErrorStringForProtocolLogger = "ABSubscriptionDisabled"
								};
							}
							if (operationRetryManagerResult.Exception is DataValidationException)
							{
								throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, operationRetryManagerResult.Exception, false)
								{
									ErrorStringForProtocolLogger = "AnrError:ADDataInvalid"
								};
							}
							if (operationRetryManagerResult.Exception is ABOperationException)
							{
								ABOperationException ex = (ABOperationException)operationRetryManagerResult.Exception;
								throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, operationRetryManagerResult.Exception, false)
								{
									ErrorStringForProtocolLogger = "AnrError:ABOperationException"
								};
							}
							if (operationRetryManagerResult.Exception is ABTransientException)
							{
								AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestTracer, null, "AnrManager.ConstructStoreRecipientAddress(): ABTransientException was thrown by FindByLegacyExchangeDN: {0}", operationRetryManagerResult.Exception.Message);
							}
						}
						if (recipient == null)
						{
							return null;
						}
						recipientAddress.SmtpAddress = recipient.EmailAddress;
					}
					else
					{
						recipientAddress.SmtpAddress = text;
					}
				}
				else
				{
					if (!string.Equals(recipientAddress.RoutingType, "SMTP", StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}
					recipientAddress.SmtpAddress = recipientAddress.RoutingAddress;
				}
			}
			recipientAddress.StoreObjectId = storeId;
			return recipientAddress;
		}

		private void ExpandPDL(RecipientAddress address, List<ResolvedRecipient> recipients)
		{
			Participant[] array;
			try
			{
				array = DistributionList.ExpandDeep(this.mailboxSession, address.StoreObjectId);
			}
			catch (ObjectNotFoundException)
			{
				return;
			}
			foreach (Participant participant in array)
			{
				StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null)
				{
					StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
					RecipientAddress recipientAddress;
					try
					{
						recipientAddress = this.ConstructStoreRecipientAddress(participant, participant.DisplayName, originItemId);
					}
					catch (ObjectNotFoundException)
					{
						goto IL_FF;
					}
					if (recipientAddress != null)
					{
						recipients.Add(new ResolvedRecipient(recipientAddress));
					}
				}
				else
				{
					RecipientAddress recipientAddress2 = new RecipientAddress();
					recipientAddress2.RoutingType = participant.RoutingType;
					recipientAddress2.SmtpAddress = (participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string);
					recipientAddress2.DisplayName = participant.DisplayName;
					recipientAddress2.RoutingAddress = participant.EmailAddress;
					if (!string.IsNullOrEmpty(recipientAddress2.RoutingType) && participant.RoutingType == "EX")
					{
						recipientAddress2.AddressOrigin = AddressOrigin.Directory;
					}
					else
					{
						recipientAddress2.AddressOrigin = AddressOrigin.OneOff;
					}
					if (recipientAddress2.SmtpAddress != null)
					{
						recipients.Add(new ResolvedRecipient(recipientAddress2));
					}
				}
				IL_FF:;
			}
		}

		private AnrManager.LookupState GetNamesByAnrFromAD(string name, List<ResolvedRecipient> recipients)
		{
			IList<ABObject> addressBookObjects = null;
			OperationRetryManagerResult operationRetryManagerResult = AnrManager.retryManager.TryRun(delegate
			{
				addressBookObjects = this.GetAddressBookSession().FindByANR(name, 1000);
			});
			if (operationRetryManagerResult.Succeeded)
			{
				using (IEnumerator<ABObject> enumerator = addressBookObjects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ABObject abobject = enumerator.Current;
						if (abobject.CanEmail)
						{
							ResolvedRecipient resolvedRecipient = new ResolvedRecipient(new RecipientAddress
							{
								AddressOrigin = AddressOrigin.Directory,
								DisplayName = abobject.DisplayName,
								RoutingAddress = abobject.LegacyExchangeDN,
								RoutingType = "EX",
								SmtpAddress = abobject.EmailAddress
							});
							ABContact abcontact = abobject as ABContact;
							if (abcontact != null)
							{
								resolvedRecipient.Picture = abcontact.Picture;
							}
							recipients.Add(resolvedRecipient);
						}
					}
					goto IL_F7;
				}
			}
			AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestTracer, 0, "AnrManager.GetNamesByAnrFromAD(): Exception thrown by FindByANR: {0}", operationRetryManagerResult.Exception);
			IL_F7:
			switch (recipients.Count)
			{
			case 0:
				return AnrManager.LookupState.FoundNone;
			case 1:
				return AnrManager.LookupState.FoundExact;
			default:
				return AnrManager.LookupState.FoundMany;
			}
		}

		private AnrManager.LookupState GetNamesByAnrFromContacts(string name, List<ResolvedRecipient> recipients)
		{
			if (this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts) == null)
			{
				return AnrManager.LookupState.FoundNone;
			}
			AnrManager.LookupState result;
			using (ContactsFolder contactsFolder = ContactsFolder.Bind(this.mailboxSession, DefaultFolderType.Contacts))
			{
				if (!contactsFolder.IsValidAmbiguousName(name))
				{
					result = AnrManager.LookupState.FoundNone;
				}
				else
				{
					object[][] array = contactsFolder.ResolveAmbiguousNameView(name, int.MaxValue, null, AnrManager.anrContactProperties);
					List<RecipientAddress> list = new List<RecipientAddress>();
					int num = 0;
					while (array != null && num < array.GetLength(0))
					{
						object[] array2 = array[num];
						Participant participant = array2[1] as Participant;
						if (participant != null)
						{
							string displayName = array2[0] as string;
							VersionedId versionedId = (VersionedId)array2[2];
							StoreObjectId storeId = (versionedId == null) ? null : versionedId.ObjectId;
							RecipientAddress recipientAddress = this.ConstructStoreRecipientAddress(participant, displayName, storeId);
							if (recipientAddress != null)
							{
								if (recipientAddress.RoutingType != null && string.Equals(recipientAddress.RoutingType, "MAPIPDL", StringComparison.OrdinalIgnoreCase))
								{
									list.Add(recipientAddress);
								}
								else
								{
									recipients.Add(new ResolvedRecipient(recipientAddress));
								}
							}
						}
						num++;
					}
					bool flag = recipients.Count + list.Count == 1;
					foreach (RecipientAddress address in list)
					{
						this.ExpandPDL(address, recipients);
					}
					if (recipients.Count == 0)
					{
						result = AnrManager.LookupState.FoundNone;
					}
					else if (flag)
					{
						result = AnrManager.LookupState.FoundExact;
					}
					else
					{
						result = AnrManager.LookupState.FoundMany;
					}
				}
			}
			return result;
		}

		private const string ADAnrLookupFormat = "{0}:{1}";

		private const int MaxRetryCount = 3;

		private static PropertyDefinition[] anrContactProperties = new PropertyDefinition[]
		{
			ParticipantSchema.DisplayName,
			ContactBaseSchema.AnrViewParticipant,
			ItemSchema.Id
		};

		private static ABOperationRetryManager retryManager = new ABOperationRetryManager(3);

		private readonly int lcid;

		private readonly int nameLimit;

		private IAirSyncUser user;

		private MailboxSession mailboxSession;

		private ABSession addressBookSession;

		private enum LookupState
		{
			FoundNone,
			FoundExact,
			FoundMany
		}
	}
}

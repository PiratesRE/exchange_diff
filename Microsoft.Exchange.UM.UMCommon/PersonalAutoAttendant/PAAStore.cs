using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PAAStore : DisposableBase, IPAAStore, IDisposeTrackable, IDisposable
	{
		private PAAStore(UMSubscriber u)
		{
			u.AddReference();
			this.Initialize(u);
		}

		private PAAStore(ExchangePrincipal principal)
		{
			try
			{
				if (principal == null)
				{
					throw new ArgumentNullException("principal");
				}
				UMSubscriber umsubscriber = UMRecipient.Factory.FromPrincipal<UMSubscriber>(principal);
				if (umsubscriber == null)
				{
					throw new InvalidPrincipalException();
				}
				this.Initialize(umsubscriber);
			}
			catch (LocalizedException ex)
			{
				this.DebugTrace("{0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		public static IPAAStore Create(UMSubscriber u)
		{
			return new PAAStore(u);
		}

		public static IPAAStore Create(ExchangePrincipal principal)
		{
			return new PAAStore(principal);
		}

		public IList<PersonalAutoAttendant> GetAutoAttendants()
		{
			base.CheckDisposed();
			PAAStoreStatus paastoreStatus;
			return this.GetAutoAttendants(PAAValidationMode.None, out paastoreStatus);
		}

		public IList<PersonalAutoAttendant> GetAutoAttendants(PAAValidationMode validationMode)
		{
			if (validationMode == PAAValidationMode.StopOnFirstError)
			{
				throw new LocalizedException(new LocalizedString("Some exception"));
			}
			PAAStoreStatus paastoreStatus;
			return this.GetAutoAttendants(validationMode, out paastoreStatus);
		}

		public bool TryGetAutoAttendants(PAAValidationMode validationMode, out IList<PersonalAutoAttendant> autoAttendants)
		{
			PAAStoreStatus paastoreStatus;
			autoAttendants = this.GetAutoAttendantsFromStore(validationMode, out paastoreStatus, true);
			return autoAttendants != null && autoAttendants.Count > 0;
		}

		public IList<PersonalAutoAttendant> GetAutoAttendants(PAAValidationMode validationMode, out PAAStoreStatus storeStatus)
		{
			return this.GetAutoAttendantsFromStore(validationMode, out storeStatus, false);
		}

		public void Save(IList<PersonalAutoAttendant> autoattendants)
		{
			base.CheckDisposed();
			if (autoattendants == null)
			{
				throw new ArgumentNullException("autoattendants");
			}
			if (autoattendants.Count > 9)
			{
				throw new MaxPAACountReachedException(9);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::Save() #autoattendants={0}", new object[]
			{
				autoattendants.Count
			});
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
			{
				using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
				{
					using (Stream stream = config.GetStream())
					{
						stream.SetLength(0L);
						PAAParser.Instance.Serialize(autoattendants, stream);
						config.Save();
					}
				}
			}
		}

		public PersonalAutoAttendant GetAutoAttendant(Guid identity, PAAValidationMode validationMode)
		{
			base.CheckDisposed();
			IList<PersonalAutoAttendant> autoAttendants = this.GetAutoAttendants();
			int num = -1;
			PersonalAutoAttendant personalAutoAttendant = this.FindAutoAttendantByGuid(autoAttendants, identity, out num);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendant(id= {0},validationMode = {1}) returning autoattendant={2}", new object[]
			{
				identity,
				validationMode,
				(personalAutoAttendant != null) ? personalAutoAttendant.Name : "<null>"
			});
			if (personalAutoAttendant != null && validationMode != PAAValidationMode.None)
			{
				this.Validate(personalAutoAttendant, validationMode);
			}
			return personalAutoAttendant;
		}

		public bool TryGetAutoAttendant(Guid identity, PAAValidationMode validationMode, out PersonalAutoAttendant autoAttendant)
		{
			base.CheckDisposed();
			autoAttendant = null;
			IList<PersonalAutoAttendant> autoattendants = null;
			if (!this.TryGetAutoAttendants(validationMode, out autoattendants))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::TryGetAutoAttendant(id= {0},validationMode = {1}) Did not find autoattendants", new object[]
				{
					identity,
					validationMode
				});
				return false;
			}
			int num = -1;
			autoAttendant = this.FindAutoAttendantByGuid(autoattendants, identity, out num);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::TryGetAutoAttendant(id= {0},validationMode = {1}) returning autoattendant={2}", new object[]
			{
				identity,
				validationMode,
				(autoAttendant != null) ? autoAttendant.Name : "<null>"
			});
			if (autoAttendant != null && validationMode != PAAValidationMode.None)
			{
				this.Validate(autoAttendant, validationMode);
			}
			return autoAttendant != null;
		}

		public void DeleteAutoAttendant(Guid identity)
		{
			base.CheckDisposed();
			IList<PersonalAutoAttendant> autoAttendants = this.GetAutoAttendants();
			int num = -1;
			PersonalAutoAttendant paa = this.FindAutoAttendantByGuid(autoAttendants, identity, out num);
			if (num == -1)
			{
				throw new ObjectNotFoundException(Strings.PersonalAutoAttendantNotFound(identity.ToString()));
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::DeleteAutoAttendant({0}) deleting autoattendant at index = {1}", new object[]
			{
				identity,
				num
			});
			autoAttendants.RemoveAt(num);
			this.Save(autoAttendants);
			this.DeleteGreeting(paa);
		}

		public void DeletePAAConfiguration()
		{
			base.CheckDisposed();
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::Delete()", new object[0]);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
			{
				mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					"UM.E14.PersonalAutoAttendants"
				});
			}
		}

		public GreetingBase OpenGreeting(PersonalAutoAttendant paa)
		{
			base.CheckDisposed();
			if (paa == null)
			{
				throw new ArgumentNullException("paa");
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::OpenGreeting({0}) opening autoattendant greeting for autoattendant = {1}", new object[]
			{
				paa.Name,
				paa.Identity
			});
			object[] args = new object[]
			{
				this.subscriber,
				paa.Greeting
			};
			return (GreetingBase)Activator.CreateInstance(XsoConfigurationFolder.CoreTypeLoader.XsoGreetingType, BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
		}

		public void GetUserPermissions(out bool enabledForPersonalAutoAttendant, out bool enabledForOutdialing)
		{
			base.CheckDisposed();
			enabledForPersonalAutoAttendant = this.subscriber.IsPAAEnabled;
			enabledForOutdialing = this.subscriber.IsEnabledForOutcalling;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetUserPermissions() returns paaEnabled={0} outdialingEnabled={1}", new object[]
			{
				enabledForPersonalAutoAttendant,
				enabledForOutdialing
			});
		}

		public bool Validate(PersonalAutoAttendant paa, PAAValidationMode validationMode)
		{
			base.CheckDisposed();
			if (paa == null)
			{
				throw new ArgumentNullException("paa");
			}
			bool flag = false;
			if (validationMode != PAAValidationMode.None)
			{
				flag = paa.Validate(this.dataValidator, validationMode);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::Validate(ID={0},Enabled={1} Mode={2}) returns {3}", new object[]
			{
				paa.Identity,
				paa.Enabled,
				validationMode,
				flag
			});
			return flag;
		}

		public IList<string> GetExtensionsInPrimaryDialPlan()
		{
			base.CheckDisposed();
			return Utils.GetExtensionsInDialPlanValidForPAA(this.subscriber.DialPlan, this.subscriber.ADRecipient);
		}

		public bool ValidatePhoneNumberForOutdialing(string number, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidatePhoneNumberForOutdialing(number, out result);
		}

		public bool ValidateADContactForOutdialing(string legacyExchangeDN, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidateADContactForOutdialing(legacyExchangeDN, out result);
		}

		public bool ValidateADContactForTransferToMailbox(string legacyExchangeDN, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidateADContactForTransferToMailbox(legacyExchangeDN, out result);
		}

		public bool ValidateContactItemCallerId(StoreObjectId storeId, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidateContactItemCallerId(storeId, out result);
		}

		public bool ValidateADContactCallerId(string exchangeLegacyDN, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidateADContactCallerId(exchangeLegacyDN, out result);
		}

		public bool ValidatePhoneNumberCallerId(string number, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidatePhoneNumberCallerId(number, out result);
		}

		public bool ValidateContactFolderCallerId(out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidateContactFolderCallerId(out result);
		}

		public bool ValidatePersonaContactCallerId(string emailAddress, out IDataValidationResult result)
		{
			base.CheckDisposed();
			return this.dataValidator.ValidatePersonaContactCallerId(emailAddress, out result);
		}

		public void DeleteGreeting(PersonalAutoAttendant paa)
		{
			if (paa != null && paa.Greeting != null)
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
				{
					mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
					{
						"Um.CustomGreetings." + paa.Greeting
					});
				}
			}
		}

		public bool ValidateExtensions(IList<string> extensions, out PAAValidationResult result, out string extensionInError)
		{
			base.CheckDisposed();
			ValidateArgument.NotNull(extensions, "extensions");
			return this.dataValidator.ValidateExtensions(extensions, out result, out extensionInError);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.dataValidator != null)
				{
					this.dataValidator.Dispose();
				}
				this.subscriber.ReleaseReference();
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::Dispose() disposed = {0}", new object[]
				{
					base.IsDisposed
				});
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PAAStore>(this);
		}

		private PersonalAutoAttendant FindAutoAttendantByGuid(IList<PersonalAutoAttendant> autoattendants, Guid identity, out int index)
		{
			index = -1;
			PersonalAutoAttendant result = null;
			if (autoattendants != null)
			{
				for (int i = 0; i < autoattendants.Count; i++)
				{
					if (autoattendants[i].Identity == identity)
					{
						index = i;
						result = autoattendants[i];
						break;
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendantByGuid({0}) found autoattendant at index = {1}", new object[]
			{
				identity,
				index
			});
			return result;
		}

		private void DebugTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this.GetHashCode(), this.tracePrefix + formatString, formatObjects);
		}

		private void Initialize(UMSubscriber user)
		{
			this.subscriber = user;
			this.dataValidator = new UserDataValidator(user);
			this.tracePrefix = string.Format(CultureInfo.InvariantCulture, "{0}({1}): ", new object[]
			{
				base.GetType().Name,
				this.subscriber.ExchangePrincipal.MailboxInfo.DisplayName
			});
		}

		private UserConfiguration GetConfig(MailboxSession session)
		{
			base.CheckDisposed();
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = session.UserConfigurationManager.GetMailboxConfiguration("UM.E14.PersonalAutoAttendants", UserConfigurationTypes.Stream);
			}
			catch (ObjectNotFoundException)
			{
				PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, this.subscriber.MailAddress);
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, data, "Creating UM General configuration folder for user: _EmailAddress.", new object[0]);
				userConfiguration = session.UserConfigurationManager.CreateMailboxConfiguration("UM.E14.PersonalAutoAttendants", UserConfigurationTypes.Stream);
			}
			catch (CorruptDataException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, this, "Exception : {0}", new object[]
				{
					ex
				});
				throw;
			}
			catch (InvalidOperationException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, this, "Exception : {0}", new object[]
				{
					ex2
				});
				throw;
			}
			if (userConfiguration == null)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._UserDisplayName, this.subscriber.ADRecipient.DisplayName);
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, this, data2, "get_GeneralConfiguration() returning NULL configuration for user: _DisplayName", new object[0]);
				throw new InvalidOperationException("Could not bind to PAA store");
			}
			return userConfiguration;
		}

		private IList<PersonalAutoAttendant> GetAutoAttendantsFromStore(PAAValidationMode validationMode, out PAAStoreStatus storeStatus, bool suppressExceptions)
		{
			base.CheckDisposed();
			storeStatus = PAAStoreStatus.None;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() validate={0}", new object[]
			{
				validationMode
			});
			LatencyDetectionContext latencyDetectionContext = null;
			IList<PersonalAutoAttendant> list = new PAAStore.PAAList();
			if (this.subscriber.RequiresRedirectForCallAnswering())
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() subscriber RequiresRedirectForCallAnswering", new object[0]);
				return list;
			}
			try
			{
				latencyDetectionContext = PAAUtils.GetAutoAttendantsFromStoreFactory.CreateContext(CommonConstants.ApplicationVersion, CallId.Id ?? string.Empty, new IPerformanceDataProvider[]
				{
					RpcDataProvider.Instance,
					PerformanceContext.Current
				});
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
				{
					using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
					{
						using (Stream stream = config.GetStream())
						{
							if (stream.Length > 0L)
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() commencing parsing stream from mailbox [bytes = {0}]", new object[]
								{
									stream.Length
								});
								PAAParser.Instance.Parse(list, stream);
								storeStatus = PAAStoreStatus.Valid;
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() finished parsing stream from mailbox", new object[0]);
							}
						}
					}
				}
			}
			catch (XmlException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() Exception = {0}", new object[]
				{
					ex
				});
				storeStatus = PAAStoreStatus.Corrupted;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CorruptedPAAStore, this.subscriber.MailAddress, new object[]
				{
					this.subscriber.MailAddress
				});
				if (!suppressExceptions)
				{
					throw;
				}
			}
			catch (CorruptedPAAStoreException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() Exception = {0}", new object[]
				{
					ex2
				});
				storeStatus = PAAStoreStatus.Corrupted;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CorruptedPAAStore, this.subscriber.MailAddress, new object[]
				{
					this.subscriber.MailAddress
				});
				if (!suppressExceptions)
				{
					throw;
				}
			}
			catch (LocalizedException ex3)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() Exception = {0}", new object[]
				{
					ex3
				});
				if (!suppressExceptions)
				{
					throw;
				}
				return list;
			}
			finally
			{
				TaskPerformanceData[] array = latencyDetectionContext.StopAndFinalizeCollection();
				TaskPerformanceData taskPerformanceData = array[0];
				PerformanceData end = taskPerformanceData.End;
				if (end != PerformanceData.Zero)
				{
					PerformanceData difference = taskPerformanceData.Difference;
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() GetAutoAttendants from Config [RPCRequests = {0} RPCLatency = {1}", new object[]
					{
						difference.Count,
						difference.Milliseconds
					});
				}
			}
			if (storeStatus == PAAStoreStatus.Corrupted)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CorruptedPAAStore, this.subscriber.MailAddress, new object[]
				{
					this.subscriber.MailAddress
				});
				return list;
			}
			if (validationMode != PAAValidationMode.None)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (!list[i].IsCompatible)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() AutoAttendant with ID {0} is Incompatible. Skipping validation", new object[]
						{
							list[i].Identity
						});
					}
					else
					{
						this.Validate(list[i], validationMode);
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAStore::GetAutoAttendants() returning #autoattendants={0} StoreStatus = {1}", new object[]
			{
				list.Count,
				storeStatus
			});
			return list;
		}

		private const int MaxPAACount = 9;

		private UMSubscriber subscriber;

		private string tracePrefix = string.Empty;

		private IDataValidator dataValidator;

		internal class PAAList : IList<PersonalAutoAttendant>, ICollection<PersonalAutoAttendant>, IEnumerable<PersonalAutoAttendant>, IEnumerable
		{
			internal PAAList()
			{
				this.autoattendants = new List<PersonalAutoAttendant>();
			}

			public int Count
			{
				get
				{
					return this.autoattendants.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public PersonalAutoAttendant this[int index]
			{
				get
				{
					return this.autoattendants[index];
				}
				set
				{
					this.autoattendants[index] = value;
				}
			}

			public int IndexOf(PersonalAutoAttendant item)
			{
				return this.autoattendants.IndexOf(item);
			}

			public void Insert(int index, PersonalAutoAttendant item)
			{
				this.autoattendants.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				this.autoattendants.RemoveAt(index);
			}

			public void Add(PersonalAutoAttendant item)
			{
				this.autoattendants.Add(item);
			}

			public void Clear()
			{
				this.autoattendants.Clear();
			}

			public bool Contains(PersonalAutoAttendant item)
			{
				return this.autoattendants.Contains(item);
			}

			public void CopyTo(PersonalAutoAttendant[] array, int arrayIndex)
			{
				this.autoattendants.CopyTo(array, arrayIndex);
			}

			public bool Remove(PersonalAutoAttendant item)
			{
				return this.autoattendants.Remove(item);
			}

			public IEnumerator<PersonalAutoAttendant> GetEnumerator()
			{
				return this.autoattendants.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.autoattendants.GetEnumerator();
			}

			private List<PersonalAutoAttendant> autoattendants;
		}
	}
}

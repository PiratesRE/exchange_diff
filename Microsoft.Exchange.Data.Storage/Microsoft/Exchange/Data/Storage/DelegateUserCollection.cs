using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelegateUserCollection : Collection<DelegateUser>, IDelegateUserCollectionBridge
	{
		public DelegateRestoreInfo DelegateRestoreInfo
		{
			get
			{
				return this.restoreInfo;
			}
		}

		public DelegateUserCollection(MailboxSession session)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			session.CheckCapabilities(session.Capabilities.CanHaveDelegateUsers, "CanHaveDelegateUsers");
			this.session = session;
			this.restoreInfo = default(DelegateRestoreInfo);
			this.testBridge = this;
			this.isCrossPremiseDelegateAllowed = this.GetCrossPremiseDelegateStatus();
			this.Load();
		}

		private void Load()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			IExchangePrincipal[] array;
			string[] array2;
			string[] array3;
			int[] array4;
			int[] array5;
			this.delegateRuleType = this.LoadOutlookDelegateInfo(list, out array, out array2, out array3, out array4, out array5);
			if (array2.Length == 0)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Load. No delegates found.");
				this.restoreInfo.SendOnBehalf = new List<ADObjectId>(0);
				return;
			}
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("Number of delegates for the user: {0}, X-premise delegates: {1}", array2.Length, list.Count));
			int num = 0;
			for (int i = 0; i < array2.Length; i++)
			{
				DelegateUser delegateUser;
				if (this.isCrossPremiseDelegateAllowed && array[i] == null && list.Count > num)
				{
					string key = list[num].Key;
					string value = list[num].Value;
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("Cross premise delegate, Name: {0} , SMTP: {1}", key, value));
					delegateUser = DelegateUser.InternalCreate(key, value, new Dictionary<DefaultFolderType, PermissionLevel>(DelegateUserCollection.Folders.Length));
					this.GetADRecipient(delegateUser);
					if (delegateUser.ADRecipient == null)
					{
						delegateUser.Name = array2[i];
						delegateUser.Problems = DelegateProblems.NoADUser;
						delegateUser.LegacyDistinguishedName = array3[i];
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("No ADuser found for X-Premise delegate; name: {0} , LegacyDistinguishedName: {1}", delegateUser.Name, delegateUser.LegacyDistinguishedName));
					}
					num++;
				}
				else
				{
					delegateUser = DelegateUser.InternalCreate(array[i], new Dictionary<DefaultFolderType, PermissionLevel>(DelegateUserCollection.Folders.Length));
					if (array[i] == null)
					{
						delegateUser.Name = array2[i];
						delegateUser.Problems = DelegateProblems.NoADUser;
						delegateUser.LegacyDistinguishedName = array3[i];
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("No ADuser found for same forest delegate; name: {0} , LegacyDistinguishedName: {1}", delegateUser.Name, delegateUser.LegacyDistinguishedName));
					}
				}
				if (i < array4.Length)
				{
					delegateUser.CanViewPrivateItems = ((array4[i] & DelegateUserCollection.CanViewPrivateItemsFlag) == DelegateUserCollection.CanViewPrivateItemsFlag);
				}
				if (i < array5.Length)
				{
					delegateUser.Flags2 = array5[i];
				}
				ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::Load. Adding user {0}.", delegateUser.Name);
				base.Add(delegateUser);
			}
			if (!this.session.MailboxOwner.ObjectId.IsNullOrEmpty())
			{
				this.LoadFolderPermissions();
				this.LoadDelegateRule();
				IExchangePrincipal exchangePrincipal = DelegateUserCollection.InternalGetNonCachedExchangePrincipal(this.session);
				IEnumerable<ADObjectId> delegates = exchangePrincipal.Delegates;
				this.restoreInfo.SendOnBehalf = new List<ADObjectId>(delegates);
				using (IEnumerator<DelegateUser> enumerator = base.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DelegateUser delegateUser2 = enumerator.Current;
						if (delegateUser2.PrimarySmtpAddress != null && ((delegateUser2.Delegate != null && !delegates.Contains(delegateUser2.Delegate.ObjectId)) || (delegateUser2.ADRecipient != null && !delegates.Contains(delegateUser2.ADRecipient.Id))))
						{
							ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::Load. No public delegate or Send on Behalf entry in AD for: {0}.", delegateUser2.PrimarySmtpAddress);
							delegateUser2.Problems |= DelegateProblems.NoADPublicDelegate;
						}
						if (!delegateUser2.IsReceiveMeetingMessageCopiesValid)
						{
							ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::Load. ReceiveMeetingMessageCopies is set but lack required permissions for: {0}.", delegateUser2.PrimarySmtpAddress);
							delegateUser2.Problems |= DelegateProblems.InvalidReceiveMeetingMessageCopies;
						}
					}
					return;
				}
			}
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Load. Setting restoreInfo as MailboxOwner info is not complete.");
			this.restoreInfo.SendOnBehalf = new List<ADObjectId>(0);
		}

		public DelegateRuleType DelegateRuleType
		{
			get
			{
				this.CheckSaved();
				ExTraceGlobals.DelegateTracer.TraceDebug<DelegateRuleType>((long)this.GetHashCode(), "DelegateUserCollection::DelegateRuleType. Delegate rule type: {0}", this.delegateRuleType);
				return this.delegateRuleType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<DelegateRuleType>(value, "value");
				ExTraceGlobals.DelegateTracer.TraceDebug<DelegateRuleType>((long)this.GetHashCode(), "DelegateUserCollection::DelegateRuleType. Setting Delegate rule type: {0}", value);
				this.CheckSaved();
				this.delegateRuleType = value;
			}
		}

		public bool Remove(IExchangePrincipal delegatePrincipal)
		{
			if (delegatePrincipal == null)
			{
				throw new ArgumentNullException("delegatePrincipal");
			}
			ExTraceGlobals.DelegateTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "DelegateUserCollection::Remove. Attempting to remove delegate: {0}", delegatePrincipal);
			this.CheckSaved();
			foreach (DelegateUser delegateUser in this)
			{
				if (delegateUser.Delegate != null && delegateUser.Delegate.LegacyDn.Equals(delegatePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::Remove. Removing user: {0}", delegateUser.Delegate.LegacyDn);
					return base.Remove(delegateUser);
				}
			}
			return false;
		}

		internal void SetIDelegateUserCollectionBridge(IDelegateUserCollectionBridge testBridge)
		{
			this.testBridge = testBridge;
		}

		public IList<Exception> CreateDelegateForwardingRule()
		{
			return new List<Exception>();
		}

		public IList<Exception> UpdateSendOnBehalfOfPermissions()
		{
			return new List<Exception>();
		}

		public IList<Exception> SetFolderPermissions()
		{
			return new List<Exception>();
		}

		public IList<Exception> SetOulookLocalFreeBusyData()
		{
			return new List<Exception>();
		}

		public IList<Exception> RollbackDelegateState()
		{
			return new List<Exception>();
		}

		protected override void InsertItem(int index, DelegateUser delegateUser)
		{
			if (delegateUser == null)
			{
				throw new ArgumentNullException("delegateUser");
			}
			this.CheckSaved();
			this.Validate(delegateUser);
			this.GetADRecipient(delegateUser);
			base.InsertItem(index, delegateUser);
			ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::InsertItem. Inserted delegate: {0} successfully", (delegateUser.Delegate != null) ? delegateUser.Delegate.MailboxInfo.PrimarySmtpAddress.ToString() : delegateUser.Name);
		}

		protected override void SetItem(int index, DelegateUser delegateUser)
		{
			if (delegateUser == null)
			{
				throw new ArgumentNullException("delegateUser");
			}
			this.CheckSaved();
			this.Validate(delegateUser);
			this.GetADRecipient(delegateUser);
			base.SetItem(index, delegateUser);
			ExTraceGlobals.DelegateTracer.TraceDebug<string, int>((long)this.GetHashCode(), "DelegateUserCollection::SetItem. Set delegate: {0} at index: {1} successfully", (delegateUser.Delegate != null) ? delegateUser.Delegate.MailboxInfo.PrimarySmtpAddress.ToString() : delegateUser.Name, index);
		}

		private DelegateValidationProblem CheckValue(DelegateUser value)
		{
			if (value == null)
			{
				ExTraceGlobals.DelegateTracer.TraceError((long)this.GetHashCode(), "DelegateUserCollection::CheckValue. value was null");
				return DelegateValidationProblem.Null;
			}
			if (value.Delegate != null)
			{
				if (value.Delegate.LegacyDn.Equals(this.session.MailboxOwnerLegacyDN, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.DelegateTracer.TraceError<string>((long)this.GetHashCode(), "DelegateUserCollection::CheckValue. Pricipal: {0} is attempting to set himself as his own delegate", value.Delegate.MailboxInfo.PrimarySmtpAddress.ToString());
					return DelegateValidationProblem.IsOwner;
				}
				foreach (DelegateUser delegateUser in this)
				{
					if (delegateUser.Delegate != null && value.Delegate.LegacyDn.Equals(delegateUser.Delegate.LegacyDn, StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.DelegateTracer.TraceError<string>((long)this.GetHashCode(), "DelegateUserCollection::CheckValue. Attempting to set a duplicate delegate: {0}", delegateUser.Delegate.MailboxInfo.PrimarySmtpAddress.ToString());
						return DelegateValidationProblem.Duplicate;
					}
				}
			}
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::CheckValue. No errors found");
			return DelegateValidationProblem.NoError;
		}

		private void RemoveUnknownEntries()
		{
			int i = 0;
			while (i < base.Count)
			{
				DelegateUser delegateUser = base[i];
				if ((delegateUser.Problems & DelegateProblems.NoDelegateInfo) == DelegateProblems.NoDelegateInfo || (delegateUser.Problems & DelegateProblems.NoADUser) == DelegateProblems.NoADUser)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::RemoveUnknownEntries. Either NoDelegateInfo or NoAdUser. Removing delegate: {0}", delegateUser.Name);
					base.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		private DelegateRuleType LoadOutlookDelegateInfo(List<KeyValuePair<string, string>> exchangePrincipalNotFoundList, out IExchangePrincipal[] principals, out string[] names, out string[] distinguishedNames, out int[] flags, out int[] flags2)
		{
			DelegateRuleType delegateRuleType = DelegateRuleType.ForwardAndDelete;
			names = null;
			flags = null;
			flags2 = null;
			principals = null;
			distinguishedNames = null;
			FolderSaveResult folderSaveResult;
			byte[] freeBusyMsgId = FreeBusyUtil.GetFreeBusyMsgId(this.session, out folderSaveResult);
			if (folderSaveResult != null)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Adding exception to save problems: 'CannotStamplocalFreeBusyId'");
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FreeBusyDelegateInfo, new FolderSaveException(ServerStrings.CannotStamplocalFreeBusyId, folderSaveResult)));
			}
			if (freeBusyMsgId != null && freeBusyMsgId.Length > 0)
			{
				try
				{
					using (MessageItem messageItem = MessageItem.Bind(this.session, StoreObjectId.FromProviderSpecificId(freeBusyMsgId), FreeBusyUtil.FreeBusyMessageProperties))
					{
						byte[][] valueOrDefault = messageItem.GetValueOrDefault<byte[][]>(InternalSchema.DelegateEntryIds, Array<byte[]>.Empty);
						names = messageItem.GetValueOrDefault<string[]>(InternalSchema.DelegateNames, Array<string>.Empty);
						flags = messageItem.GetValueOrDefault<int[]>(InternalSchema.DelegateFlags, Array<int>.Empty);
						flags2 = messageItem.GetValueOrDefault<int[]>(InternalSchema.DelegateFlags2, Array<int>.Empty);
						if (names.Length != valueOrDefault.Length || flags.Length != valueOrDefault.Length || flags2.Length != valueOrDefault.Length)
						{
							names = new string[valueOrDefault.Length];
							flags = new int[valueOrDefault.Length];
							flags2 = new int[valueOrDefault.Length];
						}
						List<IExchangePrincipal> list = new List<IExchangePrincipal>(valueOrDefault.Length);
						List<string> list2 = new List<string>(valueOrDefault.Length);
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Loading Restore Info");
						this.restoreInfo.Principals = list;
						this.restoreInfo.Ids = valueOrDefault;
						this.restoreInfo.Names = names;
						this.restoreInfo.Flags = flags;
						this.restoreInfo.Flags2 = flags2;
						this.restoreInfo.BossWantsCopy = messageItem.GetValueOrDefault<bool>(InternalSchema.DelegateBossWantsCopy);
						this.restoreInfo.BossWantsInfo = messageItem.GetValueOrDefault<bool>(InternalSchema.DelegateBossWantsInfo);
						this.restoreInfo.DontMailDelegate = messageItem.GetValueOrDefault<bool>(InternalSchema.DelegateDontMail);
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Extracting participant");
						Lazy<ADSessionSettings> lazy = new Lazy<ADSessionSettings>(() => this.session.GetADSessionSettings());
						for (int i = 0; i < valueOrDefault.Length; i++)
						{
							ParticipantEntryId participantEntryId = ParticipantEntryId.TryFromEntryId(valueOrDefault[i]);
							list2.Add(null);
							ADParticipantEntryId adparticipantEntryId = participantEntryId as ADParticipantEntryId;
							if (adparticipantEntryId != null)
							{
								ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. LegacyDN: {0}", adparticipantEntryId.LegacyDN);
								list2[i] = adparticipantEntryId.LegacyDN;
							}
							Participant.Builder builder = new Participant.Builder();
							builder.SetPropertiesFrom(participantEntryId);
							if (names[i] != null)
							{
								builder.DisplayName = names[i];
							}
							Participant participant = builder.ToParticipant();
							if (names[i] == null)
							{
								names[i] = participant.DisplayName;
								ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Participant display name: {0}", participant.DisplayName);
							}
							list.Add(null);
							try
							{
								if (participant.RoutingType == "EX")
								{
									ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("DelegateUserCollection::LoadOutlookDelegateInfo. Routing type: EX, email address: {0}", participant.EmailAddress));
									list[i] = ExchangePrincipal.FromLegacyDN(lazy.Value, participant.EmailAddress, RemotingOptions.AllowCrossSite);
								}
								else if (participant.RoutingType == "SMTP")
								{
									ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), string.Format("DelegateUserCollection::LoadOutlookDelegateInfo. Routing type: SMTP, email address: {0}", participant.EmailAddress));
									list[i] = ExchangePrincipal.FromProxyAddress(lazy.Value, "SMTP:" + participant.EmailAddress, RemotingOptions.AllowCrossSite);
								}
								else
								{
									ExTraceGlobals.DelegateTracer.TraceError((long)this.GetHashCode(), string.Format("Participant routing type is not recognized, Routing type: {0}, email address: {1}", participant.RoutingType, participant.EmailAddress));
								}
							}
							catch (ObjectNotFoundException)
							{
								ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Couldn't find the user in AD");
								exchangePrincipalNotFoundList.Add(new KeyValuePair<string, string>(participant.DisplayName, participant.EmailAddress));
								ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Adding the delegate {0} to ExchangePrincipalNotFoundList", participant.DisplayName);
							}
						}
						principals = list.ToArray();
						distinguishedNames = list2.ToArray();
						delegateRuleType = FreeBusyUtil.GetDelegateRuleType(messageItem);
						ExTraceGlobals.DelegateTracer.TraceDebug<DelegateRuleType>((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. Return type: {0}", delegateRuleType);
					}
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadOutlookDelegateInfo. No FreeBusyMessage");
				}
			}
			names = (names ?? Array<string>.Empty);
			flags = (flags ?? Array<int>.Empty);
			flags2 = (flags2 ?? Array<int>.Empty);
			principals = (principals ?? Array<IExchangePrincipal>.Empty);
			distinguishedNames = (distinguishedNames ?? Array<string>.Empty);
			return delegateRuleType;
		}

		private void BuildAndSaveOutlookDelegateInfo()
		{
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::BuildAndSaveOutlookDelegateInfo. Creating arrays to update Outlook DelegateInfo");
			string[] array = new string[base.Count];
			int[] array2 = new int[base.Count];
			int[] array3 = new int[base.Count];
			byte[][] array4 = new byte[base.Count][];
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::BuildAndSaveOutlookDelegateInfo. Looping through the collection and filling them in");
			for (int i = 0; i < base.Count; i++)
			{
				Participant participant = null;
				DelegateUser delegateUser = base[i];
				if (delegateUser.PrimarySmtpAddress != null)
				{
					if (delegateUser.Delegate != null)
					{
						array[i] = delegateUser.Delegate.MailboxInfo.DisplayName;
						participant = new Participant(delegateUser.Delegate);
					}
					else if (this.isCrossPremiseDelegateAllowed)
					{
						array[i] = delegateUser.Name;
						participant = new Participant(delegateUser.Name, delegateUser.PrimarySmtpAddress, "SMTP");
					}
				}
				else
				{
					array[i] = ClientStrings.UnknownDelegateUser;
					participant = new Participant(ClientStrings.UnknownDelegateUser, ClientStrings.UnknownDelegateUser, null);
				}
				ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.SupportsADParticipantEntryId);
				if (participantEntryId != null)
				{
					array4[i] = participantEntryId.ToByteArray();
				}
				else if (delegateUser.LegacyDistinguishedName != null)
				{
					ADParticipantEntryId adparticipantEntryId = new ADParticipantEntryId(delegateUser.LegacyDistinguishedName, null, false);
					array4[i] = adparticipantEntryId.ToByteArray();
				}
				else
				{
					array4[i] = Array<byte>.Empty;
				}
				array2[i] = (delegateUser.CanViewPrivateItems ? DelegateUserCollection.CanViewPrivateItemsFlag : 0);
				array3[i] = delegateUser.Flags2;
			}
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::BuildAndSaveOutlookDelegateInfo. Saving Outlook delegate info");
			this.SaveOutlookDelegateInfo(array, array2, array3, array4, this.delegateRuleType == DelegateRuleType.Forward, this.delegateRuleType == DelegateRuleType.ForwardAndSetAsInformationalUpdate, false);
		}

		private void SaveOutlookDelegateInfo(string[] names, int[] flags, int[] flags2, byte[][] ids, bool bossWantsCopy, bool bossWantsInfo, bool restoring)
		{
			Exception ex = null;
			try
			{
				FolderSaveResult folderSaveResult;
				byte[] freeBusyMsgId = FreeBusyUtil.GetFreeBusyMsgId(this.session, out folderSaveResult);
				if (folderSaveResult != null)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveOutlookDelegateInfo. Adding exception to save problems: CannotStamplocalFreeBusyId");
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FreeBusyDelegateInfo, new FolderSaveException(ServerStrings.CannotStamplocalFreeBusyId, folderSaveResult)));
				}
				using (MessageItem messageItem = MessageItem.Bind(this.session, StoreObjectId.FromProviderSpecificId(freeBusyMsgId), FreeBusyUtil.FreeBusyMessageProperties))
				{
					messageItem.OpenAsReadWrite();
					messageItem[InternalSchema.DelegateNames] = names;
					messageItem[InternalSchema.DelegateEntryIds] = ids;
					messageItem[InternalSchema.DelegateFlags] = flags;
					messageItem[InternalSchema.DelegateEntryIds2] = ids;
					messageItem[InternalSchema.DelegateFlags2] = flags2;
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveOutlookDelegateInfo. Setting delegate forward settings");
					if (this.delegateRuleType == DelegateRuleType.NoForward)
					{
						messageItem[InternalSchema.DelegateDontMail] = true;
					}
					else if (this.delegateRuleType == DelegateRuleType.Forward)
					{
						messageItem[InternalSchema.DelegateDontMail] = false;
						messageItem[InternalSchema.DelegateBossWantsCopy] = true;
						messageItem[InternalSchema.DelegateBossWantsInfo] = false;
					}
					else if (this.delegateRuleType == DelegateRuleType.ForwardAndDelete)
					{
						messageItem[InternalSchema.DelegateDontMail] = false;
						messageItem[InternalSchema.DelegateBossWantsCopy] = false;
						messageItem[InternalSchema.DelegateBossWantsInfo] = false;
					}
					else
					{
						if (this.delegateRuleType != DelegateRuleType.ForwardAndSetAsInformationalUpdate)
						{
							ExTraceGlobals.DelegateTracer.TraceError<DelegateRuleType>((long)this.GetHashCode(), "DelegateUserCollection::SaveOutlookDelegateInfo. Invalid DelegateRuleType: {0}", this.delegateRuleType);
							throw new InvalidOperationException("Invalid DelegateRuleType");
						}
						messageItem[InternalSchema.DelegateDontMail] = false;
						messageItem[InternalSchema.DelegateBossWantsCopy] = true;
						messageItem[InternalSchema.DelegateBossWantsInfo] = true;
					}
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveOutlookDelegateInfo. Saving local FB message");
					messageItem.Save(SaveMode.ResolveConflicts);
				}
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceError<Exception>((long)this.GetHashCode(), "DelegateUserCollection::SaveOutlookDelegateInfo. StorePermanentException occurred: {0}", ex);
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(restoring ? DelegateSaveState.RestoreFreeBusyDelegateInfo : DelegateSaveState.FreeBusyDelegateInfo, ex));
			}
		}

		private void RestoreOutlookDelegateInfo()
		{
			this.SaveOutlookDelegateInfo(this.restoreInfo.Names, this.restoreInfo.Flags, this.restoreInfo.Flags2, this.restoreInfo.Ids, this.restoreInfo.BossWantsCopy, this.restoreInfo.BossWantsInfo, false);
		}

		private void LoadFolderPermissions()
		{
			List<string> list = new List<string>(base.Count);
			foreach (DelegateUser delegateUser in this)
			{
				if (delegateUser.Delegate != null)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Delegate was not null so adding LegacyDN: {0}", delegateUser.Delegate.LegacyDn);
					list.Add(delegateUser.Delegate.LegacyDn);
				}
			}
			Result<ADRecipient>[] array = DelegateUserCollection.InternalFindAdRecipients(this.session, list);
			for (int i = 0; i < array.Length; i++)
			{
				foreach (DelegateUser delegateUser2 in this)
				{
					if (delegateUser2.Delegate != null && array[i].Data != null && array[i].Data.LegacyExchangeDN.Equals(delegateUser2.Delegate.LegacyDn, StringComparison.OrdinalIgnoreCase))
					{
						delegateUser2.ADRecipient = array[i].Data;
						break;
					}
				}
			}
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Creating dictionary for restore info");
			this.restoreInfo.FolderPermissions = new Dictionary<StoreObjectId, IDictionary<ADRecipient, MemberRights>>(DelegateUserCollection.Folders.Length);
			if (base.Count > 0)
			{
				DefaultFolderType[] array2;
				if (this.session.LogonType == LogonType.Transport || this.session.LogonType == LogonType.Admin)
				{
					array2 = DelegateUserCollection.FoldersForAdminLogons;
				}
				else
				{
					array2 = DelegateUserCollection.Folders;
				}
				foreach (DefaultFolderType defaultFolderType in array2)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<DefaultFolderType>((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. FolderType: {0}", defaultFolderType);
					if (defaultFolderType != DefaultFolderType.FreeBusyData)
					{
						using (Folder folder = Folder.Bind(this.session, defaultFolderType))
						{
							PermissionSet permissionSet = folder.GetPermissionSet();
							Dictionary<ADRecipient, MemberRights> dictionary = new Dictionary<ADRecipient, MemberRights>(base.Count);
							foreach (DelegateUser delegateUser3 in this)
							{
								if (delegateUser3.ADRecipient != null)
								{
									ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Checking to see if delegate already has any permissions granted on the folder. Delegate: {0}", delegateUser3.ADRecipient.PrimarySmtpAddress);
									Permission entry = permissionSet.GetEntry(new PermissionSecurityPrincipal(delegateUser3.ADRecipient));
									if (entry != null)
									{
										dictionary.Add(delegateUser3.ADRecipient, entry.MemberRights);
										if (defaultFolderType == DefaultFolderType.Calendar)
										{
											entry.MemberRights &= ~MemberRights.FreeBusyDetailed;
											entry.MemberRights &= ~MemberRights.FreeBusySimple;
										}
										ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress, PermissionLevel>((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Delegate has permissions on the folder. Delegate: {0} Permissions: {1}", delegateUser3.ADRecipient.PrimarySmtpAddress, entry.PermissionLevel);
										delegateUser3.FolderPermissions.Add(defaultFolderType, entry.PermissionLevel);
									}
								}
							}
							ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Adding restore info");
							this.restoreInfo.FolderPermissions.Add(folder.Id.ObjectId, dictionary);
						}
					}
				}
			}
		}

		private void UpdateFolderPermissions()
		{
			Exception ex = null;
			try
			{
				List<string> list = new List<string>();
				if (this.restoreInfo.Principals != null)
				{
					foreach (IExchangePrincipal exchangePrincipal in this.restoreInfo.Principals)
					{
						if (exchangePrincipal != null)
						{
							ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Checking to see if delegate was removed. Delegate: {0}", exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
							bool flag = true;
							foreach (DelegateUser delegateUser in this)
							{
								if (delegateUser.Delegate != null && delegateUser.Delegate.LegacyDn.Equals(exchangePrincipal.LegacyDn))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Delegate will be removed. Delegate: {0}", exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
								list.Add(exchangePrincipal.LegacyDn);
							}
						}
					}
				}
				Result<ADRecipient>[] array = DelegateUserCollection.InternalFindAdRecipients(this.session, list);
				foreach (DefaultFolderType defaultFolderType in DelegateUserCollection.Folders)
				{
					using (Folder folder = Folder.Bind(this.session, defaultFolderType))
					{
						PermissionSet permissionSet = folder.GetPermissionSet();
						ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Checking folder: {0}", folder.DisplayName);
						foreach (DelegateUser delegateUser2 in this)
						{
							if (delegateUser2.ADRecipient != null)
							{
								PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(delegateUser2.ADRecipient);
								Permission permission = permissionSet.GetEntry(securityPrincipal);
								bool flag2 = true;
								PermissionLevel permissionLevel;
								if (defaultFolderType == DefaultFolderType.FreeBusyData)
								{
									permissionLevel = PermissionLevel.Editor;
								}
								else
								{
									flag2 = delegateUser2.FolderPermissions.TryGetValue(defaultFolderType, out permissionLevel);
								}
								ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress, PermissionLevel>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Delegate: {0} was assigned permissions: {1}", delegateUser2.ADRecipient.PrimarySmtpAddress, permissionLevel);
								if (!flag2 || permission == null || permission.PermissionLevel != PermissionLevel.Custom || permissionLevel != PermissionLevel.Custom)
								{
									FreeBusyAccess freeBusyAccess = FreeBusyAccess.Details;
									if (permission != null)
									{
										if (defaultFolderType == DefaultFolderType.Calendar)
										{
											if ((permission.MemberRights & MemberRights.FreeBusyDetailed) == MemberRights.FreeBusyDetailed)
											{
												freeBusyAccess = FreeBusyAccess.Details;
											}
											else if ((permission.MemberRights & MemberRights.FreeBusySimple) == MemberRights.FreeBusySimple)
											{
												freeBusyAccess = FreeBusyAccess.Basic;
											}
											else
											{
												freeBusyAccess = FreeBusyAccess.None;
											}
											ExTraceGlobals.DelegateTracer.TraceDebug<FreeBusyAccess>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. FreeBusy was set to: {0}", freeBusyAccess);
										}
										ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Removing permissionSet for user: {0}", delegateUser2.ADRecipient.PrimarySmtpAddress);
										permissionSet.RemoveEntry(securityPrincipal);
									}
									if (flag2)
									{
										ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Adding permissionSet for user: {0}", delegateUser2.ADRecipient.PrimarySmtpAddress);
										permission = permissionSet.AddEntry(securityPrincipal, permissionLevel);
										if (defaultFolderType == DefaultFolderType.Calendar)
										{
											if (freeBusyAccess == FreeBusyAccess.Details)
											{
												permission.MemberRights |= (MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed);
											}
											else if (freeBusyAccess == FreeBusyAccess.Basic)
											{
												permission.MemberRights |= MemberRights.FreeBusySimple;
											}
											ExTraceGlobals.DelegateTracer.TraceDebug<MemberRights>((long)this.GetHashCode(), "DelegateUserCollection::LoadFolderPermissions. Permission member rights were set to: {0}", permission.MemberRights);
										}
									}
								}
							}
						}
						foreach (Result<ADRecipient> result in array)
						{
							if (result.Data != null)
							{
								ExTraceGlobals.DelegateTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Removing delegate: {0}", result.Data.PrimarySmtpAddress);
								permissionSet.RemoveEntry(new PermissionSecurityPrincipal(result.Data));
							}
						}
						FolderSaveResult folderSaveResult = folder.Save();
						if (folderSaveResult.OperationResult != OperationResult.Succeeded)
						{
							Exception value = new FolderSaveException(ServerStrings.MapiCannotSavePermissions, folderSaveResult);
							this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FolderPermissions, value));
							ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Adding exception to save problems: {0}", ex);
						}
					}
				}
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::UpdateFolderPermissions. Adding exception to save problems: {0}", ex);
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FolderPermissions, ex));
			}
		}

		private void RestoreFolderPermissions()
		{
			Exception ex = null;
			try
			{
				foreach (KeyValuePair<StoreObjectId, IDictionary<ADRecipient, MemberRights>> keyValuePair in this.restoreInfo.FolderPermissions)
				{
					using (Folder folder = Folder.Bind(this.session, keyValuePair.Key))
					{
						PermissionSet permissionSet = folder.GetPermissionSet();
						foreach (KeyValuePair<ADRecipient, MemberRights> keyValuePair2 in keyValuePair.Value)
						{
							PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(keyValuePair2.Key);
							Permission permission = permissionSet.GetEntry(securityPrincipal);
							if (permission != null)
							{
								permissionSet.RemoveEntry(securityPrincipal);
							}
							permission = permissionSet.AddEntry(securityPrincipal, PermissionLevel.None);
							permission.MemberRights = keyValuePair2.Value;
						}
						folder.Save();
					}
				}
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::RestoreFolderPermissions. Adding exception to save problems: {0}", ex);
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.RestoreFolderPermissions, ex));
			}
		}

		private void LoadDelegateRule()
		{
			using (Folder folder = Folder.Bind(this.session, DefaultFolderType.Inbox))
			{
				this.restoreInfo.DelegateRule = this.FindDelegateForwardingRule(folder, false, true);
				Rule delegateRule = this.restoreInfo.DelegateRule;
				Participant[] array = Array<Participant>.Empty;
				if (delegateRule != null)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Delegate rule found.");
					RuleAction.Delegate @delegate = delegateRule.Actions[0] as RuleAction.Delegate;
					if (@delegate != null)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Creating participant array.");
						AdrEntry[] recipients = @delegate.Recipients;
						array = new Participant[recipients.Length];
						for (int i = 0; i < recipients.Length; i++)
						{
							array[i] = Rule.ParticipantFromAdrEntry(recipients[i]);
						}
					}
				}
				Dictionary<Participant, DelegateUser> dictionary = new Dictionary<Participant, DelegateUser>();
				if (base.Count > 0)
				{
					foreach (DelegateUser delegateUser in this)
					{
						delegateUser.ReceivesMeetingMessageCopies = false;
						ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Loading delegate rule for: {0}.", delegateUser.Name);
						if (array.Length > 0 && delegateUser.PrimarySmtpAddress != null)
						{
							ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Creating temp participant: {0}.", delegateUser.PrimarySmtpAddress);
							Participant v = null;
							if (delegateUser.Delegate != null)
							{
								v = new Participant(delegateUser.Delegate);
							}
							else if (this.isCrossPremiseDelegateAllowed)
							{
								if (delegateUser.ADRecipient != null && delegateUser.ADRecipient.LegacyExchangeDN != null)
								{
									v = new Participant(delegateUser.Name, delegateUser.ADRecipient.LegacyExchangeDN, "EX");
								}
								else
								{
									v = new Participant(delegateUser.Name, delegateUser.PrimarySmtpAddress, "SMTP");
								}
							}
							Participant[] array2 = array;
							int j = 0;
							while (j < array2.Length)
							{
								Participant participant = array2[j];
								if (participant.AreAddressesEqual(v))
								{
									delegateUser.ReceivesMeetingMessageCopies = true;
									ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Delegate: {0} will receive copies of meeting.", delegateUser.PrimarySmtpAddress);
									if (!dictionary.ContainsKey(participant))
									{
										dictionary.Add(participant, delegateUser);
										break;
									}
									break;
								}
								else
								{
									j++;
								}
							}
						}
					}
				}
				ExTraceGlobals.DelegateTracer.TraceDebug(0L, "DelegateUserCollection::LoadDelegateRule. Checking for orphan participants.");
				if (array.Length > 0)
				{
					Lazy<ADSessionSettings> lazy = new Lazy<ADSessionSettings>(() => this.session.GetADSessionSettings());
					foreach (Participant participant2 in array)
					{
						if (!dictionary.ContainsKey(participant2))
						{
							IExchangePrincipal exchangePrincipal = null;
							try
							{
								if (participant2.RoutingType == "EX")
								{
									exchangePrincipal = ExchangePrincipal.FromLegacyDN(lazy.Value, participant2.EmailAddress, RemotingOptions.AllowCrossSite);
								}
								else if (participant2.RoutingType == "SMTP")
								{
									exchangePrincipal = ExchangePrincipal.FromProxyAddress(lazy.Value, "SMTP:" + participant2.EmailAddress, RemotingOptions.AllowCrossSite);
								}
							}
							catch (ObjectNotFoundException)
							{
								ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Couldn't find user: {0} in AD", participant2.EmailAddress);
							}
							ExTraceGlobals.DelegateTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Creating orphaned user: {0}", exchangePrincipal);
							DelegateUser delegateUser2 = DelegateUser.InternalCreate(exchangePrincipal, new Dictionary<DefaultFolderType, PermissionLevel>());
							if (exchangePrincipal == null)
							{
								delegateUser2.Name = ClientStrings.UnknownDelegateUser;
								delegateUser2.Problems |= DelegateProblems.NoADUser;
								ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::LoadDelegateRule. Couldn't create orphaned user as the user isn't in AD");
							}
							delegateUser2.Problems |= DelegateProblems.NoDelegateInfo;
						}
					}
				}
			}
		}

		private Rule FindDelegateForwardingRule(Folder inbox, bool restoring, bool loading)
		{
			Exception ex = null;
			try
			{
				StoreSession storeSession = this.session;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					Rule[] rules = inbox.MapiFolder.GetRules(new PropTag[0]);
					foreach (Rule rule in rules)
					{
						StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
						if (ordinalIgnoreCase.Equals(rule.Name, DelegateUserCollection.DelegateRuleName) && ordinalIgnoreCase.Equals(rule.Provider, DelegateUserCollection.DelegateRuleProvider))
						{
							foreach (RuleAction ruleAction in rule.Actions)
							{
								if (ruleAction is RuleAction.Delegate)
								{
									ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::FindDelegateForwardingRule. Found rule has a delegate action.");
									return rule;
								}
							}
						}
					}
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::FindDelegateForwardingRule. Couldn't find forwarding rule.");
					return null;
				}
				catch (MapiPermanentException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("DelegateUserCollection.SaveRule().", new object[0]),
						ex2
					});
				}
				catch (MapiRetryableException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex3, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("DelegateUserCollection.SaveRule().", new object[0]),
						ex3
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
			catch (StoragePermanentException ex4)
			{
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::FindDelegateForwardingRule. Exception occurred: {0}", ex);
				if (!loading)
				{
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(restoring ? DelegateSaveState.RestoreDelegateForwardingRule : DelegateSaveState.DelegateForwardingRule, ex));
				}
			}
			return null;
		}

		private void SaveRule(Rule rule, bool restoring)
		{
			Exception ex = null;
			try
			{
				using (Folder folder = Folder.Bind(this.session, DefaultFolderType.Inbox))
				{
					ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveRule. Finding and getting rid of any old delegate forwarding rules");
					Rule rule2;
					while ((rule2 = this.FindDelegateForwardingRule(folder, restoring, false)) != null)
					{
						StoreSession storeSession = this.session;
						bool flag = false;
						try
						{
							if (storeSession != null)
							{
								storeSession.BeginMapiCall();
								storeSession.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveRule. Deleting old rule");
							folder.MapiFolder.DeleteRules(new Rule[]
							{
								rule2
							});
						}
						catch (MapiPermanentException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DelegateUserCollection.SaveRule().", new object[0]),
								ex2
							});
						}
						catch (MapiRetryableException ex3)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex3, storeSession, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DelegateUserCollection.SaveRule().", new object[0]),
								ex3
							});
						}
						finally
						{
							try
							{
								if (storeSession != null)
								{
									storeSession.EndMapiCall();
									if (flag)
									{
										storeSession.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
					if (base.Count > 0 && rule != null)
					{
						StoreSession storeSession2 = this.session;
						bool flag2 = false;
						try
						{
							if (storeSession2 != null)
							{
								storeSession2.BeginMapiCall();
								storeSession2.BeginServerHealthCall();
								flag2 = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::SaveRule. Adding new rule");
							folder.MapiFolder.AddRules(new Rule[]
							{
								rule
							});
						}
						catch (MapiPermanentException ex4)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex4, storeSession2, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DelegateUserCollection.SaveRule().", new object[0]),
								ex4
							});
						}
						catch (MapiRetryableException ex5)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex5, storeSession2, this, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("DelegateUserCollection.SaveRule().", new object[0]),
								ex5
							});
						}
						finally
						{
							try
							{
								if (storeSession2 != null)
								{
									storeSession2.EndMapiCall();
									if (flag2)
									{
										storeSession2.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
				}
			}
			catch (StoragePermanentException ex6)
			{
				ex = ex6;
			}
			catch (StorageTransientException ex7)
			{
				ex = ex7;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::SaveRule. Adding exception to save problems: {0}", ex);
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(restoring ? DelegateSaveState.RestoreDelegateForwardingRule : DelegateSaveState.DelegateForwardingRule, ex));
			}
		}

		internal static Rule BuildDelegateForwardingRule(IList<Participant> participants, bool bossWantsCopy)
		{
			ExTraceGlobals.DelegateTracer.TraceDebug<int>(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Creating new rule. Number of participants {0}", (participants == null) ? 0 : participants.Count);
			Rule rule = new Rule();
			rule.Level = 0;
			rule.StateFlags = RuleStateFlags.Enabled;
			rule.Name = DelegateUserCollection.DelegateRuleName;
			rule.Provider = DelegateUserCollection.DelegateRuleProvider;
			ExTraceGlobals.DelegateTracer.TraceDebug<string, string>(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Filled in the basic info. Name: {0} Provider: {1}", rule.Name, rule.Provider);
			ExTraceGlobals.DelegateTracer.TraceDebug(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Adding condition");
			rule.Condition = Restriction.And(new Restriction[]
			{
				Restriction.Content(PropTag.MessageClass, "IPM.Schedule.Meeting", ContentFlags.Prefix),
				Restriction.Not(Restriction.Exist((PropTag)1071841291U)),
				Restriction.Or(new Restriction[]
				{
					Restriction.Not(Restriction.Exist(PropTag.Sensitivity)),
					Restriction.NE(PropTag.Sensitivity, Sensitivity.Private)
				})
			});
			AdrEntry[] array = new AdrEntry[participants.Count];
			for (int i = 0; i < participants.Count; i++)
			{
				array[i] = Rule.AdrEntryFromParticipant(participants[i]);
				ExTraceGlobals.DelegateTracer.TraceDebug<string>(0L, "DelegateUserCollection::BuildDelegateForwardingRule. participant: {0}", participants[i].DisplayName);
			}
			RuleAction[] array2 = new RuleAction[1 + (bossWantsCopy ? 0 : 1)];
			ExTraceGlobals.DelegateTracer.TraceDebug(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Creating the delegate forward operation");
			array2[0] = new RuleAction.Delegate(array);
			if (!bossWantsCopy)
			{
				ExTraceGlobals.DelegateTracer.TraceDebug(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Boss doesn't want copy so message will be deleted.");
				array2[1] = new RuleAction.Delete();
			}
			rule.Actions = array2;
			ExTraceGlobals.DelegateTracer.TraceDebug(0L, "DelegateUserCollection::BuildDelegateForwardingRule. Successfully built delegate forwarding rule");
			return rule;
		}

		private void RestoreDelegateForwardingRule()
		{
			this.SaveRule(this.restoreInfo.DelegateRule, true);
		}

		private void CheckSaved()
		{
			if (this.saveState != DelegateSaveState.None)
			{
				ExTraceGlobals.DelegateTracer.TraceError((long)this.GetHashCode(), "DelegateUserCollection::CheckSaved. The DelegateUserCollection cannot be used after a save. Use a new DelegateUserCollection.");
				throw new InvalidOperationException("The DelegateUserCollection cannot be used after a save. Use a new DelegateUserCollection.");
			}
		}

		private void GetADRecipient(DelegateUser user)
		{
			try
			{
				IRecipientSession adrecipientSession = this.session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				if (user.Delegate != null)
				{
					user.ADRecipient = adrecipientSession.FindByLegacyExchangeDN(user.Delegate.LegacyDn);
				}
				else if (user.PrimarySmtpAddress != null && this.isCrossPremiseDelegateAllowed)
				{
					ADRecipient adrecipient;
					ADRecipient.TryGetFromProxyAddress(ProxyAddress.Parse(user.PrimarySmtpAddress), adrecipientSession, out adrecipient);
					user.ADRecipient = adrecipient;
				}
				else
				{
					user.ADRecipient = null;
				}
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "DelegateUser.Create. Failed due to directory exception {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "DelegateUser.Create. Failed due to directory exception {0}.", new object[]
				{
					ex2
				});
			}
		}

		private bool GetCrossPremiseDelegateStatus()
		{
			return this.session != null && this.session.MailboxOwner != null && !this.session.IsGroupMailbox() && DelegateUserCollection.IsCrossPremiseDelegateEnabled(this.session.MailboxOwner);
		}

		private void Validate(DelegateUser value)
		{
			DelegateValidationProblem delegateValidationProblem = this.CheckValue(value);
			if (delegateValidationProblem != DelegateValidationProblem.NoError)
			{
				throw new DelegateUserValidationException(ServerStrings.DelegateValidationFailed(value.Name), delegateValidationProblem);
			}
		}

		private static ExchangePrincipal InternalGetNonCachedExchangePrincipal(MailboxSession session)
		{
			IRecipientSession adrecipientSession = session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			return ExchangePrincipal.FromDirectoryObjectId(adrecipientSession, session.MailboxOwner.ObjectId, RemotingOptions.LocalConnectionsOnly);
		}

		private static Result<ADRecipient>[] InternalFindAdRecipients(MailboxSession session, List<string> userDNs)
		{
			Result<ADRecipient>[] result = null;
			try
			{
				IRecipientSession adrecipientSession = session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				result = adrecipientSession.FindADRecipientsByLegacyExchangeDNs(userDNs.ToArray());
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "ExchangePrincipal::FromLegacyDn. Failed due to directory exception {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "ExchangePrincipal::FromLegacyDn. Failed due to directory exception {0}.", new object[]
				{
					ex2
				});
			}
			return result;
		}

		public static bool IsCrossPremiseDelegateEnabled(IExchangePrincipal exchangePrincipal)
		{
			VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
			return configuration.DataStorage.CrossPremiseDelegate.Enabled;
		}

		private void RestoreADSendOnBehalf()
		{
			Exception ex = null;
			if (!this.session.MailboxOwner.ObjectId.IsNullOrEmpty())
			{
				try
				{
					this.SaveDelegates(this.session.GetADRecipientSession(false, ConsistencyMode.FullyConsistent), this.session.MailboxOwner.ObjectId, this.restoreInfo.SendOnBehalf);
				}
				catch (StoragePermanentException ex2)
				{
					ex = ex2;
				}
				catch (StorageTransientException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					ExTraceGlobals.DelegateTracer.TraceError<Exception>((long)this.GetHashCode(), "DelegateUserCollection::RestoreADSendOnBehalf.  Adding exception to save problems: {0}", ex);
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.RestoreADSendOnBehalf, ex));
				}
			}
		}

		private void SaveDelegates(IRecipientSession recipientSession, ADObjectId adObjectId, IEnumerable<ADObjectId> delegates)
		{
			ADUser aduser = recipientSession.Read(adObjectId) as ADUser;
			if (aduser != null)
			{
				aduser.GrantSendOnBehalfTo.Clear();
				aduser.GrantSendOnBehalfTo.AddRange(delegates.Take(StorageLimits.Instance.MaxDelegates).Distinct<ADObjectId>());
				try
				{
					recipientSession.Save(aduser);
				}
				catch (DataValidationException innerException)
				{
					throw new CorruptDataException(ServerStrings.ExCannotSaveInvalidObject(aduser), innerException);
				}
				catch (DataSourceOperationException ex)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "DelegateUserCollection::SaveDelegates. Failed due to directory exception {0}.", new object[]
					{
						ex
					});
				}
				catch (DataSourceTransientException ex2)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "DelegateUserCollection::SaveDelegates. Failed due to directory exception {0}.", new object[]
					{
						ex2
					});
				}
			}
		}

		public DelegateUserCollectionSaveResult Save(bool removeUnknown)
		{
			this.CheckSaved();
			if (this.session.MailboxOwner.ObjectId.IsNullOrEmpty())
			{
				ExTraceGlobals.DelegateTracer.TraceError((long)this.GetHashCode(), "DelegateUserCollection::Save. Cannot save when SOB is empty.");
				throw new NotSupportedException("Cannot save when SOB is empty.");
			}
			if (removeUnknown)
			{
				this.RemoveUnknownEntries();
			}
			foreach (DelegateUser delegateUser in this)
			{
				delegateUser.Validate();
			}
			this.saveState = DelegateSaveState.None;
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Save. Write the delegate info to the local Free/Busy message");
			IList<Exception> list = this.testBridge.SetOulookLocalFreeBusyData();
			if (list.Count == 0)
			{
				this.BuildAndSaveOutlookDelegateInfo();
			}
			else
			{
				foreach (Exception ex in list)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::Save. Adding exception to save problems: {0}", ex);
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FreeBusyDelegateInfo, ex));
				}
			}
			if (this.saveProblems.Count > 0)
			{
				return this.ErrorCleanup();
			}
			this.saveState |= DelegateSaveState.FreeBusyDelegateInfo;
			list = this.testBridge.CreateDelegateForwardingRule();
			if (list.Count == 0)
			{
				List<Participant> list2 = new List<Participant>();
				foreach (DelegateUser delegateUser2 in this)
				{
					if (delegateUser2.PrimarySmtpAddress != null && delegateUser2.ReceivesMeetingMessageCopies)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::Save. Delegate: {0} is valid so adding him to forwarding rule", delegateUser2.PrimarySmtpAddress);
						if (delegateUser2.Delegate != null)
						{
							list2.Add(new Participant(delegateUser2.Delegate));
						}
						else if (this.isCrossPremiseDelegateAllowed)
						{
							if (delegateUser2.ADRecipient != null && delegateUser2.ADRecipient.LegacyExchangeDN != null)
							{
								list2.Add(new Participant(delegateUser2.Name, delegateUser2.ADRecipient.LegacyExchangeDN, "EX"));
							}
							else
							{
								list2.Add(new Participant(delegateUser2.Name, delegateUser2.PrimarySmtpAddress, "SMTP"));
							}
						}
					}
				}
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Save. Building the forwarding rule");
				Rule rule;
				if (list2.Count > 0 && this.delegateRuleType != DelegateRuleType.NoForward)
				{
					rule = DelegateUserCollection.BuildDelegateForwardingRule(list2, this.delegateRuleType == DelegateRuleType.Forward || this.delegateRuleType == DelegateRuleType.ForwardAndSetAsInformationalUpdate);
				}
				else
				{
					rule = null;
				}
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Save. Saving the forwarding rule");
				this.SaveRule(rule, false);
			}
			else
			{
				foreach (Exception ex2 in list)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::Save. Adding exception to save problems: {0}", ex2);
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.DelegateForwardingRule, ex2));
				}
			}
			if (this.saveProblems.Count > 0)
			{
				return this.ErrorCleanup();
			}
			this.saveState |= DelegateSaveState.DelegateForwardingRule;
			list = this.testBridge.UpdateSendOnBehalfOfPermissions();
			if (list.Count == 0)
			{
				this.UpdateADSendOnBehalf();
			}
			else
			{
				foreach (Exception value in list)
				{
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.ADSendOnBehalf, value));
				}
			}
			if (this.saveProblems.Count > 0)
			{
				return this.ErrorCleanup();
			}
			this.saveState |= DelegateSaveState.ADSendOnBehalf;
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::Save. Updating folder permissions");
			list = this.testBridge.SetFolderPermissions();
			if (list.Count == 0)
			{
				this.UpdateFolderPermissions();
			}
			else
			{
				foreach (Exception ex3 in list)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::Save. Adding exception to save problems: {0}", ex3);
					this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.FolderPermissions, ex3));
				}
			}
			if (this.saveProblems.Count > 0)
			{
				return this.ErrorCleanup();
			}
			this.saveState |= DelegateSaveState.FolderPermissions;
			return new DelegateUserCollectionSaveResult(this.saveProblems);
		}

		private DelegateUserCollectionSaveResult ErrorCleanup()
		{
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup.");
			if (this.saveProblems.Count > 0)
			{
				IList<Exception> list = this.testBridge.RollbackDelegateState();
				if (list.Count == 0)
				{
					if ((this.saveState & DelegateSaveState.ADSendOnBehalf) == DelegateSaveState.ADSendOnBehalf)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup.Problem occurred while setting ADSendOnBehalf. Attempting to restore.");
						this.RestoreADSendOnBehalf();
					}
					if ((this.saveState & DelegateSaveState.DelegateForwardingRule) == DelegateSaveState.DelegateForwardingRule)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup. Problem occurred while setting DelegateForwardingRule. Attempting to restore.");
						this.RestoreDelegateForwardingRule();
					}
					if ((this.saveState & DelegateSaveState.FolderPermissions) == DelegateSaveState.FolderPermissions)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup. Problem occurred while setting FolderPermissions. Attempting to restore.");
						this.RestoreFolderPermissions();
					}
					if ((this.saveState & DelegateSaveState.FreeBusyDelegateInfo) == DelegateSaveState.FreeBusyDelegateInfo)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup. Problem occurred while setting FreeBusyDelegateInfo. Attempting to restore.");
						this.RestoreOutlookDelegateInfo();
					}
				}
				else
				{
					foreach (Exception ex in list)
					{
						ExTraceGlobals.DelegateTracer.TraceDebug<Exception>((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup. Adding exception to save problems: {0}", ex);
						this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.RestoreFreeBusyDelegateInfo, ex));
					}
				}
			}
			this.saveState = DelegateSaveState.None;
			base.Clear();
			this.Load();
			ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::ErrorCleanup. Successfully completed.");
			return new DelegateUserCollectionSaveResult(this.saveProblems);
		}

		private void UpdateADSendOnBehalf()
		{
			Exception ex = null;
			IList<ADObjectId> list = new List<ADObjectId>();
			foreach (DelegateUser delegateUser in this)
			{
				if (delegateUser.Delegate != null)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateADSendOnBehalf. Adding delegate: {0} to send on behalf delegate property", delegateUser.Delegate.MailboxInfo.PrimarySmtpAddress.ToString());
					list.Add(delegateUser.Delegate.ObjectId);
				}
				else if (delegateUser.ADRecipient != null && delegateUser.ADRecipient.Id != null)
				{
					ExTraceGlobals.DelegateTracer.TraceDebug<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateADSendOnBehalf. Adding cross premise delegate: {0} to send on behalf delegate property", delegateUser.PrimarySmtpAddress);
					list.Add(delegateUser.ADRecipient.Id);
				}
				else
				{
					ExTraceGlobals.DelegateTracer.TraceError<string>((long)this.GetHashCode(), "DelegateUserCollection::UpdateADSendOnBehalf. Delegate {0} is not added to SOBDelegates collection! Both ExchangePrincipal and ADRecipient informations are missing for this user.", delegateUser.PrimarySmtpAddress);
				}
			}
			try
			{
				ExTraceGlobals.DelegateTracer.TraceDebug((long)this.GetHashCode(), "DelegateUserCollection::UpdateADSendOnBehalf. Saving send on behalf of property");
				this.SaveDelegates(this.session.GetADRecipientSession(false, ConsistencyMode.FullyConsistent), this.session.MailboxOwner.ObjectId, list);
				this.saveState |= DelegateSaveState.ADSendOnBehalf;
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.DelegateTracer.TraceError<Exception>((long)this.GetHashCode(), "DelegateUserCollection::UpdateADSendOnBehalf.  Adding exception to save problems: {0}", ex);
				this.saveProblems.Add(new KeyValuePair<DelegateSaveState, Exception>(DelegateSaveState.ADSendOnBehalf, ex));
			}
		}

		private readonly bool isCrossPremiseDelegateAllowed;

		private DelegateSaveState saveState;

		private readonly MailboxSession session;

		private DelegateRuleType delegateRuleType = DelegateRuleType.ForwardAndDelete;

		private DelegateRestoreInfo restoreInfo;

		private Collection<KeyValuePair<DelegateSaveState, Exception>> saveProblems = new Collection<KeyValuePair<DelegateSaveState, Exception>>();

		private IDelegateUserCollectionBridge testBridge;

		internal static readonly DefaultFolderType[] Folders = new DefaultFolderType[]
		{
			DefaultFolderType.Inbox,
			DefaultFolderType.Calendar,
			DefaultFolderType.Contacts,
			DefaultFolderType.Tasks,
			DefaultFolderType.Notes,
			DefaultFolderType.Journal,
			DefaultFolderType.FreeBusyData
		};

		internal static readonly DefaultFolderType[] FoldersForAdminLogons = new DefaultFolderType[]
		{
			DefaultFolderType.Inbox,
			DefaultFolderType.Calendar,
			DefaultFolderType.FreeBusyData
		};

		internal static readonly string DelegateRuleName = string.Empty;

		private static readonly int CanViewPrivateItemsFlag = 1;

		public static readonly string DelegateRuleProvider = "Schedule+ EMS Interface";
	}
}

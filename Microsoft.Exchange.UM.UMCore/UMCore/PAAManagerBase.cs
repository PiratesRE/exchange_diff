using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PAAManagerBase : ActivityManager, IPAAUI
	{
		internal PAAManagerBase(ActivityManager manager, ActivityManagerConfig config) : base(manager, config)
		{
			this.paaMenuItems = new PAAMenuItem[12];
			for (int i = 1; i <= 10; i++)
			{
				PAAMenuItem paamenuItem = new PAAMenuItem();
				paamenuItem.MenuKey = i;
				paamenuItem.Enabled = false;
				paamenuItem.Context = null;
				paamenuItem.MenuType = null;
				paamenuItem.TargetName = null;
				paamenuItem.TargetPhone = null;
				this.paaMenuItems[i] = paamenuItem;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::ctor()", new object[0]);
		}

		public bool HaveActions
		{
			get
			{
				return 0 != this.personalAutoAttendant.KeyMappingList.Count;
			}
		}

		public bool HaveAutoActions
		{
			get
			{
				return this.personalAutoAttendant != null && 0 != this.personalAutoAttendant.AutoActionsList.Count;
			}
		}

		public bool HaveGreeting
		{
			get
			{
				return this.haveGreeting;
			}
			set
			{
				this.haveGreeting = value;
			}
		}

		public bool AutoGeneratePrompt
		{
			get
			{
				return true;
			}
		}

		public bool TransferToVoiceMessageEnabled
		{
			get
			{
				return this.transferToVoiceMessageEnabled;
			}
		}

		internal bool Key1Enabled
		{
			get
			{
				return this.paaMenuItems[1].Enabled;
			}
			set
			{
				this.paaMenuItems[1].Enabled = value;
			}
		}

		internal string MenuType1
		{
			get
			{
				return this.paaMenuItems[1].MenuType;
			}
			set
			{
				this.paaMenuItems[1].MenuType = value;
			}
		}

		internal string Context1
		{
			get
			{
				return this.paaMenuItems[1].Context;
			}
			set
			{
				this.paaMenuItems[1].Context = value;
			}
		}

		internal object TargetName1
		{
			get
			{
				return this.paaMenuItems[1].TargetName;
			}
			set
			{
				this.paaMenuItems[1].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone1
		{
			get
			{
				return this.paaMenuItems[1].TargetPhone;
			}
			set
			{
				this.paaMenuItems[1].TargetPhone = value;
			}
		}

		internal bool Key2Enabled
		{
			get
			{
				return this.paaMenuItems[2].Enabled;
			}
			set
			{
				this.paaMenuItems[2].Enabled = value;
			}
		}

		internal string MenuType2
		{
			get
			{
				return this.paaMenuItems[2].MenuType;
			}
			set
			{
				this.paaMenuItems[2].MenuType = value;
			}
		}

		internal string Context2
		{
			get
			{
				return this.paaMenuItems[2].Context;
			}
			set
			{
				this.paaMenuItems[2].Context = value;
			}
		}

		internal object TargetName2
		{
			get
			{
				return this.paaMenuItems[2].TargetName;
			}
			set
			{
				this.paaMenuItems[2].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone2
		{
			get
			{
				return this.paaMenuItems[2].TargetPhone;
			}
			set
			{
				this.paaMenuItems[2].TargetPhone = value;
			}
		}

		internal bool Key3Enabled
		{
			get
			{
				return this.paaMenuItems[3].Enabled;
			}
			set
			{
				this.paaMenuItems[3].Enabled = value;
			}
		}

		internal string MenuType3
		{
			get
			{
				return this.paaMenuItems[3].MenuType;
			}
			set
			{
				this.paaMenuItems[3].MenuType = value;
			}
		}

		internal string Context3
		{
			get
			{
				return this.paaMenuItems[3].Context;
			}
			set
			{
				this.paaMenuItems[3].Context = value;
			}
		}

		internal object TargetName3
		{
			get
			{
				return this.paaMenuItems[3].TargetName;
			}
			set
			{
				this.paaMenuItems[3].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone3
		{
			get
			{
				return this.paaMenuItems[3].TargetPhone;
			}
			set
			{
				this.paaMenuItems[3].TargetPhone = value;
			}
		}

		internal bool Key4Enabled
		{
			get
			{
				return this.paaMenuItems[4].Enabled;
			}
			set
			{
				this.paaMenuItems[4].Enabled = value;
			}
		}

		internal string MenuType4
		{
			get
			{
				return this.paaMenuItems[4].MenuType;
			}
			set
			{
				this.paaMenuItems[4].MenuType = value;
			}
		}

		internal string Context4
		{
			get
			{
				return this.paaMenuItems[4].Context;
			}
			set
			{
				this.paaMenuItems[4].Context = value;
			}
		}

		internal object TargetName4
		{
			get
			{
				return this.paaMenuItems[4].TargetName;
			}
			set
			{
				this.paaMenuItems[4].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone4
		{
			get
			{
				return this.paaMenuItems[4].TargetPhone;
			}
			set
			{
				this.paaMenuItems[4].TargetPhone = value;
			}
		}

		internal bool Key5Enabled
		{
			get
			{
				return this.paaMenuItems[5].Enabled;
			}
			set
			{
				this.paaMenuItems[5].Enabled = value;
			}
		}

		internal string MenuType5
		{
			get
			{
				return this.paaMenuItems[5].MenuType;
			}
			set
			{
				this.paaMenuItems[5].MenuType = value;
			}
		}

		internal string Context5
		{
			get
			{
				return this.paaMenuItems[5].Context;
			}
			set
			{
				this.paaMenuItems[5].Context = value;
			}
		}

		internal object TargetName5
		{
			get
			{
				return this.paaMenuItems[5].TargetName;
			}
			set
			{
				this.paaMenuItems[5].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone5
		{
			get
			{
				return this.paaMenuItems[5].TargetPhone;
			}
			set
			{
				this.paaMenuItems[5].TargetPhone = value;
			}
		}

		internal bool Key6Enabled
		{
			get
			{
				return this.paaMenuItems[6].Enabled;
			}
			set
			{
				this.paaMenuItems[6].Enabled = value;
			}
		}

		internal string MenuType6
		{
			get
			{
				return this.paaMenuItems[6].MenuType;
			}
			set
			{
				this.paaMenuItems[6].MenuType = value;
			}
		}

		internal string Context6
		{
			get
			{
				return this.paaMenuItems[6].Context;
			}
			set
			{
				this.paaMenuItems[6].Context = value;
			}
		}

		internal object TargetName6
		{
			get
			{
				return this.paaMenuItems[6].TargetName;
			}
			set
			{
				this.paaMenuItems[6].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone6
		{
			get
			{
				return this.paaMenuItems[6].TargetPhone;
			}
			set
			{
				this.paaMenuItems[6].TargetPhone = value;
			}
		}

		internal bool Key7Enabled
		{
			get
			{
				return this.paaMenuItems[7].Enabled;
			}
			set
			{
				this.paaMenuItems[7].Enabled = value;
			}
		}

		internal string MenuType7
		{
			get
			{
				return this.paaMenuItems[7].MenuType;
			}
			set
			{
				this.paaMenuItems[7].MenuType = value;
			}
		}

		internal string Context7
		{
			get
			{
				return this.paaMenuItems[7].Context;
			}
			set
			{
				this.paaMenuItems[7].Context = value;
			}
		}

		internal object TargetName7
		{
			get
			{
				return this.paaMenuItems[7].TargetName;
			}
			set
			{
				this.paaMenuItems[7].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone7
		{
			get
			{
				return this.paaMenuItems[7].TargetPhone;
			}
			set
			{
				this.paaMenuItems[7].TargetPhone = value;
			}
		}

		internal bool Key8Enabled
		{
			get
			{
				return this.paaMenuItems[8].Enabled;
			}
			set
			{
				this.paaMenuItems[8].Enabled = value;
			}
		}

		internal string MenuType8
		{
			get
			{
				return this.paaMenuItems[8].MenuType;
			}
			set
			{
				this.paaMenuItems[8].MenuType = value;
			}
		}

		internal string Context8
		{
			get
			{
				return this.paaMenuItems[8].Context;
			}
			set
			{
				this.paaMenuItems[8].Context = value;
			}
		}

		internal object TargetName8
		{
			get
			{
				return this.paaMenuItems[8].TargetName;
			}
			set
			{
				this.paaMenuItems[8].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone8
		{
			get
			{
				return this.paaMenuItems[8].TargetPhone;
			}
			set
			{
				this.paaMenuItems[8].TargetPhone = value;
			}
		}

		internal bool Key9Enabled
		{
			get
			{
				return this.paaMenuItems[9].Enabled;
			}
			set
			{
				this.paaMenuItems[9].Enabled = value;
			}
		}

		internal string MenuType9
		{
			get
			{
				return this.paaMenuItems[9].MenuType;
			}
			set
			{
				this.paaMenuItems[9].MenuType = value;
			}
		}

		internal string Context9
		{
			get
			{
				return this.paaMenuItems[9].Context;
			}
			set
			{
				this.paaMenuItems[9].Context = value;
			}
		}

		internal object TargetName9
		{
			get
			{
				return this.paaMenuItems[9].TargetName;
			}
			set
			{
				this.paaMenuItems[9].TargetName = value;
			}
		}

		internal PhoneNumber TargetPhone9
		{
			get
			{
				return this.paaMenuItems[9].TargetPhone;
			}
			set
			{
				this.paaMenuItems[9].TargetPhone = value;
			}
		}

		internal bool HaveRecordedName
		{
			get
			{
				return false;
			}
		}

		internal ITempWavFile PersonalGreeting
		{
			get
			{
				return this.personalGreeting;
			}
			set
			{
				this.personalGreeting = value;
			}
		}

		internal object RecordedName
		{
			get
			{
				if (this.recordedName == null)
				{
					this.recordedName = base.GetRecordedName(this.Subscriber.ADRecipient);
				}
				return this.recordedName;
			}
		}

		internal bool MainMenuUninterruptible
		{
			get
			{
				return this.mainMenuUninterruptible;
			}
		}

		protected PersonalAutoAttendant PersonalAutoAttendant
		{
			get
			{
				return this.personalAutoAttendant;
			}
			set
			{
				this.personalAutoAttendant = value;
			}
		}

		protected PAAMenuItem[] PAAMenuItems
		{
			get
			{
				return this.paaMenuItems;
			}
			set
			{
				this.paaMenuItems = value;
			}
		}

		protected Dictionary<int, PAAManagerBase.PAAPresentationObject> Menu
		{
			get
			{
				return this.menu;
			}
		}

		protected UMSubscriber Subscriber
		{
			get
			{
				return this.subscriber;
			}
			set
			{
				this.subscriber = value;
			}
		}

		public void SetADTransferTargetMenuItem(int key, string type, string context, string legacyExchangeDN, ADRecipient recipient)
		{
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PII, legacyExchangeDN),
				PIIMessage.Create(PIIType._UserDisplayName, (recipient != null) ? recipient.DisplayName : "<null>")
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "SetADTransferTargetMenuItem() Key {0} Type {1} Context {2} LegDN _PII Recipient _UserDisplayName", new object[]
			{
				key,
				type,
				context
			});
			this.SetMenuItem(key, type, context);
			string text = "tempname";
			base.WriteVariable(text, null);
			if (recipient != null)
			{
				base.SetRecordedName(text, recipient);
			}
			else
			{
				int num = legacyExchangeDN.LastIndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
				string varValue = legacyExchangeDN;
				if (num != -1 && legacyExchangeDN.Length > num + 4)
				{
					varValue = legacyExchangeDN.Substring(num + 4);
				}
				base.WriteVariable(text, varValue);
			}
			this.paaMenuItems[key].TargetName = (this.ReadVariable(text) ?? string.Empty);
			base.WriteVariable(text, null);
		}

		public void SetPhoneNumberTransferMenuItem(int key, string type, string context, string phoneNumberString)
		{
			this.SetMenuItem(key, type, context);
			this.paaMenuItems[key].TargetPhone = PhoneNumber.Parse(phoneNumberString);
		}

		public void SetFindMeMenuItem(int key, string type, string context)
		{
			this.SetMenuItem(key, type, context);
		}

		public void SetMenuItemTransferToVoiceMail()
		{
			this.transferToVoiceMessageEnabled = true;
		}

		public virtual void SetADTransferTarget(ADRecipient mailboxTransferTarget)
		{
			throw new NotImplementedException();
		}

		public virtual void SetBlindTransferEnabled(bool enabled, PhoneNumber target)
		{
			throw new NotImplementedException();
		}

		public virtual void SetPermissionCheckFailure()
		{
			throw new NotImplementedException();
		}

		public virtual void SetTransferToMailboxEnabled()
		{
			throw new NotImplementedException();
		}

		public virtual void SetTransferToVoiceMessageEnabled()
		{
			throw new NotImplementedException();
		}

		public virtual void SetInvalidADContact()
		{
			throw new NotImplementedException();
		}

		public virtual void SetFindMeNumbers(FindMe[] numbers)
		{
			throw new NotImplementedException();
		}

		internal string PrepareToExecutePAA(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::PrepareToExecutePAA()", new object[0]);
			KeyMappings keyMappings = this.HaveAutoActions ? this.personalAutoAttendant.AutoActionsList : this.personalAutoAttendant.KeyMappingList;
			foreach (KeyMappingBase keyMappingBase in keyMappings.SortedMenu)
			{
				PAAManagerBase.PAAPresentationObject paapresentationObject = PAAManagerBase.PAAPresentationObject.Create(keyMappingBase, this.Subscriber);
				this.menu[keyMappingBase.Key] = paapresentationObject;
				paapresentationObject.SetVariablesForMainMenu(this);
			}
			if (!this.personalAutoAttendant.EnableBargeIn)
			{
				this.mainMenuUninterruptible = true;
			}
			return null;
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::Start()", new object[0]);
			this.PersonalAutoAttendant = null;
			base.Start(vo, refInfo);
		}

		internal virtual string GetGreeting(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetGreeting() PAA = {0}", new object[]
			{
				this.personalAutoAttendant.Identity.ToString()
			});
			this.LoadGreetingForPAA(this.personalAutoAttendant);
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PAAManagerBase>(this);
		}

		protected void LoadGreetingForPAA(PersonalAutoAttendant paa)
		{
			ITempWavFile tempWavFile = null;
			this.haveGreeting = false;
			this.personalGreeting = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAManagerBase::LoadGreetingForPAA() PAA {0}", new object[]
			{
				paa.Identity
			});
			try
			{
				using (IPAAStore ipaastore = PAAStore.Create(this.subscriber))
				{
					using (GreetingBase greetingBase = ipaastore.OpenGreeting(paa))
					{
						if (greetingBase != null)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAManagerBase::LoadGreetingForPAA() Found greeting for PAA {0}", new object[]
							{
								paa.Identity
							});
							tempWavFile = greetingBase.Get();
						}
						else
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PAAManagerBase::LoadGreetingForPAA() Did not find greeting for PAA {0}", new object[]
							{
								paa.Identity
							});
						}
					}
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception downloading PAA Greeting {0}", new object[]
				{
					ex
				});
			}
			catch (ObjectDisposedException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception downloading PAA Greeting {0}", new object[]
				{
					ex2
				});
			}
			if (tempWavFile != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Downloaded greeting for PAA {0}", new object[]
				{
					paa.Identity
				});
				this.personalGreeting = tempWavFile;
				this.haveGreeting = true;
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Could not download greeting for PAA {0}", new object[]
			{
				paa.Identity
			});
		}

		private void SetMenuItem(int key, string type, string context)
		{
			PAAMenuItem paamenuItem = this.paaMenuItems[key];
			paamenuItem.Enabled = true;
			paamenuItem.MenuType = type;
			paamenuItem.Context = context;
		}

		private bool haveGreeting;

		private bool transferToVoiceMessageEnabled;

		private object recordedName;

		private ITempWavFile personalGreeting;

		private PersonalAutoAttendant personalAutoAttendant;

		private UMSubscriber subscriber;

		private PAAMenuItem[] paaMenuItems;

		private bool mainMenuUninterruptible;

		private Dictionary<int, PAAManagerBase.PAAPresentationObject> menu = new Dictionary<int, PAAManagerBase.PAAPresentationObject>();

		internal abstract class PAAPresentationObject
		{
			protected PAAPresentationObject(KeyMappingBase menuItem)
			{
				this.configMenuItem = menuItem;
			}

			protected int Key
			{
				get
				{
					return this.configMenuItem.Key;
				}
			}

			protected string Context
			{
				get
				{
					return this.configMenuItem.Context;
				}
			}

			protected KeyMappingBase ConfigMenuItem
			{
				get
				{
					return this.configMenuItem;
				}
			}

			internal static PAAManagerBase.PAAPresentationObject Create(KeyMappingBase menuItem, UMSubscriber subscriber)
			{
				PAAManagerBase.PAAPresentationObject result = null;
				switch (menuItem.KeyMappingType)
				{
				case KeyMappingTypeEnum.TransferToNumber:
					result = new PAAManagerBase.TransferToNumberUI((TransferToNumber)menuItem);
					break;
				case KeyMappingTypeEnum.TransferToADContactMailbox:
					result = new PAAManagerBase.TransferToADContactMailboxUI((TransferToADContactMailbox)menuItem, subscriber);
					break;
				case KeyMappingTypeEnum.TransferToADContactPhone:
					result = new PAAManagerBase.TransferToADContactPhoneUI((TransferToADContactPhone)menuItem, subscriber);
					break;
				case KeyMappingTypeEnum.TransferToVoicemail:
					result = new PAAManagerBase.TransferToVoiceMailUI((TransferToVoicemail)menuItem);
					break;
				case KeyMappingTypeEnum.FindMe:
					result = new PAAManagerBase.TransferToFindMeUI((TransferToFindMe)menuItem);
					break;
				}
				return result;
			}

			internal virtual void SetVariablesForMainMenu(IPAAUI manager)
			{
			}

			internal virtual void SetVariablesForTransfer(IPAAUI manager)
			{
			}

			private KeyMappingBase configMenuItem;
		}

		internal class TransferToNumberUI : PAAManagerBase.PAAPresentationObject
		{
			internal TransferToNumberUI(TransferToNumber menuItem) : base(menuItem)
			{
			}

			internal override void SetVariablesForMainMenu(IPAAUI manager)
			{
				TransferToNumber transferToNumber = base.ConfigMenuItem as TransferToNumber;
				manager.SetPhoneNumberTransferMenuItem(base.Key, base.ConfigMenuItem.KeyMappingType.ToString(), base.Context, transferToNumber.PhoneNumberString);
			}

			internal override void SetVariablesForTransfer(IPAAUI manager)
			{
				TransferToNumber transferToNumber = (TransferToNumber)base.ConfigMenuItem;
				manager.SetBlindTransferEnabled(false, null);
				if (transferToNumber.ValidationResult == PAAValidationResult.Valid)
				{
					IPhoneNumberTarget phoneNumberTarget = transferToNumber;
					PhoneNumber dialableNumber = phoneNumberTarget.GetDialableNumber();
					manager.SetBlindTransferEnabled(true, dialableNumber);
					return;
				}
				if (transferToNumber.ValidationResult == PAAValidationResult.PermissionCheckFailure)
				{
					manager.SetPermissionCheckFailure();
					return;
				}
				manager.SetPermissionCheckFailure();
			}
		}

		internal abstract class TransferToADContactUI : PAAManagerBase.PAAPresentationObject
		{
			internal TransferToADContactUI(TransferToADContact menuItem, UMSubscriber subscriber) : base(menuItem)
			{
				this.legacyExchangeDN = menuItem.LegacyExchangeDN;
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(subscriber);
				this.recipient = iadrecipientLookup.LookupByLegacyExchangeDN(this.legacyExchangeDN);
			}

			protected ADRecipient Recipient
			{
				get
				{
					return this.recipient;
				}
			}

			protected string LegacyExchangeDN
			{
				get
				{
					return this.legacyExchangeDN;
				}
			}

			internal override void SetVariablesForMainMenu(IPAAUI manager)
			{
				manager.SetADTransferTargetMenuItem(base.Key, base.ConfigMenuItem.KeyMappingType.ToString(), base.ConfigMenuItem.Context, this.legacyExchangeDN, this.Recipient);
			}

			private ADRecipient recipient;

			private string legacyExchangeDN;
		}

		internal class TransferToADContactPhoneUI : PAAManagerBase.TransferToADContactUI
		{
			internal TransferToADContactPhoneUI(TransferToADContactPhone menuItem, UMSubscriber subscriber) : base(menuItem, subscriber)
			{
			}

			internal override void SetVariablesForTransfer(IPAAUI manager)
			{
				manager.SetBlindTransferEnabled(false, null);
				if (base.ConfigMenuItem.ValidationResult == PAAValidationResult.Valid)
				{
					IPhoneNumberTarget phoneNumberTarget = (IPhoneNumberTarget)base.ConfigMenuItem;
					PhoneNumber dialableNumber = phoneNumberTarget.GetDialableNumber();
					manager.SetBlindTransferEnabled(true, dialableNumber);
					return;
				}
				if (base.ConfigMenuItem.ValidationResult == PAAValidationResult.PermissionCheckFailure)
				{
					manager.SetPermissionCheckFailure();
					return;
				}
				manager.SetInvalidADContact();
			}
		}

		internal class TransferToADContactMailboxUI : PAAManagerBase.TransferToADContactUI
		{
			internal TransferToADContactMailboxUI(TransferToADContactMailbox menuItem, UMSubscriber subscriber) : base(menuItem, subscriber)
			{
			}

			internal override void SetVariablesForTransfer(IPAAUI manager)
			{
				if (base.Recipient != null)
				{
					manager.SetADTransferTarget(base.Recipient);
					manager.SetTransferToMailboxEnabled();
					return;
				}
				manager.SetInvalidADContact();
			}
		}

		internal class TransferToVoiceMailUI : PAAManagerBase.PAAPresentationObject
		{
			internal TransferToVoiceMailUI(TransferToVoicemail menuItem) : base(menuItem)
			{
			}

			internal override void SetVariablesForMainMenu(IPAAUI manager)
			{
				manager.SetMenuItemTransferToVoiceMail();
			}

			internal override void SetVariablesForTransfer(IPAAUI manager)
			{
				manager.SetTransferToVoiceMessageEnabled();
			}
		}

		internal class TransferToFindMeUI : PAAManagerBase.PAAPresentationObject
		{
			internal TransferToFindMeUI(TransferToFindMe menuItem) : base(menuItem)
			{
			}

			internal override void SetVariablesForMainMenu(IPAAUI manager)
			{
				KeyMappingBase configMenuItem = base.ConfigMenuItem;
				manager.SetFindMeMenuItem(base.Key, base.ConfigMenuItem.KeyMappingType.ToString(), base.Context);
			}

			internal override void SetVariablesForTransfer(IPAAUI manager)
			{
				TransferToFindMe transferToFindMe = base.ConfigMenuItem as TransferToFindMe;
				manager.SetFindMeNumbers(transferToFindMe.Numbers.NumberList);
			}
		}
	}
}

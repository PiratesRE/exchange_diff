using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PersonalOptionsManager : ActivityManager
	{
		internal PersonalOptionsManager(ActivityManager manager, PersonalOptionsManager.ConfigClass config) : base(manager, config)
		{
			this.TimeZoneIndex = -1;
			base.WriteVariable("greeting", null);
		}

		private int TimeZoneIndex
		{
			get
			{
				object obj = this.ReadVariable("timeZoneIndex");
				if (obj == null)
				{
					return -1;
				}
				return (int)obj;
			}
			set
			{
				base.WriteVariable("timeZoneIndex", value);
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			CultureInfo telephonyCulture = callerInfo.TelephonyCulture;
			base.WriteVariable("Oof", callerInfo.ConfigFolder.IsOof);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = callerInfo.CreateSessionLock())
			{
				base.WriteVariable("emailOof", CommonUtil.GetEmailOOFStatus(mailboxSessionLock.Session));
			}
			base.WriteVariable("timeFormat24", CommonUtil.Is24HourTimeFormat(telephonyCulture.DateTimeFormat.ShortTimePattern));
			string text;
			string text2;
			CommonUtil.GetStandardTimeFormats(telephonyCulture, out text, out text2);
			base.WriteVariable("canToggleTimeFormat", text != null && text2 != null);
			ADUser aduser = (ADUser)callerInfo.ADRecipient;
			bool flag = callerInfo.IsASREnabled && Util.IsSpeechCulture(callerInfo.TelephonyCulture);
			base.WriteVariable("canToggleASR", flag);
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
			if (!u.IsAuthenticated || (this.GlobalManager.LimitedOVAAccess && !u.ConfigFolder.IsFirstTimeUser))
			{
				base.CheckAuthorization(u);
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Personal Options Manager asked to do action {0}.", new object[]
			{
				action
			});
			string input = null;
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			GreetingBase greetingBase = null;
			TransitionBase transition;
			try
			{
				if (PersonalOptionsManager.IsFetchAction(action, callerInfo, out greetingBase, out input))
				{
					if (this.FetchGreeting(greetingBase))
					{
						input = null;
					}
				}
				else if (PersonalOptionsManager.IsSaveAction(action, callerInfo, out greetingBase))
				{
					this.SaveGreeting(greetingBase);
				}
				else if (PersonalOptionsManager.IsDeleteAction(action, callerInfo, out greetingBase))
				{
					greetingBase.Delete();
				}
				else if (string.Equals(action, "validatePassword", StringComparison.OrdinalIgnoreCase))
				{
					input = this.ValidatePassword(callerInfo, vo);
				}
				else if (string.Equals(action, "matchPasswords", StringComparison.OrdinalIgnoreCase))
				{
					input = this.MatchPasswords(callerInfo, vo);
				}
				else if (string.Equals(action, "getSystemTask", StringComparison.OrdinalIgnoreCase))
				{
					if (this.systemTaskContext == null)
					{
						this.systemTaskContext = new PersonalOptionsManager.SystemTaskContext(vo.CurrentCallContext.CallerInfo, base.Manager);
					}
					bool flag = (bool)this.GlobalManager.ReadVariable("skipPinCheck");
					if (this.systemTaskContext.IsFirstTimeUserTask)
					{
						input = "firstTimeUserTask";
					}
					else if (this.systemTaskContext.IsChangePasswordTask && !flag)
					{
						input = "changePasswordTask";
					}
					else if (this.systemTaskContext.IsOofStatusTask)
					{
						input = "oofStatusTask";
					}
					base.WriteVariable("adminMinPwdLen", callerInfo.PasswordPolicy.MinimumLength);
					base.WriteVariable("adminOldPwdLen", callerInfo.PasswordPolicy.PreviousPasswordsDisallowed);
				}
				else if (string.Equals(action, "getFirstTimeUserTask", StringComparison.OrdinalIgnoreCase))
				{
					if (this.firstTimeUserContext == null)
					{
						this.firstTimeUserContext = new PersonalOptionsManager.FirstTimeUserContext(vo.CurrentCallContext.CallerInfo);
					}
					input = this.GetNextFirstTimeUserTask();
				}
				else if (string.Equals(action, "firstTimeUserComplete", StringComparison.OrdinalIgnoreCase))
				{
					callerInfo.ConfigFolder.IsFirstTimeUser = false;
					callerInfo.ConfigFolder.Save();
				}
				else if (string.Equals(action, "toggleOOF", StringComparison.OrdinalIgnoreCase))
				{
					this.ToggleOOF(vo);
				}
				else if (string.Equals(action, "toggleEmailOOF", StringComparison.OrdinalIgnoreCase))
				{
					this.ToggleEmailOOF(vo);
				}
				else if (string.Equals(action, "toggleTimeFormat", StringComparison.OrdinalIgnoreCase))
				{
					this.ToggleTimeFormat(vo);
				}
				else if (string.Equals(action, "toggleASR", StringComparison.OrdinalIgnoreCase))
				{
					this.ToggleASR(vo);
				}
				else if (string.Equals(action, "findTimeZone", StringComparison.OrdinalIgnoreCase))
				{
					input = this.FindTimeZone();
				}
				else if (string.Equals(action, "nextTimeZone", StringComparison.OrdinalIgnoreCase))
				{
					input = this.NextTimeZone();
				}
				else if (string.Equals(action, "selectTimeZone", StringComparison.OrdinalIgnoreCase))
				{
					input = this.SelectTimeZone(vo);
				}
				else if (string.Equals(action, "firstTimeZone", StringComparison.OrdinalIgnoreCase))
				{
					this.FirstTimeZone();
				}
				else
				{
					if (!string.Equals(action, "setGreetingsAction", StringComparison.OrdinalIgnoreCase))
					{
						return base.ExecuteAction(action, vo);
					}
					base.WriteVariable("lastAction", PersonalOptionsManager.LastAction.Greetings.ToString());
				}
				transition = base.CurrentActivity.GetTransition(input);
			}
			finally
			{
				if (greetingBase != null)
				{
					greetingBase.Dispose();
				}
			}
			return transition;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PersonalOptionsManager>(this);
		}

		private static bool IsFetchAction(string action, UMSubscriber user, out GreetingBase greeting, out string autoEvent)
		{
			greeting = null;
			autoEvent = null;
			if (string.Equals(action, "getExternal", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail);
				autoEvent = "noExternal";
				return true;
			}
			if (string.Equals(action, "getOof", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away);
				autoEvent = "noOof";
				return true;
			}
			if (string.Equals(action, "getName", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenNameGreeting();
				autoEvent = "noName";
				return true;
			}
			return false;
		}

		private static bool IsSaveAction(string action, UMSubscriber user, out GreetingBase greeting)
		{
			greeting = null;
			if (string.Equals(action, "saveExternal", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail);
				return true;
			}
			if (string.Equals(action, "saveOof", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away);
				return true;
			}
			if (string.Equals(action, "saveName", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenNameGreeting();
				return true;
			}
			return false;
		}

		private static bool IsDeleteAction(string action, UMSubscriber user, out GreetingBase greeting)
		{
			greeting = null;
			if (string.Equals(action, "deleteExternal", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail);
				return true;
			}
			if (string.Equals(action, "deleteOof", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Away);
				return true;
			}
			if (string.Equals(action, "deleteName", StringComparison.OrdinalIgnoreCase))
			{
				greeting = user.ConfigFolder.OpenNameGreeting();
				return true;
			}
			return false;
		}

		private string SelectTimeZone(BaseUMCallSession vo)
		{
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			string text = (string)this.ReadVariable("lastInput");
			if (string.IsNullOrEmpty(text))
			{
				return "invalidTimeZone";
			}
			int num = int.Parse(text, CultureInfo.InvariantCulture) - 1;
			if (num < 0 || num >= this.timeZoneList.Count)
			{
				return "invalidTimeZone";
			}
			ExTimeZone exTimeZone = this.timeZoneList[num];
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = callerInfo.CreateSessionLock())
			{
				CommonUtil.SetOwaTimeZone(mailboxSessionLock.Session, exTimeZone.Id);
				mailboxSessionLock.Session.ExTimeZone = exTimeZone;
			}
			base.WriteVariable("currentTimeZone", exTimeZone);
			return null;
		}

		private void FirstTimeZone()
		{
			this.TimeZoneIndex = -1;
			this.NextTimeZone();
		}

		private string NextTimeZone()
		{
			ExTimeZone exTimeZone = (ExTimeZone)this.ReadVariable("currentTimeZone");
			if (++this.TimeZoneIndex < this.timeZoneList.Count)
			{
				ExTimeZone exTimeZone2 = this.timeZoneList[this.TimeZoneIndex];
				base.WriteVariable("currentTimeZone", exTimeZone2);
				if (exTimeZone == null || exTimeZone.TimeZoneInformation.StandardBias != exTimeZone2.TimeZoneInformation.StandardBias)
				{
					TimeSpan standardBias = exTimeZone2.TimeZoneInformation.StandardBias;
					base.WriteVariable("offsetHours", Math.Abs(standardBias.Hours));
					base.WriteVariable("offsetMinutes", Math.Abs(standardBias.Minutes));
					base.WriteVariable("playGMTOffset", true);
					base.WriteVariable("positiveOffset", exTimeZone2.TimeZoneInformation.StandardBias.TotalMinutes >= 0.0);
				}
				else
				{
					base.WriteVariable("playGMTOffset", false);
				}
				return null;
			}
			base.WriteVariable("currentTimeZone", null);
			return "endOfTimeZoneList";
		}

		private string FindTimeZone()
		{
			string text = (string)this.ReadVariable("lastInput");
			if (string.IsNullOrEmpty(text))
			{
				return "invalidTimeFormat";
			}
			if (text.EndsWith("#", StringComparison.InvariantCulture))
			{
				text = text.Remove(text.Length - 1);
			}
			if (text.Length < 3 || text.Length > 4)
			{
				return "invalidTimeFormat";
			}
			TimeSpan targetLocalTime;
			try
			{
				int num = int.Parse(text.Substring(text.Length - 2, 2), CultureInfo.InvariantCulture);
				int num2 = int.Parse(text.Substring(0, text.Length - 2), CultureInfo.InvariantCulture);
				if (num > 59 || num2 > 23)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PersonalOptions::FindTimeZone: Either hours({0}) or minutes({1}) are out of range.", new object[]
					{
						num2,
						num
					});
					return "invalidTimeFormat";
				}
				targetLocalTime = new TimeSpan(num2, num, 0);
			}
			catch (FormatException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PersonalOptions::FindTimeZone: {0}.", new object[]
				{
					ex
				});
				return "invalidTimeFormat";
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PersonalOptions::FindTimeZone: {0}.", new object[]
				{
					ex2
				});
				return "invalidTimeFormat";
			}
			this.timeZoneList = CommonUtil.GetTimeZonesForLocalTime(targetLocalTime, Constants.TimeZoneErrorSpan);
			if (this.timeZoneList.Count > 0)
			{
				this.FirstTimeZone();
			}
			if (this.timeZoneList.Count <= 0)
			{
				return "invalidTimeZone";
			}
			return null;
		}

		private void ToggleASR(BaseUMCallSession vo)
		{
			base.LastRecoEvent = string.Empty;
			base.UseASR = !base.UseASR;
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			callerInfo.ConfigFolder.UseAsr = base.UseASR;
			callerInfo.ConfigFolder.Save();
		}

		private void ToggleOOF(BaseUMCallSession vo)
		{
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			callerInfo.ConfigFolder.IsOof = !callerInfo.ConfigFolder.IsOof;
			callerInfo.ConfigFolder.Save();
			base.WriteVariable("Oof", callerInfo.ConfigFolder.IsOof);
			base.Manager.WriteVariable("Oof", callerInfo.ConfigFolder.IsOof);
			base.WriteVariable("lastAction", PersonalOptionsManager.LastAction.ToggleOOF.ToString());
		}

		private void ToggleEmailOOF(BaseUMCallSession vo)
		{
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			bool flag = !(bool)this.ReadVariable("emailOof");
			string replyText = Strings.DefaultEmailOOFText(callerInfo.DisplayName).ToString(callerInfo.TelephonyCulture);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = callerInfo.CreateSessionLock())
			{
				CommonUtil.SetEmailOOFStatus(mailboxSessionLock.Session, flag, replyText);
			}
			base.WriteVariable("emailOof", flag);
		}

		private void ToggleTimeFormat(BaseUMCallSession vo)
		{
			CultureInfo telephonyCulture = vo.CurrentCallContext.CallerInfo.TelephonyCulture;
			string text;
			string text2;
			CommonUtil.GetStandardTimeFormats(telephonyCulture, out text, out text2);
			string text3;
			if (CommonUtil.Is24HourTimeFormat(telephonyCulture.DateTimeFormat.ShortTimePattern))
			{
				text3 = text;
			}
			else
			{
				text3 = text2;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ToggleTimeFormat -> Current:{0} - New:{1}.", new object[]
			{
				telephonyCulture.DateTimeFormat.ShortTimePattern,
				text3
			});
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = vo.CurrentCallContext.CallerInfo.CreateSessionLock())
			{
				CommonUtil.SetOwaTimeFormat(mailboxSessionLock.Session, text3);
			}
			telephonyCulture.DateTimeFormat.ShortTimePattern = text3;
			base.WriteVariable("timeFormat24", CommonUtil.Is24HourTimeFormat(telephonyCulture.DateTimeFormat.ShortTimePattern));
		}

		private string GetNextFirstTimeUserTask()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GetNextFirstTimeUserTask::FirstTimeUserContext={0}.", new object[]
			{
				this.firstTimeUserContext
			});
			if (this.firstTimeUserContext.PlayPassword)
			{
				this.firstTimeUserContext.PlayPassword = false;
				return "changePasswordTask";
			}
			if (this.firstTimeUserContext.PlayNameSetup)
			{
				this.firstTimeUserContext.PlayNameSetup = false;
				return "recordNameTask";
			}
			if (this.firstTimeUserContext.PlayExternalSetup)
			{
				this.firstTimeUserContext.PlayExternalSetup = false;
				return "recordExternalTask";
			}
			return null;
		}

		private string ValidatePassword(UMSubscriber user, BaseUMCallSession vo)
		{
			string result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Validating password.  Password failure count is {0}.", new object[]
			{
				this.changePasswordFailures
			});
			UmPasswordManager umPasswordManager = new UmPasswordManager(user);
			if (umPasswordManager.IsValidPassword(base.Password))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Provided password is valid in ValidatePasswordAction.", new object[0]);
				this.firstPwd = base.Password;
				result = "passwordValidated";
			}
			else
			{
				this.changePasswordFailures++;
				this.firstPwd = null;
				base.Password = null;
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Password is invalid in ValidatePasswordAction. Failure Count={0}.", new object[]
				{
					this.changePasswordFailures
				});
				if (this.changePasswordFailures >= 5)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Disconnecting user after count={0} failed attempts in change password.", new object[]
					{
						this.changePasswordFailures
					});
					this.DropCall(vo, DropCallReason.UserError);
					result = "stopEvent";
				}
			}
			return result;
		}

		private string MatchPasswords(UMSubscriber user, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Validating password.  Password failure count is {0}.", new object[]
			{
				this.changePasswordFailures
			});
			string result = null;
			if (base.Password == this.firstPwd)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Passwords match in MatchPasswords.  Setting user password.", new object[0]);
				this.changePasswordFailures = 0;
				UmPasswordManager umPasswordManager = new UmPasswordManager(user);
				umPasswordManager.SetPassword(base.Password);
				result = "passwordsMatch";
			}
			else
			{
				this.changePasswordFailures++;
				this.firstPwd = null;
				base.Password = null;
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Passwords do not match in MatchPasswordsAction. Failure Count={0}.", new object[]
				{
					this.changePasswordFailures
				});
				if (this.changePasswordFailures >= 5)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Disconnecting user after count={0} failed attempts in change password.", new object[]
					{
						this.changePasswordFailures
					});
					this.DropCall(vo, DropCallReason.UserError);
					result = "stopEvent";
				}
			}
			return result;
		}

		private bool FetchGreeting(GreetingBase g)
		{
			base.WriteVariable("greeting", null);
			base.RecordContext.Reset();
			ITempWavFile tempWavFile = g.Get();
			if (tempWavFile == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "No greeting found for {0}.  Returning false.", new object[]
				{
					g.GetType()
				});
				return false;
			}
			base.WriteVariable("greeting", tempWavFile);
			return true;
		}

		private void SaveGreeting(GreetingBase g)
		{
			ITempWavFile recording = base.RecordContext.Recording;
			base.RecordContext.Reset();
			if (recording != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Saving greeting at file={0} to store.", new object[]
				{
					recording.FilePath
				});
				g.Put(recording.FilePath);
			}
		}

		private EncryptedBuffer firstPwd;

		private int changePasswordFailures;

		private List<ExTimeZone> timeZoneList;

		private PersonalOptionsManager.SystemTaskContext systemTaskContext;

		private PersonalOptionsManager.FirstTimeUserContext firstTimeUserContext;

		internal enum LastAction
		{
			ToggleOOF,
			Greetings
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing PersonalOptions activity manager.", new object[0]);
				return new PersonalOptionsManager(manager, this);
			}
		}

		private class SystemTaskContext
		{
			internal SystemTaskContext(UMSubscriber user, ActivityManager manager)
			{
				UmPasswordManager umPasswordManager = new UmPasswordManager(user);
				this.firstTimeUserTask = user.ConfigFolder.IsFirstTimeUser;
				this.changePasswordTask = umPasswordManager.IsExpired;
				this.oofStatusTask = (manager != null && Shortcut.OOF == manager.LastShortcut);
			}

			internal bool IsFirstTimeUserTask
			{
				get
				{
					return this.firstTimeUserTask;
				}
			}

			internal bool IsChangePasswordTask
			{
				get
				{
					return this.changePasswordTask;
				}
			}

			internal bool IsOofStatusTask
			{
				get
				{
					return this.oofStatusTask;
				}
			}

			private bool firstTimeUserTask;

			private bool changePasswordTask;

			private bool oofStatusTask;
		}

		private class FirstTimeUserContext
		{
			internal FirstTimeUserContext(UMSubscriber user)
			{
				GreetingBase greetingBase = null;
				try
				{
					UmPasswordManager umPasswordManager = new UmPasswordManager(user);
					this.playPassword = umPasswordManager.IsExpired;
					greetingBase = user.ConfigFolder.OpenNameGreeting();
					this.playNameSetup = !greetingBase.Exists();
					greetingBase.Dispose();
					greetingBase = user.ConfigFolder.OpenCustomMailboxGreeting(MailboxGreetingEnum.Voicemail);
					this.playExternalSetup = !greetingBase.Exists();
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "FirstTimeUserContext initialized {0}.", new object[]
					{
						this
					});
				}
				finally
				{
					if (greetingBase != null)
					{
						greetingBase.Dispose();
					}
				}
			}

			internal bool PlayPassword
			{
				get
				{
					return this.playPassword;
				}
				set
				{
					this.playPassword = value;
				}
			}

			internal bool PlayNameSetup
			{
				get
				{
					return this.playNameSetup;
				}
				set
				{
					this.playNameSetup = value;
				}
			}

			internal bool PlayExternalSetup
			{
				get
				{
					return this.playExternalSetup;
				}
				set
				{
					this.playExternalSetup = value;
				}
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "PlayPassword={0}, PlayNameSetup={1}, PlayExternalSetup={2}", new object[]
				{
					this.PlayPassword,
					this.PlayNameSetup,
					this.PlayExternalSetup
				});
			}

			private bool playPassword;

			private bool playNameSetup;

			private bool playExternalSetup;
		}
	}
}

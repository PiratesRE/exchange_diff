using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayOnPhoneAAManager : ActivityManager
	{
		internal PlayOnPhoneAAManager(ActivityManager manager, PlayOnPhoneAAManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool FileExists
		{
			get
			{
				return this.specifiedFileExists;
			}
		}

		internal string ExistingFilePath
		{
			get
			{
				return this.existingFileFullPath;
			}
		}

		internal bool PlayingExistingGreetingFirstTime
		{
			get
			{
				return this.playingExistingGreetingFirstTime;
			}
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneAAManager : Start() ", new object[0]);
			this.aa = vo.CurrentCallContext.AutoAttendantInfo;
			PlayOnPhoneAAGreetingRequest playOnPhoneAAGreetingRequest = vo.CurrentCallContext.WebServiceRequest as PlayOnPhoneAAGreetingRequest;
			this.filename = playOnPhoneAAGreetingRequest.FileName;
			this.configCache = vo.CurrentCallContext.UMConfigCache;
			this.userRecordingTheGreeting = playOnPhoneAAGreetingRequest.UserRecordingTheGreeting;
			this.CheckIfFileExists();
			vo.CurrentCallContext.WebServiceRequest.EndResult = UMOperationResult.InProgress;
			base.Start(vo, refInfo);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			if (!this.greetingSaved)
			{
				this.SetOperationResultFailed(vo);
			}
			base.OnUserHangup(vo, callSessionEventArgs);
		}

		internal string ExistingGreetingAlreadyPlayed(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneAAManager : ExistingGreetingAlreadyPlayed() ", new object[0]);
			this.playingExistingGreetingFirstTime = false;
			return null;
		}

		internal string SaveGreeting(BaseUMCallSession vo)
		{
			string filePath = base.RecordContext.Recording.FilePath;
			IPublishingSession publishingSession = null;
			try
			{
				publishingSession = PublishingPoint.GetPublishingSession(this.userRecordingTheGreeting, this.aa);
				publishingSession.Upload(filePath, this.filename);
			}
			catch (PublishingException)
			{
				return "failedToSaveGreeting";
			}
			finally
			{
				if (publishingSession != null)
				{
					publishingSession.Dispose();
				}
			}
			this.configCache.SetPrompt<UMAutoAttendant>(this.aa, filePath, this.filename);
			this.greetingSaved = true;
			return null;
		}

		internal string SetOperationResultFailed(BaseUMCallSession vo)
		{
			vo.CurrentCallContext.WebServiceRequest.EndResult = UMOperationResult.Failure;
			return null;
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			if (reason != DropCallReason.GracefulHangup)
			{
				this.SetOperationResultFailed(vo);
			}
			base.DropCall(vo, reason);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PlayOnPhoneAAManager>(this);
		}

		private void CheckIfFileExists()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneAAManager : CheckIfFileExists() ", new object[0]);
			try
			{
				this.existingFileFullPath = this.configCache.CheckIfFileExists<UMAutoAttendant>(this.aa, this.filename);
			}
			catch (FileNotFoundException)
			{
			}
			catch (IOException)
			{
			}
			this.specifiedFileExists = (this.existingFileFullPath != null);
		}

		private UMAutoAttendant aa;

		private UMConfigCache configCache;

		private string filename;

		private string userRecordingTheGreeting;

		private string existingFileFullPath;

		private bool specifiedFileExists;

		private bool playingExistingGreetingFirstTime = true;

		private bool greetingSaved;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing PlayOnPhoneAAManager activity manager.", new object[0]);
				return new PlayOnPhoneAAManager(manager, this);
			}
		}
	}
}

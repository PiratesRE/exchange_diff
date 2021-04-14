using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayOnPhonePAAManager : PAAManagerBase
	{
		internal PlayOnPhonePAAManager(ActivityManager manager, PlayOnPhonePAAManager.ConfigClass config) : base(manager, config)
		{
		}

		internal ITempWavFile Recording
		{
			get
			{
				return (ITempWavFile)this.ReadVariable("recording");
			}
			set
			{
				base.WriteVariable("recording", value);
			}
		}

		internal bool ValidPAA
		{
			get
			{
				return this.validPAA;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager::Start()", new object[0]);
			base.Subscriber = vo.CurrentCallContext.CallerInfo;
			base.Start(vo, refInfo);
		}

		internal string GetAutoAttendant(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager::GetAutoAttendant()", new object[0]);
			PlayOnPhonePAAGreetingRequest playOnPhonePAAGreetingRequest = (PlayOnPhonePAAGreetingRequest)vo.CurrentCallContext.WebServiceRequest;
			Guid identity = playOnPhonePAAGreetingRequest.Identity;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager::GetAutoAttendant() identity = {0}", new object[]
			{
				identity.ToString()
			});
			using (IPAAStore ipaastore = PAAStore.Create(base.Subscriber))
			{
				PersonalAutoAttendant autoAttendant = ipaastore.GetAutoAttendant(identity, PAAValidationMode.None);
				if (autoAttendant == null)
				{
					CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager::GetAutoAttendant() did not find a PAA with identity = {0}", new object[]
					{
						identity.ToString()
					});
					this.validPAA = false;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager::GetAutoAttendant() Found a PAA with identity = {0}", new object[]
					{
						identity.ToString()
					});
					this.validPAA = true;
				}
				base.PersonalAutoAttendant = autoAttendant;
			}
			return null;
		}

		internal string ClearRecording(BaseUMCallSession vo)
		{
			base.RecordContext.Reset();
			this.Recording = null;
			return null;
		}

		internal string SaveGreeting(BaseUMCallSession vo)
		{
			ITempWavFile recording = this.Recording;
			using (IPAAStore ipaastore = PAAStore.Create(base.Subscriber))
			{
				using (GreetingBase greetingBase = ipaastore.OpenGreeting(base.PersonalAutoAttendant))
				{
					greetingBase.Put(recording.FilePath);
				}
			}
			return null;
		}

		internal string DeleteGreeting(BaseUMCallSession vo)
		{
			using (IPAAStore ipaastore = PAAStore.Create(base.Subscriber))
			{
				using (GreetingBase greetingBase = ipaastore.OpenGreeting(base.PersonalAutoAttendant))
				{
					greetingBase.Delete();
				}
				base.PersonalGreeting = null;
				base.HaveGreeting = false;
			}
			return null;
		}

		private bool validPAA;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PlayOnPhonePAAManager.ConfigClass::CreateActivityManager()", new object[0]);
				return new PlayOnPhonePAAManager(manager, this);
			}
		}
	}
}

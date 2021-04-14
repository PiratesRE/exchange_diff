using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Speech.Synthesis;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class PromptPlayer : DisposableBase
	{
		public PromptPlayer(UcmaCallSession session)
		{
			this.diag = new DiagnosticHelper(this, ExTraceGlobals.UCMATracer);
			this.session = session;
			this.CreatePlayer();
			this.CreateSynthesizers();
			this.Subscribe();
		}

		public event EventHandler<PromptPlayer.PlayerCompletedEventArgs> SpeakCompleted;

		public event EventHandler<BookmarkReachedEventArgs> BookmarkReached;

		public bool MediaDropped
		{
			get
			{
				return this.connectorShouldBeActive && !this.connector.IsActive;
			}
		}

		public void AttachFlow(AudioVideoFlow flow)
		{
			this.ucmaPlayer.AttachFlow(flow);
			this.connector.AttachFlow(flow);
			this.synth.SetOutputToAudioStream(this.connector, UcmaCallSession.SpeechAudioFormatInfo);
		}

		public void Play(ArrayList prompts, CultureInfo culture, TimeSpan offset)
		{
			this.diag.Trace("PromptPlayer::Play", new object[0]);
			this.activePlayer = this.DeterminePlayerToUse(prompts);
			switch (this.activePlayer)
			{
			case PromptPlayer.PlayerType.Ucma:
				this.StartUcmaPlayer((TempFilePrompt)prompts[0], offset);
				return;
			case PromptPlayer.PlayerType.Synth:
				this.StartSynth(this.BuildPrompts(prompts, culture), offset);
				return;
			default:
				return;
			}
		}

		public void Cancel()
		{
			this.diag.Trace("PromptPlayer::Cancel", new object[0]);
			switch (this.activePlayer)
			{
			case PromptPlayer.PlayerType.Ucma:
				this.CancelUcmaPlayer();
				return;
			case PromptPlayer.PlayerType.Synth:
				this.CancelSynth();
				return;
			default:
				return;
			}
		}

		public void Skip(TimeSpan timeToSkip)
		{
			try
			{
				switch (this.activePlayer)
				{
				case PromptPlayer.PlayerType.Ucma:
					this.ucmaPlayer.Skip((int)timeToSkip.TotalMilliseconds);
					break;
				case PromptPlayer.PlayerType.Synth:
					this.synth.Skip(timeToSkip);
					break;
				}
			}
			catch (InvalidOperationException ex)
			{
				this.diag.Trace("Ignoring IOP in Skip", new object[]
				{
					ex
				});
			}
		}

		public void Pause()
		{
			if (this.ucmaPlayer != null && this.ucmaPlayer.State == null)
			{
				this.ucmaPlayer.Pause();
			}
			if (this.synth != null && (this.synth.State == null || this.synth.State == 1))
			{
				this.synth.Pause();
			}
		}

		public void Resume()
		{
			if (this.ucmaPlayer != null && this.ucmaPlayer.State == 2)
			{
				this.ucmaPlayer.Start();
			}
			if (this.synth != null && this.synth.State == 2)
			{
				this.synth.Resume();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PromptPlayer>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.connector != null)
				{
					this.connector.Dispose();
					this.connector = null;
				}
				if (this.synth != null)
				{
					this.synth.Dispose();
					this.synth = null;
				}
				this.DisposeWmaFileSourceObject();
				this.DisposeWmaBackingFile();
			}
		}

		private void CreatePlayer()
		{
			this.ucmaPlayer = new Player();
		}

		private void CreateSynthesizers()
		{
			this.connector = new SpeechSynthesisConnector();
			this.connector.AudioFormat = 2;
			this.synth = new SpeechSynthesizer();
		}

		private void Subscribe()
		{
			this.PlayerSubscribe();
			this.SynthSubscribe(this.synth);
		}

		private void PlayerSubscribe()
		{
			this.ucmaPlayer.StateChanged += delegate(object sender, PlayerStateChangedEventArgs args)
			{
				this.session.Serializer.SerializeEvent<PlayerStateChangedEventArgs>(sender, args, new SerializableEventHandler<PlayerStateChangedEventArgs>(this.Player_StateChanged), this.session, false, "Player_StateChanged");
			};
		}

		private void SynthSubscribe(SpeechSynthesizer s)
		{
			s.BookmarkReached += delegate(object sender, BookmarkReachedEventArgs args)
			{
				this.session.Serializer.SerializeEvent<BookmarkReachedEventArgs>(sender, args, new SerializableEventHandler<BookmarkReachedEventArgs>(this.Synth_BookmarkReached), this.session, false, "Synthesizer_BookmarkReached");
			};
			s.SpeakCompleted += delegate(object sender, SpeakCompletedEventArgs args)
			{
				this.session.Serializer.SerializeEvent<SpeakCompletedEventArgs>(sender, args, new SerializableEventHandler<SpeakCompletedEventArgs>(this.Synth_SpeakCompleted), this.session, false, "Synthesizer_SpeakCompleted");
			};
			s.SpeakStarted += delegate(object sender, SpeakStartedEventArgs args)
			{
				this.session.Serializer.SerializeEvent<SpeakStartedEventArgs>(sender, args, new SerializableEventHandler<SpeakStartedEventArgs>(this.Synth_SpeakStarted), this.session, false, "Synthesizer_SpeakStarted");
			};
		}

		private void Player_StateChanged(object sender, PlayerStateChangedEventArgs e)
		{
			if (e.PreviousState == null)
			{
				if (e.State == 1)
				{
					this.FireSpeakCompleted(e.TransitionReason == 0);
					return;
				}
			}
			else if (e.PreviousState == 1 && e.State == null && this.pendingOffset != TimeSpan.Zero)
			{
				this.ucmaPlayer.Skip((int)this.pendingOffset.TotalMilliseconds);
				this.pendingOffset = TimeSpan.Zero;
			}
		}

		private void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (e.Error != null)
				{
					this.diag.Trace("PromptPlayer::Synth_SpeakCompleted returned with Cancelled set. Error = {0}", new object[]
					{
						e.Error
					});
				}
				else
				{
					this.diag.Trace("PromptPlayer::Synth_SpeakCompleted returned with Cancelled set. No Error returned", new object[0]);
				}
			}
			this.FireSpeakCompleted(e.Cancelled);
		}

		private void Synth_BookmarkReached(object sender, BookmarkReachedEventArgs e)
		{
			if (this.BookmarkReached != null)
			{
				this.BookmarkReached(sender, e);
			}
		}

		private void Synth_SpeakStarted(object sender, SpeakStartedEventArgs e)
		{
			if (this.synth.State == 2 && this.pendingOffset > TimeSpan.Zero)
			{
				this.synth.Skip(this.pendingOffset);
				this.synth.Resume();
				this.pendingOffset = TimeSpan.Zero;
			}
		}

		private void FireSpeakCompleted(bool cancelled)
		{
			this.diag.Trace("PromptPlayer::FireSpeakCompleted", new object[0]);
			this.activePlayer = PromptPlayer.PlayerType.None;
			if (this.SpeakCompleted != null)
			{
				PromptPlayer.PlayerCompletedEventArgs e = new PromptPlayer.PlayerCompletedEventArgs
				{
					Cancelled = cancelled
				};
				this.SpeakCompleted(this, e);
			}
		}

		private PromptPlayer.PlayerType DeterminePlayerToUse(ArrayList prompts)
		{
			ValidateArgument.NotNull(prompts, "prompts");
			ExAssert.RetailAssert(prompts.Count > 0, "no prompts!");
			PromptPlayer.PlayerType playerType;
			if (prompts.Count > 1)
			{
				playerType = PromptPlayer.PlayerType.Synth;
			}
			else if (prompts[0] is TempFilePrompt)
			{
				playerType = PromptPlayer.PlayerType.Ucma;
			}
			else
			{
				playerType = PromptPlayer.PlayerType.Synth;
			}
			this.diag.Trace("Chose player type {0}", new object[]
			{
				playerType
			});
			return playerType;
		}

		private PromptBuilder BuildPrompts(ArrayList prompts, CultureInfo culture)
		{
			PromptBuilder promptBuilder = new PromptBuilder(culture);
			foreach (object obj in prompts)
			{
				Prompt prompt = (Prompt)obj;
				promptBuilder.AppendSsmlMarkup(prompt.ToSsml());
			}
			return promptBuilder;
		}

		private void StartSynth(PromptBuilder prompts, TimeSpan offset)
		{
			this.connector.Stop();
			this.connectorShouldBeActive = true;
			this.connector.Start();
			if (offset > TimeSpan.Zero)
			{
				this.synth.Pause();
				this.pendingOffset = offset;
			}
			else
			{
				prompts.AppendBreak(PromptPlayer.endMenuBreak);
			}
			this.synth.SpeakAsync(prompts);
		}

		private void StartUcmaPlayer(TempFilePrompt prompt, TimeSpan offset)
		{
			this.pendingOffset = offset;
			this.ucmaPlayer.Stop();
			this.CreateWmaBackingFile();
			this.WrapPromptInWmaEnvelope(prompt);
			this.PrepareAndSetMediaSource();
			this.SetUcmaProsodyRate(prompt);
			this.ucmaPlayer.Start();
		}

		private void SetUcmaProsodyRate(Prompt prompt)
		{
			PlaybackSpeed playbackSpeed = 100;
			string prosodyRate;
			switch (prosodyRate = prompt.ProsodyRate)
			{
			case "-60%":
				playbackSpeed = 50;
				break;
			case "-30%":
				playbackSpeed = 75;
				break;
			case "-15%":
				playbackSpeed = 75;
				break;
			case "+0%":
				playbackSpeed = 100;
				break;
			case "+15%":
				playbackSpeed = 150;
				break;
			case "+30%":
				playbackSpeed = 175;
				break;
			case "+60%":
				playbackSpeed = 200;
				break;
			}
			this.ucmaPlayer.PlaybackSpeed = playbackSpeed;
			this.diag.Trace("Ucma playback speed.  Prosody='{0}', Speed='{1}'", new object[]
			{
				prompt.ProsodyRate,
				playbackSpeed
			});
		}

		private void WrapPromptInWmaEnvelope(TempFilePrompt prompt)
		{
			this.diag.Trace("PromptPlayer::WrapPromptInWmaEnvelope", new object[0]);
			using (PcmReader pcmReader = new PcmReader(prompt.FileName))
			{
				using (WmaWriter wmaWriter = WmaWriter.Create(this.backingWmaFile.FilePath, pcmReader.WaveFormat, WmaCodec.Pcm))
				{
					MediaMethods.ConvertWavToWma(pcmReader, wmaWriter);
				}
			}
		}

		private void CreateWmaBackingFile()
		{
			this.DisposeWmaBackingFile();
			this.backingWmaFile = TempFileFactory.CreateTempWmaFile();
		}

		private void DisposeWmaBackingFile()
		{
			if (this.backingWmaFile != null)
			{
				this.backingWmaFile.Dispose();
				this.backingWmaFile = null;
			}
		}

		private void PrepareAndSetMediaSource()
		{
			this.diag.Trace("PromptPlayer::PrepareAndSetMediaSource", new object[0]);
			this.DisposeWmaFileSourceObject();
			this.wmaFileSourceObject = new WmaFileSource(this.backingWmaFile.FilePath);
			IAsyncResult asyncResult = this.wmaFileSourceObject.BeginPrepareSource(0, null, null);
			this.wmaFileSourceObject.EndPrepareSource(asyncResult);
			this.ucmaPlayer.SetSource(this.wmaFileSourceObject);
		}

		private void DisposeWmaFileSourceObject()
		{
			if (this.wmaFileSourceObject != null)
			{
				this.wmaFileSourceObject.Close();
				this.wmaFileSourceObject = null;
			}
		}

		private void CancelUcmaPlayer()
		{
			this.diag.Trace("PromptPlayer::StopUcmaPlayer", new object[0]);
			if (this.ucmaPlayer.State == null)
			{
				this.ucmaPlayer.Stop();
			}
		}

		private void CancelSynth()
		{
			this.synth.SpeakAsyncCancelAll();
			this.connectorShouldBeActive = false;
			this.connector.Stop();
		}

		private static TimeSpan endMenuBreak = TimeSpan.FromMilliseconds(200.0);

		private ITempFile backingWmaFile;

		private WmaFileSource wmaFileSourceObject;

		private Player ucmaPlayer;

		private SpeechSynthesisConnector connector;

		private SpeechSynthesizer synth;

		private PromptPlayer.PlayerType activePlayer;

		private UcmaCallSession session;

		private DiagnosticHelper diag;

		private TimeSpan pendingOffset;

		private bool connectorShouldBeActive;

		private enum PlayerType
		{
			None,
			Ucma,
			Synth
		}

		internal class PlayerCompletedEventArgs : EventArgs
		{
			public bool Cancelled { get; set; }
		}
	}
}

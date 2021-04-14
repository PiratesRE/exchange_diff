using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMSpeechEventArgs : EventArgs, IPlaybackEventArgs
	{
		internal UMSpeechEventArgs()
		{
			this.elapsedTime = TimeSpan.Zero;
		}

		public TimeSpan PlayTime
		{
			get
			{
				return this.elapsedTime;
			}
			set
			{
				this.elapsedTime = value;
			}
		}

		public int LastPrompt
		{
			get
			{
				return this.lastPrompt;
			}
			set
			{
				this.lastPrompt = value;
			}
		}

		public bool WasPlaybackStopped
		{
			get
			{
				return this.wasPlaybackStopped;
			}
			set
			{
				this.wasPlaybackStopped = value;
			}
		}

		public string BookmarkReached
		{
			get
			{
				return this.bookmark;
			}
			set
			{
				this.bookmark = value;
			}
		}

		internal IUMRecognitionResult Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		private IUMRecognitionResult result;

		private TimeSpan elapsedTime;

		private int lastPrompt;

		private bool wasPlaybackStopped;

		private string bookmark;
	}
}

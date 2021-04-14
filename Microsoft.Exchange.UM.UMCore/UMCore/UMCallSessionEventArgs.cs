using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMCallSessionEventArgs : EventArgs, IPlaybackEventArgs
	{
		internal UMCallSessionEventArgs()
		{
			this.Reset();
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

		internal Exception Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		internal bool WasPlaybackStopped
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

		internal IPEndPoint PimgEndpoint
		{
			get
			{
				return this.pimgEndpoint;
			}
			set
			{
				this.pimgEndpoint = value;
			}
		}

		internal Socket UMEndSocket
		{
			get
			{
				return this.umEndSocket;
			}
			set
			{
				this.umEndSocket = value;
			}
		}

		internal byte[] DtmfDigits
		{
			get
			{
				return this.dtmfDigits;
			}
			set
			{
				Array.Clear(this.dtmfDigits, 0, this.dtmfDigits.Length);
				this.dtmfDigits = (value ?? new byte[0]);
			}
		}

		internal TimeSpan RecordTime
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

		internal bool SendDtmfCompleted
		{
			get
			{
				return this.sendDtmfCompleted;
			}
			set
			{
				this.sendDtmfCompleted = value;
			}
		}

		internal TimeSpan TotalRecordTime
		{
			get
			{
				return this.totalRecordTime;
			}
			set
			{
				this.totalRecordTime = value;
			}
		}

		internal void Reset(int maxDigits)
		{
			if (this.umEndSocket != null)
			{
				this.umEndSocket.Close();
				this.umEndSocket = null;
			}
			this.pimgEndpoint = null;
			this.DtmfDigits = new byte[maxDigits];
			this.elapsedTime = TimeSpan.Zero;
			this.totalRecordTime = TimeSpan.Zero;
			this.lastPrompt = 0;
			this.error = null;
			this.sendDtmfCompleted = false;
			this.wasPlaybackStopped = false;
		}

		internal void Reset()
		{
			this.Reset(0);
		}

		private Socket umEndSocket;

		private IPEndPoint pimgEndpoint;

		private byte[] dtmfDigits = new byte[0];

		private TimeSpan elapsedTime;

		private TimeSpan totalRecordTime;

		private int lastPrompt;

		private Exception error;

		private bool sendDtmfCompleted;

		private bool wasPlaybackStopped;
	}
}

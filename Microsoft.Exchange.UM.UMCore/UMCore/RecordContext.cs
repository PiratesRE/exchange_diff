using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecordContext
	{
		internal RecordContext()
		{
			this.Reset();
		}

		internal int TotalSeconds
		{
			get
			{
				return this.totalSeconds;
			}
			set
			{
				this.totalSeconds = value;
			}
		}

		internal int TotalFailures
		{
			get
			{
				return this.totalFailures;
			}
			set
			{
				this.totalFailures = value;
			}
		}

		internal bool Append
		{
			get
			{
				return this.append;
			}
			set
			{
				this.append = value;
			}
		}

		internal ITempWavFile Recording
		{
			get
			{
				return this.recording;
			}
			set
			{
				this.recording = value;
			}
		}

		internal string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal void Reset()
		{
			this.totalSeconds = 0;
			this.totalFailures = 0;
			this.append = false;
			this.recording = null;
			this.id = string.Empty;
		}

		private int totalSeconds;

		private int totalFailures;

		private bool append;

		private ITempWavFile recording;

		private string id;
	}
}

using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TranscriptionContext
	{
		internal CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		internal bool ShouldRunTranscriptionStage
		{
			get
			{
				return this.transcriptionEnabled;
			}
			set
			{
				this.transcriptionEnabled = value;
			}
		}

		internal TimeSpan BacklogContribution
		{
			get
			{
				return this.backlogContribution;
			}
			set
			{
				this.backlogContribution = value;
			}
		}

		internal TopNData TopN
		{
			get
			{
				return this.topN;
			}
			set
			{
				this.topN = value;
			}
		}

		private CultureInfo culture;

		private bool transcriptionEnabled;

		private TimeSpan backlogContribution = TimeSpan.Zero;

		private TopNData topN;
	}
}

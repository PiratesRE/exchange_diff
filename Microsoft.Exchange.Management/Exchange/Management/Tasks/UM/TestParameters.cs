using System;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal struct TestParameters
	{
		internal TestParameters(string uri, string pin, string phone, string dpname, bool momTest)
		{
			this.remoteUri = uri;
			this.pin = pin;
			this.phone = phone;
			this.dpname = dpname;
			this.isMOMTest = momTest;
			this.diagInitialSilenceInMilisecs = 0;
			this.diagDtmfSequence = string.Empty;
			this.diagInterDtmfGapInMilisecs = 0;
			this.diagDtmfDurationInMilisecs = 0;
			this.diagInterDtmfGapDiffInMilisecs = string.Empty;
		}

		internal string RemoteUri
		{
			get
			{
				return this.remoteUri;
			}
		}

		internal bool IsMOMTest
		{
			get
			{
				return this.isMOMTest;
			}
		}

		internal string PIN
		{
			get
			{
				return this.pin;
			}
			set
			{
				this.pin = value;
			}
		}

		internal string Phone
		{
			get
			{
				return this.phone;
			}
			set
			{
				this.phone = value;
			}
		}

		internal string DpName
		{
			get
			{
				return this.dpname;
			}
			set
			{
				this.dpname = value;
			}
		}

		internal string DiagDtmfSequence
		{
			get
			{
				return this.diagDtmfSequence;
			}
			set
			{
				this.diagDtmfSequence = value;
			}
		}

		internal int DiagInitialSilenceInMilisecs
		{
			get
			{
				return this.diagInitialSilenceInMilisecs;
			}
			set
			{
				this.diagInitialSilenceInMilisecs = value;
			}
		}

		internal int DiagInterDtmfGapInMilisecs
		{
			get
			{
				return this.diagInterDtmfGapInMilisecs;
			}
			set
			{
				this.diagInterDtmfGapInMilisecs = value;
			}
		}

		internal int DiagDtmfDurationInMilisecs
		{
			get
			{
				return this.diagDtmfDurationInMilisecs;
			}
			set
			{
				this.diagDtmfDurationInMilisecs = value;
			}
		}

		internal string DiagInterDtmfGapDiffInMilisecs
		{
			get
			{
				return this.diagInterDtmfGapDiffInMilisecs;
			}
			set
			{
				this.diagInterDtmfGapDiffInMilisecs = value;
			}
		}

		private string remoteUri;

		private string pin;

		private string phone;

		private string dpname;

		private bool isMOMTest;

		private int diagInitialSilenceInMilisecs;

		private string diagDtmfSequence;

		private int diagInterDtmfGapInMilisecs;

		private int diagDtmfDurationInMilisecs;

		private string diagInterDtmfGapDiffInMilisecs;
	}
}

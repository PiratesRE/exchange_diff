using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct StorageIcsState
	{
		public StorageIcsState(byte[] stateIdsetGiven, byte[] stateCnsetSeen, byte[] stateCnsetSeenFAI, byte[] stateCnsetRead)
		{
			this.stateIdsetGiven = stateIdsetGiven;
			this.stateCnsetSeen = stateCnsetSeen;
			this.stateCnsetSeenFAI = stateCnsetSeenFAI;
			this.stateCnsetRead = stateCnsetRead;
		}

		public byte[] StateIdsetGiven
		{
			get
			{
				if (this.stateIdsetGiven == null)
				{
					return StorageIcsState.stateEmpty;
				}
				return this.stateIdsetGiven;
			}
			set
			{
				this.stateIdsetGiven = value;
			}
		}

		public byte[] StateCnsetSeen
		{
			get
			{
				if (this.stateCnsetSeen == null)
				{
					return StorageIcsState.stateEmpty;
				}
				return this.stateCnsetSeen;
			}
			set
			{
				this.stateCnsetSeen = value;
			}
		}

		public byte[] StateCnsetSeenFAI
		{
			get
			{
				if (this.stateCnsetSeenFAI == null)
				{
					return StorageIcsState.stateEmpty;
				}
				return this.stateCnsetSeenFAI;
			}
			set
			{
				this.stateCnsetSeenFAI = value;
			}
		}

		public byte[] StateCnsetRead
		{
			get
			{
				if (this.stateCnsetRead == null)
				{
					return StorageIcsState.stateEmpty;
				}
				return this.stateCnsetRead;
			}
			set
			{
				this.stateCnsetRead = value;
			}
		}

		private static readonly byte[] stateEmpty = Array<byte>.Empty;

		private byte[] stateIdsetGiven;

		private byte[] stateCnsetSeen;

		private byte[] stateCnsetSeenFAI;

		private byte[] stateCnsetRead;
	}
}

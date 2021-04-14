using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LogCopyStatus
	{
		internal LogCopyStatus(CopyType copyType, string nodeName, bool isCrossSite, ulong logGeneration, ulong inspectedLogGeneration, ulong replayedLogGeneration)
		{
			this.timeReceivedUTC = DateTime.UtcNow;
			this.copyType = copyType;
			this.nodeName = nodeName;
			this.isCrossSite = isCrossSite;
			this.logGeneration = logGeneration;
			this.inspectedLogGeneration = inspectedLogGeneration;
			this.replayedLogGeneration = replayedLogGeneration;
		}

		internal DateTime TimeReceivedUTC
		{
			get
			{
				return this.timeReceivedUTC;
			}
		}

		internal CopyType CopyType
		{
			get
			{
				return this.copyType;
			}
		}

		internal string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		internal bool IsCrossSite
		{
			get
			{
				return this.isCrossSite;
			}
		}

		internal ulong LogGeneration
		{
			get
			{
				return this.logGeneration;
			}
		}

		internal ulong InspectedLogGeneration
		{
			get
			{
				return this.inspectedLogGeneration;
			}
		}

		internal ulong ReplayedLogGeneration
		{
			get
			{
				return this.replayedLogGeneration;
			}
		}

		private readonly DateTime timeReceivedUTC;

		private readonly string nodeName;

		private readonly bool isCrossSite;

		private readonly CopyType copyType;

		private readonly ulong logGeneration;

		private readonly ulong inspectedLogGeneration;

		private readonly ulong replayedLogGeneration;
	}
}

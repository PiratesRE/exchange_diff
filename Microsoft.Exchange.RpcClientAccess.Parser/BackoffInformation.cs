using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class BackoffInformation
	{
		public BackoffInformation(byte logonId, uint duration, BackoffRopData[] backoffRopData, byte[] additionalData)
		{
			this.logonId = logonId;
			this.duration = duration;
			this.backoffRopData = backoffRopData;
			this.additionalData = additionalData;
		}

		internal byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		internal uint Duration
		{
			get
			{
				return this.duration;
			}
		}

		internal BackoffRopData[] BackoffRopData
		{
			get
			{
				return this.backoffRopData;
			}
		}

		internal byte[] AdditionalData
		{
			get
			{
				return this.additionalData;
			}
		}

		private readonly byte logonId;

		private readonly uint duration;

		private readonly BackoffRopData[] backoffRopData;

		private readonly byte[] additionalData;
	}
}

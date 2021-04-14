﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeRpcFailedSeedingException : TaskServerException
	{
		public ReplayServiceResumeRpcFailedSeedingException() : base(ReplayStrings.ReplayServiceResumeRpcFailedSeedingException)
		{
		}

		public ReplayServiceResumeRpcFailedSeedingException(Exception innerException) : base(ReplayStrings.ReplayServiceResumeRpcFailedSeedingException, innerException)
		{
		}

		protected ReplayServiceResumeRpcFailedSeedingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TPRProviderNotListeningException : TransientException
	{
		public TPRProviderNotListeningException() : base(ReplayStrings.TPRProviderNotListening)
		{
		}

		public TPRProviderNotListeningException(Exception innerException) : base(ReplayStrings.TPRProviderNotListening, innerException)
		{
		}

		protected TPRProviderNotListeningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoteClusterWin2k8ToWin2k3NotSupportedException : ClusCommonFailException
	{
		public RemoteClusterWin2k8ToWin2k3NotSupportedException() : base(Strings.RemoteClusterWin2k8ToWin2k3NotSupportedException)
		{
		}

		public RemoteClusterWin2k8ToWin2k3NotSupportedException(Exception innerException) : base(Strings.RemoteClusterWin2k8ToWin2k3NotSupportedException, innerException)
		{
		}

		protected RemoteClusterWin2k8ToWin2k3NotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoteClusterWin2k3ToWin2k8NotSupportedException : ClusCommonFailException
	{
		public RemoteClusterWin2k3ToWin2k8NotSupportedException() : base(Strings.RemoteClusterWin2k3ToWin2k8NotSupportedException)
		{
		}

		public RemoteClusterWin2k3ToWin2k8NotSupportedException(Exception innerException) : base(Strings.RemoteClusterWin2k3ToWin2k8NotSupportedException, innerException)
		{
		}

		protected RemoteClusterWin2k3ToWin2k8NotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

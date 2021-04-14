using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StartAfterOrCompleteAfterCannotBeSetForOfflineMovesException : RecipientTaskException
	{
		public StartAfterOrCompleteAfterCannotBeSetForOfflineMovesException() : base(Strings.StartAfterOrCompleteAfterCannotBeSetForOfflineMoves)
		{
		}

		public StartAfterOrCompleteAfterCannotBeSetForOfflineMovesException(Exception innerException) : base(Strings.StartAfterOrCompleteAfterCannotBeSetForOfflineMoves, innerException)
		{
		}

		protected StartAfterOrCompleteAfterCannotBeSetForOfflineMovesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

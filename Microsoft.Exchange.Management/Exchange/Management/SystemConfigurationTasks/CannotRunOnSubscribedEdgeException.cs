using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRunOnSubscribedEdgeException : LocalizedException
	{
		public CannotRunOnSubscribedEdgeException() : base(Strings.CannotRunOnSubscribedEdge)
		{
		}

		public CannotRunOnSubscribedEdgeException(Exception innerException) : base(Strings.CannotRunOnSubscribedEdge, innerException)
		{
		}

		protected CannotRunOnSubscribedEdgeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

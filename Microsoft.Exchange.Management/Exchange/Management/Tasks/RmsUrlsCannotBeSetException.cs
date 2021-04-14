using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsUrlsCannotBeSetException : LocalizedException
	{
		public RmsUrlsCannotBeSetException() : base(Strings.RmsUrlsCannotBeSet)
		{
		}

		public RmsUrlsCannotBeSetException(Exception innerException) : base(Strings.RmsUrlsCannotBeSet, innerException)
		{
		}

		protected RmsUrlsCannotBeSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

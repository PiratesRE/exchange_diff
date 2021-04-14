using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmMoveNotApplicableForDbException : AmServerException
	{
		public AmMoveNotApplicableForDbException() : base(ServerStrings.AmMoveNotApplicableForDbException)
		{
		}

		public AmMoveNotApplicableForDbException(Exception innerException) : base(ServerStrings.AmMoveNotApplicableForDbException, innerException)
		{
		}

		protected AmMoveNotApplicableForDbException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

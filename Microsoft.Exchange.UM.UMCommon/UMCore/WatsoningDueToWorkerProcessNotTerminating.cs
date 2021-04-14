using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class WatsoningDueToWorkerProcessNotTerminating : LocalizedException
	{
		public WatsoningDueToWorkerProcessNotTerminating() : base(Strings.WatsoningDueToWorkerProcessNotTerminating)
		{
		}

		public WatsoningDueToWorkerProcessNotTerminating(Exception innerException) : base(Strings.WatsoningDueToWorkerProcessNotTerminating, innerException)
		{
		}

		protected WatsoningDueToWorkerProcessNotTerminating(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

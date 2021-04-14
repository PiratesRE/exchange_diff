using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ActiveMonitoringOperationFailedWithEcException : ActiveMonitoringServerException
	{
		public ActiveMonitoringOperationFailedWithEcException(int ec) : base(ServerStrings.ActiveMonitoringOperationFailedWithEcException(ec))
		{
			this.ec = ec;
		}

		public ActiveMonitoringOperationFailedWithEcException(int ec, Exception innerException) : base(ServerStrings.ActiveMonitoringOperationFailedWithEcException(ec), innerException)
		{
			this.ec = ec;
		}

		protected ActiveMonitoringOperationFailedWithEcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ec = (int)info.GetValue("ec", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ec", this.ec);
		}

		public int Ec
		{
			get
			{
				return this.ec;
			}
		}

		private readonly int ec;
	}
}

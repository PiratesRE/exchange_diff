using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MobileRecoRPCShutdownException : MobileRecoRequestCannotBeHandledException
	{
		public MobileRecoRPCShutdownException(Guid id) : base(Strings.MobileRecoRPCShutdownException(id))
		{
			this.id = id;
		}

		public MobileRecoRPCShutdownException(Guid id, Exception innerException) : base(Strings.MobileRecoRPCShutdownException(id), innerException)
		{
			this.id = id;
		}

		protected MobileRecoRPCShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (Guid)info.GetValue("id", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly Guid id;
	}
}

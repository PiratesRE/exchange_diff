using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidRecoRequestIdException : MobileRecoInvalidRequestException
	{
		public InvalidRecoRequestIdException(Guid id) : base(Strings.InvalidRecoRequestId(id))
		{
			this.id = id;
		}

		public InvalidRecoRequestIdException(Guid id, Exception innerException) : base(Strings.InvalidRecoRequestId(id), innerException)
		{
			this.id = id;
		}

		protected InvalidRecoRequestIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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

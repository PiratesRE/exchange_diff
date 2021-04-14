using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EmptyRecoRequestIdException : MobileRecoInvalidRequestException
	{
		public EmptyRecoRequestIdException(Guid id) : base(Strings.EmptyRecoRequestId(id))
		{
			this.id = id;
		}

		public EmptyRecoRequestIdException(Guid id, Exception innerException) : base(Strings.EmptyRecoRequestId(id), innerException)
		{
			this.id = id;
		}

		protected EmptyRecoRequestIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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

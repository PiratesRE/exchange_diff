using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UserNotFoundException : LocalizedException
	{
		public UserNotFoundException(Guid id) : base(Strings.UserNotFoundException(id))
		{
			this.id = id;
		}

		public UserNotFoundException(Guid id, Exception innerException) : base(Strings.UserNotFoundException(id), innerException)
		{
			this.id = id;
		}

		protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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

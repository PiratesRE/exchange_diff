using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AdUserNotFoundException : ObjectNotFoundException
	{
		public AdUserNotFoundException(string errMessage) : base(ServerStrings.AdUserNotFoundException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public AdUserNotFoundException(string errMessage, Exception innerException) : base(ServerStrings.AdUserNotFoundException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected AdUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMessage", this.errMessage);
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		private readonly string errMessage;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoSupportException : StoragePermanentException
	{
		public NoSupportException(LocalizedString exceptionMessage) : base(ServerStrings.NoSupportException(exceptionMessage))
		{
			this.exceptionMessage = exceptionMessage;
		}

		public NoSupportException(LocalizedString exceptionMessage, Exception innerException) : base(ServerStrings.NoSupportException(exceptionMessage), innerException)
		{
			this.exceptionMessage = exceptionMessage;
		}

		protected NoSupportException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exceptionMessage = (LocalizedString)info.GetValue("exceptionMessage", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public LocalizedString ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly LocalizedString exceptionMessage;
	}
}

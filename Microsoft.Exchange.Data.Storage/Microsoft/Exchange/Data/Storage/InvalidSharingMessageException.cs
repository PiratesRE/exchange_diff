using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSharingMessageException : StoragePermanentException
	{
		public InvalidSharingMessageException(string property) : base(ServerStrings.InvalidSharingMessageException(property))
		{
			this.property = property;
		}

		public InvalidSharingMessageException(string property, Exception innerException) : base(ServerStrings.InvalidSharingMessageException(property), innerException)
		{
			this.property = property;
		}

		protected InvalidSharingMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		private readonly string property;
	}
}

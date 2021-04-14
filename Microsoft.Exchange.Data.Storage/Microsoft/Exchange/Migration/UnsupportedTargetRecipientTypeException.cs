using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnsupportedTargetRecipientTypeException : MigrationPermanentException
	{
		public UnsupportedTargetRecipientTypeException(string type) : base(Strings.UnsupportedTargetRecipientTypeError(type))
		{
			this.type = type;
		}

		public UnsupportedTargetRecipientTypeException(string type, Exception innerException) : base(Strings.UnsupportedTargetRecipientTypeError(type), innerException)
		{
			this.type = type;
		}

		protected UnsupportedTargetRecipientTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string type;
	}
}

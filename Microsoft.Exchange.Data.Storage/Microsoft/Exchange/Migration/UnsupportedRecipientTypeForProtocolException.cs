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
	internal class UnsupportedRecipientTypeForProtocolException : MigrationPermanentException
	{
		public UnsupportedRecipientTypeForProtocolException(string type, string protocol) : base(Strings.ErrorUnsupportedRecipientTypeForProtocol(type, protocol))
		{
			this.type = type;
			this.protocol = protocol;
		}

		public UnsupportedRecipientTypeForProtocolException(string type, string protocol, Exception innerException) : base(Strings.ErrorUnsupportedRecipientTypeForProtocol(type, protocol), innerException)
		{
			this.type = type;
			this.protocol = protocol;
		}

		protected UnsupportedRecipientTypeForProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (string)info.GetValue("type", typeof(string));
			this.protocol = (string)info.GetValue("protocol", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
			info.AddValue("protocol", this.protocol);
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		private readonly string type;

		private readonly string protocol;
	}
}

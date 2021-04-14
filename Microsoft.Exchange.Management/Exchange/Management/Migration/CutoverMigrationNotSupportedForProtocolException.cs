using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CutoverMigrationNotSupportedForProtocolException : LocalizedException
	{
		public CutoverMigrationNotSupportedForProtocolException(string protocol) : base(Strings.CutoverMigrationNotSupportedForProtocol(protocol))
		{
			this.protocol = protocol;
		}

		public CutoverMigrationNotSupportedForProtocolException(string protocol, Exception innerException) : base(Strings.CutoverMigrationNotSupportedForProtocol(protocol), innerException)
		{
			this.protocol = protocol;
		}

		protected CutoverMigrationNotSupportedForProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.protocol = (string)info.GetValue("protocol", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("protocol", this.protocol);
		}

		public string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		private readonly string protocol;
	}
}

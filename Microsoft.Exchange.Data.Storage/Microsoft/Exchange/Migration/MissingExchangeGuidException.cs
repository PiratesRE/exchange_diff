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
	internal class MissingExchangeGuidException : MigrationPermanentException
	{
		public MissingExchangeGuidException(string identity) : base(Strings.ErrorMissingExchangeGuid(identity))
		{
			this.identity = identity;
		}

		public MissingExchangeGuidException(string identity, Exception innerException) : base(Strings.ErrorMissingExchangeGuid(identity), innerException)
		{
			this.identity = identity;
		}

		protected MissingExchangeGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}

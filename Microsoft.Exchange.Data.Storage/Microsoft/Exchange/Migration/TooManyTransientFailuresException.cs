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
	internal class TooManyTransientFailuresException : MigrationPermanentException
	{
		public TooManyTransientFailuresException(string batchIdentity) : base(Strings.ErrorTooManyTransientFailures(batchIdentity))
		{
			this.batchIdentity = batchIdentity;
		}

		public TooManyTransientFailuresException(string batchIdentity, Exception innerException) : base(Strings.ErrorTooManyTransientFailures(batchIdentity), innerException)
		{
			this.batchIdentity = batchIdentity;
		}

		protected TooManyTransientFailuresException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.batchIdentity = (string)info.GetValue("batchIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("batchIdentity", this.batchIdentity);
		}

		public string BatchIdentity
		{
			get
			{
				return this.batchIdentity;
			}
		}

		private readonly string batchIdentity;
	}
}

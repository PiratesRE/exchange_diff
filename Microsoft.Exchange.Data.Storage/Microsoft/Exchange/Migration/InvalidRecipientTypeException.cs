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
	internal class InvalidRecipientTypeException : MigrationPermanentException
	{
		public InvalidRecipientTypeException(string actual, string expected) : base(Strings.ErrorInvalidRecipientType(actual, expected))
		{
			this.actual = actual;
			this.expected = expected;
		}

		public InvalidRecipientTypeException(string actual, string expected, Exception innerException) : base(Strings.ErrorInvalidRecipientType(actual, expected), innerException)
		{
			this.actual = actual;
			this.expected = expected;
		}

		protected InvalidRecipientTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actual = (string)info.GetValue("actual", typeof(string));
			this.expected = (string)info.GetValue("expected", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actual", this.actual);
			info.AddValue("expected", this.expected);
		}

		public string Actual
		{
			get
			{
				return this.actual;
			}
		}

		public string Expected
		{
			get
			{
				return this.expected;
			}
		}

		private readonly string actual;

		private readonly string expected;
	}
}

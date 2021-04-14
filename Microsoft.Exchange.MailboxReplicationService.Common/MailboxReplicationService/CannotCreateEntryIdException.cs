using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotCreateEntryIdException : MailboxReplicationPermanentException
	{
		public CannotCreateEntryIdException(string input) : base(MrsStrings.CannotCreateEntryId(input))
		{
			this.input = input;
		}

		public CannotCreateEntryIdException(string input, Exception innerException) : base(MrsStrings.CannotCreateEntryId(input), innerException)
		{
			this.input = input;
		}

		protected CannotCreateEntryIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.input = (string)info.GetValue("input", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("input", this.input);
		}

		public string Input
		{
			get
			{
				return this.input;
			}
		}

		private readonly string input;
	}
}

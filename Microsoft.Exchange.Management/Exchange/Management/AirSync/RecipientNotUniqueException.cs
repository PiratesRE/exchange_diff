using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientNotUniqueException : LocalizedException
	{
		public RecipientNotUniqueException(string recipient) : base(Strings.RecipientNotUniqueException(recipient))
		{
			this.recipient = recipient;
		}

		public RecipientNotUniqueException(string recipient, Exception innerException) : base(Strings.RecipientNotUniqueException(recipient), innerException)
		{
			this.recipient = recipient;
		}

		protected RecipientNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		private readonly string recipient;
	}
}

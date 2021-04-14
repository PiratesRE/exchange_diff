using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoRecipientsForRecipientIdException : LocalizedException
	{
		public NoRecipientsForRecipientIdException(string recipId) : base(Strings.NoRecipientsForRecipientId(recipId))
		{
			this.recipId = recipId;
		}

		public NoRecipientsForRecipientIdException(string recipId, Exception innerException) : base(Strings.NoRecipientsForRecipientId(recipId), innerException)
		{
			this.recipId = recipId;
		}

		protected NoRecipientsForRecipientIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipId = (string)info.GetValue("recipId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipId", this.recipId);
		}

		public string RecipId
		{
			get
			{
				return this.recipId;
			}
		}

		private readonly string recipId;
	}
}

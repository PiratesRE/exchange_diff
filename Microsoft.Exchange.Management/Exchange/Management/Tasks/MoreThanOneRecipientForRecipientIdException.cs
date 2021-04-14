using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoreThanOneRecipientForRecipientIdException : LocalizedException
	{
		public MoreThanOneRecipientForRecipientIdException(string recipId) : base(Strings.MoreThanOneRecipientForRecipientId(recipId))
		{
			this.recipId = recipId;
		}

		public MoreThanOneRecipientForRecipientIdException(string recipId, Exception innerException) : base(Strings.MoreThanOneRecipientForRecipientId(recipId), innerException)
		{
			this.recipId = recipId;
		}

		protected MoreThanOneRecipientForRecipientIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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

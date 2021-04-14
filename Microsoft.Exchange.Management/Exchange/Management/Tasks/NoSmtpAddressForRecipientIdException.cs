using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoSmtpAddressForRecipientIdException : LocalizedException
	{
		public NoSmtpAddressForRecipientIdException(string recipId) : base(Strings.NoSmtpAddressForRecipientId(recipId))
		{
			this.recipId = recipId;
		}

		public NoSmtpAddressForRecipientIdException(string recipId, Exception innerException) : base(Strings.NoSmtpAddressForRecipientId(recipId), innerException)
		{
			this.recipId = recipId;
		}

		protected NoSmtpAddressForRecipientIdException(SerializationInfo info, StreamingContext context) : base(info, context)
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

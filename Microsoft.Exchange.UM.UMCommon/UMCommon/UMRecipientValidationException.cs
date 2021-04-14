using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMRecipientValidationException : LocalizedException
	{
		public UMRecipientValidationException(string recipient, string fieldName) : base(Strings.UMRecipientValidation(recipient, fieldName))
		{
			this.recipient = recipient;
			this.fieldName = fieldName;
		}

		public UMRecipientValidationException(string recipient, string fieldName, Exception innerException) : base(Strings.UMRecipientValidation(recipient, fieldName), innerException)
		{
			this.recipient = recipient;
			this.fieldName = fieldName;
		}

		protected UMRecipientValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
			this.fieldName = (string)info.GetValue("fieldName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
			info.AddValue("fieldName", this.fieldName);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
		}

		private readonly string recipient;

		private readonly string fieldName;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMInvalidPartnerMessageException : LocalizedException
	{
		public UMInvalidPartnerMessageException(string fieldName) : base(Strings.UMInvalidPartnerMessageException(fieldName))
		{
			this.fieldName = fieldName;
		}

		public UMInvalidPartnerMessageException(string fieldName, Exception innerException) : base(Strings.UMInvalidPartnerMessageException(fieldName), innerException)
		{
			this.fieldName = fieldName;
		}

		protected UMInvalidPartnerMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fieldName = (string)info.GetValue("fieldName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fieldName", this.fieldName);
		}

		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
		}

		private readonly string fieldName;
	}
}

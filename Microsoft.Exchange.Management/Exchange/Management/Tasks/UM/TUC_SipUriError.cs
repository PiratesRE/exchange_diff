using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_SipUriError : LocalizedException
	{
		public TUC_SipUriError(string field) : base(Strings.SipUriError(field))
		{
			this.field = field;
		}

		public TUC_SipUriError(string field, Exception innerException) : base(Strings.SipUriError(field), innerException)
		{
			this.field = field;
		}

		protected TUC_SipUriError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.field = (string)info.GetValue("field", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("field", this.field);
		}

		public string Field
		{
			get
			{
				return this.field;
			}
		}

		private readonly string field;
	}
}

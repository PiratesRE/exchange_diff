using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnexpectedFilterTypePermanentException : ContentFilterPermanentException
	{
		public UnexpectedFilterTypePermanentException(string filterTypeName) : base(MrsStrings.UnexpectedFilterType(filterTypeName))
		{
			this.filterTypeName = filterTypeName;
		}

		public UnexpectedFilterTypePermanentException(string filterTypeName, Exception innerException) : base(MrsStrings.UnexpectedFilterType(filterTypeName), innerException)
		{
			this.filterTypeName = filterTypeName;
		}

		protected UnexpectedFilterTypePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filterTypeName = (string)info.GetValue("filterTypeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filterTypeName", this.filterTypeName);
		}

		public string FilterTypeName
		{
			get
			{
				return this.filterTypeName;
			}
		}

		private readonly string filterTypeName;
	}
}

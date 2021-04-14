using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FilterOperatorMustBeEQorNEPermanentException : ContentFilterPermanentException
	{
		public FilterOperatorMustBeEQorNEPermanentException(string propertyName) : base(MrsStrings.FilterOperatorMustBeEQorNE(propertyName))
		{
			this.propertyName = propertyName;
		}

		public FilterOperatorMustBeEQorNEPermanentException(string propertyName, Exception innerException) : base(MrsStrings.FilterOperatorMustBeEQorNE(propertyName), innerException)
		{
			this.propertyName = propertyName;
		}

		protected FilterOperatorMustBeEQorNEPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string propertyName;
	}
}

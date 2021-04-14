using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncPropertyValidationException : SyncPermanentException
	{
		public SyncPropertyValidationException(string property, string value) : base(Strings.SyncPropertyValidationException(property, value))
		{
			this.property = property;
			this.value = value;
		}

		public SyncPropertyValidationException(string property, string value, Exception innerException) : base(Strings.SyncPropertyValidationException(property, value), innerException)
		{
			this.property = property;
			this.value = value;
		}

		protected SyncPropertyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("value", this.value);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string property;

		private readonly string value;
	}
}

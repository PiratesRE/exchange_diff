using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PropertyNotFoundException : LocalizedException
	{
		public PropertyNotFoundException(string propertyName) : base(Strings.PropertyNotFoundExceptionMessage(propertyName))
		{
			this.propertyName = propertyName;
		}

		public PropertyNotFoundException(string propertyName, Exception innerException) : base(Strings.PropertyNotFoundExceptionMessage(propertyName), innerException)
		{
			this.propertyName = propertyName;
		}

		protected PropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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

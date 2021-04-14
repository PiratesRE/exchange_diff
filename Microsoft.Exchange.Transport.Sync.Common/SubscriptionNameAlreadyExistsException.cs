using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionNameAlreadyExistsException : LocalizedException
	{
		public SubscriptionNameAlreadyExistsException(string name) : base(Strings.SubscriptionNameAlreadyExists(name))
		{
			this.name = name;
		}

		public SubscriptionNameAlreadyExistsException(string name, Exception innerException) : base(Strings.SubscriptionNameAlreadyExists(name), innerException)
		{
			this.name = name;
		}

		protected SubscriptionNameAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}

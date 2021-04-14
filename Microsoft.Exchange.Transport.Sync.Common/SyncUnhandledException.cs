using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncUnhandledException : LocalizedException
	{
		public SyncUnhandledException(Type type) : base(Strings.SyncUnhandledException(type))
		{
			this.type = type;
		}

		public SyncUnhandledException(Type type, Exception innerException) : base(Strings.SyncUnhandledException(type), innerException)
		{
			this.type = type;
		}

		protected SyncUnhandledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (Type)info.GetValue("type", typeof(Type));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly Type type;
	}
}

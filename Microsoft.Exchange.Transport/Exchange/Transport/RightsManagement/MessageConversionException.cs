using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	[Serializable]
	internal class MessageConversionException : LocalizedException
	{
		public MessageConversionException(LocalizedString message, bool isTransient) : base(message)
		{
			this.isTransient = isTransient;
		}

		public MessageConversionException(LocalizedString message, Exception innerException, bool isTransient) : base(message, innerException)
		{
			this.isTransient = isTransient;
		}

		protected MessageConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.isTransient = info.GetBoolean("IsTransient");
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("IsTransient", this.isTransient);
		}

		public bool IsTransient
		{
			get
			{
				return this.isTransient;
			}
		}

		private const string SerializationIsTransientAttributeName = "IsTransient";

		private bool isTransient;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedSwitchValueException : LocalizedException
	{
		public UnexpectedSwitchValueException(string enumValue) : base(Strings.UnexpectedSwitchValueException(enumValue))
		{
			this.enumValue = enumValue;
		}

		public UnexpectedSwitchValueException(string enumValue, Exception innerException) : base(Strings.UnexpectedSwitchValueException(enumValue), innerException)
		{
			this.enumValue = enumValue;
		}

		protected UnexpectedSwitchValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.enumValue = (string)info.GetValue("enumValue", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("enumValue", this.enumValue);
		}

		public string EnumValue
		{
			get
			{
				return this.enumValue;
			}
		}

		private readonly string enumValue;
	}
}

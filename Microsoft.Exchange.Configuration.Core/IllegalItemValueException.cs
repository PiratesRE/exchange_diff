using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Core.LocStrings;

namespace Microsoft.Exchange.Configuration.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IllegalItemValueException : WinRMDataExchangeException
	{
		public IllegalItemValueException(string value) : base(Strings.IllegalItemValue(value))
		{
			this.value = value;
		}

		public IllegalItemValueException(string value, Exception innerException) : base(Strings.IllegalItemValue(value), innerException)
		{
			this.value = value;
		}

		protected IllegalItemValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string value;
	}
}

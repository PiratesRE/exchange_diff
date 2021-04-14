using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NumericArgumentLengthInvalidException : LocalizedException
	{
		public NumericArgumentLengthInvalidException(string value, string argument, int maxSize) : base(Strings.ExceptionNumericArgumentLengthInvalid(value, argument, maxSize))
		{
			this.value = value;
			this.argument = argument;
			this.maxSize = maxSize;
		}

		public NumericArgumentLengthInvalidException(string value, string argument, int maxSize, Exception innerException) : base(Strings.ExceptionNumericArgumentLengthInvalid(value, argument, maxSize), innerException)
		{
			this.value = value;
			this.argument = argument;
			this.maxSize = maxSize;
		}

		protected NumericArgumentLengthInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			this.argument = (string)info.GetValue("argument", typeof(string));
			this.maxSize = (int)info.GetValue("maxSize", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("value", this.value);
			info.AddValue("argument", this.argument);
			info.AddValue("maxSize", this.maxSize);
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string Argument
		{
			get
			{
				return this.argument;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		private readonly string value;

		private readonly string argument;

		private readonly int maxSize;
	}
}

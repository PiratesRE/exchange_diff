using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisTasksValidationStringLengthOutOfRangeException : LocalizedException
	{
		public IisTasksValidationStringLengthOutOfRangeException(string propertyName, int minLength, int maxLength) : base(Strings.IisTasksValidationStringLengthOutOfRangeException(propertyName, minLength, maxLength))
		{
			this.propertyName = propertyName;
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		public IisTasksValidationStringLengthOutOfRangeException(string propertyName, int minLength, int maxLength, Exception innerException) : base(Strings.IisTasksValidationStringLengthOutOfRangeException(propertyName, minLength, maxLength), innerException)
		{
			this.propertyName = propertyName;
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		protected IisTasksValidationStringLengthOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
			this.minLength = (int)info.GetValue("minLength", typeof(int));
			this.maxLength = (int)info.GetValue("maxLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
			info.AddValue("minLength", this.minLength);
			info.AddValue("maxLength", this.maxLength);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public int MinLength
		{
			get
			{
				return this.minLength;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		private readonly string propertyName;

		private readonly int minLength;

		private readonly int maxLength;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExceededMaximumCollectionCountException : LocalizedException
	{
		public ExceededMaximumCollectionCountException(string propertyName, int maxLength, int actualLength) : base(Strings.ErrorExceededMaximumCollectionCount(propertyName, maxLength, actualLength))
		{
			this.propertyName = propertyName;
			this.maxLength = maxLength;
			this.actualLength = actualLength;
		}

		public ExceededMaximumCollectionCountException(string propertyName, int maxLength, int actualLength, Exception innerException) : base(Strings.ErrorExceededMaximumCollectionCount(propertyName, maxLength, actualLength), innerException)
		{
			this.propertyName = propertyName;
			this.maxLength = maxLength;
			this.actualLength = actualLength;
		}

		protected ExceededMaximumCollectionCountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
			this.maxLength = (int)info.GetValue("maxLength", typeof(int));
			this.actualLength = (int)info.GetValue("actualLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
			info.AddValue("maxLength", this.maxLength);
			info.AddValue("actualLength", this.actualLength);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public int ActualLength
		{
			get
			{
				return this.actualLength;
			}
		}

		private readonly string propertyName;

		private readonly int maxLength;

		private readonly int actualLength;
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class ReferenceException : LocalizedException
	{
		public string ReferenceValue { get; private set; }

		public ReferenceException(string referenceValue, LocalizedException innerException) : base(innerException.LocalizedString, innerException)
		{
			this.ReferenceValue = referenceValue;
		}

		public ReferenceException(string referenceValue, Exception innerException) : base(new LocalizedString(innerException.Message), innerException)
		{
			this.ReferenceValue = referenceValue;
		}

		public ReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ReferenceValue = info.GetString("ReferenceValue");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ReferenceValue", this.ReferenceValue);
		}
	}
}

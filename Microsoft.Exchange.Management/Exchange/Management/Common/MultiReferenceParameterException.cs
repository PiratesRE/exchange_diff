using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class MultiReferenceParameterException : LocalizedException
	{
		public ReferenceParameterException[] ReferenceParameterExceptions { get; private set; }

		public MultiReferenceParameterException(LocalizedString localizedString, ReferenceParameterException[] referenceParameterExceptions) : base(localizedString)
		{
			this.ReferenceParameterExceptions = referenceParameterExceptions;
		}

		public MultiReferenceParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ReferenceParameterExceptions = (ReferenceParameterException[])info.GetValue("ReferenceParameterExceptions", typeof(ReferenceParameterException[]));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ReferenceParameterExceptions", this.ReferenceParameterExceptions);
		}
	}
}

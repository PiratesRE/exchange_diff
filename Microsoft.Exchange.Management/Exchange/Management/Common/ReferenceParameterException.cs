using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class ReferenceParameterException : LocalizedException
	{
		public string Parameter { get; private set; }

		public ReferenceException[] ReferenceExceptions { get; private set; }

		public ReferenceParameterException(LocalizedString localizedString, string parameter, ReferenceException[] referenceExceptions) : base(localizedString)
		{
			this.Parameter = parameter;
			this.ReferenceExceptions = referenceExceptions;
		}

		public ReferenceParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Parameter = info.GetString("Parameter");
			this.ReferenceExceptions = (ReferenceException[])info.GetValue("ReferenceExceptions", typeof(ReferenceException[]));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Parameter", this.Parameter);
			info.AddValue("ReferenceExceptions", this.ReferenceExceptions);
		}
	}
}

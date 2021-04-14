using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public abstract class ReplicationCheckException : LocalizedException
	{
		public ReplicationCheckException(LocalizedString localizedString) : this(localizedString, null)
		{
		}

		public ReplicationCheckException(LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
			this.localizedString = localizedString;
		}

		protected ReplicationCheckException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}

		public new LocalizedString LocalizedString
		{
			get
			{
				return this.localizedString;
			}
		}

		private LocalizedString localizedString;
	}
}

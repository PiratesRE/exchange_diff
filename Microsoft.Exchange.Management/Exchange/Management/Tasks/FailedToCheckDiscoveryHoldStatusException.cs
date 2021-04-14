using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToCheckDiscoveryHoldStatusException : RecipientTaskException
	{
		public FailedToCheckDiscoveryHoldStatusException(LocalizedString error) : base(Strings.FailedToCheckDiscoveryHoldStatusException(error))
		{
			this.error = error;
		}

		public FailedToCheckDiscoveryHoldStatusException(LocalizedString error, Exception innerException) : base(Strings.FailedToCheckDiscoveryHoldStatusException(error), innerException)
		{
			this.error = error;
		}

		protected FailedToCheckDiscoveryHoldStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (LocalizedString)info.GetValue("error", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public LocalizedString Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly LocalizedString error;
	}
}

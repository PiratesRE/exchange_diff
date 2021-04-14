using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FatalAutodiscoverException : LocalizedException
	{
		public FatalAutodiscoverException(string failure) : base(Strings.messageFatalAutodiscoverException(failure))
		{
			this.failure = failure;
		}

		public FatalAutodiscoverException(string failure, Exception innerException) : base(Strings.messageFatalAutodiscoverException(failure), innerException)
		{
			this.failure = failure;
		}

		protected FatalAutodiscoverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failure", this.failure);
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string failure;
	}
}

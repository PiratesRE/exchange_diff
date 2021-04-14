using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToRemoveUserPhotoException : LocalizedException
	{
		public UnableToRemoveUserPhotoException(string identity, string failure) : base(Strings.UnableToRemoveUserPhotoException(identity, failure))
		{
			this.identity = identity;
			this.failure = failure;
		}

		public UnableToRemoveUserPhotoException(string identity, string failure, Exception innerException) : base(Strings.UnableToRemoveUserPhotoException(identity, failure), innerException)
		{
			this.identity = identity;
			this.failure = failure;
		}

		protected UnableToRemoveUserPhotoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("failure", this.failure);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string identity;

		private readonly string failure;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AssociatedMRSRequestExistsException : LocalizedException
	{
		public AssociatedMRSRequestExistsException(string requestType) : base(Strings.ErrorAssociatedMRSRequestExists(requestType))
		{
			this.requestType = requestType;
		}

		public AssociatedMRSRequestExistsException(string requestType, Exception innerException) : base(Strings.ErrorAssociatedMRSRequestExists(requestType), innerException)
		{
			this.requestType = requestType;
		}

		protected AssociatedMRSRequestExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestType = (string)info.GetValue("requestType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestType", this.requestType);
		}

		public string RequestType
		{
			get
			{
				return this.requestType;
			}
		}

		private readonly string requestType;
	}
}

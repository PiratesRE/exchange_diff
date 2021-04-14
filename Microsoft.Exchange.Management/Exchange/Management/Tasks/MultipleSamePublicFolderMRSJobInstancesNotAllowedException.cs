using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleSamePublicFolderMRSJobInstancesNotAllowedException : ManagementObjectAlreadyExistsException
	{
		public MultipleSamePublicFolderMRSJobInstancesNotAllowedException(string requestType) : base(Strings.ErrorSamePublicFolderMRSJobInstancesNotAllowed(requestType))
		{
			this.requestType = requestType;
		}

		public MultipleSamePublicFolderMRSJobInstancesNotAllowedException(string requestType, Exception innerException) : base(Strings.ErrorSamePublicFolderMRSJobInstancesNotAllowed(requestType), innerException)
		{
			this.requestType = requestType;
		}

		protected MultipleSamePublicFolderMRSJobInstancesNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
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

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IMSAdminHelperGetDataPathsCouldNotAllocateException : TaskTransientException
	{
		public IMSAdminHelperGetDataPathsCouldNotAllocateException() : base(Strings.IMSAdminHelperGetDataPathsCouldNotAllocateException)
		{
		}

		public IMSAdminHelperGetDataPathsCouldNotAllocateException(Exception innerException) : base(Strings.IMSAdminHelperGetDataPathsCouldNotAllocateException, innerException)
		{
		}

		protected IMSAdminHelperGetDataPathsCouldNotAllocateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

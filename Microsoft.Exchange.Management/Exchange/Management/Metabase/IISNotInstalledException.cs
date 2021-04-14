using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IISNotInstalledException : DataSourceOperationException
	{
		public IISNotInstalledException() : base(Strings.IISNotInstalledException)
		{
		}

		public IISNotInstalledException(Exception innerException) : base(Strings.IISNotInstalledException, innerException)
		{
		}

		protected IISNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

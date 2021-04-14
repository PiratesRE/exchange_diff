using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatacenterInvalidRegistryException : LocalizedException
	{
		public DatacenterInvalidRegistryException() : base(DiagnosticsResources.DatacenterInvalidRegistryException)
		{
		}

		public DatacenterInvalidRegistryException(Exception innerException) : base(DiagnosticsResources.DatacenterInvalidRegistryException, innerException)
		{
		}

		protected DatacenterInvalidRegistryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

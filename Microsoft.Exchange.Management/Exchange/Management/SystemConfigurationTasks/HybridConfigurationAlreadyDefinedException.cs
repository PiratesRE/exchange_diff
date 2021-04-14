using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HybridConfigurationAlreadyDefinedException : HybridConfigurationException
	{
		public HybridConfigurationAlreadyDefinedException() : base(HybridStrings.ErrorHybridConfigurationAlreadyDefined)
		{
		}

		public HybridConfigurationAlreadyDefinedException(Exception innerException) : base(HybridStrings.ErrorHybridConfigurationAlreadyDefined, innerException)
		{
		}

		protected HybridConfigurationAlreadyDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NonOperationalAdmissionControlException : LocalizedException
	{
		public NonOperationalAdmissionControlException(ResourceKey resource) : base(Strings.NonOperationalAdmissionControl(resource))
		{
			this.resource = resource;
		}

		public NonOperationalAdmissionControlException(ResourceKey resource, Exception innerException) : base(Strings.NonOperationalAdmissionControl(resource), innerException)
		{
			this.resource = resource;
		}

		protected NonOperationalAdmissionControlException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resource = (ResourceKey)info.GetValue("resource", typeof(ResourceKey));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resource", this.resource);
		}

		public ResourceKey Resource
		{
			get
			{
				return this.resource;
			}
		}

		private readonly ResourceKey resource;
	}
}

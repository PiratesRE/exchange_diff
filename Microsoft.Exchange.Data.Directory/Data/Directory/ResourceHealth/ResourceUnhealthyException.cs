using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	[Serializable]
	internal class ResourceUnhealthyException : TransientException
	{
		public ResourceUnhealthyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.resourceKey = (ResourceKey)info.GetValue("resource", typeof(ResourceKey));
			}
		}

		public ResourceUnhealthyException(ResourceKey resourceKey) : base(DirectoryStrings.ExceptionResourceUnhealthy(resourceKey))
		{
			this.resourceKey = resourceKey;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("resource", this.resourceKey);
			}
		}

		public ResourceKey ResourceKey
		{
			get
			{
				return this.resourceKey;
			}
		}

		private const string ResourceKeyField = "resource";

		private ResourceKey resourceKey;
	}
}

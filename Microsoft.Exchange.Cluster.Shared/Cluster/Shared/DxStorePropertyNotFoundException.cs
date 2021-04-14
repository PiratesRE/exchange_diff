using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStorePropertyNotFoundException : LocalizedException
	{
		public DxStorePropertyNotFoundException(string propertyName) : base(Strings.DxStorePropertyNotFoundException(propertyName))
		{
			this.propertyName = propertyName;
		}

		public DxStorePropertyNotFoundException(string propertyName, Exception innerException) : base(Strings.DxStorePropertyNotFoundException(propertyName), innerException)
		{
			this.propertyName = propertyName;
		}

		protected DxStorePropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string propertyName;
	}
}

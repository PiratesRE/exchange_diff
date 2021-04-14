using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceStaleStoreException : DxStoreInstanceServerException
	{
		public DxStoreInstanceStaleStoreException() : base(Strings.DxStoreInstanceStaleStore)
		{
		}

		public DxStoreInstanceStaleStoreException(Exception innerException) : base(Strings.DxStoreInstanceStaleStore, innerException)
		{
		}

		protected DxStoreInstanceStaleStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

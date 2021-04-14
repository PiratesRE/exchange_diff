using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreAccessClientException : DxStoreClientException
	{
		public DxStoreAccessClientException(string errMsg2) : base(Strings.DxStoreAccessClientException(errMsg2))
		{
			this.errMsg2 = errMsg2;
		}

		public DxStoreAccessClientException(string errMsg2, Exception innerException) : base(Strings.DxStoreAccessClientException(errMsg2), innerException)
		{
			this.errMsg2 = errMsg2;
		}

		protected DxStoreAccessClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg2 = (string)info.GetValue("errMsg2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg2", this.errMsg2);
		}

		public string ErrMsg2
		{
			get
			{
				return this.errMsg2;
			}
		}

		private readonly string errMsg2;
	}
}

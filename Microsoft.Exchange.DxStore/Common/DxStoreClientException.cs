using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreClientException : LocalizedException
	{
		public DxStoreClientException(string errMsg) : base(Strings.DxStoreClientException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public DxStoreClientException(string errMsg, Exception innerException) : base(Strings.DxStoreClientException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected DxStoreClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string errMsg;
	}
}

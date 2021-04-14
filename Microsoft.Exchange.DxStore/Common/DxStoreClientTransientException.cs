using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreClientTransientException : TransientException
	{
		public DxStoreClientTransientException(string errMsg) : base(Strings.DxStoreClientTransientException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public DxStoreClientTransientException(string errMsg, Exception innerException) : base(Strings.DxStoreClientTransientException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected DxStoreClientTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreServerTransientException : TransientException
	{
		public DxStoreServerTransientException(string errMsg) : base(Strings.DxStoreServerTransientException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public DxStoreServerTransientException(string errMsg, Exception innerException) : base(Strings.DxStoreServerTransientException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected DxStoreServerTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreServerException : LocalizedException
	{
		public DxStoreServerException(string errMsg) : base(Strings.DxStoreServerException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public DxStoreServerException(string errMsg, Exception innerException) : base(Strings.DxStoreServerException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected DxStoreServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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

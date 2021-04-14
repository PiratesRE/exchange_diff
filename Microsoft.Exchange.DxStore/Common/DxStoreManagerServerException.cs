using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreManagerServerException : DxStoreServerException
	{
		public DxStoreManagerServerException(string errMsg4) : base(Strings.DxStoreManagerServerException(errMsg4))
		{
			this.errMsg4 = errMsg4;
		}

		public DxStoreManagerServerException(string errMsg4, Exception innerException) : base(Strings.DxStoreManagerServerException(errMsg4), innerException)
		{
			this.errMsg4 = errMsg4;
		}

		protected DxStoreManagerServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg4 = (string)info.GetValue("errMsg4", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg4", this.errMsg4);
		}

		public string ErrMsg4
		{
			get
			{
				return this.errMsg4;
			}
		}

		private readonly string errMsg4;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceClientTransientException : DxStoreClientTransientException
	{
		public DxStoreInstanceClientTransientException(string errMsg3) : base(Strings.DxStoreInstanceClientTransientException(errMsg3))
		{
			this.errMsg3 = errMsg3;
		}

		public DxStoreInstanceClientTransientException(string errMsg3, Exception innerException) : base(Strings.DxStoreInstanceClientTransientException(errMsg3), innerException)
		{
			this.errMsg3 = errMsg3;
		}

		protected DxStoreInstanceClientTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg3 = (string)info.GetValue("errMsg3", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg3", this.errMsg3);
		}

		public string ErrMsg3
		{
			get
			{
				return this.errMsg3;
			}
		}

		private readonly string errMsg3;
	}
}

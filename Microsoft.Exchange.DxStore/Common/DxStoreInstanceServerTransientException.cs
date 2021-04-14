using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceServerTransientException : DxStoreServerTransientException
	{
		public DxStoreInstanceServerTransientException(string errMsg3) : base(Strings.DxStoreInstanceServerTransientException(errMsg3))
		{
			this.errMsg3 = errMsg3;
		}

		public DxStoreInstanceServerTransientException(string errMsg3, Exception innerException) : base(Strings.DxStoreInstanceServerTransientException(errMsg3), innerException)
		{
			this.errMsg3 = errMsg3;
		}

		protected DxStoreInstanceServerTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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

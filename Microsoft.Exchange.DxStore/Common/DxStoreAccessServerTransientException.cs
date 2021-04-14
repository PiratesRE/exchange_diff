using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreAccessServerTransientException : DxStoreServerTransientException
	{
		public DxStoreAccessServerTransientException(string errMsg1) : base(Strings.DxStoreAccessServerTransientException(errMsg1))
		{
			this.errMsg1 = errMsg1;
		}

		public DxStoreAccessServerTransientException(string errMsg1, Exception innerException) : base(Strings.DxStoreAccessServerTransientException(errMsg1), innerException)
		{
			this.errMsg1 = errMsg1;
		}

		protected DxStoreAccessServerTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg1 = (string)info.GetValue("errMsg1", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg1", this.errMsg1);
		}

		public string ErrMsg1
		{
			get
			{
				return this.errMsg1;
			}
		}

		private readonly string errMsg1;
	}
}

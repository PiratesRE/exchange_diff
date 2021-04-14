using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreManagerClientTransientException : DxStoreClientTransientException
	{
		public DxStoreManagerClientTransientException(string errMsg5) : base(Strings.DxStoreManagerClientTransientException(errMsg5))
		{
			this.errMsg5 = errMsg5;
		}

		public DxStoreManagerClientTransientException(string errMsg5, Exception innerException) : base(Strings.DxStoreManagerClientTransientException(errMsg5), innerException)
		{
			this.errMsg5 = errMsg5;
		}

		protected DxStoreManagerClientTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg5 = (string)info.GetValue("errMsg5", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg5", this.errMsg5);
		}

		public string ErrMsg5
		{
			get
			{
				return this.errMsg5;
			}
		}

		private readonly string errMsg5;
	}
}

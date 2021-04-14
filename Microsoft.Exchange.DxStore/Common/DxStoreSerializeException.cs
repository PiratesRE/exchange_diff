using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreSerializeException : LocalizedException
	{
		public DxStoreSerializeException(string err) : base(Strings.SerializeError(err))
		{
			this.err = err;
		}

		public DxStoreSerializeException(string err, Exception innerException) : base(Strings.SerializeError(err), innerException)
		{
			this.err = err;
		}

		protected DxStoreSerializeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("err", this.err);
		}

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string err;
	}
}

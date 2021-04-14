using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PooledConnectionOpenTimeoutException : TransientException
	{
		public PooledConnectionOpenTimeoutException(string msg) : base(Strings.PooledConnectionOpenTimeoutError(msg))
		{
			this.msg = msg;
		}

		public PooledConnectionOpenTimeoutException(string msg, Exception innerException) : base(Strings.PooledConnectionOpenTimeoutError(msg), innerException)
		{
			this.msg = msg;
		}

		protected PooledConnectionOpenTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}

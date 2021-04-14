using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DataOutOfSyncException : LocalizedException
	{
		public DataOutOfSyncException(int statusCode) : base(Strings.DataOutOfSyncException(statusCode))
		{
			this.statusCode = statusCode;
		}

		public DataOutOfSyncException(int statusCode, Exception innerException) : base(Strings.DataOutOfSyncException(statusCode), innerException)
		{
			this.statusCode = statusCode;
		}

		protected DataOutOfSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.statusCode = (int)info.GetValue("statusCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("statusCode", this.statusCode);
		}

		public int StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		private readonly int statusCode;
	}
}

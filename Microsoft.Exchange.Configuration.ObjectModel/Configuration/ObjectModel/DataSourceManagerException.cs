using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataSourceManagerException : LocalizedException
	{
		public DataSourceManagerException(LocalizedString message) : base(message)
		{
		}

		public DataSourceManagerException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DataSourceManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthTaskCasHasNoTopologySiteException : LocalizedException
	{
		public CasHealthTaskCasHasNoTopologySiteException() : base(Strings.CasHealthTaskCasHasNoTopologySite)
		{
		}

		public CasHealthTaskCasHasNoTopologySiteException(Exception innerException) : base(Strings.CasHealthTaskCasHasNoTopologySite, innerException)
		{
		}

		protected CasHealthTaskCasHasNoTopologySiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

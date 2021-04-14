using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleCoexistenceDomainsFoundException : LocalizedException
	{
		public MultipleCoexistenceDomainsFoundException() : base(Strings.MultipleCoexistenceDomainsFoundException)
		{
		}

		public MultipleCoexistenceDomainsFoundException(Exception innerException) : base(Strings.MultipleCoexistenceDomainsFoundException, innerException)
		{
		}

		protected MultipleCoexistenceDomainsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

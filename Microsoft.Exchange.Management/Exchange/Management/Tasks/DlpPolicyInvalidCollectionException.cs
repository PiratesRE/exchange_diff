using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DlpPolicyInvalidCollectionException : LocalizedException
	{
		public DlpPolicyInvalidCollectionException() : base(Strings.DlpPolicyInvalidCollectionError)
		{
		}

		public DlpPolicyInvalidCollectionException(Exception innerException) : base(Strings.DlpPolicyInvalidCollectionError, innerException)
		{
		}

		protected DlpPolicyInvalidCollectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

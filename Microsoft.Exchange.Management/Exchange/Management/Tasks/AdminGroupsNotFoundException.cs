using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminGroupsNotFoundException : LocalizedException
	{
		public AdminGroupsNotFoundException() : base(Strings.AdminGroupsNotFoundException)
		{
		}

		public AdminGroupsNotFoundException(Exception innerException) : base(Strings.AdminGroupsNotFoundException, innerException)
		{
		}

		protected AdminGroupsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

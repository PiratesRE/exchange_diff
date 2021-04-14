using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotResolveWindowsIdentityException : LocalizedException
	{
		public CannotResolveWindowsIdentityException() : base(Strings.CannotResolveWindowsIdentityException)
		{
		}

		public CannotResolveWindowsIdentityException(Exception innerException) : base(Strings.CannotResolveWindowsIdentityException, innerException)
		{
		}

		protected CannotResolveWindowsIdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

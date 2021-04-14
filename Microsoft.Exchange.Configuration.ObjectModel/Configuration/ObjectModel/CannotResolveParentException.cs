using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotResolveParentException : LocalizedException
	{
		public CannotResolveParentException(string ou) : base(Strings.CannotResolveParentOrganization(ou))
		{
			this.ou = ou;
		}

		public CannotResolveParentException(string ou, Exception innerException) : base(Strings.CannotResolveParentOrganization(ou), innerException)
		{
			this.ou = ou;
		}

		protected CannotResolveParentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ou = (string)info.GetValue("ou", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ou", this.ou);
		}

		public string Ou
		{
			get
			{
				return this.ou;
			}
		}

		private readonly string ou;
	}
}

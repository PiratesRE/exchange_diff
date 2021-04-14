using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PropertyNotSupportedOnLegacyObjectException : LocalizedException
	{
		public PropertyNotSupportedOnLegacyObjectException(string user, string propname) : base(Strings.PropertyNotSupportedOnLegacyObjectException(user, propname))
		{
			this.user = user;
			this.propname = propname;
		}

		public PropertyNotSupportedOnLegacyObjectException(string user, string propname, Exception innerException) : base(Strings.PropertyNotSupportedOnLegacyObjectException(user, propname), innerException)
		{
			this.user = user;
			this.propname = propname;
		}

		protected PropertyNotSupportedOnLegacyObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.propname = (string)info.GetValue("propname", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("propname", this.propname);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string Propname
		{
			get
			{
				return this.propname;
			}
		}

		private readonly string user;

		private readonly string propname;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConfigObjectNotFoundException : LocalizedException
	{
		public ConfigObjectNotFoundException(string identity, Type classType) : base(Strings.ConfigObjectNotFound(identity, classType))
		{
			this.identity = identity;
			this.classType = classType;
		}

		public ConfigObjectNotFoundException(string identity, Type classType, Exception innerException) : base(Strings.ConfigObjectNotFound(identity, classType), innerException)
		{
			this.identity = identity;
			this.classType = classType;
		}

		protected ConfigObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.classType = (Type)info.GetValue("classType", typeof(Type));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("classType", this.classType);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public Type ClassType
		{
			get
			{
				return this.classType;
			}
		}

		private readonly string identity;

		private readonly Type classType;
	}
}

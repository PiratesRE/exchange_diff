using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsOutofConfigWriteScopeException : LocalizedException
	{
		public IsOutofConfigWriteScopeException(string type, string id) : base(Strings.ErrorIsOutofConfigWriteScope(type, id))
		{
			this.type = type;
			this.id = id;
		}

		public IsOutofConfigWriteScopeException(string type, string id, Exception innerException) : base(Strings.ErrorIsOutofConfigWriteScope(type, id), innerException)
		{
			this.type = type;
			this.id = id;
		}

		protected IsOutofConfigWriteScopeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.type = (string)info.GetValue("type", typeof(string));
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.type);
			info.AddValue("id", this.id);
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string type;

		private readonly string id;
	}
}

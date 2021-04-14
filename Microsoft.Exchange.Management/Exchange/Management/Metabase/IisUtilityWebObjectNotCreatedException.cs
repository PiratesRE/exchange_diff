using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisUtilityWebObjectNotCreatedException : LocalizedException
	{
		public IisUtilityWebObjectNotCreatedException(string parent, string name, string type) : base(Strings.IisUtilityWebObjectNotCreatedException(parent, name, type))
		{
			this.parent = parent;
			this.name = name;
			this.type = type;
		}

		public IisUtilityWebObjectNotCreatedException(string parent, string name, string type, Exception innerException) : base(Strings.IisUtilityWebObjectNotCreatedException(parent, name, type), innerException)
		{
			this.parent = parent;
			this.name = name;
			this.type = type;
		}

		protected IisUtilityWebObjectNotCreatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parent = (string)info.GetValue("parent", typeof(string));
			this.name = (string)info.GetValue("name", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parent", this.parent);
			info.AddValue("name", this.name);
			info.AddValue("type", this.type);
		}

		public string Parent
		{
			get
			{
				return this.parent;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string parent;

		private readonly string name;

		private readonly string type;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsapiFilterNotRemovedException : LocalizedException
	{
		public IsapiFilterNotRemovedException(string parent, string name) : base(Strings.IsapiFilterNotRemovedException(parent, name))
		{
			this.parent = parent;
			this.name = name;
		}

		public IsapiFilterNotRemovedException(string parent, string name, Exception innerException) : base(Strings.IsapiFilterNotRemovedException(parent, name), innerException)
		{
			this.parent = parent;
			this.name = name;
		}

		protected IsapiFilterNotRemovedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parent = (string)info.GetValue("parent", typeof(string));
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parent", this.parent);
			info.AddValue("name", this.name);
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

		private readonly string parent;

		private readonly string name;
	}
}

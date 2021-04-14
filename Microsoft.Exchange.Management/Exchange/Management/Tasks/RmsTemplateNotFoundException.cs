using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsTemplateNotFoundException : LocalizedException
	{
		public RmsTemplateNotFoundException(string name) : base(Strings.RmsTemplateNotFound(name))
		{
			this.name = name;
		}

		public RmsTemplateNotFoundException(string name, Exception innerException) : base(Strings.RmsTemplateNotFound(name), innerException)
		{
			this.name = name;
		}

		protected RmsTemplateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}

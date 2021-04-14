using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExportDestinationAccessDeniedException : LocalizedException
	{
		public ExportDestinationAccessDeniedException(string name) : base(Strings.ExportDestinationAccessDenied(name))
		{
			this.name = name;
		}

		public ExportDestinationAccessDeniedException(string name, Exception innerException) : base(Strings.ExportDestinationAccessDenied(name), innerException)
		{
			this.name = name;
		}

		protected ExportDestinationAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
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

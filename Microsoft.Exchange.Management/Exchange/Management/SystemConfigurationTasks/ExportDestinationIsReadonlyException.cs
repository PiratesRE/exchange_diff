using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExportDestinationIsReadonlyException : LocalizedException
	{
		public ExportDestinationIsReadonlyException(string name) : base(Strings.ExportDestinationIsReadonly(name))
		{
			this.name = name;
		}

		public ExportDestinationIsReadonlyException(string name, Exception innerException) : base(Strings.ExportDestinationIsReadonly(name), innerException)
		{
			this.name = name;
		}

		protected ExportDestinationIsReadonlyException(SerializationInfo info, StreamingContext context) : base(info, context)
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

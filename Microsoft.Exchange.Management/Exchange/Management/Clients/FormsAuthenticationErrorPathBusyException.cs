using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Clients
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FormsAuthenticationErrorPathBusyException : TaskTransientException
	{
		public FormsAuthenticationErrorPathBusyException(string metabasePath) : base(Strings.FormsAuthenticationErrorPathBusyException(metabasePath))
		{
			this.metabasePath = metabasePath;
		}

		public FormsAuthenticationErrorPathBusyException(string metabasePath, Exception innerException) : base(Strings.FormsAuthenticationErrorPathBusyException(metabasePath), innerException)
		{
			this.metabasePath = metabasePath;
		}

		protected FormsAuthenticationErrorPathBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.metabasePath = (string)info.GetValue("metabasePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("metabasePath", this.metabasePath);
		}

		public string MetabasePath
		{
			get
			{
				return this.metabasePath;
			}
		}

		private readonly string metabasePath;
	}
}

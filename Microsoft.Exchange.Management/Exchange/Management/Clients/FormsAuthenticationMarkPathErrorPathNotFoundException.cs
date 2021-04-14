using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Clients
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FormsAuthenticationMarkPathErrorPathNotFoundException : DataSourceOperationException
	{
		public FormsAuthenticationMarkPathErrorPathNotFoundException(string metabasePath) : base(Strings.FormsAuthenticationMarkPathErrorPathNotFoundException(metabasePath))
		{
			this.metabasePath = metabasePath;
		}

		public FormsAuthenticationMarkPathErrorPathNotFoundException(string metabasePath, Exception innerException) : base(Strings.FormsAuthenticationMarkPathErrorPathNotFoundException(metabasePath), innerException)
		{
			this.metabasePath = metabasePath;
		}

		protected FormsAuthenticationMarkPathErrorPathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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

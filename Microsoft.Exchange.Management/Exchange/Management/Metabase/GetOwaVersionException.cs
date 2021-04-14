using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetOwaVersionException : DataSourceOperationException
	{
		public GetOwaVersionException(string owaDllPath) : base(Strings.GetOwaVersionException(owaDllPath))
		{
			this.owaDllPath = owaDllPath;
		}

		public GetOwaVersionException(string owaDllPath, Exception innerException) : base(Strings.GetOwaVersionException(owaDllPath), innerException)
		{
			this.owaDllPath = owaDllPath;
		}

		protected GetOwaVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.owaDllPath = (string)info.GetValue("owaDllPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("owaDllPath", this.owaDllPath);
		}

		public string OwaDllPath
		{
			get
			{
				return this.owaDllPath;
			}
		}

		private readonly string owaDllPath;
	}
}

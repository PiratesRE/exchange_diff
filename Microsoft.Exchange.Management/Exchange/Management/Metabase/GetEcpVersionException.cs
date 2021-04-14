using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetEcpVersionException : DataSourceOperationException
	{
		public GetEcpVersionException(string ecpDllPath) : base(Strings.GetEcpVersionException(ecpDllPath))
		{
			this.ecpDllPath = ecpDllPath;
		}

		public GetEcpVersionException(string ecpDllPath, Exception innerException) : base(Strings.GetEcpVersionException(ecpDllPath), innerException)
		{
			this.ecpDllPath = ecpDllPath;
		}

		protected GetEcpVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ecpDllPath = (string)info.GetValue("ecpDllPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ecpDllPath", this.ecpDllPath);
		}

		public string EcpDllPath
		{
			get
			{
				return this.ecpDllPath;
			}
		}

		private readonly string ecpDllPath;
	}
}

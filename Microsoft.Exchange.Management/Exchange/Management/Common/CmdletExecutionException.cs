using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CmdletExecutionException : LocalizedException
	{
		public CmdletExecutionException(string cmdlet) : base(Strings.CmdletExecutionError(cmdlet))
		{
			this.cmdlet = cmdlet;
		}

		public CmdletExecutionException(string cmdlet, Exception innerException) : base(Strings.CmdletExecutionError(cmdlet), innerException)
		{
			this.cmdlet = cmdlet;
		}

		protected CmdletExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cmdlet = (string)info.GetValue("cmdlet", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cmdlet", this.cmdlet);
		}

		public string Cmdlet
		{
			get
			{
				return this.cmdlet;
			}
		}

		private readonly string cmdlet;
	}
}

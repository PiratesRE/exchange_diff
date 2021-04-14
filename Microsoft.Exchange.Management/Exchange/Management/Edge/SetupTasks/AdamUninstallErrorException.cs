using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamUninstallErrorException : LocalizedException
	{
		public AdamUninstallErrorException(string error) : base(Strings.AdamUninstallError(error))
		{
			this.error = error;
		}

		public AdamUninstallErrorException(string error, Exception innerException) : base(Strings.AdamUninstallError(error), innerException)
		{
			this.error = error;
		}

		protected AdamUninstallErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}

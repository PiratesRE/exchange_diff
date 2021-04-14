using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExportIOFailureException : LocalizedException
	{
		public ExportIOFailureException(string err) : base(Strings.ExportIOFailure(err))
		{
			this.err = err;
		}

		public ExportIOFailureException(string err, Exception innerException) : base(Strings.ExportIOFailure(err), innerException)
		{
			this.err = err;
		}

		protected ExportIOFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("err", this.err);
		}

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string err;
	}
}

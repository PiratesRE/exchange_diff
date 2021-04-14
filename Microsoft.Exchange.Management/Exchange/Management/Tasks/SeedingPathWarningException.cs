using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedingPathWarningException : LocalizedException
	{
		public SeedingPathWarningException(string error) : base(Strings.SeedingPathWarningException(error))
		{
			this.error = error;
		}

		public SeedingPathWarningException(string error, Exception innerException) : base(Strings.SeedingPathWarningException(error), innerException)
		{
			this.error = error;
		}

		protected SeedingPathWarningException(SerializationInfo info, StreamingContext context) : base(info, context)
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

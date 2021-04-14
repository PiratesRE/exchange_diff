using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedingPathErrorException : LocalizedException
	{
		public SeedingPathErrorException(string error) : base(Strings.SeedingPathErrorException(error))
		{
			this.error = error;
		}

		public SeedingPathErrorException(string error, Exception innerException) : base(Strings.SeedingPathErrorException(error), innerException)
		{
			this.error = error;
		}

		protected SeedingPathErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
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

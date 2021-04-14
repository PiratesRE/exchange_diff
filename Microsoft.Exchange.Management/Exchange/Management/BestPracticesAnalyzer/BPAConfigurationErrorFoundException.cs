using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.BestPracticesAnalyzer
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BPAConfigurationErrorFoundException : LocalizedException
	{
		public BPAConfigurationErrorFoundException(string error) : base(Strings.BPAConfigurationErrorFound(error))
		{
			this.error = error;
		}

		public BPAConfigurationErrorFoundException(string error, Exception innerException) : base(Strings.BPAConfigurationErrorFound(error), innerException)
		{
			this.error = error;
		}

		protected BPAConfigurationErrorFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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

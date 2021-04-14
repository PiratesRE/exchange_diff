using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutodiscoverTaskException : LocalizedException
	{
		public AutodiscoverTaskException(string field, string autodiscoverData) : base(Strings.messageAutodiscoverTaskException(field, autodiscoverData))
		{
			this.field = field;
			this.autodiscoverData = autodiscoverData;
		}

		public AutodiscoverTaskException(string field, string autodiscoverData, Exception innerException) : base(Strings.messageAutodiscoverTaskException(field, autodiscoverData), innerException)
		{
			this.field = field;
			this.autodiscoverData = autodiscoverData;
		}

		protected AutodiscoverTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.field = (string)info.GetValue("field", typeof(string));
			this.autodiscoverData = (string)info.GetValue("autodiscoverData", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("field", this.field);
			info.AddValue("autodiscoverData", this.autodiscoverData);
		}

		public string Field
		{
			get
			{
				return this.field;
			}
		}

		public string AutodiscoverData
		{
			get
			{
				return this.autodiscoverData;
			}
		}

		private readonly string field;

		private readonly string autodiscoverData;
	}
}

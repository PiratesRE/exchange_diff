using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_MakeCallError : LocalizedException
	{
		public TUC_MakeCallError(string host, string error) : base(Strings.MakeCallError(host, error))
		{
			this.host = host;
			this.error = error;
		}

		public TUC_MakeCallError(string host, string error, Exception innerException) : base(Strings.MakeCallError(host, error), innerException)
		{
			this.host = host;
			this.error = error;
		}

		protected TUC_MakeCallError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.host = (string)info.GetValue("host", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("host", this.host);
			info.AddValue("error", this.error);
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string host;

		private readonly string error;
	}
}

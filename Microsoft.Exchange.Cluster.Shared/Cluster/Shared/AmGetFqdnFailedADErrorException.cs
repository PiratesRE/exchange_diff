using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmGetFqdnFailedADErrorException : AmServerNameResolveFqdnException
	{
		public AmGetFqdnFailedADErrorException(string nodeName, string adError) : base(Strings.AmGetFqdnFailedADError(nodeName, adError))
		{
			this.nodeName = nodeName;
			this.adError = adError;
		}

		public AmGetFqdnFailedADErrorException(string nodeName, string adError, Exception innerException) : base(Strings.AmGetFqdnFailedADError(nodeName, adError), innerException)
		{
			this.nodeName = nodeName;
			this.adError = adError;
		}

		protected AmGetFqdnFailedADErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.adError = (string)info.GetValue("adError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("adError", this.adError);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		public string AdError
		{
			get
			{
				return this.adError;
			}
		}

		private readonly string nodeName;

		private readonly string adError;
	}
}

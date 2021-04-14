using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Clients
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OwaIsapiFilterException : LocalizedException
	{
		public OwaIsapiFilterException(string errorMessage, int hresult) : base(Strings.OwaIsapiFilterException(errorMessage, hresult))
		{
			this.errorMessage = errorMessage;
			this.hresult = hresult;
		}

		public OwaIsapiFilterException(string errorMessage, int hresult, Exception innerException) : base(Strings.OwaIsapiFilterException(errorMessage, hresult), innerException)
		{
			this.errorMessage = errorMessage;
			this.hresult = hresult;
		}

		protected OwaIsapiFilterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
			this.hresult = (int)info.GetValue("hresult", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
			info.AddValue("hresult", this.hresult);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		public int Hresult
		{
			get
			{
				return this.hresult;
			}
		}

		private readonly string errorMessage;

		private readonly int hresult;
	}
}

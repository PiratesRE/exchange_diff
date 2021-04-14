using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CookieAndParameterServiceInstanceIdMismatchException : LocalizedException
	{
		public CookieAndParameterServiceInstanceIdMismatchException(string cookieServiceInstanceId, string paramterServiceInstanceId) : base(Strings.CookieAndParameterServiceInstanceIdMismatch(cookieServiceInstanceId, paramterServiceInstanceId))
		{
			this.cookieServiceInstanceId = cookieServiceInstanceId;
			this.paramterServiceInstanceId = paramterServiceInstanceId;
		}

		public CookieAndParameterServiceInstanceIdMismatchException(string cookieServiceInstanceId, string paramterServiceInstanceId, Exception innerException) : base(Strings.CookieAndParameterServiceInstanceIdMismatch(cookieServiceInstanceId, paramterServiceInstanceId), innerException)
		{
			this.cookieServiceInstanceId = cookieServiceInstanceId;
			this.paramterServiceInstanceId = paramterServiceInstanceId;
		}

		protected CookieAndParameterServiceInstanceIdMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cookieServiceInstanceId = (string)info.GetValue("cookieServiceInstanceId", typeof(string));
			this.paramterServiceInstanceId = (string)info.GetValue("paramterServiceInstanceId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cookieServiceInstanceId", this.cookieServiceInstanceId);
			info.AddValue("paramterServiceInstanceId", this.paramterServiceInstanceId);
		}

		public string CookieServiceInstanceId
		{
			get
			{
				return this.cookieServiceInstanceId;
			}
		}

		public string ParamterServiceInstanceId
		{
			get
			{
				return this.paramterServiceInstanceId;
			}
		}

		private readonly string cookieServiceInstanceId;

		private readonly string paramterServiceInstanceId;
	}
}

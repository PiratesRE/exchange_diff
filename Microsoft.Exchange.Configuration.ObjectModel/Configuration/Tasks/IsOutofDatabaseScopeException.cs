using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IsOutofDatabaseScopeException : LocalizedException
	{
		public IsOutofDatabaseScopeException(string id, string exceptionDetails) : base(Strings.ErrorIsOutofDatabaseScope(id, exceptionDetails))
		{
			this.id = id;
			this.exceptionDetails = exceptionDetails;
		}

		public IsOutofDatabaseScopeException(string id, string exceptionDetails, Exception innerException) : base(Strings.ErrorIsOutofDatabaseScope(id, exceptionDetails), innerException)
		{
			this.id = id;
			this.exceptionDetails = exceptionDetails;
		}

		protected IsOutofDatabaseScopeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
			this.exceptionDetails = (string)info.GetValue("exceptionDetails", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
			info.AddValue("exceptionDetails", this.exceptionDetails);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string ExceptionDetails
		{
			get
			{
				return this.exceptionDetails;
			}
		}

		private readonly string id;

		private readonly string exceptionDetails;
	}
}

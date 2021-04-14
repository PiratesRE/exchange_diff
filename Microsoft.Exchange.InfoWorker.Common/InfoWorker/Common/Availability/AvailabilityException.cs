using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AvailabilityException : LocalizedException
	{
		public string ServerName { get; set; }

		public string LocationIdentifier { get; set; }

		public AvailabilityException(ErrorConstants errorCode, LocalizedString localizedString) : base(localizedString)
		{
			base.ErrorCode = (int)errorCode;
			this.ServerName = ExceptionDefaults.DefaultMachineName;
			this.LocationIdentifier = string.Empty;
		}

		public AvailabilityException(ErrorConstants errorCode, LocalizedString localizedString, uint locationIdentifier) : base(localizedString)
		{
			base.ErrorCode = (int)errorCode;
			this.ServerName = ExceptionDefaults.DefaultMachineName;
			this.LocationIdentifier = locationIdentifier.ToString();
		}

		public AvailabilityException(ErrorConstants errorCode, LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
			base.ErrorCode = (int)errorCode;
			this.ServerName = ExceptionDefaults.DefaultMachineName;
			this.LocationIdentifier = string.Empty;
		}

		public AvailabilityException(ErrorConstants errorCode, LocalizedString localizedString, Exception innerException, uint locationIdentifier) : base(localizedString, innerException)
		{
			base.ErrorCode = (int)errorCode;
			this.ServerName = ExceptionDefaults.DefaultMachineName;
			this.LocationIdentifier = locationIdentifier.ToString();
		}

		public AvailabilityException(string serverName, ErrorConstants errorCode, LocalizedString localizedString) : base(localizedString)
		{
			base.ErrorCode = (int)errorCode;
			this.ServerName = serverName;
			this.LocationIdentifier = string.Empty;
		}

		protected AvailabilityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ServerName = (string)info.GetValue("ServerName", typeof(LocalizedString));
			this.LocationIdentifier = string.Empty;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ServerName", this.ServerName);
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.LocationIdentifier))
			{
				return string.Format("{0}{1}. Name of the server where exception originated: {2}", base.ToString(), Environment.NewLine, this.ServerName);
			}
			return string.Format("{0}{1}. Name of the server where exception originated: {2}. LID: {3}", new object[]
			{
				base.ToString(),
				Environment.NewLine,
				this.ServerName,
				this.LocationIdentifier
			});
		}

		private const string ServerNameKey = "ServerName";
	}
}

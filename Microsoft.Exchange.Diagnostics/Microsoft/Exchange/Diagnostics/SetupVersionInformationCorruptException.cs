using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SetupVersionInformationCorruptException : LocalizedException
	{
		public SetupVersionInformationCorruptException(string keyPath) : base(DiagnosticsResources.ExceptionSetupVersionInformationCorrupt(keyPath))
		{
			this.keyPath = keyPath;
		}

		public SetupVersionInformationCorruptException(string keyPath, Exception innerException) : base(DiagnosticsResources.ExceptionSetupVersionInformationCorrupt(keyPath), innerException)
		{
			this.keyPath = keyPath;
		}

		protected SetupVersionInformationCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyPath = (string)info.GetValue("keyPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyPath", this.keyPath);
		}

		public string KeyPath
		{
			get
			{
				return this.keyPath;
			}
		}

		private readonly string keyPath;
	}
}

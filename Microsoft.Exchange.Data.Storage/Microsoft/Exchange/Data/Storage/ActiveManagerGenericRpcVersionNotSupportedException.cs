using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ActiveManagerGenericRpcVersionNotSupportedException : AmServerException
	{
		public ActiveManagerGenericRpcVersionNotSupportedException(int requestServerVersion, int requestCommandId, int requestCommandMajorVersion, int requestCommandMinorVersion, int actualServerVersion, int actualMajorVersion, int actualMinorVersion) : base(ServerStrings.ActiveManagerGenericRpcVersionNotSupported(requestServerVersion, requestCommandId, requestCommandMajorVersion, requestCommandMinorVersion, actualServerVersion, actualMajorVersion, actualMinorVersion))
		{
			this.requestServerVersion = requestServerVersion;
			this.requestCommandId = requestCommandId;
			this.requestCommandMajorVersion = requestCommandMajorVersion;
			this.requestCommandMinorVersion = requestCommandMinorVersion;
			this.actualServerVersion = actualServerVersion;
			this.actualMajorVersion = actualMajorVersion;
			this.actualMinorVersion = actualMinorVersion;
		}

		public ActiveManagerGenericRpcVersionNotSupportedException(int requestServerVersion, int requestCommandId, int requestCommandMajorVersion, int requestCommandMinorVersion, int actualServerVersion, int actualMajorVersion, int actualMinorVersion, Exception innerException) : base(ServerStrings.ActiveManagerGenericRpcVersionNotSupported(requestServerVersion, requestCommandId, requestCommandMajorVersion, requestCommandMinorVersion, actualServerVersion, actualMajorVersion, actualMinorVersion), innerException)
		{
			this.requestServerVersion = requestServerVersion;
			this.requestCommandId = requestCommandId;
			this.requestCommandMajorVersion = requestCommandMajorVersion;
			this.requestCommandMinorVersion = requestCommandMinorVersion;
			this.actualServerVersion = actualServerVersion;
			this.actualMajorVersion = actualMajorVersion;
			this.actualMinorVersion = actualMinorVersion;
		}

		protected ActiveManagerGenericRpcVersionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestServerVersion = (int)info.GetValue("requestServerVersion", typeof(int));
			this.requestCommandId = (int)info.GetValue("requestCommandId", typeof(int));
			this.requestCommandMajorVersion = (int)info.GetValue("requestCommandMajorVersion", typeof(int));
			this.requestCommandMinorVersion = (int)info.GetValue("requestCommandMinorVersion", typeof(int));
			this.actualServerVersion = (int)info.GetValue("actualServerVersion", typeof(int));
			this.actualMajorVersion = (int)info.GetValue("actualMajorVersion", typeof(int));
			this.actualMinorVersion = (int)info.GetValue("actualMinorVersion", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestServerVersion", this.requestServerVersion);
			info.AddValue("requestCommandId", this.requestCommandId);
			info.AddValue("requestCommandMajorVersion", this.requestCommandMajorVersion);
			info.AddValue("requestCommandMinorVersion", this.requestCommandMinorVersion);
			info.AddValue("actualServerVersion", this.actualServerVersion);
			info.AddValue("actualMajorVersion", this.actualMajorVersion);
			info.AddValue("actualMinorVersion", this.actualMinorVersion);
		}

		public int RequestServerVersion
		{
			get
			{
				return this.requestServerVersion;
			}
		}

		public int RequestCommandId
		{
			get
			{
				return this.requestCommandId;
			}
		}

		public int RequestCommandMajorVersion
		{
			get
			{
				return this.requestCommandMajorVersion;
			}
		}

		public int RequestCommandMinorVersion
		{
			get
			{
				return this.requestCommandMinorVersion;
			}
		}

		public int ActualServerVersion
		{
			get
			{
				return this.actualServerVersion;
			}
		}

		public int ActualMajorVersion
		{
			get
			{
				return this.actualMajorVersion;
			}
		}

		public int ActualMinorVersion
		{
			get
			{
				return this.actualMinorVersion;
			}
		}

		private readonly int requestServerVersion;

		private readonly int requestCommandId;

		private readonly int requestCommandMajorVersion;

		private readonly int requestCommandMinorVersion;

		private readonly int actualServerVersion;

		private readonly int actualMajorVersion;

		private readonly int actualMinorVersion;
	}
}

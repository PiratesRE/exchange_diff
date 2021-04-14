using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CiSeederGenericException : SeedInProgressException
	{
		public CiSeederGenericException(string sourceServer, string destServer, string specificError) : base(ReplayStrings.CiSeederGenericException(sourceServer, destServer, specificError))
		{
			this.sourceServer = sourceServer;
			this.destServer = destServer;
			this.specificError = specificError;
		}

		public CiSeederGenericException(string sourceServer, string destServer, string specificError, Exception innerException) : base(ReplayStrings.CiSeederGenericException(sourceServer, destServer, specificError), innerException)
		{
			this.sourceServer = sourceServer;
			this.destServer = destServer;
			this.specificError = specificError;
		}

		protected CiSeederGenericException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
			this.destServer = (string)info.GetValue("destServer", typeof(string));
			this.specificError = (string)info.GetValue("specificError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceServer", this.sourceServer);
			info.AddValue("destServer", this.destServer);
			info.AddValue("specificError", this.specificError);
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		public string DestServer
		{
			get
			{
				return this.destServer;
			}
		}

		public string SpecificError
		{
			get
			{
				return this.specificError;
			}
		}

		private readonly string sourceServer;

		private readonly string destServer;

		private readonly string specificError;
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EcpServerVersion
	{
		public EcpServerVersion(ServerVersion serverVersion)
		{
			this.serverVersion = serverVersion;
		}

		[DataMember]
		public int Major
		{
			get
			{
				return this.serverVersion.Major;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public int Minor
		{
			get
			{
				return this.serverVersion.Minor;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public int Revision
		{
			get
			{
				return this.serverVersion.Revision;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		private ServerVersion serverVersion;
	}
}

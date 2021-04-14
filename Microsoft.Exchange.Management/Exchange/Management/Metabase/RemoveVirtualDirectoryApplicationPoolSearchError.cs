using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveVirtualDirectoryApplicationPoolSearchError : LocalizedException
	{
		public RemoveVirtualDirectoryApplicationPoolSearchError(string applicationPool, int hresult) : base(Strings.RemoveVirtualDirectoryApplicationPoolSearchError(applicationPool, hresult))
		{
			this.applicationPool = applicationPool;
			this.hresult = hresult;
		}

		public RemoveVirtualDirectoryApplicationPoolSearchError(string applicationPool, int hresult, Exception innerException) : base(Strings.RemoveVirtualDirectoryApplicationPoolSearchError(applicationPool, hresult), innerException)
		{
			this.applicationPool = applicationPool;
			this.hresult = hresult;
		}

		protected RemoveVirtualDirectoryApplicationPoolSearchError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.applicationPool = (string)info.GetValue("applicationPool", typeof(string));
			this.hresult = (int)info.GetValue("hresult", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("applicationPool", this.applicationPool);
			info.AddValue("hresult", this.hresult);
		}

		public string ApplicationPool
		{
			get
			{
				return this.applicationPool;
			}
		}

		public int Hresult
		{
			get
			{
				return this.hresult;
			}
		}

		private readonly string applicationPool;

		private readonly int hresult;
	}
}

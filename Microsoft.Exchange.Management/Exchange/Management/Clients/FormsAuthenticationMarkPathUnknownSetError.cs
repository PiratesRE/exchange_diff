using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Clients
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FormsAuthenticationMarkPathUnknownSetError : DataSourceOperationException
	{
		public FormsAuthenticationMarkPathUnknownSetError(string metabasePath, int propertyID, int hresult) : base(Strings.FormsAuthenticationMarkPathUnknownSetError(metabasePath, propertyID, hresult))
		{
			this.metabasePath = metabasePath;
			this.propertyID = propertyID;
			this.hresult = hresult;
		}

		public FormsAuthenticationMarkPathUnknownSetError(string metabasePath, int propertyID, int hresult, Exception innerException) : base(Strings.FormsAuthenticationMarkPathUnknownSetError(metabasePath, propertyID, hresult), innerException)
		{
			this.metabasePath = metabasePath;
			this.propertyID = propertyID;
			this.hresult = hresult;
		}

		protected FormsAuthenticationMarkPathUnknownSetError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.metabasePath = (string)info.GetValue("metabasePath", typeof(string));
			this.propertyID = (int)info.GetValue("propertyID", typeof(int));
			this.hresult = (int)info.GetValue("hresult", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("metabasePath", this.metabasePath);
			info.AddValue("propertyID", this.propertyID);
			info.AddValue("hresult", this.hresult);
		}

		public string MetabasePath
		{
			get
			{
				return this.metabasePath;
			}
		}

		public int PropertyID
		{
			get
			{
				return this.propertyID;
			}
		}

		public int Hresult
		{
			get
			{
				return this.hresult;
			}
		}

		private readonly string metabasePath;

		private readonly int propertyID;

		private readonly int hresult;
	}
}

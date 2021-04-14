using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Clients
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FormsAuthenticationMarkPathCannotMarkSecureAttributeException : DataSourceOperationException
	{
		public FormsAuthenticationMarkPathCannotMarkSecureAttributeException(string metabasePath, int propertyID) : base(Strings.FormsAuthenticationMarkPathCannotMarkSecureAttributeException(metabasePath, propertyID))
		{
			this.metabasePath = metabasePath;
			this.propertyID = propertyID;
		}

		public FormsAuthenticationMarkPathCannotMarkSecureAttributeException(string metabasePath, int propertyID, Exception innerException) : base(Strings.FormsAuthenticationMarkPathCannotMarkSecureAttributeException(metabasePath, propertyID), innerException)
		{
			this.metabasePath = metabasePath;
			this.propertyID = propertyID;
		}

		protected FormsAuthenticationMarkPathCannotMarkSecureAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.metabasePath = (string)info.GetValue("metabasePath", typeof(string));
			this.propertyID = (int)info.GetValue("propertyID", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("metabasePath", this.metabasePath);
			info.AddValue("propertyID", this.propertyID);
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

		private readonly string metabasePath;

		private readonly int propertyID;
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncDataSourceNotInPreferredSiteException : DataSourceTransientException
	{
		public BackSyncDataSourceNotInPreferredSiteException(string domainController) : base(DirectoryStrings.BackSyncDataSourceInDifferentSiteMessage(domainController))
		{
			this.domainController = domainController;
		}

		public BackSyncDataSourceNotInPreferredSiteException(string domainController, Exception innerException) : base(DirectoryStrings.BackSyncDataSourceInDifferentSiteMessage(domainController), innerException)
		{
			this.domainController = domainController;
		}

		protected BackSyncDataSourceNotInPreferredSiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domainController = (string)info.GetValue("domainController", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domainController", this.domainController);
		}

		public string DomainController
		{
			get
			{
				return this.domainController;
			}
		}

		private readonly string domainController;
	}
}

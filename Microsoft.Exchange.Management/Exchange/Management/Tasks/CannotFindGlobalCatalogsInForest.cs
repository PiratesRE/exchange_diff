using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindGlobalCatalogsInForest : LocalizedException
	{
		public CannotFindGlobalCatalogsInForest(string forestFqdn) : base(Strings.CannotFindGlobalCatalogsInForest(forestFqdn))
		{
			this.forestFqdn = forestFqdn;
		}

		public CannotFindGlobalCatalogsInForest(string forestFqdn, Exception innerException) : base(Strings.CannotFindGlobalCatalogsInForest(forestFqdn), innerException)
		{
			this.forestFqdn = forestFqdn;
		}

		protected CannotFindGlobalCatalogsInForest(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.forestFqdn = (string)info.GetValue("forestFqdn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("forestFqdn", this.forestFqdn);
		}

		public string ForestFqdn
		{
			get
			{
				return this.forestFqdn;
			}
		}

		private readonly string forestFqdn;
	}
}

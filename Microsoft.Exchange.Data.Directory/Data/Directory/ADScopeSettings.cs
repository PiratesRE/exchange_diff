using System;

namespace Microsoft.Exchange.Data.Directory
{
	public class ADScopeSettings
	{
		public OrganizationId Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		private OrganizationId scope;
	}
}

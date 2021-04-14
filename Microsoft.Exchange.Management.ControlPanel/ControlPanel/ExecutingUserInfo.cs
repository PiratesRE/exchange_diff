using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExecutingUserInfo : ValuePair
	{
		public ExecutingUserInfo()
		{
			this.ValueProperty = "PrimarySmtpAddress";
		}

		[DefaultValue("PrimarySmtpAddress")]
		public string ValueProperty { get; set; }

		[DefaultValue("")]
		public override object Value
		{
			get
			{
				RecipientObjectResolverRow recipientObjectResolverRow = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
				{
					RbacPrincipal.Current.ExecutingUserId
				}).FirstOrDefault<RecipientObjectResolverRow>();
				return recipientObjectResolverRow.GetType().GetProperty(this.ValueProperty).GetValue(recipientObjectResolverRow, null);
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}

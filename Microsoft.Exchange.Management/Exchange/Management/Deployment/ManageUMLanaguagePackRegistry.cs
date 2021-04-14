using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageUMLanaguagePackRegistry : Task
	{
		[Parameter(Mandatory = true)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			try
			{
				this.languagePack = UmLanguagePackUtils.GetUmLanguagePack(this.Language);
			}
			catch (UnSupportedUMLanguageException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}

		protected UmLanguagePack languagePack;
	}
}

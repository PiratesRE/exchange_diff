using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetWebAppVirtualDirectory<T> : SetExchangeVirtualDirectory<T> where T : ExchangeWebAppVirtualDirectory, new()
	{
		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(IISNotInstalledException).IsInstanceOfType(exception);
		}

		[Parameter]
		public bool BasicAuthentication
		{
			get
			{
				return (bool)base.Fields["BasicAuthentication"];
			}
			set
			{
				base.Fields["BasicAuthentication"] = value;
			}
		}

		[Parameter]
		public bool WindowsAuthentication
		{
			get
			{
				return (bool)base.Fields["WindowsAuthentication"];
			}
			set
			{
				base.Fields["WindowsAuthentication"] = value;
			}
		}

		[Parameter]
		public bool LiveIdAuthentication
		{
			get
			{
				return (bool)base.Fields["LiveIdAuthentication"];
			}
			set
			{
				base.Fields["LiveIdAuthentication"] = value;
			}
		}

		[Parameter]
		public GzipLevel GzipLevel
		{
			get
			{
				return (GzipLevel)base.Fields["GzipLevel"];
			}
			set
			{
				base.Fields["GzipLevel"] = value;
			}
		}

		protected bool FormsAuthentication
		{
			get
			{
				return (bool)base.Fields["FormsAuthentication"];
			}
			set
			{
				base.Fields["FormsAuthentication"] = value;
			}
		}

		protected bool DigestAuthentication
		{
			get
			{
				return (bool)base.Fields["DigestAuthentication"];
			}
			set
			{
				base.Fields["DigestAuthentication"] = value;
			}
		}

		protected bool AdfsAuthentication
		{
			get
			{
				return (bool)base.Fields["AdfsAuthentication"];
			}
			set
			{
				base.Fields["AdfsAuthentication"] = value;
			}
		}

		protected bool OAuthAuthentication
		{
			get
			{
				return (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
			set
			{
				base.InternalAuthenticationMethods = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.Fields.Contains("WindowsAuthentication"))
			{
				T dataObject = this.DataObject;
				if (dataObject.WindowsAuthentication)
				{
					goto IL_56;
				}
			}
			if (!base.Fields.Contains("DigestAuthentication"))
			{
				return;
			}
			T dataObject2 = this.DataObject;
			if (!dataObject2.DigestAuthentication)
			{
				return;
			}
			IL_56:
			if (base.Fields.Contains("WindowsAuthentication"))
			{
				T dataObject3 = this.DataObject;
				if (dataObject3.FormsAuthentication)
				{
					T dataObject4 = this.DataObject;
					dataObject4.FormsAuthentication = false;
					this.FormsAuthentication = false;
				}
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			WebAppVirtualDirectoryHelper.UpdateFromMetabase((ExchangeWebAppVirtualDirectory)dataObject);
			dataObject.ResetChangeTracking();
			base.StampChangesOn(dataObject);
		}

		protected virtual void UpdateDataObject(T dataObject)
		{
			if ((base.Fields.Contains("FormsAuthentication") && this.FormsAuthentication != dataObject.FormsAuthentication) || (base.Fields.Contains("LiveIdAuthentication") && this.LiveIdAuthentication != dataObject.LiveIdAuthentication) || (base.Fields.Contains("AdfsAuthentication") && this.AdfsAuthentication != dataObject.AdfsAuthentication) || (base.Fields.Contains("OAuthAuthentication") && this.OAuthAuthentication != dataObject.OAuthAuthentication) || (base.Fields.Contains("GzipLevel") && this.GzipLevel != dataObject.GzipLevel) || dataObject.IsChanged(ADOwaVirtualDirectorySchema.LogonFormat) || dataObject.IsChanged(ExchangeWebAppVirtualDirectorySchema.FormsAuthentication))
			{
				this.WriteWarning(Strings.NeedIisRestartWarning);
			}
		}

		protected sealed override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			T t = (T)((object)base.PrepareDataObject());
			if (base.HasErrors)
			{
				return null;
			}
			this.UpdateDataObject(t);
			bool flag = base.Fields.Contains("FormsAuthentication");
			bool flag2 = false;
			if (flag)
			{
				flag2 = this.FormsAuthentication;
			}
			if (flag && !flag2)
			{
				t.FormsAuthentication = false;
			}
			if (base.Fields.Contains("BasicAuthentication"))
			{
				t.BasicAuthentication = this.BasicAuthentication;
			}
			if (base.Fields.Contains("DigestAuthentication"))
			{
				t.DigestAuthentication = this.DigestAuthentication;
			}
			if (base.Fields.Contains("WindowsAuthentication"))
			{
				t.WindowsAuthentication = this.WindowsAuthentication;
			}
			if (base.Fields.Contains("LiveIdAuthentication"))
			{
				t.LiveIdAuthentication = this.LiveIdAuthentication;
			}
			if (base.Fields.Contains("AdfsAuthentication"))
			{
				t.AdfsAuthentication = this.AdfsAuthentication;
			}
			if (base.Fields.Contains("OAuthAuthentication"))
			{
				t.OAuthAuthentication = this.OAuthAuthentication;
			}
			if (flag && flag2)
			{
				t.FormsAuthentication = true;
			}
			if (!t.BasicAuthentication && !t.DigestAuthentication && !t.WindowsAuthentication && !t.FormsAuthentication && !t.LiveIdAuthentication && !t.AdfsAuthentication && !t.OAuthAuthentication)
			{
				this.WriteWarning(Strings.NoAuthenticationWarning(this.Identity.ToString(), this.CmdletName));
			}
			TaskLogger.LogExit();
			return t;
		}

		private string CmdletName
		{
			get
			{
				object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CmdletAttribute), false);
				CmdletAttribute cmdletAttribute = (CmdletAttribute)customAttributes[0];
				return cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
			}
		}

		internal void CheckGzipLevelIsNotError(GzipLevel gzipLevel)
		{
			if (gzipLevel == GzipLevel.Error)
			{
				base.WriteError(new TaskException(Strings.GzipCannotBeSetToError), ErrorCategory.NotSpecified, null);
			}
		}
	}
}

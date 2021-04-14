using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[OutputType(new Type[]
	{
		typeof(PushNotificationProxyPresentationObject)
	})]
	[Cmdlet("Enable", "PushNotificationProxy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class EnablePushNotificationProxy : ProxyCmdletBaseClass
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableProxy(this.Identity.ToString(), base.CurrentProxyStatus);
			}
		}

		[Parameter(Mandatory = false)]
		public string Uri
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.Url];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Url] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Organization
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			PushNotificationPublisherSettingsFactory pushNotificationPublisherSettingsFactory = new PushNotificationPublisherSettingsFactory();
			PushNotificationPublisherSettings pushNotificationPublisherSettings = pushNotificationPublisherSettingsFactory.Create(this.DataObject);
			try
			{
				pushNotificationPublisherSettings.Validate();
			}
			catch (PushNotificationConfigurationException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			PushNotificationApp pushNotificationApp = (PushNotificationApp)base.PrepareDataObject();
			if (base.Fields.IsModified(PushNotificationAppSchema.AuthenticationKey))
			{
				pushNotificationApp.AuthenticationKey = (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			if (base.Fields.IsModified(PushNotificationAppSchema.Url))
			{
				pushNotificationApp.Url = (string)base.Fields[PushNotificationAppSchema.Url];
			}
			pushNotificationApp.Enabled = new bool?(true);
			return pushNotificationApp;
		}
	}
}

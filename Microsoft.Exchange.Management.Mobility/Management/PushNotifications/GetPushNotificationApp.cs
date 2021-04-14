using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Get", "PushNotificationApp", DefaultParameterSetName = "Identity")]
	[OutputType(new Type[]
	{
		typeof(PushNotificationAppPresentationObject)
	})]
	public sealed class GetPushNotificationApp : GetSystemConfigurationObjectTask<PushNotificationAppIdParameter, PushNotificationApp>
	{
		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
				return configurationSession.GetOrgContainerId().GetDescendantId(PushNotificationApp.RdnContainer);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.Fields["Platform"] != null)
				{
					return base.OptionalIdentityData.AdditionalFilter;
				}
				return base.InternalFilter;
			}
		}

		[Parameter]
		public PushNotificationPlatform Platform
		{
			get
			{
				if (base.Fields["Platform"] != null)
				{
					return (PushNotificationPlatform)base.Fields["Platform"];
				}
				return PushNotificationPlatform.None;
			}
			set
			{
				base.Fields["Platform"] = value;
				base.OptionalIdentityData.AdditionalFilter = new ComparisonFilter(ComparisonOperator.Equal, PushNotificationAppSchema.Platform, base.Fields["Platform"]);
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return base.Fields["Enabled"] == null || (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter]
		public SwitchParameter UseClearTextAuthenticationKeys
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseClearTextAuthenticationKeys"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseClearTextAuthenticationKeys"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PushNotificationApp pushNotificationApp = dataObject as PushNotificationApp;
			PushNotificationAppPresentationObject pushNotificationAppPresentationObject = new PushNotificationAppPresentationObject(pushNotificationApp);
			if (!base.Fields.IsModified("Enabled") || pushNotificationAppPresentationObject.Enabled == this.Enabled)
			{
				if (this.UseClearTextAuthenticationKeys && pushNotificationApp.IsAuthenticationKeyEncrypted != null && pushNotificationApp.IsAuthenticationKeyEncrypted.Value)
				{
					PushNotificationDataProtector pushNotificationDataProtector = new PushNotificationDataProtector(null);
					pushNotificationApp.AuthenticationKey = pushNotificationDataProtector.Decrypt(pushNotificationApp.AuthenticationKey).AsUnsecureString();
					pushNotificationApp.IsAuthenticationKeyEncrypted = new bool?(false);
					pushNotificationAppPresentationObject = new PushNotificationAppPresentationObject(pushNotificationApp);
				}
				base.WriteResult(pushNotificationAppPresentationObject);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 156, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\PushNotifications\\GetPushNotificationApp.cs");
		}

		private const string EnabledParameterName = "Enabled";
	}
}

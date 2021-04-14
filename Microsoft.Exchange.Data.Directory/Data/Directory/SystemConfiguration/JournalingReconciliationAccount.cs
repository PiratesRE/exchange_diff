using System;
using System.Management.Automation;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class JournalingReconciliationAccount : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return JournalingReconciliationAccount.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchJournalingReconciliationRemoteAccount";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return JournalingReconciliationAccount.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[Parameter]
		public Uri ArchiveUri
		{
			get
			{
				return (Uri)this[JournalingReconciliationAccountSchema.ArchiveUri];
			}
			set
			{
				this[JournalingReconciliationAccountSchema.ArchiveUri] = value;
			}
		}

		[Parameter]
		public PSCredential AuthenticationCredential
		{
			get
			{
				return (PSCredential)this[JournalingReconciliationAccountSchema.AuthenticationCredential];
			}
			set
			{
				this[JournalingReconciliationAccountSchema.AuthenticationCredential] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Mailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[JournalingReconciliationAccountSchema.Mailboxes];
			}
			internal set
			{
				this[JournalingReconciliationAccountSchema.Mailboxes] = value;
			}
		}

		internal NetworkCredential GetNetworkCredential()
		{
			string text = (string)this[JournalingReconciliationAccountSchema.UserName];
			string text2 = (string)this[JournalingReconciliationAccountSchema.Password];
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				return null;
			}
			return new NetworkCredential(text, text2);
		}

		public const string TaskNoun = "JournalingReconciliationAccount";

		public const string ReconciliationAccountContainer = "Journaling Reconciliation Accounts";

		public const string TransportSettingsContainer = "Transport Settings";

		private const string MostDerivedObjectClassInternal = "msExchJournalingReconciliationRemoteAccount";

		private static readonly string RootDnRelative = "CN=Journaling Reconciliation Accounts,CN=Transport Settings";

		private static readonly ADObjectId parentPath = new ADObjectId(JournalingReconciliationAccount.RootDnRelative);

		private static readonly JournalingReconciliationAccountSchema schema = ObjectSchema.GetInstance<JournalingReconciliationAccountSchema>();
	}
}

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class SetPopImapConfiguration<TDataObject> : SetSingletonSystemConfigurationObjectTask<TDataObject> where TDataObject : PopImapAdConfiguration, new()
	{
		public SetPopImapConfiguration()
		{
			TDataObject tdataObject = Activator.CreateInstance<TDataObject>();
			this.protocolName = tdataObject.ProtocolName;
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return PopImapAdConfiguration.GetRootId(this.ServerObject, this.protocolName);
			}
		}

		protected Server ServerObject
		{
			get
			{
				if (this.serverObject == null)
				{
					ServerIdParameter serverIdParameter = this.Server ?? ServerIdParameter.Parse(Environment.MachineName);
					this.serverObject = (Server)base.GetDataObject<Server>(serverIdParameter, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				}
				return this.serverObject;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			TDataObject dataObject = this.DataObject;
			if (dataObject.ExchangeVersion.IsOlderThan(PopImapAdConfiguration.MinimumSupportedExchangeObjectVersion))
			{
				TDataObject dataObject2 = this.DataObject;
				string identity = dataObject2.Identity.ToString();
				TDataObject dataObject3 = this.DataObject;
				base.WriteError(new TaskException(Strings.ErrorSetOlderVirtualDirectory(identity, dataObject3.ExchangeVersion.ToString(), PopImapAdConfiguration.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			this.ValidateSetServerRoleSpecificParameters();
			HashSet<int> hashSet = new HashSet<int>();
			TDataObject dataObject4 = this.DataObject;
			foreach (IPBinding ipbinding in dataObject4.UnencryptedOrTLSBindings)
			{
				hashSet.Add(ipbinding.Port);
			}
			HashSet<int> hashSet2 = new HashSet<int>();
			TDataObject dataObject5 = this.DataObject;
			foreach (IPBinding ipbinding2 in dataObject5.SSLBindings)
			{
				hashSet2.Add(ipbinding2.Port);
			}
			object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CmdletAttribute), false);
			string noun = (customAttributes.Length > 0) ? ((CmdletAttribute)customAttributes[0]).NounName : string.Empty;
			TDataObject dataObject6 = this.DataObject;
			foreach (ProtocolConnectionSettings protocolConnectionSettings in dataObject6.InternalConnectionSettings)
			{
				if (((protocolConnectionSettings.EncryptionType == null || protocolConnectionSettings.EncryptionType == EncryptionType.TLS) && !hashSet.Contains(protocolConnectionSettings.Port)) || (protocolConnectionSettings.EncryptionType == EncryptionType.SSL && !hashSet2.Contains(protocolConnectionSettings.Port)))
				{
					string name = PopImapAdConfigurationSchema.InternalConnectionSettings.Name;
					TDataObject dataObject7 = this.DataObject;
					this.WriteWarning(Strings.PopImapSettingsPortMismatch(name, dataObject7.ProtocolName, noun));
					break;
				}
			}
			TDataObject dataObject8 = this.DataObject;
			foreach (ProtocolConnectionSettings protocolConnectionSettings2 in dataObject8.ExternalConnectionSettings)
			{
				if (((protocolConnectionSettings2.EncryptionType == null || protocolConnectionSettings2.EncryptionType == EncryptionType.TLS) && !hashSet.Contains(protocolConnectionSettings2.Port)) || (protocolConnectionSettings2.EncryptionType == EncryptionType.SSL && !hashSet2.Contains(protocolConnectionSettings2.Port)))
				{
					string name2 = PopImapAdConfigurationSchema.ExternalConnectionSettings.Name;
					TDataObject dataObject9 = this.DataObject;
					this.WriteWarning(Strings.PopImapSettingsPortMismatch(name2, dataObject9.ProtocolName, noun));
					break;
				}
			}
			if (this.DataObject.propertyBag.Changed)
			{
				TDataObject dataObject10 = this.DataObject;
				string protocol = dataObject10.ProtocolName;
				TDataObject dataObject11 = this.DataObject;
				this.WriteWarning(Strings.ChangesTakeEffectAfterRestartingPopImapService(protocol, dataObject11.ProtocolName, this.ServerObject.Name));
			}
			TaskLogger.LogExit();
		}

		protected virtual void ValidateSetServerRoleSpecificParameters()
		{
			if ((this.ServerObject.IsClientAccessServer && this.ServerObject.IsCafeServer) || !this.ServerObject.IsE15OrLater)
			{
				return;
			}
			if (this.ServerObject.IsCafeServer)
			{
				foreach (string text in this.BrickRoleRequiredForFields)
				{
					if (base.UserSpecifiedParameters[text] != null)
					{
						this.WriteError(new ExInvalidArgumentForServerRoleException(text, Strings.InstallCafeRoleDescription), ErrorCategory.InvalidArgument, null, false);
					}
				}
				return;
			}
			if (this.ServerObject.IsClientAccessServer)
			{
				foreach (string text2 in this.CafeRoleRequiredForFields)
				{
					if (base.UserSpecifiedParameters[text2] != null)
					{
						this.WriteError(new ExInvalidArgumentForServerRoleException(text2, Strings.InstallMailboxRoleDescription), ErrorCategory.InvalidArgument, null, false);
					}
				}
			}
		}

		private readonly string protocolName;

		private readonly string[] BrickRoleRequiredForFields = new string[]
		{
			"ProxyTargetPort",
			"CalendarItemRetrievalOption",
			"EnableExactRFC822Size",
			"MessageRetrievalMimeFormat",
			"OwaServerUrl",
			"SuppressReadReceipt"
		};

		private readonly string[] CafeRoleRequiredForFields = new string[]
		{
			"Banner",
			"ExternalConnectionSettings",
			"InternalConnectionSettings",
			"MaxConnectionFromSingleIP",
			"PreAuthenticatedConnectionTimeout",
			"UnencryptedOrTLSBindings",
			"SSLBindings"
		};

		private Server serverObject;
	}
}

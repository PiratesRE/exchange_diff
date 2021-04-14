using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class PswsPropertyConverterModule : TaskIOPipelineBase, ITaskModule
	{
		public bool NeedEncodeDecodeKeyProperties
		{
			get
			{
				return Constants.IsPowerShellWebService && this.context != null && this.context.ExchangeRunspaceConfig != null && this.context.ExchangeRunspaceConfig.ConfigurationSettings.EncodeDecodeKey;
			}
		}

		private bool ForceToReturnNonOrgHierarchyIdentity
		{
			get
			{
				return Constants.IsPowerShellWebService && this.context != null && OrganizationId.ForestWideOrgId.Equals(this.context.UserInfo.ExecutingUserOrganizationId) && !OrganizationId.ForestWideOrgId.Equals(this.context.UserInfo.CurrentOrganizationId);
			}
		}

		public PswsPropertyConverterModule(TaskContext context)
		{
			this.context = context;
		}

		public void Init(ITaskEvent task)
		{
			task.PreInit += this.DecodeKeyProperties;
			this.context.CommandShell.PrependTaskIOPipelineHandler(this);
		}

		public void Dispose()
		{
		}

		public static bool TryDecodeIIdentityParameter(IIdentityParameter identity, out IIdentityParameter decodedIdentity)
		{
			decodedIdentity = null;
			if (identity == null)
			{
				return false;
			}
			string rawIdentity = identity.RawIdentity;
			if (string.IsNullOrWhiteSpace(rawIdentity))
			{
				return false;
			}
			Type type = identity.GetType();
			string text;
			if (!UrlTokenConverter.TryUrlTokenDecode(rawIdentity, out text))
			{
				return false;
			}
			try
			{
				decodedIdentity = (Activator.CreateInstance(type, new object[]
				{
					text
				}) as IIdentityParameter);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (MissingMethodException arg)
			{
				throw new InvalidOperationException(string.Format("DEV Bug: The type {0} must have ctor(string) in order to be parsed from string input. Exception: {1}", type, arg));
			}
			return decodedIdentity != null;
		}

		public static void DecodeKeyProperties(string cmdletName, PropertyBag inputFields)
		{
			List<string> propertiesNeedUrlTokenInputDecode = PswsKeyProperties.GetPropertiesNeedUrlTokenInputDecode(cmdletName);
			foreach (string key in propertiesNeedUrlTokenInputDecode)
			{
				if (inputFields.IsModified(key))
				{
					object obj = inputFields[key];
					IIdentityParameter value;
					if (obj != null && PswsPropertyConverterModule.TryDecodeIIdentityParameter((IIdentityParameter)obj, out value))
					{
						inputFields[key] = value;
					}
				}
			}
		}

		public static bool TryConvertOutputObjectKeyProperties(ConvertOutputPropertyEventArgs args, out object convertedValue)
		{
			convertedValue = null;
			ConfigurableObject configurableObject = args.ConfigurableObject;
			PropertyDefinition property = args.Property;
			string propertyInStr = args.PropertyInStr;
			object value = args.Value;
			if (value == null)
			{
				return false;
			}
			if (!PswsKeyProperties.IsKeyProperty(configurableObject, property, propertyInStr))
			{
				return false;
			}
			if (value is string)
			{
				convertedValue = UrlTokenConverter.UrlTokenEncode((string)value);
				return true;
			}
			if (value is ObjectId)
			{
				convertedValue = new UrlTokenEncodedObjectId(value.ToString());
				return true;
			}
			if (value is IUrlTokenEncode)
			{
				((IUrlTokenEncode)value).ReturnUrlTokenEncodedString = true;
				convertedValue = value;
				return true;
			}
			throw new NotSupportedException(string.Format("Value with type {0} is not supported.", value.GetType()));
		}

		private void DecodeKeyProperties(object sender, EventArgs e)
		{
			if (!this.NeedEncodeDecodeKeyProperties)
			{
				return;
			}
			string invocationName = this.context.InvocationInfo.InvocationName;
			PropertyBag fields = this.context.InvocationInfo.Fields;
			PswsPropertyConverterModule.DecodeKeyProperties(invocationName, fields);
		}

		public override bool WriteObject(object input, out object output)
		{
			ConfigurableObject configurableObject = input as ConfigurableObject;
			if (configurableObject != null)
			{
				if (this.NeedEncodeDecodeKeyProperties)
				{
					configurableObject.OutputPropertyConverter += PswsPropertyConverterModule.TryConvertOutputObjectKeyProperties;
				}
				if (this.ForceToReturnNonOrgHierarchyIdentity)
				{
					NonOrgHierarchyConverter @object = new NonOrgHierarchyConverter(this.context.UserInfo.CurrentOrganizationId);
					configurableObject.OutputPropertyConverter += @object.TryConvertKeyToNonOrgHierarchy;
				}
			}
			output = input;
			return true;
		}

		private TaskContext context;
	}
}

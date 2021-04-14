using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ReplicaListProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public ReplicaListProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ReplicaListProperty CreateCommand(CommandContext commandContext)
		{
			return new ReplicaListProperty(commandContext);
		}

		public static string[] GetReplicaListFromStoreObject(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Null store object specified");
				return null;
			}
			PublicFolderSession publicFolderSession = storeObject.Session as PublicFolderSession;
			if (publicFolderSession == null || !publicFolderSession.IsPublicFolderSession)
			{
				ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Not a public folder session " + storeObject.Session.ToString());
				throw new InvalidPropertyRequestException(new PropertyUri(PropertyUriEnum.ReplicaList));
			}
			string[] array = storeObject.TryGetProperty(FolderSchema.ReplicaList) as string[];
			if (array == null || array.Length < 1)
			{
				ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Replica list property is empty on object " + storeObject.StoreObjectId.ToString());
				return null;
			}
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(publicFolderSession.OrganizationId);
			if (value == null)
			{
				ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Cannot retrieve public folder cache for organization " + publicFolderSession.OrganizationId);
				return null;
			}
			List<string> list = new List<string>();
			foreach (string text in array)
			{
				Guid mailboxGuid;
				if (Guid.TryParse(text, out mailboxGuid))
				{
					PublicFolderRecipient localMailboxRecipient = value.GetLocalMailboxRecipient(mailboxGuid);
					if (localMailboxRecipient != null)
					{
						list.Add(localMailboxRecipient.PrimarySmtpAddress.ToString());
					}
					else
					{
						ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Cannot find a recipient matching GUID " + mailboxGuid.ToString());
					}
				}
				else
				{
					ReplicaListProperty.Tracer.TraceError(0L, "ReplicaListProperty.GetReplicaListFromStoreObject: Replica GUID is invalid " + text);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return null;
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			this.commandContext.GetPropertyDefinitions().Single<PropertyDefinition>();
			string[] replicaListFromStoreObject = ReplicaListProperty.GetReplicaListFromStoreObject(storeObject);
			if (replicaListFromStoreObject != null)
			{
				serviceObject[propertyInformation] = replicaListFromStoreObject;
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.GetFolderCallTracer;
	}
}

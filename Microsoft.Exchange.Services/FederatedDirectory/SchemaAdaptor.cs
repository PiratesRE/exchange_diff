using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal abstract class SchemaAdaptor
	{
		public static void FromGroupMailboxToDirectoryObject(RequestSchema requestSchema, GetFederatedDirectoryGroupResponse groupMailbox, DirectoryObjectAccessor directoryObjectAccessor)
		{
			IEnumerable<SchemaAdaptor> adaptors = SchemaAdaptor.GetAdaptors(requestSchema);
			foreach (SchemaAdaptor schemaAdaptor in adaptors)
			{
				schemaAdaptor.FromGroupMailboxToDirectoryObject(groupMailbox, directoryObjectAccessor);
			}
		}

		public static void FromUserToDirectoryObject(RequestSchema requestSchema, GetFederatedDirectoryUserResponse user, DirectoryObjectAccessor directoryObjectAccessor)
		{
			IEnumerable<SchemaAdaptor> adaptors = SchemaAdaptor.GetAdaptors(requestSchema);
			foreach (SchemaAdaptor schemaAdaptor in adaptors)
			{
				schemaAdaptor.FromUserToDirectoryObject(user, directoryObjectAccessor);
			}
		}

		public static void FromDirectoryObjectToCmdletParameter(RequestSchema requestSchema, DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
		{
			IEnumerable<SchemaAdaptor> adaptors = SchemaAdaptor.GetAdaptors(requestSchema);
			foreach (SchemaAdaptor schemaAdaptor in adaptors)
			{
				schemaAdaptor.FromDirectoryObjectToCmdletParameter(directoryObjectAccessor, cmdlet);
			}
		}

		public static void FromDirectoryObjectToCmdletParameter(RequestSchema requestSchema, DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
		{
			IEnumerable<SchemaAdaptor> adaptors = SchemaAdaptor.GetAdaptors(requestSchema);
			foreach (SchemaAdaptor schemaAdaptor in adaptors)
			{
				schemaAdaptor.FromDirectoryObjectToCmdletParameter(directoryObjectAccessor, cmdlet);
			}
		}

		protected virtual void FromGroupMailboxToDirectoryObject(GetFederatedDirectoryGroupResponse groupMailbox, DirectoryObjectAccessor directoryObjectAccessor)
		{
		}

		protected virtual void FromUserToDirectoryObject(GetFederatedDirectoryUserResponse user, DirectoryObjectAccessor directoryObjectAccessor)
		{
		}

		protected virtual void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
		{
		}

		protected virtual void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
		{
		}

		private static T GetValueFromDirectoryObject<T>(DirectoryObjectAccessor directoryObjectAccessor, string propertyName) where T : class
		{
			Property property = directoryObjectAccessor.GetProperty(propertyName);
			if (property == null)
			{
				return default(T);
			}
			return property.Value as T;
		}

		protected static void FromIdentityDetailsToRelations(IList<FederatedDirectoryIdentityDetailsType> identitiesDetails, RelationSetAccessor relations)
		{
			if (identitiesDetails == null)
			{
				return;
			}
			relations.SetRelations(from id in identitiesDetails
			select new Guid(id.ExternalDirectoryObjectId));
		}

		protected static RecipientIdParameter[] GetIdsFromRelations(IEnumerable<Relation> relations)
		{
			if (relations == null)
			{
				return Array<RecipientIdParameter>.Empty;
			}
			List<RecipientIdParameter> list = new List<RecipientIdParameter>(10);
			foreach (Relation relation in relations)
			{
				list.Add(new RecipientIdParameter(relation.TargetObjectId.ToString()));
			}
			return list.ToArray();
		}

		private static IList<SchemaAdaptor> GetAdaptors(RequestSchema requestSchema)
		{
			if (requestSchema.IncludeAllProperties && requestSchema.IncludeAllResources && requestSchema.IncludeAllRelations)
			{
				return SchemaAdaptor.All;
			}
			List<SchemaAdaptor> list = new List<SchemaAdaptor>(10);
			list.Add(SchemaAdaptor.Id);
			if (requestSchema.IncludeAllProperties)
			{
				list.AddRange(SchemaAdaptor.Properties.Values);
			}
			else if (requestSchema.Properties != null)
			{
				foreach (string text in requestSchema.Properties)
				{
					SchemaAdaptor item;
					if (SchemaAdaptor.Properties.TryGetValue(text, out item))
					{
						list.Add(item);
					}
					else
					{
						SchemaAdaptor.Tracer.TraceError<string>(0L, "SchemaAdaptor.GetAdaptors() found unsupported property in RequestSchema: {0}", text);
					}
				}
			}
			if (requestSchema.IncludeAllResources)
			{
				list.AddRange(SchemaAdaptor.Resources.Values);
			}
			else if (requestSchema.Resources != null)
			{
				foreach (string text2 in requestSchema.Resources)
				{
					SchemaAdaptor item2;
					if (SchemaAdaptor.Resources.TryGetValue(text2, out item2))
					{
						list.Add(item2);
					}
					else
					{
						SchemaAdaptor.Tracer.TraceError<string>(0L, "SchemaAdaptor.GetAdaptors() found unsupported resource in RequestSchema: {0}", text2);
					}
				}
			}
			if (requestSchema.IncludeAllRelations)
			{
				list.AddRange(SchemaAdaptor.Relations.Values);
			}
			else if (requestSchema.Relations != null)
			{
				foreach (RelationRequestSchema relationRequestSchema in requestSchema.Relations)
				{
					SchemaAdaptor item3;
					if (SchemaAdaptor.Relations.TryGetValue(relationRequestSchema.Name, out item3))
					{
						list.Add(item3);
					}
					else
					{
						SchemaAdaptor.Tracer.TraceError<string>(0L, "SchemaAdaptor.GetAdaptors() found unsupported relation in RequestSchema: {0}", relationRequestSchema.Name);
					}
				}
			}
			return list;
		}

		private static SchemaAdaptor[] MergeAdaptors(params IEnumerable<SchemaAdaptor>[] adaptorSets)
		{
			List<SchemaAdaptor> list = new List<SchemaAdaptor>(10);
			foreach (IEnumerable<SchemaAdaptor> collection in adaptorSets)
			{
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		protected static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private static readonly SchemaAdaptor Id = new SchemaAdaptor.IdAdaptor();

		private static readonly SchemaAdaptor MailProperty = new SchemaAdaptor.MailPropertyAdaptor();

		private static readonly SchemaAdaptor AliasProperty = new SchemaAdaptor.AliasPropertyAdaptor();

		private static readonly SchemaAdaptor DescriptionProperty = new SchemaAdaptor.DescriptionPropertyAdaptor();

		private static readonly SchemaAdaptor DisplayNameProperty = new SchemaAdaptor.DisplayNamePropertyAdaptor();

		private static readonly SchemaAdaptor AllowAccessToRelation = new SchemaAdaptor.AllowAccessToRelationAdaptor();

		private static readonly SchemaAdaptor MembersRelation = new SchemaAdaptor.MembersRelationAdaptor();

		private static readonly SchemaAdaptor OwnersRelation = new SchemaAdaptor.OwnersRelationAdaptor();

		private static readonly SchemaAdaptor MembershipRelation = new SchemaAdaptor.MembershipRelationAdaptor();

		private static readonly SchemaAdaptor SiteUrlResource = new SchemaAdaptor.SiteUrlResourceAdaptor();

		private static readonly Dictionary<string, SchemaAdaptor> Properties = new Dictionary<string, SchemaAdaptor>
		{
			{
				"Mail",
				SchemaAdaptor.MailProperty
			},
			{
				"Alias",
				SchemaAdaptor.AliasProperty
			},
			{
				"DisplayName",
				SchemaAdaptor.DisplayNameProperty
			},
			{
				"Description",
				SchemaAdaptor.DescriptionProperty
			}
		};

		private static readonly Dictionary<string, SchemaAdaptor> Resources = new Dictionary<string, SchemaAdaptor>
		{
			{
				"SiteUrl",
				SchemaAdaptor.SiteUrlResource
			}
		};

		private static readonly Dictionary<string, SchemaAdaptor> Relations = new Dictionary<string, SchemaAdaptor>
		{
			{
				"Members",
				SchemaAdaptor.MembersRelation
			},
			{
				"Owners",
				SchemaAdaptor.OwnersRelation
			},
			{
				"Membership",
				SchemaAdaptor.MembershipRelation
			},
			{
				"AllowAccessTo",
				SchemaAdaptor.AllowAccessToRelation
			}
		};

		private static readonly SchemaAdaptor[] Required = new SchemaAdaptor[]
		{
			SchemaAdaptor.Id
		};

		private static readonly SchemaAdaptor[] All = SchemaAdaptor.MergeAdaptors(new IEnumerable<SchemaAdaptor>[]
		{
			SchemaAdaptor.Required,
			SchemaAdaptor.Properties.Values,
			SchemaAdaptor.Resources.Values,
			SchemaAdaptor.Relations.Values
		});

		private sealed class IdAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
				{
					return;
				}
				cmdlet.ExternalDirectoryObjectId = directoryObjectAccessor.DirectoryObject.Id.ToString();
				if (string.IsNullOrEmpty(cmdlet.Name))
				{
					Property property = directoryObjectAccessor.GetProperty("Alias");
					if (property != null)
					{
						string text = property.Value as string;
						if (!string.IsNullOrEmpty(text))
						{
							cmdlet.Name = text;
						}
					}
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				cmdlet.Identity = new RecipientIdParameter(directoryObjectAccessor.DirectoryObject.Id.ToString());
			}
		}

		private sealed class MailPropertyAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.MailPropertyAdaptor.PropertyName);
				if (!string.IsNullOrEmpty(valueFromDirectoryObject))
				{
					cmdlet.PrimarySmtpAddress = new SmtpAddress(valueFromDirectoryObject);
				}
			}

			private static readonly string PropertyName = "Mail";
		}

		private sealed class AliasPropertyAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.AliasPropertyAdaptor.PropertyName);
				if (!string.IsNullOrEmpty(valueFromDirectoryObject))
				{
					cmdlet.Alias = valueFromDirectoryObject;
					cmdlet.Name = valueFromDirectoryObject;
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				Property property = directoryObjectAccessor.GetProperty(SchemaAdaptor.AliasPropertyAdaptor.PropertyName);
				if (property != null && property.IsModified)
				{
					string value = property.Value as string;
					if (!string.IsNullOrEmpty(value))
					{
						throw new NotImplementedException("Set-GroupMailbox does not allow to change Alias property yet");
					}
				}
			}

			private static readonly string PropertyName = "Alias";
		}

		private sealed class DescriptionPropertyAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.DescriptionPropertyAdaptor.PropertyName);
				if (!string.IsNullOrEmpty(valueFromDirectoryObject))
				{
					cmdlet.Description = valueFromDirectoryObject;
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.DescriptionPropertyAdaptor.PropertyName);
				if (!string.IsNullOrEmpty(valueFromDirectoryObject))
				{
					cmdlet.Description = valueFromDirectoryObject;
				}
			}

			private static readonly string PropertyName = "Description";
		}

		private sealed class DisplayNamePropertyAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.DisplayNamePropertyAdaptor.PropertyName);
				if (!string.IsNullOrEmpty(valueFromDirectoryObject))
				{
					cmdlet.DisplayName = valueFromDirectoryObject;
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				string valueFromDirectoryObject = SchemaAdaptor.GetValueFromDirectoryObject<string>(directoryObjectAccessor, SchemaAdaptor.DisplayNamePropertyAdaptor.PropertyName);
				if (valueFromDirectoryObject != null)
				{
					cmdlet.DisplayName = valueFromDirectoryObject;
				}
			}

			private static readonly string PropertyName = "DisplayName";
		}

		private sealed class AllowAccessToRelationAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.AllowAccessToRelationAdaptor.RelationName);
				ModernGroupTypeInfo modernGroupType = ModernGroupTypeInfo.Public;
				if (relationSet != null && relationSet.RelationSet.Count == 0)
				{
					modernGroupType = ModernGroupTypeInfo.Private;
				}
				cmdlet.ModernGroupType = modernGroupType;
			}

			private static readonly string RelationName = "AllowAccessTo";
		}

		private sealed class MembersRelationAdaptor : SchemaAdaptor
		{
			protected override void FromGroupMailboxToDirectoryObject(GetFederatedDirectoryGroupResponse groupMailbox, DirectoryObjectAccessor directoryObjectAccessor)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.MembersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					SchemaAdaptor.FromIdentityDetailsToRelations(groupMailbox.Members, relationSet);
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.MembersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					cmdlet.Members = SchemaAdaptor.GetIdsFromRelations(relationSet.RelationSet);
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.MembersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					cmdlet.AddedMembers = SchemaAdaptor.GetIdsFromRelations(relationSet.AddedRelations);
					cmdlet.RemovedMembers = SchemaAdaptor.GetIdsFromRelations(relationSet.RemovedRelations);
				}
			}

			private static readonly string RelationName = ExchangeDirectorySchema.MembersRelation.Name;
		}

		private sealed class OwnersRelationAdaptor : SchemaAdaptor
		{
			protected override void FromGroupMailboxToDirectoryObject(GetFederatedDirectoryGroupResponse groupMailbox, DirectoryObjectAccessor directoryObjectAccessor)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.OwnersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					SchemaAdaptor.FromIdentityDetailsToRelations(groupMailbox.Owners, relationSet);
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, NewGroupMailbox cmdlet)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.OwnersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					cmdlet.Owners = SchemaAdaptor.GetIdsFromRelations(relationSet.RelationSet);
				}
			}

			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.OwnersRelationAdaptor.RelationName);
				if (relationSet != null)
				{
					cmdlet.AddOwners = SchemaAdaptor.GetIdsFromRelations(relationSet.AddedRelations);
					cmdlet.RemoveOwners = SchemaAdaptor.GetIdsFromRelations(relationSet.RemovedRelations);
				}
			}

			private static readonly string RelationName = ExchangeDirectorySchema.OwnersRelation.Name;
		}

		private sealed class MembershipRelationAdaptor : SchemaAdaptor
		{
			protected override void FromUserToDirectoryObject(GetFederatedDirectoryUserResponse user, DirectoryObjectAccessor directoryObjectAccessor)
			{
				RelationSetAccessor relationSet = directoryObjectAccessor.GetRelationSet(SchemaAdaptor.MembershipRelationAdaptor.RelationName);
				if (relationSet != null && user.Groups != null && user.Groups.Length > 0)
				{
					foreach (FederatedDirectoryGroupType federatedDirectoryGroupType in user.Groups)
					{
						RelationAccessor relationAccessor = relationSet.AddRelation(new Guid(federatedDirectoryGroupType.ExternalDirectoryObjectId));
						relationAccessor.SetProperty(ExchangeDirectorySchema.JoinDateProperty.Name, (DateTime)federatedDirectoryGroupType.JoinDateTime, false);
					}
				}
			}

			private static readonly string RelationName = ExchangeDirectorySchema.MembershipRelation.Name;
		}

		private sealed class SiteUrlResourceAdaptor : SchemaAdaptor
		{
			protected override void FromDirectoryObjectToCmdletParameter(DirectoryObjectAccessor directoryObjectAccessor, SetGroupMailbox cmdlet)
			{
				Resource resource = directoryObjectAccessor.GetResource("SiteUrl");
				if (resource != null)
				{
					string text = resource.Value as string;
					if (!string.IsNullOrEmpty(text))
					{
						cmdlet.SharePointUrl = new Uri(text);
					}
				}
			}
		}
	}
}

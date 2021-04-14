using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class ExchangeDirectoryAdaptor : BaseAdaptor
	{
		public override void Initialize(NameValueCollection parameters)
		{
			base.Parameters = parameters;
			ExchangeDirectorySchema.Initialize();
			base.AdapterId = ExchangeDirectorySchema.AdaptorId;
			base.ServiceName = "Exchange";
			base.PropertyTypes = ExchangeDirectorySchema.PropertyDefinitions;
			base.ResourceTypes = ExchangeDirectorySchema.ResourceDefinitions;
			base.RelationTypes = ExchangeDirectorySchema.RelationDefinitions;
			BaseAdaptor.Tracer.TraceDebug<ExchangeDirectoryAdaptor>((long)this.GetHashCode(), "ExchangeDirectoryAdaptor.Initialize() called: schema initialized with: {0}", this);
		}

		public override void LoadDirectoryObjectData(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema, out IDirectoryObjectState state)
		{
			ExchangeDirectoryAdaptor.EnsureObject(directoryObjectAccessor);
			Group group = directoryObjectAccessor.DirectoryObject as Group;
			if (group != null)
			{
				if (this.GetGroupMailbox(directoryObjectAccessor, requestSchema))
				{
					state = new DirectoryObjectState
					{
						Version = 1L,
						IsCommitted = true
					};
					return;
				}
				throw new FederatedDirectoryException(CoreResources.GetGroupMailboxFailed(group.Id.ToString(), "NotFound"));
			}
			else
			{
				User user = directoryObjectAccessor.DirectoryObject as User;
				if (user != null)
				{
					if (this.GetUser(directoryObjectAccessor, requestSchema))
					{
						state = new DirectoryObjectState
						{
							Version = 1L,
							IsCommitted = true
						};
						return;
					}
					throw new FederatedDirectoryException(CoreResources.GetFederatedDirectoryUserFailed(user.Id.ToString(), "NotFound"));
				}
				else
				{
					Tenant tenant = directoryObjectAccessor.DirectoryObject as Tenant;
					if (tenant == null)
					{
						state = null;
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 0, this.GetHashCode(), "ExchangeDirectoryAdaptor.LoadDirectoryObjectData(): ignoring directory object '{0}' because it is of unknown type: {1}", new object[]
						{
							directoryObjectAccessor.DirectoryObject.Id,
							directoryObjectAccessor.DirectoryObject.DirectoryObjectType
						});
						return;
					}
					if (this.GetTenant(directoryObjectAccessor, requestSchema))
					{
						state = new DirectoryObjectState
						{
							Version = 1L,
							IsCommitted = true
						};
						return;
					}
					throw new FederatedDirectoryException(CoreResources.GetTenantFailed(tenant.Id.ToString(), "NotFound"));
				}
			}
		}

		public override void RemoveDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor)
		{
			ExchangeDirectoryAdaptor.EnsureObject(directoryObjectAccessor);
			Group group = directoryObjectAccessor.DirectoryObject as Group;
			if (group != null)
			{
				this.RemoveGroupMailbox(directoryObjectAccessor);
				return;
			}
			LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 0, this.GetHashCode(), "ExchangeDirectoryAdaptor.RemoveDirectoryObject(): ignoring directory object '{0}' because it is of unknown type: {1}", new object[]
			{
				directoryObjectAccessor.DirectoryObject.Id,
				directoryObjectAccessor.DirectoryObject.DirectoryObjectType
			});
		}

		public override void CommitDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor)
		{
			ExchangeDirectoryAdaptor.EnsureObject(directoryObjectAccessor);
			Group group = directoryObjectAccessor.DirectoryObject as Group;
			if (group != null)
			{
				DirectoryObjectState directoryObjectState = (DirectoryObjectState)directoryObjectAccessor.GetState(base.ServiceName);
				if (directoryObjectState != null && directoryObjectState.IsNew)
				{
					IActivityScope activityScope = null;
					bool flag = false;
					try
					{
						activityScope = ActivityContext.GetCurrentActivityScope();
						if (activityScope == null)
						{
							activityScope = ActivityContext.Start(null);
							activityScope.SetProperty(ExtensibleLoggerMetadata.EventId, "PSDirectInvoke_NewGroupMailbox");
							flag = true;
						}
						this.CreateGroupMailbox(directoryObjectAccessor, activityScope.ActivityId);
						goto IL_81;
					}
					finally
					{
						if (flag)
						{
							activityScope.End();
						}
					}
				}
				this.UpdateGroupMailbox(directoryObjectAccessor, ExchangeDirectoryAdaptor.UpdateGroupCommitDirectoryObjectSchema);
				IL_81:
				if (directoryObjectState != null)
				{
					directoryObjectState.IsCommitted = true;
					directoryObjectState.Version += 1L;
					return;
				}
			}
			else
			{
				LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 0, this.GetHashCode(), "ExchangeDirectoryAdaptor.CommitDirectoryObject(): ignoring directory object '{0}' because it is of unknown type: {1}", new object[]
				{
					directoryObjectAccessor.DirectoryObject.Id,
					directoryObjectAccessor.DirectoryObject.DirectoryObjectType
				});
			}
		}

		public override bool TryRelationExists(DirectorySession directorySession, string relationName, Guid parentObjectId, DirectoryObjectType parentObjectObjectType, Guid targetObjectId, out bool relationExists)
		{
			relationExists = false;
			return true;
		}

		public override void NotifyChanges(DirectoryObjectAccessor directoryObjectAccessor)
		{
			ExchangeDirectoryAdaptor.EnsureObject(directoryObjectAccessor);
			if (!ExEnvironment.IsTest)
			{
				LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 3, this.GetHashCode(), "ExchangeDirectoryAdaptor.NotifyChanges(): Ignoring Object={0}, Type={1}.", new object[]
				{
					directoryObjectAccessor.DirectoryObject.Id,
					directoryObjectAccessor.DirectoryObject.DirectoryObjectType
				});
				return;
			}
			Group group = directoryObjectAccessor.DirectoryObject as Group;
			if (group == null)
			{
				LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 0, this.GetHashCode(), "ExchangeDirectoryAdaptor.NotifyChanges(): ignoring directory object '{0}' because it is of unknown type: {1}", new object[]
				{
					directoryObjectAccessor.DirectoryObject.Id,
					directoryObjectAccessor.DirectoryObject.DirectoryObjectType
				});
				return;
			}
			DirectoryObjectState directoryObjectState = (DirectoryObjectState)directoryObjectAccessor.GetState(base.ServiceName);
			if (directoryObjectState != null && directoryObjectState.IsNew)
			{
				this.UpdateGroupMailbox(directoryObjectAccessor, ExchangeDirectoryAdaptor.CreateGroupNotifyChangesSchema);
				return;
			}
			this.UpdateGroupMailbox(directoryObjectAccessor, ExchangeDirectoryAdaptor.UpdateGroupNotifyChangesSchema);
		}

		private bool GetGroupMailbox(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema)
		{
			if (ExchangeDirectoryAdaptor.ContainsRelation(requestSchema, ExchangeDirectoryAdaptor.GetGroupMailboxRelations) || ExchangeDirectoryAdaptor.ContainsResource(requestSchema, ExchangeDirectoryAdaptor.GetGroupMailboxResources))
			{
				Group group = (Group)directoryObjectAccessor.DirectoryObject;
				ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, exchangeDirectorySessionContext.AccessingUser.OrganizationId.ToADSessionSettings(), 417, "GetGroupMailbox", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\FederatedDirectory\\ExchangeDirectoryAdaptor.cs");
				ADUser aduser = tenantOrRootOrgRecipientSession.FindADUserByExternalDirectoryObjectId(group.Id.ToString());
				if (aduser == null)
				{
					throw new FederatedDirectoryException(CoreResources.GetFederatedDirectoryUserFailed(group.Id.ToString(), "NotFound"));
				}
				if (ExchangeDirectoryAdaptor.ContainsResource(requestSchema, ExchangeDirectoryAdaptor.MailboxUrlResources))
				{
					MailboxUrls mailboxUrls = new MailboxUrls(ExchangePrincipal.FromADUser(aduser, null), false);
					directoryObjectAccessor.SetResource(ExchangeDirectorySchema.CalendarUrlResource.Name, mailboxUrls.CalendarUrl, false);
					directoryObjectAccessor.SetResource(ExchangeDirectorySchema.InboxUrlResource.Name, mailboxUrls.InboxUrl, false);
					directoryObjectAccessor.SetResource(ExchangeDirectorySchema.PeopleUrlResource.Name, mailboxUrls.PeopleUrl, false);
					directoryObjectAccessor.SetResource(ExchangeDirectorySchema.GroupPictureUrlResource.Name, mailboxUrls.PhotoUrl, false);
				}
				GetFederatedDirectoryGroupResponse groupMailbox = new GetFederatedDirectoryGroupResponse
				{
					Members = (ExchangeDirectoryAdaptor.ContainsRelation(requestSchema, ExchangeDirectorySchema.MembersRelation.Name) ? ExchangeDirectoryAdaptor.GetMembers(tenantOrRootOrgRecipientSession, aduser) : null),
					Owners = (ExchangeDirectoryAdaptor.ContainsRelation(requestSchema, ExchangeDirectorySchema.OwnersRelation.Name) ? ExchangeDirectoryAdaptor.GetOwners(tenantOrRootOrgRecipientSession, aduser) : null)
				};
				SchemaAdaptor.FromGroupMailboxToDirectoryObject(requestSchema, groupMailbox, directoryObjectAccessor);
			}
			return true;
		}

		private bool GetTenant(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema)
		{
			ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
			BaseAdaptor.Tracer.TraceDebug((long)this.GetHashCode(), "ExchangeDirectoryAdaptor.GetTenant()");
			if (directoryObjectAccessor.DirectoryObject.Id.Equals(exchangeDirectorySessionContext.TenantContextId))
			{
				directoryObjectAccessor.SetProperty("DefaultDomain", exchangeDirectorySessionContext.AccessingUser.PrimarySmtpAddress.Domain, false);
				return true;
			}
			return false;
		}

		private void CreateGroupMailbox(DirectoryObjectAccessor directoryObjectAccessor, Guid activityId)
		{
			ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
			exchangeDirectorySessionContext.CreationDiagnostics.RecordAADTime();
			exchangeDirectorySessionContext.CreationDiagnostics.CmdletLogCorrelationId = activityId;
			using (PSLocalTask<NewGroupMailbox, GroupMailbox> pslocalTask = CmdletTaskFactory.Instance.CreateNewGroupMailboxTask(exchangeDirectorySessionContext.AccessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.ExecutingUser = new RecipientIdParameter(exchangeDirectorySessionContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				RequestSchema requestSchemaFromDirectoryObjectChanges = ExchangeDirectoryAdaptor.GetRequestSchemaFromDirectoryObjectChanges(directoryObjectAccessor, ExchangeDirectoryAdaptor.CreateGroupCommitDirectoryObjectSchema);
				SchemaAdaptor.FromDirectoryObjectToCmdletParameter(requestSchemaFromDirectoryObjectChanges, directoryObjectAccessor, pslocalTask.Task);
				LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.NewGroupMailboxToString(pslocalTask.Task));
				pslocalTask.Task.Execute();
				exchangeDirectorySessionContext.CreationDiagnostics.RecordMailboxTime();
				LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.TaskOutputToString(pslocalTask.AdditionalIO));
				if (pslocalTask.Error != null)
				{
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceError), 1, this.GetHashCode(), "ExchangeDirectoryAdaptor.CreateGroupMailbox() failed: {0}", new object[]
					{
						pslocalTask.ErrorMessage
					});
					throw new FederatedDirectoryException(CoreResources.NewGroupMailboxFailed(directoryObjectAccessor.DirectoryObject.Id.ToString(), pslocalTask.ErrorMessage));
				}
				exchangeDirectorySessionContext.CreationDiagnostics.MailboxCreatedSuccessfully = true;
				directoryObjectAccessor.SetProperty(ExchangeDirectorySchema.ExchangeDirectoryObjectIdProperty.Name, pslocalTask.Result.Guid, true);
				directoryObjectAccessor.SetProperty("Mail", pslocalTask.Result.PrimarySmtpAddress.ToString(), true);
				this.EnsureGroupIsInDirectoryCache(exchangeDirectorySessionContext.AccessingUser, pslocalTask.Result);
				LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 4, this.GetHashCode(), "ExchangeDirectoryAdaptor.CreateGroupMailbox() completed. Id={0}", new object[]
				{
					pslocalTask.Result.Identity
				});
			}
		}

		private void EnsureGroupIsInDirectoryCache(ADUser accessingUser, GroupMailbox group)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, accessingUser.OrganizationId.ToADSessionSettings(), 558, "EnsureGroupIsInDirectoryCache", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\FederatedDirectory\\ExchangeDirectoryAdaptor.cs");
			tenantOrRootOrgRecipientSession.DomainController = group.OriginatingServer;
			ProxyAddress proxyAddress = new SmtpProxyAddress(group.PrimarySmtpAddress.ToString(), true);
			ADUser aduser = tenantOrRootOrgRecipientSession.FindByProxyAddress(proxyAddress) as ADUser;
			OWAMiniRecipient owaminiRecipient = tenantOrRootOrgRecipientSession.FindMiniRecipientByProxyAddress<OWAMiniRecipient>(proxyAddress, OWAMiniRecipientSchema.AdditionalProperties);
			bool flag = aduser != null;
			bool flag2 = owaminiRecipient != null;
			LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 4, this.GetHashCode(), "ExchangeDirectoryAdaptor.EnsureGroupIsInDirectoryCache: ProxyAddress={0}, DomainController={1}, ADUser={2}, OWAMiniRecipient={3}", new object[]
			{
				proxyAddress,
				group.OriginatingServer,
				flag ? (aduser.IsCached ? "Cached" : "NotCached") : "NotFound",
				flag2 ? (owaminiRecipient.IsCached ? "Cached" : "NotCached") : "NotFound"
			});
		}

		private static bool IsRequestSchemaEmpty(RequestSchema requestSchema)
		{
			return !requestSchema.IncludeAllProperties && requestSchema.Properties.Count == 0 && !requestSchema.IncludeAllResources && requestSchema.Resources.Count == 0 && !requestSchema.IncludeAllRelations && requestSchema.Relations.Count == 0;
		}

		private void UpdateGroupMailbox(DirectoryObjectAccessor directoryObjectAccessor, HashSet<string> schema)
		{
			ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
			RequestSchema requestSchemaFromDirectoryObjectChanges = ExchangeDirectoryAdaptor.GetRequestSchemaFromDirectoryObjectChanges(directoryObjectAccessor, schema);
			if (!ExchangeDirectoryAdaptor.IsRequestSchemaEmpty(requestSchemaFromDirectoryObjectChanges))
			{
				using (PSLocalTask<SetGroupMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetGroupMailboxTask(exchangeDirectorySessionContext.AccessingPrincipal))
				{
					pslocalTask.CaptureAdditionalIO = true;
					pslocalTask.Task.ExecutingUser = new RecipientIdParameter(exchangeDirectorySessionContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
					SchemaAdaptor.FromDirectoryObjectToCmdletParameter(requestSchemaFromDirectoryObjectChanges, directoryObjectAccessor, pslocalTask.Task);
					LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.SetGroupMailboxToString(pslocalTask.Task));
					pslocalTask.Task.Execute();
					LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.TaskOutputToString(pslocalTask.AdditionalIO));
					if (pslocalTask.Error != null)
					{
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceError), 1, this.GetHashCode(), "ExchangeDirectoryAdaptor.UpdateGroupMailbox() failed: {0}", new object[]
						{
							pslocalTask.ErrorMessage
						});
						throw new FederatedDirectoryException(CoreResources.SetGroupMailboxFailed(directoryObjectAccessor.DirectoryObject.Id.ToString(), pslocalTask.ErrorMessage));
					}
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 4, this.GetHashCode(), "ExchangeDirectoryAdaptor.UpdateGroupMailbox() completed", new object[0]);
					return;
				}
			}
			BaseAdaptor.Tracer.TraceDebug((long)this.GetHashCode(), "ExchangeDirectoryAdaptor.UpdateGroupMailbox() nothing to do");
		}

		private void RemoveGroupMailbox(DirectoryObjectAccessor directoryObjectAccessor)
		{
			ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
			using (PSLocalTask<RemoveGroupMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateRemoveGroupMailboxTask(exchangeDirectorySessionContext.AccessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.ExecutingUser = new RecipientIdParameter(exchangeDirectorySessionContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				pslocalTask.Task.Identity = new MailboxIdParameter(directoryObjectAccessor.DirectoryObject.Id.ToString());
				LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.RemoveGroupMailboxToString(pslocalTask.Task));
				pslocalTask.Task.Execute();
				LogWriter.SimpleLog(new ExchangeDirectoryAdaptor.TaskOutputToString(pslocalTask.AdditionalIO));
				if (pslocalTask.Error != null)
				{
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceError), 1, this.GetHashCode(), "ExchangeDirectoryAdaptor.RemoveGroupMailbox() failed: {0}", new object[]
					{
						pslocalTask.ErrorMessage
					});
					throw new FederatedDirectoryException(CoreResources.RemoveGroupMailboxFailed(directoryObjectAccessor.DirectoryObject.Id.ToString(), pslocalTask.ErrorMessage));
				}
				LogWriter.TraceAndLog(new LogWriter.TraceMethod(BaseAdaptor.Tracer.TraceDebug), 4, this.GetHashCode(), "ExchangeDirectoryAdaptor.RemoveGroupMailbox() succeeded", new object[0]);
			}
		}

		private bool GetUser(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema)
		{
			if (ExchangeDirectoryAdaptor.ContainsRelation(requestSchema, ExchangeDirectoryAdaptor.GetUserRelations) || ExchangeDirectoryAdaptor.ContainsResource(requestSchema, ExchangeDirectoryAdaptor.GetUserResources))
			{
				User user = (User)directoryObjectAccessor.DirectoryObject;
				ExchangeDirectorySessionContext exchangeDirectorySessionContext = (ExchangeDirectorySessionContext)directoryObjectAccessor.DirectoryObject.DirectorySession.SessionContext;
				IRecipientSession adSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, exchangeDirectorySessionContext.AccessingUser.OrganizationId.ToADSessionSettings(), 707, "GetUser", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\FederatedDirectory\\ExchangeDirectoryAdaptor.cs");
				ADUser adUser = adSession.FindADUserByExternalDirectoryObjectId(user.Id.ToString());
				if (adUser == null)
				{
					throw new FederatedDirectoryException(CoreResources.GetFederatedDirectoryUserFailed(user.Id.ToString(), "NotFound"));
				}
				GetFederatedDirectoryUserResponse user2;
				if (adUser.RecipientType == RecipientType.MailUser)
				{
					user2 = new GetFederatedDirectoryUserResponse
					{
						Groups = new FederatedDirectoryGroupType[0]
					};
				}
				else
				{
					if (adUser.RecipientType != RecipientType.UserMailbox)
					{
						throw new FederatedDirectoryException(CoreResources.GetFederatedDirectoryUserFailed(user.Id.ToString(), "RecipientType"));
					}
					List<GroupMailbox> groupMailboxes = null;
					GroupMailboxAccessLayer.Execute("GetFederatedDirectoryUser", adSession, adUser.ExchangeGuid, adUser.OrganizationId, "Client=MSExchangeRPC;Action=ExchangeDirectoryAdaptor", delegate(GroupMailboxAccessLayer accessLayer)
					{
						UserMailboxLocator user3 = UserMailboxLocator.Instantiate(adSession, adUser);
						groupMailboxes = accessLayer.GetJoinedGroups(user3, false).ToList<GroupMailbox>();
					});
					if (ExchangeDirectoryAdaptor.ContainsResource(requestSchema, ExchangeDirectoryAdaptor.MailboxUrlResources))
					{
						MailboxUrls mailboxUrls = new MailboxUrls(ExchangePrincipal.FromADUser(adUser, null), false);
						directoryObjectAccessor.SetResource(ExchangeDirectorySchema.UserPictureUrlResource.Name, mailboxUrls.PhotoUrl, false);
					}
					user2 = new GetFederatedDirectoryUserResponse
					{
						Groups = ExchangeDirectoryAdaptor.ConvertToFederatedDirectoryGroupTypeList(groupMailboxes)
					};
				}
				SchemaAdaptor.FromUserToDirectoryObject(requestSchema, user2, directoryObjectAccessor);
			}
			return true;
		}

		private static FederatedDirectoryGroupType[] ConvertToFederatedDirectoryGroupTypeList(List<GroupMailbox> groupMailboxes)
		{
			List<FederatedDirectoryGroupType> list = new List<FederatedDirectoryGroupType>(groupMailboxes.Count);
			foreach (GroupMailbox groupMailbox in groupMailboxes)
			{
				list.Add(new FederatedDirectoryGroupType
				{
					ExternalDirectoryObjectId = groupMailbox.Locator.ExternalId,
					JoinDateTime = groupMailbox.JoinDate
				});
			}
			return list.ToArray();
		}

		private static FederatedDirectoryIdentityDetailsType[] GetOwners(IRecipientSession recipientSession, ADUser adUser)
		{
			ADObjectId[] array = ((MultiValuedProperty<ADObjectId>)adUser[GroupMailboxSchema.Owners]).ToArray();
			Result<ADRawEntry>[] array2 = recipientSession.ReadMultiple(array, ExchangeDirectoryAdaptor.IdentityProperties);
			List<FederatedDirectoryIdentityDetailsType> list = new List<FederatedDirectoryIdentityDetailsType>(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				Result<ADRawEntry> result = array2[i];
				if (result.Error != null || result.Data == null)
				{
					ExTraceGlobals.ModernGroupsTracer.TraceWarning<string, string>(0L, "GetFederatedDirectoryGroup.GetOwners: Unable to resolve \"{0}\":\"{1}\".", array[i].ToString(), (result.Error != null) ? result.Error.ToString() : string.Empty);
				}
				else
				{
					string text = (string)result.Data[ADRecipientSchema.ExternalDirectoryObjectId];
					if (string.IsNullOrEmpty(text))
					{
						ExTraceGlobals.ModernGroupsTracer.TraceWarning<string>(0L, "GetFederatedDirectoryGroup.GetOwners: Missing ExternalDirectoryObjectId for \"{0}\".", array[i].ToString());
					}
					else
					{
						list.Add(new FederatedDirectoryIdentityDetailsType
						{
							ExternalDirectoryObjectId = text
						});
					}
				}
			}
			return list.ToArray();
		}

		private static FederatedDirectoryIdentityDetailsType[] GetMembers(IRecipientSession recipientSession, ADUser adUser)
		{
			List<FederatedDirectoryIdentityDetailsType> result = new List<FederatedDirectoryIdentityDetailsType>(10);
			GroupMailboxLocator groupLocator = GroupMailboxLocator.Instantiate(recipientSession, adUser);
			GroupMailboxAccessLayer.Execute("GetFederatedDirectoryGroup", recipientSession, adUser.ExchangeGuid, adUser.OrganizationId, "Client=WebServices;Action=GetFederatedDirectoryGroup", delegate(GroupMailboxAccessLayer accessLayer)
			{
				IEnumerable<UserMailbox> members = accessLayer.GetMembers(groupLocator, false, null);
				foreach (UserMailbox userMailbox in members)
				{
					if (string.IsNullOrEmpty(userMailbox.Locator.ExternalId))
					{
						ExTraceGlobals.ModernGroupsTracer.TraceWarning<string>(0L, "GetFederatedDirectoryGroup.GetOwners: Missing ExternalDirectoryObjectId for \"{0}\".", userMailbox.Locator.LegacyDn);
					}
					else
					{
						result.Add(new FederatedDirectoryIdentityDetailsType
						{
							ExternalDirectoryObjectId = userMailbox.Locator.ExternalId
						});
					}
				}
			});
			return result.ToArray();
		}

		private static void EnsureObject(DirectoryObjectAccessor directoryObjectAccessor)
		{
			ArgumentValidator.ThrowIfNull("directoryObjectAccessor", directoryObjectAccessor);
			ArgumentValidator.ThrowIfNull("directoryObjectAccessor.DirectoryObject", directoryObjectAccessor.DirectoryObject);
			ArgumentValidator.ThrowIfEmpty("directoryObjectAccessor.DirectoryObject.Id", directoryObjectAccessor.DirectoryObject.Id);
			ArgumentValidator.ThrowIfNull("directoryObjectAccessor.DirectoryObject.DirectorySession", directoryObjectAccessor.DirectoryObject.DirectorySession);
		}

		private static RequestSchema GetRequestSchemaFromDirectoryObjectChanges(DirectoryObjectAccessor directoryObjectAccessor, HashSet<string> schema)
		{
			RequestSchema requestSchema = new RequestSchema();
			foreach (Property property in directoryObjectAccessor.GetChanges<Property>())
			{
				string name = property.GetPropertyType().Name;
				if (schema.Contains(name))
				{
					requestSchema.Properties.Add(name);
				}
			}
			foreach (Resource resource in directoryObjectAccessor.GetChanges<Resource>())
			{
				string name2 = resource.GetResourceType().Name;
				if (schema.Contains(name2))
				{
					requestSchema.Resources.Add(name2);
				}
			}
			foreach (RelationSet relationSet in directoryObjectAccessor.GetChanges<RelationSet>())
			{
				string name3 = relationSet.GetRelationType().Name;
				if (schema.Contains(name3))
				{
					requestSchema.Relations.Add(new RelationRequestSchema
					{
						Name = name3
					});
				}
			}
			return requestSchema;
		}

		private static bool ContainsRelation(RequestSchema requestSchema, string relation)
		{
			return requestSchema.Relations.Any((RelationRequestSchema requestRelation) => StringComparer.OrdinalIgnoreCase.Equals(requestRelation.Name, relation));
		}

		private static bool ContainsRelation(RequestSchema requestSchema, string[] relations)
		{
			return requestSchema.Relations.Any((RelationRequestSchema requestRelation) => relations.Any((string relation) => StringComparer.OrdinalIgnoreCase.Equals(requestRelation.Name, relation)));
		}

		private static bool ContainsResource(RequestSchema requestSchema, string[] resources)
		{
			return requestSchema.Resources.Any((string requestResource) => resources.Any((string resource) => StringComparer.OrdinalIgnoreCase.Equals(requestResource, resource)));
		}

		private const string NewGroupMailboxTaskEventId = "PSDirectInvoke_NewGroupMailbox";

		private static readonly HashSet<string> CreateGroupCommitDirectoryObjectSchema = new HashSet<string>(new string[]
		{
			"Alias",
			"Mail",
			"DisplayName",
			"Description",
			"Members",
			"Owners",
			"AllowAccessTo"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> CreateGroupNotifyChangesSchema = new HashSet<string>(new string[]
		{
			"SiteUrl"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> UpdateGroupCommitDirectoryObjectSchema = new HashSet<string>(new string[]
		{
			"Members",
			"Owners",
			"Membership"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> UpdateGroupNotifyChangesSchema = new HashSet<string>(new string[]
		{
			"Mail",
			"DisplayName",
			"Description",
			"SiteUrl"
		}, StringComparer.OrdinalIgnoreCase);

		private static readonly string[] GetGroupMailboxRelations = new string[]
		{
			"Members",
			"Owners"
		};

		private static readonly string[] GetGroupMailboxResources = new string[]
		{
			"PictureUrl",
			"InboxUrl",
			"CalendarUrl",
			"PeopleUrl"
		};

		private static readonly string[] GetUserRelations = new string[]
		{
			"Membership"
		};

		private static readonly string[] GetUserResources = new string[]
		{
			"PictureUrl"
		};

		private static readonly string[] MailboxUrlResources = new string[]
		{
			"CalendarUrl",
			"InboxUrl",
			"PeopleUrl",
			"PictureUrl"
		};

		private static readonly ADPropertyDefinition[] IdentityProperties = new ADPropertyDefinition[]
		{
			ADRecipientSchema.ExternalDirectoryObjectId
		};

		private sealed class NewGroupMailboxToString
		{
			public NewGroupMailboxToString(NewGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				ExchangeDirectoryAdaptor.CmdletStringBuilder cmdletStringBuilder = default(ExchangeDirectoryAdaptor.CmdletStringBuilder);
				cmdletStringBuilder.Append("New-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Organization", this.cmdlet.Organization);
				cmdletStringBuilder.Append("Name", this.cmdlet.Name);
				cmdletStringBuilder.Append("ModernGroupType", this.cmdlet.ModernGroupType.ToString());
				cmdletStringBuilder.Append("Description", this.cmdlet.Description);
				cmdletStringBuilder.Append("Owners", this.cmdlet.Owners);
				cmdletStringBuilder.Append("Members", this.cmdlet.Members);
				return cmdletStringBuilder.ToString();
			}

			private readonly NewGroupMailbox cmdlet;
		}

		private sealed class SetGroupMailboxToString
		{
			public SetGroupMailboxToString(SetGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				ExchangeDirectoryAdaptor.CmdletStringBuilder cmdletStringBuilder = default(ExchangeDirectoryAdaptor.CmdletStringBuilder);
				cmdletStringBuilder.Append("Set-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Identity", this.cmdlet.Identity);
				cmdletStringBuilder.Append("Name", this.cmdlet.Name);
				cmdletStringBuilder.Append("DisplayName", this.cmdlet.DisplayName);
				cmdletStringBuilder.Append("Description", this.cmdlet.Description);
				cmdletStringBuilder.Append("SharePointUrl", this.cmdlet.SharePointUrl);
				cmdletStringBuilder.Append("Owners", this.cmdlet.Owners);
				cmdletStringBuilder.Append("AddOwners", this.cmdlet.AddOwners);
				cmdletStringBuilder.Append("RemoveOwners", this.cmdlet.RemoveOwners);
				cmdletStringBuilder.Append("AddedMembers", this.cmdlet.AddedMembers);
				cmdletStringBuilder.Append("RemovedMembers", this.cmdlet.RemovedMembers);
				if (this.cmdlet.SharePointResources != null)
				{
					StringBuilder stringBuilder = new StringBuilder(400);
					foreach (string value in this.cmdlet.SharePointResources)
					{
						stringBuilder.AppendLine(value);
					}
					cmdletStringBuilder.Append("SharePointResources", stringBuilder.ToString());
				}
				return cmdletStringBuilder.ToString();
			}

			private readonly SetGroupMailbox cmdlet;
		}

		private sealed class RemoveGroupMailboxToString
		{
			public RemoveGroupMailboxToString(RemoveGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				ExchangeDirectoryAdaptor.CmdletStringBuilder cmdletStringBuilder = default(ExchangeDirectoryAdaptor.CmdletStringBuilder);
				cmdletStringBuilder.Append("Remove-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Identity", this.cmdlet.Identity);
				return cmdletStringBuilder.ToString();
			}

			private readonly RemoveGroupMailbox cmdlet;
		}

		private struct CmdletStringBuilder
		{
			public void Append(string value)
			{
				this.InitializeIfNeeded();
				this.stringBuilder.Append(value);
			}

			public void Append(string parameterName, string parameterValue)
			{
				this.InitializeIfNeeded();
				if (!string.IsNullOrEmpty(parameterValue))
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue,
						"'"
					}));
				}
			}

			public void Append(string parameterName, ADIdParameter parameterValue)
			{
				this.InitializeIfNeeded();
				if (parameterValue != null && !string.IsNullOrEmpty(parameterValue.RawIdentity))
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue.RawIdentity,
						"'"
					}));
				}
			}

			public void Append(string parameterName, Uri parameterValue)
			{
				this.InitializeIfNeeded();
				if (parameterValue != null)
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue.ToString(),
						"'"
					}));
				}
			}

			public void Append(string parameterName, RecipientIdParameter[] ids)
			{
				this.InitializeIfNeeded();
				if (ids != null && ids.Length > 0)
				{
					this.stringBuilder.Append(" -" + parameterName + ":");
					bool flag = true;
					foreach (RecipientIdParameter recipientIdParameter in ids)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							this.stringBuilder.Append(",");
						}
						this.stringBuilder.Append("'" + recipientIdParameter.RawIdentity + "'");
					}
				}
			}

			public override string ToString()
			{
				this.InitializeIfNeeded();
				return this.stringBuilder.ToString();
			}

			private void InitializeIfNeeded()
			{
				if (this.stringBuilder == null)
				{
					this.stringBuilder = new StringBuilder(256);
				}
			}

			private StringBuilder stringBuilder;
		}

		private sealed class TaskOutputToString
		{
			public TaskOutputToString(IList<PSLocalTaskIOData> container)
			{
				this.container = container;
			}

			public override string ToString()
			{
				if (this.container != null)
				{
					StringBuilder stringBuilder = new StringBuilder(1000);
					stringBuilder.AppendLine("Output:");
					foreach (PSLocalTaskIOData pslocalTaskIOData in this.container)
					{
						stringBuilder.AppendLine(pslocalTaskIOData.ToString());
					}
					return stringBuilder.ToString();
				}
				return "No output";
			}

			private readonly IList<PSLocalTaskIOData> container;
		}
	}
}

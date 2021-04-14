using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AcrProfile
	{
		internal AcrProfile(AcrPropertyResolverChain.ResolutionFunction genericResolutionFunction, params AcrProfile[] baseProfiles) : this(baseProfiles, new AcrPropertyResolverChain(new AcrPropertyResolverChain.ResolutionFunction[]
		{
			genericResolutionFunction,
			new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToServerValueIfClientMatchesServer)
		}, null, true))
		{
		}

		private AcrProfile(AcrProfile[] baseProfiles, AcrPropertyResolver genericResolver)
		{
			if (baseProfiles != null)
			{
				foreach (AcrProfile acrProfile in baseProfiles)
				{
					int num = 0;
					foreach (AcrProfile acrProfile2 in baseProfiles)
					{
						if (acrProfile == acrProfile2)
						{
							num++;
						}
						else
						{
							foreach (PropertyDefinition key in acrProfile.propertyProfileCollection.Keys)
							{
								if (acrProfile2.propertyProfileCollection.ContainsKey(key))
								{
									throw new ArgumentException(ServerStrings.ExInvalidAcrBaseProfiles);
								}
							}
						}
					}
					if (num != 1)
					{
						throw new ArgumentException(ServerStrings.ExInvalidAcrBaseProfiles);
					}
				}
			}
			this.baseProfiles = baseProfiles;
			this.genericResolver = genericResolver;
		}

		internal AcrPropertyProfile this[PropertyDefinition propertyDefinition]
		{
			get
			{
				AcrPropertyProfile acrPropertyProfile;
				if (!this.propertyProfileCollection.TryGetValue(propertyDefinition, out acrPropertyProfile) && this.baseProfiles != null)
				{
					int num = 0;
					while (num < this.baseProfiles.Length && acrPropertyProfile == null)
					{
						acrPropertyProfile = this.baseProfiles[num][propertyDefinition];
						num++;
					}
				}
				return acrPropertyProfile;
			}
		}

		internal static AcrProfile CreateWithGenericResolver(AcrPropertyResolver resolver, params AcrProfile[] baseProfiles)
		{
			return new AcrProfile(baseProfiles, resolver);
		}

		internal HashSet<PropertyDefinition> GetPropertiesNeededForResolution(IEnumerable<PropertyDefinition> propertyDefinitions)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				foreach (PropertyDefinition item in this.GetPropertiesNeededForResolution(propertyDefinition))
				{
					hashSet.TryAdd(item);
				}
			}
			return hashSet;
		}

		internal IEnumerable<PropertyDefinition> GetPropertiesNeededForResolution(PropertyDefinition propertyDefinition)
		{
			AcrPropertyProfile profile = this[propertyDefinition];
			if (profile != null)
			{
				foreach (PropertyDefinition relatedPropertyDefinition in profile.AllProperties)
				{
					yield return relatedPropertyDefinition;
				}
			}
			else
			{
				yield return propertyDefinition;
			}
			yield break;
		}

		internal ConflictResolutionResult ResolveConflicts(Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> propertyValuesToResolve)
		{
			Dictionary<PropertyDefinition, PropertyConflict> dictionary = new Dictionary<PropertyDefinition, PropertyConflict>(propertyValuesToResolve.Count);
			foreach (KeyValuePair<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> keyValuePair in propertyValuesToResolve)
			{
				AcrPropertyProfile acrPropertyProfile = this[keyValuePair.Key];
				if (!dictionary.ContainsKey(keyValuePair.Key))
				{
					if (acrPropertyProfile != null)
					{
						AcrProfile.ResolveConflicts(dictionary, acrPropertyProfile.Resolver, propertyValuesToResolve, acrPropertyProfile.PropertiesToResolve);
					}
					else
					{
						AcrProfile.ResolveConflicts(dictionary, this.genericResolver, propertyValuesToResolve, new PropertyDefinition[]
						{
							keyValuePair.Key
						});
					}
				}
			}
			SaveResult saveResult = SaveResult.Success;
			foreach (PropertyConflict propertyConflict in dictionary.Values)
			{
				if (!propertyConflict.ConflictResolvable)
				{
					saveResult = SaveResult.IrresolvableConflict;
					break;
				}
				saveResult = SaveResult.SuccessWithConflictResolution;
			}
			return new ConflictResolutionResult(saveResult, Util.CollectionToArray<PropertyConflict>(dictionary.Values));
		}

		private static AcrPropertyProfile.ValuesToResolve[] FilterValuesToResolve(Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> propertyValuesToResolve, PropertyDefinition[] propertiesToInclude)
		{
			AcrPropertyProfile.ValuesToResolve[] array = new AcrPropertyProfile.ValuesToResolve[propertiesToInclude.Length];
			for (int i = 0; i < propertiesToInclude.Length; i++)
			{
				propertyValuesToResolve.TryGetValue(propertiesToInclude[i], out array[i]);
			}
			return array;
		}

		private static void ResolveConflicts(Dictionary<PropertyDefinition, PropertyConflict> conflicts, AcrPropertyResolver resolver, Dictionary<PropertyDefinition, AcrPropertyProfile.ValuesToResolve> propertyValuesToResolve, PropertyDefinition[] propertiesToResolve)
		{
			AcrPropertyProfile.ValuesToResolve[] array = AcrProfile.FilterValuesToResolve(propertyValuesToResolve, propertiesToResolve);
			AcrPropertyProfile.ValuesToResolve[] dependencies = AcrProfile.FilterValuesToResolve(propertyValuesToResolve, resolver.Dependencies);
			object[] array2 = resolver.Resolve(array, dependencies);
			for (int i = 0; i < propertiesToResolve.Length; i++)
			{
				PropertyConflict value = new PropertyConflict(propertiesToResolve[i], array[i].OriginalValue, array[i].ClientValue, array[i].ServerValue, (array2 != null) ? array2[i] : null, array2 != null);
				conflicts.Add(propertiesToResolve[i], value);
			}
		}

		private static AcrProfile CreateBlankProfile()
		{
			return new AcrProfile(null, new AcrProfile[0]);
		}

		private static AcrProfile CreateFollowupFlagProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[]
			{
				AcrProfile.ReminderProfile
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToEarlierTime), false, new StorePropertyDefinition[]
			{
				InternalSchema.ReplyTime
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToOredValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.IsReplyRequested
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToOredValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.IsResponseRequested
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestIntValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MapiFlagStatus
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestIntValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.TaskStatus
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.FlagRequest
			});
			return acrProfile;
		}

		private static AcrProfile CreateReminderProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[0]);
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToOredValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ReminderIsSetInternal
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToEarlierTime), false, new StorePropertyDefinition[]
			{
				InternalSchema.ReminderDueByInternal
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToEarlierTime), false, new StorePropertyDefinition[]
			{
				InternalSchema.ReminderNextTime
			});
			return acrProfile;
		}

		private static AcrProfile CreateReplyForwardRelatedProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[0]);
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.IconIndex
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.LastVerbExecuted
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.LastVerbExecutionTime
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.Flags
			});
			return acrProfile;
		}

		private static AcrProfile CreateCommonMessageProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[0]);
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestPriorityAndImportance), false, new StorePropertyDefinition[]
			{
				InternalSchema.MapiPriority,
				InternalSchema.MapiImportance
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestSensitivity), false, new StorePropertyDefinition[]
			{
				InternalSchema.MapiSensitivity,
				InternalSchema.Privacy
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToMergedStringValues), false, new StorePropertyDefinition[]
			{
				InternalSchema.Categories
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.MapiSubject
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.AppointmentStateInternal
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToModifiedValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ConversationIndexTracking
			});
			return acrProfile;
		}

		private static AcrProfile CreateAppointmentProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[]
			{
				AcrProfile.CommonMessageProfile,
				AcrProfile.ReminderProfile
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestIntValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ReminderMinutesBeforeStartInternal
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.MapiStartTime
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.MapiPRStartDate
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.Location
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.LocationDisplayName
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValueIfServerValueNotModified), true, new StorePropertyDefinition[]
			{
				InternalSchema.LidWhere
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OutlookInternalVersion
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OutlookVersion
			});
			AcrProfile.AddCalendarLoggingPropertyProfile(acrProfile);
			return acrProfile;
		}

		private static AcrProfile CreateContactProfile()
		{
			return new AcrProfile(null, new AcrProfile[]
			{
				AcrProfile.FollowupFlagProfile
			});
		}

		private static AcrProfile CreateMailboxAssociationProfile()
		{
			AcrProfile acrProfile = new AcrProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), new AcrProfile[]
			{
				AcrProfile.BlankProfile
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToIncrementHighestIntValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MailboxAssociationCurrentVersion
			});
			return acrProfile;
		}

		private static AcrProfile CreateMessageProfile()
		{
			return new AcrProfile(null, new AcrProfile[]
			{
				AcrProfile.CommonMessageProfile,
				AcrProfile.ReminderProfile,
				AcrProfile.FollowupFlagProfile,
				AcrProfile.ReplyForwardRelatedProfile
			});
		}

		private static AcrProfile CreateMeetingMessageProfile()
		{
			AcrProfile acrProfile = new AcrProfile(null, new AcrProfile[]
			{
				AcrProfile.CommonMessageProfile
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToOredValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.IsProcessed
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveResponseType), false, new StorePropertyDefinition[]
			{
				InternalSchema.MapiResponseType
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OutlookInternalVersion
			});
			AcrProfile.AddCalendarLoggingPropertyProfile(acrProfile);
			return acrProfile;
		}

		private static void AddCalendarLoggingPropertyProfile(AcrProfile profile)
		{
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ItemVersion
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ChangeList
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.CalendarLogTriggerAction
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OriginalFolderId
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OriginalCreationTime
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OriginalLastModifiedTime
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.OriginalEntryId
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ClientInfoString
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ClientProcessName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ClientMachineName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ClientBuildVersion
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MiddleTierProcessName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MiddleTierServerName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MiddleTierServerBuildVersion
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MailboxServerName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MailboxServerBuildVersion
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.MailboxDatabaseName
			});
			profile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToClientValue), false, new StorePropertyDefinition[]
			{
				InternalSchema.ResponsibleUserName
			});
		}

		private static AcrProfile CreateCategoryProfile()
		{
			AcrProfile acrProfile = AcrProfile.CreateWithGenericResolver(new AcrPropertyResolverChain(new AcrPropertyResolverChain.ResolutionFunction[]
			{
				new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToModifiedValue),
				new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToGreatestDependency<ExDateTime>)
			}, new PropertyDefinition[]
			{
				CategorySchema.LastTimeUsed
			}, false), new AcrProfile[0]);
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsed
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedCalendar
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedContacts
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedJournal
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedMail
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedNotes
			});
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				CategorySchema.LastTimeUsedTasks
			});
			return acrProfile;
		}

		private static AcrProfile CreateMasterCategoryListProfile()
		{
			AcrProfile acrProfile = AcrProfile.CreateWithGenericResolver(new AcrPropertyResolverChain(new AcrPropertyResolverChain.ResolutionFunction[]
			{
				new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToModifiedValue),
				new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToGreatestDependency<ExDateTime>)
			}, new PropertyDefinition[]
			{
				MasterCategoryListSchema.LastSavedTime
			}, false), new AcrProfile[0]);
			acrProfile.AddPropertyProfile(new AcrPropertyResolverChain.ResolutionFunction(AcrHelpers.ResolveToHighestValue<ExDateTime>), false, new StorePropertyDefinition[]
			{
				MasterCategoryListSchema.LastSavedTime
			});
			return acrProfile;
		}

		private void AddPropertyProfile(AcrPropertyResolverChain.ResolutionFunction resolutionFunction, bool requireChangeTracking, params StorePropertyDefinition[] interDependentProperties)
		{
			for (int i = 0; i < interDependentProperties.Length; i++)
			{
				PropertyDefinition propertyDefinition = interDependentProperties[i];
				if (propertyDefinition is SmartPropertyDefinition)
				{
					throw new ArgumentException("interndependentProperties cannot contain SmartProperties");
				}
				if (this.propertyProfileCollection.ContainsKey(propertyDefinition) || i < Array.LastIndexOf<PropertyDefinition>(interDependentProperties, propertyDefinition))
				{
					throw new ArgumentException(ServerStrings.ExPropertyDefinitionInMoreThanOnePropertyProfile);
				}
			}
			AcrPropertyProfile value = new AcrPropertyProfile(new AcrPropertyResolverChain(new AcrPropertyResolverChain.ResolutionFunction[]
			{
				resolutionFunction
			}, null, false), requireChangeTracking, interDependentProperties);
			foreach (StorePropertyDefinition key in interDependentProperties)
			{
				this.propertyProfileCollection.Add(key, value);
			}
		}

		private static readonly AcrProfile ReminderProfile = AcrProfile.CreateReminderProfile();

		private static readonly AcrProfile FollowupFlagProfile = AcrProfile.CreateFollowupFlagProfile();

		private static readonly AcrProfile ReplyForwardRelatedProfile = AcrProfile.CreateReplyForwardRelatedProfile();

		private readonly Dictionary<PropertyDefinition, AcrPropertyProfile> propertyProfileCollection = new Dictionary<PropertyDefinition, AcrPropertyProfile>();

		private readonly AcrProfile[] baseProfiles;

		private readonly AcrPropertyResolver genericResolver;

		internal static readonly AcrProfile BlankProfile = AcrProfile.CreateBlankProfile();

		internal static readonly AcrProfile GenericItemProfile = AcrProfile.ReminderProfile;

		internal static readonly AcrProfile CommonMessageProfile = AcrProfile.CreateCommonMessageProfile();

		internal static readonly AcrProfile FolderProfile = AcrProfile.BlankProfile;

		internal static readonly AcrProfile AppointmentProfile = AcrProfile.CreateAppointmentProfile();

		internal static readonly AcrProfile MessageProfile = AcrProfile.CreateMessageProfile();

		internal static readonly AcrProfile MeetingMessageProfile = AcrProfile.CreateMeetingMessageProfile();

		internal static readonly AcrProfile ContactProfile = AcrProfile.CreateContactProfile();

		internal static readonly AcrProfile CategoryProfile = AcrProfile.CreateCategoryProfile();

		internal static readonly AcrProfile MasterCategoryListProfile = AcrProfile.CreateMasterCategoryListProfile();

		internal static readonly AcrProfile MailboxAssociationProfile = AcrProfile.CreateMailboxAssociationProfile();
	}
}

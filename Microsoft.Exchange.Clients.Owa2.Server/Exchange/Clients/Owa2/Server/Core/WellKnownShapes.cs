using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class WellKnownShapes
	{
		public static DistinguishedFolderIdName[] RequiredDistinguishedFolders
		{
			get
			{
				return WellKnownShapes.requiredDistinguishedFolders.ToArray();
			}
		}

		public static DistinguishedFolderIdName[] FoldersToMoveToTop
		{
			get
			{
				return WellKnownShapes.foldersToMoveToTop.ToArray();
			}
		}

		public static Dictionary<WellKnownShapeName, ResponseShape> ResponseShapes
		{
			get
			{
				return WellKnownShapes.responseShapes.Member;
			}
		}

		public static string GenerateRandomCssScopeName()
		{
			Random random = new Random();
			return "rps_" + ((int)Math.Floor(random.NextDouble() * 65535.0 + 1.0)).ToString("X4");
		}

		private static FolderResponseShape Folder
		{
			get
			{
				return new FolderResponseShape
				{
					BaseShape = ShapeEnum.IdOnly,
					AdditionalProperties = new PropertyPath[]
					{
						new PropertyUri(PropertyUriEnum.FolderId),
						new PropertyUri(PropertyUriEnum.ParentFolderId),
						new PropertyUri(PropertyUriEnum.FolderDisplayName),
						new PropertyUri(PropertyUriEnum.UnreadCount),
						new PropertyUri(PropertyUriEnum.TotalCount),
						new PropertyUri(PropertyUriEnum.ChildFolderCount),
						new PropertyUri(PropertyUriEnum.FolderClass),
						new PropertyUri(PropertyUriEnum.FolderEffectiveRights),
						new PropertyUri(PropertyUriEnum.DistinguishedFolderId),
						new PropertyUri(PropertyUriEnum.FolderPolicyTag),
						new PropertyUri(PropertyUriEnum.FolderArchiveTag),
						WellKnownProperties.Hidden,
						WellKnownProperties.RetentionFlags,
						new PropertyUri(PropertyUriEnum.UnClutteredViewFolderEntryId),
						new PropertyUri(PropertyUriEnum.ClutteredViewFolderEntryId),
						new PropertyUri(PropertyUriEnum.ClutterCount),
						new PropertyUri(PropertyUriEnum.UnreadClutterCount),
						WellKnownProperties.WorkingSetSourcePartitionInternal
					}
				};
			}
		}

		private static ItemResponseShape ItemPartUniqueBody
		{
			get
			{
				ItemResponseShape itemResponseShape = WellKnownShapes.CreateExpandedShapeFromProperties<ItemResponseShape>(WellKnownShapes.CommonMailReadingPaneShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.UniqueBody),
					new PropertyUri(PropertyUriEnum.IsGroupEscalationMessage)
				});
				itemResponseShape.FlightedProperties = new Dictionary<string, PropertyPath[]>
				{
					{
						"Like",
						new PropertyPath[]
						{
							new PropertyUri(PropertyUriEnum.LikeCount),
							new PropertyUri(PropertyUriEnum.Likers)
						}
					},
					{
						"SuperSort",
						new PropertyPath[]
						{
							new PropertyUri(PropertyUriEnum.ReceivedOrRenewTime)
						}
					}
				};
				return itemResponseShape;
			}
		}

		private static ItemResponseShape ItemPartNormalizedBody
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ItemResponseShape>(WellKnownShapes.ItemPartUniqueBody, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.NormalizedBody)
				});
			}
		}

		private static ItemResponseShape ItemNormalizedBody
		{
			get
			{
				ItemResponseShape itemResponseShape = WellKnownShapes.CreateExpandedShapeFromProperties<ItemResponseShape>(WellKnownShapes.CommonMailReadingPaneShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.NormalizedBody),
					new PropertyUri(PropertyUriEnum.IsGroupEscalationMessage)
				});
				itemResponseShape.FlightedProperties = new Dictionary<string, PropertyPath[]>
				{
					{
						"SuperSort",
						new PropertyPath[]
						{
							new PropertyUri(PropertyUriEnum.ReceivedOrRenewTime)
						}
					}
				};
				return itemResponseShape;
			}
		}

		private static AttachmentResponseShape ItemAttachment
		{
			get
			{
				return WellKnownShapes.CreateItemAttachmentBaseResponseShape();
			}
		}

		private static List<PropertyPath> FindConversationBasePropertySet
		{
			get
			{
				return new List<PropertyPath>
				{
					new PropertyUri(PropertyUriEnum.ConversationGuidId),
					new PropertyUri(PropertyUriEnum.Topic),
					new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime),
					new PropertyUri(PropertyUriEnum.ConversationCategories),
					new PropertyUri(PropertyUriEnum.ConversationFlagStatus),
					new PropertyUri(PropertyUriEnum.ConversationHasAttachments),
					new PropertyUri(PropertyUriEnum.ConversationMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationGlobalMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationUnreadCount),
					new PropertyUri(PropertyUriEnum.ConversationGlobalUnreadCount),
					new PropertyUri(PropertyUriEnum.ConversationSize),
					new PropertyUri(PropertyUriEnum.ConversationItemClasses),
					new PropertyUri(PropertyUriEnum.ConversationImportance),
					new PropertyUri(PropertyUriEnum.ConversationItemIds),
					new PropertyUri(PropertyUriEnum.ConversationGlobalItemIds),
					new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime),
					new PropertyUri(PropertyUriEnum.ConversationInstanceKey),
					new PropertyUri(PropertyUriEnum.ConversationPreview),
					new PropertyUri(PropertyUriEnum.ConversationGlobalIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationDraftItemIds),
					new PropertyUri(PropertyUriEnum.ConversationHasIrm)
				};
			}
		}

		private static ConversationResponseShape FindConversationBaseResponseShape
		{
			get
			{
				return new ConversationResponseShape(ShapeEnum.IdOnly, WellKnownShapes.FindConversationBasePropertySet.ToArray())
				{
					FlightedProperties = new Dictionary<string, PropertyPath[]>
					{
						{
							"SuperSort",
							new PropertyPath[]
							{
								new PropertyUri(PropertyUriEnum.ConversationLastDeliveryOrRenewTime)
							}
						}
					}
				};
			}
		}

		private static ConversationResponseShape FindConversationNormalShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ConversationResponseShape>(WellKnownShapes.FindConversationBaseResponseShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationUniqueSenders)
				});
			}
		}

		private static ConversationResponseShape FindConversationSentItemsShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ConversationResponseShape>(WellKnownShapes.FindConversationBaseResponseShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationUniqueRecipients)
				});
			}
		}

		private static ConversationResponseShape FindConversationUberShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ConversationResponseShape>(WellKnownShapes.FindConversationBaseResponseShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationUniqueSenders),
					new PropertyUri(PropertyUriEnum.ConversationUniqueRecipients)
				});
			}
		}

		private static ConversationResponseShape InferenceFindConversationNormalShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ConversationResponseShape>(WellKnownShapes.FindConversationNormalShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationHasClutter)
				});
			}
		}

		private static ConversationResponseShape InferenceFindConversationUberShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ConversationResponseShape>(WellKnownShapes.FindConversationUberShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationHasClutter)
				});
			}
		}

		private static ItemResponseShape DiscoveryItemShape
		{
			get
			{
				return WellKnownShapes.CreateExpandedShapeFromProperties<ItemResponseShape>(WellKnownShapes.CommonMailReadingPaneEwsShape, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.NormalizedBody)
				});
			}
		}

		private static ConversationResponseShape GroupConversationListView
		{
			get
			{
				return new ConversationResponseShape(ShapeEnum.IdOnly, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ConversationGuidId),
					new PropertyUri(PropertyUriEnum.Topic),
					new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime),
					new PropertyUri(PropertyUriEnum.ConversationCategories),
					new PropertyUri(PropertyUriEnum.ConversationHasAttachments),
					new PropertyUri(PropertyUriEnum.ConversationMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationGlobalMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationItemClasses),
					new PropertyUri(PropertyUriEnum.ConversationImportance),
					new PropertyUri(PropertyUriEnum.ConversationItemIds),
					new PropertyUri(PropertyUriEnum.ConversationGlobalItemIds),
					new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime),
					new PropertyUri(PropertyUriEnum.ConversationInstanceKey),
					new PropertyUri(PropertyUriEnum.ConversationIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationGlobalIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationPreview),
					new PropertyUri(PropertyUriEnum.ConversationHasIrm),
					new PropertyUri(PropertyUriEnum.ConversationUniqueSenders)
				});
			}
		}

		private static ConversationResponseShape GroupConversationFeedView
		{
			get
			{
				ConversationResponseShape conversationResponseShape = new ConversationResponseShape();
				conversationResponseShape.BaseShape = ShapeEnum.IdOnly;
				List<PropertyPath> list = new List<PropertyPath>
				{
					new PropertyUri(PropertyUriEnum.ConversationGuidId),
					new PropertyUri(PropertyUriEnum.Topic),
					new PropertyUri(PropertyUriEnum.ConversationLastDeliveryTime),
					new PropertyUri(PropertyUriEnum.ConversationCategories),
					new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime),
					new PropertyUri(PropertyUriEnum.ConversationHasAttachments),
					new PropertyUri(PropertyUriEnum.ConversationMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationGlobalMessageCount),
					new PropertyUri(PropertyUriEnum.ConversationItemClasses),
					new PropertyUri(PropertyUriEnum.ConversationImportance),
					new PropertyUri(PropertyUriEnum.ConversationItemIds),
					new PropertyUri(PropertyUriEnum.ConversationGlobalItemIds),
					new PropertyUri(PropertyUriEnum.ConversationLastModifiedTime),
					new PropertyUri(PropertyUriEnum.ConversationInstanceKey),
					new PropertyUri(PropertyUriEnum.ConversationIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationGlobalIconIndex),
					new PropertyUri(PropertyUriEnum.ConversationHasIrm),
					new PropertyUri(PropertyUriEnum.ConversationUniqueSenders)
				};
				foreach (PropertyUriEnum uri in ConversationFeedLoader.ConversationFeedProperties)
				{
					list.Add(new PropertyUri(uri));
				}
				conversationResponseShape.AdditionalProperties = list.ToArray();
				return conversationResponseShape;
			}
		}

		private static ItemResponseShape EditableItems
		{
			get
			{
				return WellKnownShapes.CreateExpandedShape<ItemResponseShape>(WellKnownShapes.ItemPartNormalizedBody, new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.BccRecipients),
					new PropertyUri(PropertyUriEnum.Body)
				});
			}
		}

		private static ItemResponseShape MailCompose
		{
			get
			{
				return WellKnownShapes.CreateComposeResponseShape(false);
			}
		}

		private static ItemResponseShape MailListItem
		{
			get
			{
				return new ItemResponseShape
				{
					BaseShape = ShapeEnum.IdOnly,
					AdditionalProperties = new PropertyPath[]
					{
						new PropertyUri(PropertyUriEnum.ItemId),
						new PropertyUri(PropertyUriEnum.ItemParentId),
						new PropertyUri(PropertyUriEnum.ConversationId),
						new PropertyUri(PropertyUriEnum.Subject),
						WellKnownProperties.NormalizedSubject,
						new PropertyUri(PropertyUriEnum.Importance),
						new PropertyUri(PropertyUriEnum.Sensitivity),
						new PropertyUri(PropertyUriEnum.DateTimeReceived),
						new PropertyUri(PropertyUriEnum.DateTimeCreated),
						new PropertyUri(PropertyUriEnum.DateTimeSent),
						new PropertyUri(PropertyUriEnum.ItemLastModifiedTime),
						new PropertyUri(PropertyUriEnum.HasAttachments),
						new PropertyUri(PropertyUriEnum.IsDraft),
						new PropertyUri(PropertyUriEnum.ItemClass),
						new PropertyUri(PropertyUriEnum.From),
						new PropertyUri(PropertyUriEnum.Sender),
						new PropertyUri(PropertyUriEnum.InstanceKey),
						new PropertyUri(PropertyUriEnum.Size),
						new PropertyUri(PropertyUriEnum.IsRead),
						new PropertyUri(PropertyUriEnum.Flag),
						new PropertyUri(PropertyUriEnum.Preview),
						new PropertyUri(PropertyUriEnum.ItemPolicyTag),
						new PropertyUri(PropertyUriEnum.ItemArchiveTag),
						WellKnownProperties.RetentionPeriod,
						WellKnownProperties.ArchivePeriod,
						WellKnownProperties.SharingInstanceGuid,
						new PropertyUri(PropertyUriEnum.DisplayTo),
						new PropertyUri(PropertyUriEnum.Categories),
						new PropertyUri(PropertyUriEnum.IsDeliveryReceiptRequested),
						new PropertyUri(PropertyUriEnum.IsReadReceiptRequested),
						new PropertyUri(PropertyUriEnum.TaskStatus),
						new PropertyUri(PropertyUriEnum.IconIndex),
						WellKnownProperties.LastVerbExecuted,
						WellKnownProperties.LastVerbExecutionTime
					},
					FlightedProperties = new Dictionary<string, PropertyPath[]>
					{
						{
							"SuperSort",
							new PropertyPath[]
							{
								new PropertyUri(PropertyUriEnum.ReceivedOrRenewTime)
							}
						}
					}
				};
			}
		}

		private static ItemResponseShape TaskListItem
		{
			get
			{
				return new ItemResponseShape
				{
					BaseShape = ShapeEnum.IdOnly,
					AdditionalProperties = new PropertyPath[]
					{
						new PropertyUri(PropertyUriEnum.ItemId),
						new PropertyUri(PropertyUriEnum.ItemParentId),
						new PropertyUri(PropertyUriEnum.InstanceKey),
						new PropertyUri(PropertyUriEnum.Subject),
						new PropertyUri(PropertyUriEnum.Importance),
						new PropertyUri(PropertyUriEnum.Sensitivity),
						new PropertyUri(PropertyUriEnum.HasAttachments),
						new PropertyUri(PropertyUriEnum.ItemClass),
						new PropertyUri(PropertyUriEnum.IsRead),
						new PropertyUri(PropertyUriEnum.Flag),
						new PropertyUri(PropertyUriEnum.TaskDoItTime),
						new PropertyUri(PropertyUriEnum.TaskDueDate),
						new PropertyUri(PropertyUriEnum.TaskIsComplete),
						new PropertyUri(PropertyUriEnum.TaskIsTaskRecurring),
						new PropertyUri(PropertyUriEnum.TaskStatus),
						new PropertyUri(PropertyUriEnum.TaskCompleteDate),
						new PropertyUri(PropertyUriEnum.TaskStartDate),
						new PropertyUri(PropertyUriEnum.TaskDelegationState),
						new PropertyUri(PropertyUriEnum.Categories)
					}
				};
			}
		}

		private static ItemResponseShape MessageDetails
		{
			get
			{
				return new ItemResponseShape
				{
					BaseShape = ShapeEnum.IdOnly,
					AdditionalProperties = new PropertyPath[]
					{
						WellKnownProperties.InternetMessageHeaders
					}
				};
			}
		}

		private static PropertyPath[] CommonMailReadingPaneShape
		{
			get
			{
				List<PropertyPath> list = new List<PropertyPath>(WellKnownShapes.CommonMailReadingPaneEwsShape);
				list.AddRange(new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ModernReminders),
					new PropertyUri(PropertyUriEnum.RecipientCounts),
					new PropertyUri(PropertyUriEnum.AttendeeCounts)
				});
				return list.ToArray();
			}
		}

		private static PropertyPath[] CommonMailReadingPaneEwsShape
		{
			get
			{
				List<PropertyPath> list = new List<PropertyPath>(WellKnownShapes.MailListItem.AdditionalProperties);
				list.AddRange(new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.Attachments),
					new PropertyUri(PropertyUriEnum.ToRecipients),
					new PropertyUri(PropertyUriEnum.CcRecipients),
					new PropertyUri(PropertyUriEnum.BccRecipients),
					new PropertyUri(PropertyUriEnum.ResponseObjects),
					new PropertyUri(PropertyUriEnum.Start),
					new PropertyUri(PropertyUriEnum.StartWallClock),
					new PropertyUri(PropertyUriEnum.StartTimeZoneId),
					new PropertyUri(PropertyUriEnum.End),
					new PropertyUri(PropertyUriEnum.EndWallClock),
					new PropertyUri(PropertyUriEnum.EndTimeZoneId),
					WellKnownProperties.Location,
					new PropertyUri(PropertyUriEnum.MeetingRequestType),
					new PropertyUri(PropertyUriEnum.ChangeHighlights),
					new PropertyUri(PropertyUriEnum.Recurrence),
					new PropertyUri(PropertyUriEnum.RecurrenceId),
					new PropertyUri(PropertyUriEnum.ResponseType),
					new PropertyUri(PropertyUriEnum.IsResponseRequested),
					new PropertyUri(PropertyUriEnum.IsCancelled),
					new PropertyUri(PropertyUriEnum.AssociatedCalendarItemId),
					new PropertyUri(PropertyUriEnum.IsOutOfDate),
					new PropertyUri(PropertyUriEnum.IsDelegated),
					new PropertyUri(PropertyUriEnum.IsRecurring),
					new PropertyUri(PropertyUriEnum.IntendedFreeBusyStatus),
					WellKnownProperties.VoiceMessageAttachmentOrder,
					WellKnownProperties.PstnCallbackTelephoneNumber,
					WellKnownProperties.VoiceMessageDuration,
					new PropertyUri(PropertyUriEnum.EntityExtractionResult),
					new PropertyUri(PropertyUriEnum.RequiredAttendees),
					new PropertyUri(PropertyUriEnum.OptionalAttendees),
					new PropertyUri(PropertyUriEnum.CalendarItemType),
					new PropertyUri(PropertyUriEnum.InternetMessageId),
					new PropertyUri(PropertyUriEnum.Organizer),
					WellKnownProperties.IsClassified,
					WellKnownProperties.ClassificationGuid,
					WellKnownProperties.Classification,
					WellKnownProperties.ClassificationDescription,
					WellKnownProperties.ClassificationKeep,
					new PropertyUri(PropertyUriEnum.RightsManagementLicenseData),
					new PropertyUri(PropertyUriEnum.ItemRetentionDate),
					new PropertyUri(PropertyUriEnum.BlockStatus),
					new PropertyUri(PropertyUriEnum.HasBlockedImages),
					WellKnownProperties.MessageBccMe,
					new PropertyUri(PropertyUriEnum.ReplyTo),
					new PropertyUri(PropertyUriEnum.ProposedStart),
					new PropertyUri(PropertyUriEnum.ProposedEnd),
					WellKnownProperties.NativeBodyInfo,
					new PropertyUri(PropertyUriEnum.IsOrganizer),
					new PropertyUri(PropertyUriEnum.ReceivedRepresenting),
					new PropertyUri(PropertyUriEnum.ApprovalRequestData),
					new PropertyUri(PropertyUriEnum.VotingInformation),
					new PropertyUri(PropertyUriEnum.IsClutter),
					new PropertyUri(PropertyUriEnum.ReminderMessageData),
					WellKnownProperties.DocumentId
				});
				return list.ToArray();
			}
		}

		private static ItemResponseShape FullCalendarItem
		{
			get
			{
				return WellKnownShapes.CreateFullCalendarItemResponseShape();
			}
		}

		private static ItemResponseShape MailComposeNormalizedBody
		{
			get
			{
				return WellKnownShapes.CreateComposeResponseShape(true);
			}
		}

		private static ItemResponseShape QuickComposeItemPart
		{
			get
			{
				return WellKnownShapes.CreateExpandedShapeFromProperties<ItemResponseShape>(WellKnownShapes.CommonMailReadingPaneShape, new PropertyPath[0]);
			}
		}

		internal static void SetDefaultsOnItemResponseShape(ItemResponseShape shape, LayoutType layout, OwaUserConfiguration owaUserConfiguration = null)
		{
			bool flag = owaUserConfiguration == null;
			if (owaUserConfiguration != null && owaUserConfiguration.ApplicationSettings.FilterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter)
			{
				shape.FilterHtmlContent = false;
				shape.BlockExternalImagesIfSenderUntrusted = false;
			}
			else
			{
				shape.FilterHtmlContent = true;
				if (flag)
				{
					shape.BlockExternalImages = true;
				}
				else
				{
					shape.BlockExternalImagesIfSenderUntrusted = true;
				}
			}
			if (owaUserConfiguration != null && owaUserConfiguration.SegmentationSettings.PredictedActions)
			{
				shape.InferenceEnabled = true;
			}
			shape.AddBlankTargetToLinks = true;
			shape.ClientSupportsIrm = !flag;
			shape.MaximumBodySize = ((layout == LayoutType.Mouse) ? 2097152 : 51200);
			shape.MaximumRecipientsToReturn = ((layout == LayoutType.Mouse) ? 10 : 0);
			if (!flag)
			{
				shape.InlineImageUrlTemplate = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAEALAAAAAABAAEAAAIBTAA7";
				shape.InlineImageUrlOnLoadTemplate = "InlineImageLoader.GetLoader().Load(this)";
				shape.InlineImageCustomDataTemplate = "{id}";
			}
		}

		private static T CreateExpandedShape<T>(T baseShape, PropertyPath[] expandedProperties) where T : ResponseShape, new()
		{
			T result = WellKnownShapes.CreateExpandedShapeFromProperties<T>(baseShape.AdditionalProperties, expandedProperties);
			result.FlightedProperties = new Dictionary<string, PropertyPath[]>(baseShape.FlightedProperties);
			return result;
		}

		private static T CreateExpandedShapeFromProperties<T>(PropertyPath[] propertiesToClone, PropertyPath[] additionalProperties) where T : ResponseShape, new()
		{
			T result = Activator.CreateInstance<T>();
			result.BaseShape = ShapeEnum.IdOnly;
			List<PropertyPath> list = new List<PropertyPath>(propertiesToClone);
			list.AddRange(additionalProperties);
			result.AdditionalProperties = list.ToArray();
			return result;
		}

		private static ItemResponseShape CreateComposeResponseShape(bool isNormalized)
		{
			return new ItemResponseShape
			{
				BaseShape = ShapeEnum.IdOnly,
				AdditionalProperties = new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ItemClass),
					new PropertyUri(PropertyUriEnum.ItemParentId),
					new PropertyUri(PropertyUriEnum.ToRecipients),
					new PropertyUri(PropertyUriEnum.CcRecipients),
					new PropertyUri(PropertyUriEnum.BccRecipients),
					new PropertyUri(PropertyUriEnum.From),
					new PropertyUri(PropertyUriEnum.Sender),
					new PropertyUri(PropertyUriEnum.ReplyTo),
					isNormalized ? new PropertyUri(PropertyUriEnum.NormalizedBody) : new PropertyUri(PropertyUriEnum.Body),
					new PropertyUri(PropertyUriEnum.Subject),
					WellKnownProperties.NormalizedSubject,
					new PropertyUri(PropertyUriEnum.Importance),
					new PropertyUri(PropertyUriEnum.Attachments),
					new PropertyUri(PropertyUriEnum.Start),
					new PropertyUri(PropertyUriEnum.End),
					new PropertyUri(PropertyUriEnum.ResponseType),
					new PropertyUri(PropertyUriEnum.Recurrence),
					new PropertyUri(PropertyUriEnum.IsDraft),
					new PropertyUri(PropertyUriEnum.ConversationId),
					WellKnownProperties.IsClassified,
					WellKnownProperties.ClassificationGuid,
					WellKnownProperties.Classification,
					WellKnownProperties.ClassificationDescription,
					WellKnownProperties.ClassificationKeep,
					new PropertyUri(PropertyUriEnum.RightsManagementLicenseData),
					new PropertyUri(PropertyUriEnum.Sensitivity),
					new PropertyUri(PropertyUriEnum.IsDeliveryReceiptRequested),
					new PropertyUri(PropertyUriEnum.IsReadReceiptRequested),
					WellKnownProperties.NativeBodyInfo,
					WellKnownProperties.Location,
					new PropertyUri(PropertyUriEnum.IsGroupEscalationMessage)
				},
				ShouldUseNarrowGapForPTagHtmlToTextConversion = true
			};
		}

		private static AttachmentResponseShape CreateItemAttachmentBaseResponseShape()
		{
			AttachmentResponseShape attachmentResponseShape = new AttachmentResponseShape
			{
				BaseShape = ShapeEnum.IdOnly
			};
			List<PropertyPath> list = new List<PropertyPath>();
			list.AddRange(WellKnownShapes.GetItemAttachmentAdditionalProperties());
			list.AddRange(WellKnownShapes.GetFullCalendarItemAdditionalProperties());
			attachmentResponseShape.AdditionalProperties = list.ToArray();
			return attachmentResponseShape;
		}

		private static PropertyPath[] GetItemAttachmentAdditionalProperties()
		{
			return new PropertyPath[]
			{
				new PropertyUri(PropertyUriEnum.InstanceKey),
				new PropertyUri(PropertyUriEnum.NormalizedBody),
				new PropertyUri(PropertyUriEnum.ItemParentId),
				new PropertyUri(PropertyUriEnum.Attachments),
				new PropertyUri(PropertyUriEnum.Subject),
				new PropertyUri(PropertyUriEnum.Importance),
				new PropertyUri(PropertyUriEnum.Sensitivity),
				new PropertyUri(PropertyUriEnum.DateTimeCreated),
				new PropertyUri(PropertyUriEnum.DateTimeReceived),
				new PropertyUri(PropertyUriEnum.HasAttachments),
				new PropertyUri(PropertyUriEnum.IsDraft),
				new PropertyUri(PropertyUriEnum.ItemClass),
				new PropertyUri(PropertyUriEnum.From),
				new PropertyUri(PropertyUriEnum.Sender),
				new PropertyUri(PropertyUriEnum.ToRecipients),
				new PropertyUri(PropertyUriEnum.CcRecipients),
				new PropertyUri(PropertyUriEnum.BccRecipients),
				new PropertyUri(PropertyUriEnum.DisplayTo),
				new PropertyUri(PropertyUriEnum.IsRead),
				new PropertyUri(PropertyUriEnum.ConversationId),
				new PropertyUri(PropertyUriEnum.ConversationIndex),
				new PropertyUri(PropertyUriEnum.ResponseObjects),
				new PropertyUri(PropertyUriEnum.Start),
				new PropertyUri(PropertyUriEnum.StartWallClock),
				new PropertyUri(PropertyUriEnum.StartTimeZoneId),
				new PropertyUri(PropertyUriEnum.End),
				new PropertyUri(PropertyUriEnum.EndWallClock),
				new PropertyUri(PropertyUriEnum.EndTimeZoneId),
				new PropertyUri(PropertyUriEnum.MeetingRequestType),
				new PropertyUri(PropertyUriEnum.ChangeHighlights),
				new PropertyUri(PropertyUriEnum.Recurrence),
				new PropertyUri(PropertyUriEnum.RecurrenceId),
				new PropertyUri(PropertyUriEnum.ResponseType),
				new PropertyUri(PropertyUriEnum.IsResponseRequested),
				new PropertyUri(PropertyUriEnum.AssociatedCalendarItemId),
				new PropertyUri(PropertyUriEnum.IsOutOfDate),
				new PropertyUri(PropertyUriEnum.IsDelegated),
				new PropertyUri(PropertyUriEnum.IsRecurring),
				new PropertyUri(PropertyUriEnum.IntendedFreeBusyStatus),
				new PropertyUri(PropertyUriEnum.IsClutter),
				new PropertyUri(PropertyUriEnum.Preview),
				WellKnownProperties.VoiceMessageAttachmentOrder,
				WellKnownProperties.PstnCallbackTelephoneNumber,
				WellKnownProperties.VoiceMessageDuration,
				new PropertyUri(PropertyUriEnum.Flag),
				WellKnownProperties.NormalizedSubject,
				new PropertyUri(PropertyUriEnum.EntityExtractionResult),
				new PropertyUri(PropertyUriEnum.ItemLastModifiedTime),
				new PropertyUri(PropertyUriEnum.RequiredAttendees),
				new PropertyUri(PropertyUriEnum.OptionalAttendees),
				new PropertyUri(PropertyUriEnum.CalendarItemType),
				new PropertyUri(PropertyUriEnum.InternetMessageId),
				new PropertyUri(PropertyUriEnum.Organizer),
				WellKnownProperties.IsClassified,
				WellKnownProperties.ClassificationGuid,
				WellKnownProperties.Classification,
				WellKnownProperties.ClassificationDescription,
				WellKnownProperties.ClassificationKeep,
				WellKnownProperties.SharingInstanceGuid,
				new PropertyUri(PropertyUriEnum.RightsManagementLicenseData),
				new PropertyUri(PropertyUriEnum.Categories),
				new PropertyUri(PropertyUriEnum.BlockStatus),
				new PropertyUri(PropertyUriEnum.HasBlockedImages),
				WellKnownProperties.MessageBccMe,
				new PropertyUri(PropertyUriEnum.ReplyTo),
				new PropertyUri(PropertyUriEnum.ProposedStart),
				new PropertyUri(PropertyUriEnum.ProposedEnd),
				WellKnownProperties.NativeBodyInfo,
				WellKnownProperties.Location,
				new PropertyUri(PropertyUriEnum.IconIndex),
				new PropertyUri(PropertyUriEnum.TaskDueDate),
				new PropertyUri(PropertyUriEnum.TaskStartDate),
				new PropertyUri(PropertyUriEnum.TaskStatus),
				new PropertyUri(PropertyUriEnum.TaskCompleteDate),
				new PropertyUri(PropertyUriEnum.TaskPercentComplete),
				new PropertyUri(PropertyUriEnum.TaskOwner),
				new PropertyUri(PropertyUriEnum.TaskTotalWork),
				new PropertyUri(PropertyUriEnum.TaskActualWork),
				new PropertyUri(PropertyUriEnum.TaskMileage),
				new PropertyUri(PropertyUriEnum.TaskBillingInformation),
				new PropertyUri(PropertyUriEnum.TaskCompanies),
				new PropertyUri(PropertyUriEnum.TaskRecurrence),
				new PropertyUri(PropertyUriEnum.TaskIsComplete),
				new PropertyUri(PropertyUriEnum.TaskIsTaskRecurring),
				new PropertyUri(PropertyUriEnum.ReminderIsSet),
				new PropertyUri(PropertyUriEnum.ReminderDueBy)
			};
		}

		private static PropertyPath[] GetFullCalendarItemAdditionalProperties()
		{
			return new PropertyPath[]
			{
				new PropertyUri(PropertyUriEnum.UID),
				new PropertyUri(PropertyUriEnum.ItemParentId),
				new PropertyUri(PropertyUriEnum.Sensitivity),
				new PropertyUri(PropertyUriEnum.IsCancelled),
				new PropertyUri(PropertyUriEnum.IsSeriesCancelled),
				new PropertyUri(PropertyUriEnum.AppointmentState),
				new PropertyUri(PropertyUriEnum.LegacyFreeBusyStatus),
				new PropertyUri(PropertyUriEnum.IntendedFreeBusyStatus),
				new PropertyUri(PropertyUriEnum.CalendarItemType),
				new PropertyUri(PropertyUriEnum.Start),
				new PropertyUri(PropertyUriEnum.StartWallClock),
				new PropertyUri(PropertyUriEnum.StartTimeZoneId),
				new PropertyUri(PropertyUriEnum.End),
				new PropertyUri(PropertyUriEnum.EndWallClock),
				new PropertyUri(PropertyUriEnum.EndTimeZoneId),
				new PropertyUri(PropertyUriEnum.MyResponseType),
				new PropertyUri(PropertyUriEnum.IsAllDayEvent),
				new PropertyUri(PropertyUriEnum.Organizer),
				new PropertyUri(PropertyUriEnum.RequiredAttendees),
				new PropertyUri(PropertyUriEnum.OptionalAttendees),
				new PropertyUri(PropertyUriEnum.Resources),
				new PropertyUri(PropertyUriEnum.AppointmentReplyTime),
				new PropertyUri(PropertyUriEnum.AppointmentReplyName),
				new PropertyUri(PropertyUriEnum.Subject),
				new PropertyUri(PropertyUriEnum.HasAttachments),
				new PropertyUri(PropertyUriEnum.Attachments),
				new PropertyUri(PropertyUriEnum.Body),
				new PropertyUri(PropertyUriEnum.BlockStatus),
				new PropertyUri(PropertyUriEnum.HasBlockedImages),
				new PropertyUri(PropertyUriEnum.DateTimeReceived),
				new PropertyUri(PropertyUriEnum.Recurrence),
				new PropertyUri(PropertyUriEnum.ReminderIsSet),
				new PropertyUri(PropertyUriEnum.ReminderMinutesBeforeStart),
				new PropertyUri(PropertyUriEnum.CalendarIsResponseRequested),
				new PropertyUri(PropertyUriEnum.ItemClass),
				new PropertyUri(PropertyUriEnum.ItemLastModifiedTime),
				new PropertyUri(PropertyUriEnum.IsMeeting),
				new PropertyUri(PropertyUriEnum.DateTimeCreated),
				new PropertyUri(PropertyUriEnum.EntityExtractionResult),
				new PropertyUri(PropertyUriEnum.InstanceKey),
				new PropertyUri(PropertyUriEnum.ItemEffectiveRights),
				new PropertyUri(PropertyUriEnum.Categories),
				new PropertyUri(PropertyUriEnum.JoinOnlineMeetingUrl),
				new PropertyUri(PropertyUriEnum.OnlineMeetingSettings),
				new PropertyUri(PropertyUriEnum.ConversationId),
				new PropertyUri(PropertyUriEnum.IsRecurring),
				new PropertyUri(PropertyUriEnum.IsOrganizer),
				new PropertyUri(PropertyUriEnum.Preview),
				new PropertyUri(PropertyUriEnum.InboxReminders),
				WellKnownProperties.NormalizedSubject,
				WellKnownProperties.NativeBodyInfo,
				WellKnownProperties.Location
			};
		}

		private static ItemResponseShape CreateFullCalendarItemResponseShape()
		{
			return new ItemResponseShape
			{
				BaseShape = ShapeEnum.IdOnly,
				AddBlankTargetToLinks = true,
				FilterHtmlContent = true,
				InlineImageUrlTemplate = "/service.svc/s/GetFileAttachment",
				AdditionalProperties = WellKnownShapes.GetFullCalendarItemAdditionalProperties()
			};
		}

		public const int MaximumBodySizeForMouseLayout = 2097152;

		public const int MaximumBodySizeForTouchLayout = 51200;

		public const int MaximumRecipientsToReturnForMouseLayout = 10;

		public const int NoRecipientsTruncation = 0;

		public const int MaxInitialItemPartsPerConversation = 20;

		public const string TypeName = "InlineImageLoader";

		private const string PlaceHolderUri = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAEALAAAAAABAAEAAAIBTAA7";

		private const string OnLoadTemplate = "InlineImageLoader.GetLoader().Load(this)";

		private const string CustomDataTemplate = "{id}";

		private const string CssScopeNamePrefix = "rps_";

		private static LazyMember<Dictionary<WellKnownShapeName, ResponseShape>> responseShapes = new LazyMember<Dictionary<WellKnownShapeName, ResponseShape>>(() => new Dictionary<WellKnownShapeName, ResponseShape>
		{
			{
				WellKnownShapeName.ConversationListView,
				WellKnownShapes.FindConversationNormalShape
			},
			{
				WellKnownShapeName.ConversationSentItemsListView,
				WellKnownShapes.FindConversationSentItemsShape
			},
			{
				WellKnownShapeName.ConversationUberListView,
				WellKnownShapes.FindConversationUberShape
			},
			{
				WellKnownShapeName.ItemNormalizedBody,
				WellKnownShapes.ItemNormalizedBody
			},
			{
				WellKnownShapeName.EditableItems,
				WellKnownShapes.EditableItems
			},
			{
				WellKnownShapeName.Folder,
				WellKnownShapes.Folder
			},
			{
				WellKnownShapeName.ItemAttachment,
				WellKnownShapes.ItemAttachment
			},
			{
				WellKnownShapeName.ItemPartNormalizedBody,
				WellKnownShapes.ItemPartNormalizedBody
			},
			{
				WellKnownShapeName.ItemPartUniqueBody,
				WellKnownShapes.ItemPartUniqueBody
			},
			{
				WellKnownShapeName.MailCompose,
				WellKnownShapes.MailCompose
			},
			{
				WellKnownShapeName.MailListItem,
				WellKnownShapes.MailListItem
			},
			{
				WellKnownShapeName.MessageDetails,
				WellKnownShapes.MessageDetails
			},
			{
				WellKnownShapeName.TaskListItem,
				WellKnownShapes.TaskListItem
			},
			{
				WellKnownShapeName.FullCalendarItem,
				WellKnownShapes.FullCalendarItem
			},
			{
				WellKnownShapeName.MailComposeNormalizedBody,
				WellKnownShapes.MailComposeNormalizedBody
			},
			{
				WellKnownShapeName.QuickComposeItemPart,
				WellKnownShapes.QuickComposeItemPart
			},
			{
				WellKnownShapeName.GroupConversationListView,
				WellKnownShapes.GroupConversationListView
			},
			{
				WellKnownShapeName.GroupConversationFeedView,
				WellKnownShapes.GroupConversationFeedView
			},
			{
				WellKnownShapeName.InferenceConversationListView,
				WellKnownShapes.InferenceFindConversationNormalShape
			},
			{
				WellKnownShapeName.InferenceConversationUberListView,
				WellKnownShapes.InferenceFindConversationUberShape
			},
			{
				WellKnownShapeName.DiscoveryItem,
				WellKnownShapes.DiscoveryItemShape
			}
		});

		private static readonly List<DistinguishedFolderIdName> requiredDistinguishedFolders = new List<DistinguishedFolderIdName>
		{
			DistinguishedFolderIdName.deleteditems,
			DistinguishedFolderIdName.drafts,
			DistinguishedFolderIdName.inbox,
			DistinguishedFolderIdName.junkemail,
			DistinguishedFolderIdName.notes,
			DistinguishedFolderIdName.sentitems
		};

		private static readonly List<DistinguishedFolderIdName> foldersToMoveToTop = new List<DistinguishedFolderIdName>
		{
			DistinguishedFolderIdName.inbox,
			DistinguishedFolderIdName.drafts,
			DistinguishedFolderIdName.sentitems,
			DistinguishedFolderIdName.deleteditems
		};
	}
}

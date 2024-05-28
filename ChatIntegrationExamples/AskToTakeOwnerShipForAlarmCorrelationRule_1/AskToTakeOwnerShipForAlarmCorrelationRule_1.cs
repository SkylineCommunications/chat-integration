using System.Text.RegularExpressions;
using AdaptiveCards;
using Skyline.DataMiner.DcpChatIntegrationHelper.Common;
using Skyline.DataMiner.DcpChatIntegrationHelper.Internal.Common;
using Skyline.DataMiner.DcpChatIntegrationHelper.Teams;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Net.Enums;
using Skyline.DataMiner.Net.Maps;
using Skyline.DataMiner.Net.Messages;

namespace AskToTakeOwnerShipForAlarmCorrelationRule_1
{
	using System.Collections.Generic;
	using System;
	using Skyline.DataMiner.Automation;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			var chatIntegrationHelper = new ChatIntegrationHelperBuilder().Build();
			try
			{
				var teamIdParam = engine.GetScriptParam("Team ID");
				if (string.IsNullOrWhiteSpace(teamIdParam?.Value))
				{
					engine.ExitFail("'Team ID' parameter is required.");
					return;
				}

				var channelIdParam = engine.GetScriptParam("Channel ID");
				if (string.IsNullOrWhiteSpace(channelIdParam?.Value))
				{
					engine.ExitFail("'Channel ID' parameter is required.");
					return;
				}

				// See https://docs.dataminer.services/user-guide/Advanced_Modules/Automation_module/Special_parameters_available_in_DMS_Automation_scripts.html
				// See https://docs.dataminer.services/user-guide/Advanced_Modules/Automation_module/FAQ/How_do_I_parse_Correlation_Alarm_Info_data.html
				var correlationAlarmInfo = engine.GetScriptParam(65006);
				if (string.IsNullOrWhiteSpace(correlationAlarmInfo?.Value))
				{
					engine.ExitFail("This script can only be used as a correlation rule triggered by an alarm.");
					return;
				}
				var alarmInfo = correlationAlarmInfo.Value;
				engine.GenerateInformation(alarmInfo);
				var parts = alarmInfo.Split('|');
				var alarmId = Tools.ToInt32(parts[0]);
				var dmaId = Tools.ToInt32(parts[1]);
				var elementId = Tools.ToInt32(parts[2]);
				var parameterId = Tools.ToInt32(parts[3]);
				var severity = Tools.ToInt32(parts[7]);
				var type = Tools.ToInt32(parts[8]);
				var status = Tools.ToInt32(parts[9]);
				var alarmValue = parts[10];
				var alarmTime = DateTime.Parse(parts[11]);
				var owner = parts[18];

				var element = engine.FindElement(dmaId, elementId);
				var parameterName = element.Protocol.GetParameterName(parameterId);

				// Cloud identity
				// You can find these IDs by opening the DMS overview page on admin.dataminer.services
				// Example: https://admin.dataminer.services/5d8ce07f-5b73-4135-b5a2-2cf4129912c6/dms/311e8ee6-7f7e-4020-b9ae-e43356a18e28/overview
				// var organizationId = Guid.Parse("5d8ce07f-5b73-4135-b5a2-2cf4129912c6");
				// var dmsId = Guid.Parse("311e8ee6-7f7e-4020-b9ae-e43356a18e28");
				var organizationId = Guid.Parse("");
				var dmsId = Guid.Parse("");

				// Parse the name of the enum value name as display name
				var typeDisplayName = string.Join(" ", Regex.Split(((SLEnumValues)type).ToString(), @"(?<!^)(?=[A-Z])"));
				var facts = new List<AdaptiveFact>
				{
					new AdaptiveFact("Alarm ID:", alarmId.ToString()),
					new AdaptiveFact("Type:", typeDisplayName),
					new AdaptiveFact("Severity:", ((EnumSeverity)severity).ToString()),
					new AdaptiveFact("Parameter name:", parameterName),
					new AdaptiveFact("Value:", alarmValue),
					new AdaptiveFact("Status:", ((AlarmStatus)status).ToString()),
					new AdaptiveFact("DataMiner time:", alarmTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC"),
					new AdaptiveFact("Owner:", owner),
				};

				var adaptiveCardBody = new List<AdaptiveElement>()
				{
					new AdaptiveFactSet()
					{
						Facts = facts
					},
					new AdaptiveActionSet()
					{
						Actions = new List<AdaptiveAction>()
						{
							// Example custom command from https://github.com/SkylineCommunications/ChatOps-Extensions/tree/main/CustomCommandExamples#input-parameter
							AdaptiveCardHelper.Buttons.RunCustomCommand(
								"Take Ownership of Alarm",
								dmaId,
								organizationId,
								dmsId,
								new List<CustomCommandInput>()
								{
									new CustomCommandInput("DataMiner ID", CustomCommandInputType.Parameter, dmaId.ToString()),
									new CustomCommandInput("Element ID", CustomCommandInputType.Parameter, elementId.ToString()),
									new CustomCommandInput("Alarm ID", CustomCommandInputType.Parameter, alarmId.ToString()),
								},
								"Take ownership",
								skipConfirmation: true),
							AdaptiveCardHelper.Buttons.GetElement(elementId, dmaId, organizationId, dmsId, $"Show {element.ElementName}"),
							AdaptiveCardHelper.Buttons.GetAlarmsForElement(elementId, dmaId, element.ElementName, organizationId, dmsId, $"Show all alarms on {element.ElementName}"),
						}
					}
				};

				try
				{
					chatIntegrationHelper.Teams.TrySendChannelNotification(teamIdParam.Value, channelIdParam.Value, adaptiveCardBody);
				}
				catch (TeamsChatIntegrationException e)
				{
					engine.ExitFail($"Couldn't send the notification card to the channel with ID {channelIdParam.Value} with error {e.Message}.");
					return;
				}

				engine.ExitSuccess($"The notification card was sent to the channel with ID {channelIdParam.Value}!");
			}
			catch (ScriptAbortException)
			{
				// Also ExitSuccess is a ScriptAbortException
				throw;
			}
			catch (Exception e)
			{
				engine.ExitFail($"An exception occurred with the following message: {e.Message}");
			}
			finally
			{
				chatIntegrationHelper?.Dispose();
			}
		}
	}
}
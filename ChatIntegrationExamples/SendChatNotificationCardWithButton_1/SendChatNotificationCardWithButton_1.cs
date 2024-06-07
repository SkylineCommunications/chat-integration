using System;
using System.Collections.Generic;
using AdaptiveCards;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.DcpChatIntegrationHelper.Common;
using Skyline.DataMiner.DcpChatIntegrationHelper.Internal.Common;
using Skyline.DataMiner.DcpChatIntegrationHelper.Teams;
using Skyline.DataMiner.Net.Messages;

namespace SendChatNotificationCardWithButton_1
{
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
				var chatIdParam = engine.GetScriptParam("Chat ID");
				if (string.IsNullOrWhiteSpace(chatIdParam?.Value))
				{
					engine.ExitFail("'Chat ID' parameter is required.");
					return;
				}

				// Looking up the dataminerId
				var dataminerInfoResponse = engine.SendSLNetSingleResponseMessage(new GetInfoMessage(InfoType.DataMinerInfo));
				var dataminerInfo = (GetDataMinerInfoResponseMessage)dataminerInfoResponse;

				// Cloud identity
				var dmsIdentity = chatIntegrationHelper.GetDataMinerServicesDmsIdentity();

				var adaptiveCardBody = new List<AdaptiveElement>()
				{
					// Some additional examples
					new AdaptiveImage("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPoAAACMCAYAAABVl7ThAAABg2lDQ1BJQ0MgcHJvZmlsZQAAKM+VkTlIA0EYhT/jERXFwhQiFltEKxVREUuJoggKkkTwKtzdmChkN2E3wcZSsBUsPBqvwsZaWwtbQRA8QKwtrBRtRNZ/NkKCEMGBYT7ezHvMvIHAQdq03KoesOycEx2LaDOzc1rwmRqqaaSObt10s5Ox0Thlx8ctFWq96VJZ/G80JpZcEyo04SEz6+SEF4UHVnNZxTvCIXNZTwifCnc6ckHhe6UbBX5RnPI5oDJDTjw6LBwS1lIlbJSwuexYwv3C4YRlS35gpsAJxWuKrXTe/LmnemHDkj0dU7rMNsYYZ5IpNAzyrJAmR5estiguUdmPlPG3+v4pcRniWsEUxwgZLHTfj/qD3926yb7eQlJDBKqfPO+tHYJb8LXpeZ+Hnvd1BJWPcGEX/ZkDGHwXfbOohfehaR3OLouasQ3nG9DykNUd3ZcqZQaSSXg9kW+aheZrqJ8v9Pazz/EdxKWriSvY3YOOlGQvlHl3bWlvf57x+yPyDVlzcp0N15whAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAABmJLR0QA/wD/AP+gvaeTAAAAB3RJTUUH6AYFDRESJuX31gAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAACO5SURBVHhe7Z0J3FXT+sefBtVbSbMGGugvDTQoFdGniHQlUyRSLiVDAxlyqXDNdW/JRSSUEH1cZCqRK90mpVISmlRSkkYNb9P/+e39nPued599zl5rD+c9ddb389nvu9beZ9rnrGet53nWs55V6DBDBoPhqKaw/DcYDEcxRtANhizACLrBkAUYQTcYsgAj6AZDFmAE3WDIAoygGwxZgBF0gyELMIJuMGQBRtANhizACLrBkAUYQTcYsgAj6AZDFmAE3WDIAoygGwxZgBF0gyELiDTxxPfff0/Lli2jzZs307Zt26ho0aJUtmxZql69OtWvX59q1qwpjzQYDFESqqDv2rWLnnjiCXrllVdow4YNcjY1NWrUoKuvvpr69OlDJ510kpxNzRtvvEGPP/44lS5d2qpv2bKFvvrqKzr++OOtehDq1KlDlSpVssq7d++m66+/ngYOHGjVg7B27Vq64IILqFy5cnLGZuvWrfTFF19Q1apV5Yx/nnnmGRozZgyVKlVKztgcPHiQypQpQ5999pmc0efMM8+kQoUKUeHChWnnzp304IMP0pVXXilXkxP/fcazfft2mjBhAjVt2lTO6BHfBg4dOkTFihWz2oAXQ4cOpffff59ycnLkTDjs3buXLrroInrsscfkTCLnnHMO5ebmWt8h/jds2JDGjRsnVyMGgh4UHrEPt2jRAh1GoKNkyZKHR4wYIa+aHO5MEp7LgiRXg+F83V69esmVYHBHlvDasaN///7yqGD069fP9fVjxwcffCCP1Mf5WiNHjpQrqXE+L/6oWLGiPEoftzagwhVXXJHwvLCOTp06ybu443z8qaeeKleiJ7CN3rdvX6vHnjt3rpzxD0bQO+64wxo5hgwZImcTgQngBL1kFBxzzDFSCsbo0aOllMjTTz8tpWB4fdbOnTtLKThuv4Euv//+O02ZMkVqejjfv0iRIlJKDUb+qPB6bednjPKzOAkkHbC1//Wvf0ktEaijV111Fd19991033330T333EM33HADNW/eXB6RnGHDhknpyGfGjBlSSs68efOkFB1QcV9++WWpZQaXXHKJlI58YLpmKr5tdNh8sNXc+Mc//kF33nmn1JIDOx6j2VNPPSVn8qhQoYLV47uB17/rrrukZrN+/Xqr4wkKtIl4br31Vnr22Wel5g/YbvEjF6tstHz5cqnZXHrppfTuu+9KzR/4TvDdxDjjjDMs/8WaNWvkjI2fn9z5vaCDv+2226SWHOfzbrzxRho7dqzUbODT6dmzp9TUcLYBjJYHDhyQWnK6detGb775ptRsYCdDmwzC/v376ZRTTqELL7xQziQCLQT+khinn346LV68WGoRA0HXpWHDhvlsjdjBo7c8Qp8XXngh32uxoMuVRIYPH57vsThY0OVqMJyvy4IuV/zjfM1JkyYd/stf/pJwPigDBw7M93p169Y9vGTJknzncDz++OPyDHWcr8GCLldS43zexo0bD5cuXTrhvC7ONsCCLldSc8011+R7Ho50gc8Y/74s6HIlerRV95EjR9LSpUullgfOv/XWW1LTp3fv3tZIE7MjoWYeDbz22mtSygPe6ttvv11qebz99ttSCoc///zT8uxWrlxZztjAjCoooKVNnjxZanm4aXWG8NAWdDjLnNx///3Uv39/qQXjvffes6Y/MO10NPDoo49KyaZFixbW/w4dOlj/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")
					{
						PixelWidth = 200
					},
					new AdaptiveFactSet()
					{
						Facts = new List<AdaptiveFact>
						{
							new AdaptiveFact("Channel Name:", "Channel Ocho"),
							new AdaptiveFact("Service ID:", "5")
						},
					},
					new AdaptiveFactSet()
					{
						Facts = new List<AdaptiveFact>
						{
							new AdaptiveFact("Video Bitrate:", "1.643 Mbps"),
							new AdaptiveFact("Audio Bitrate:", "128 kbps")
						},
					},
					new AdaptiveFactSet()
					{
						Facts = new List<AdaptiveFact>
						{
							new AdaptiveFact("RF Status:", "Locked"),
							new AdaptiveFact("C/N:", "15.5 dB"),
							new AdaptiveFact("BER:", "0.000 E-6")
						},
					},
					new AdaptiveActionSet()
					{
						Actions = new List<AdaptiveAction>()
						{
							AdaptiveCardHelper.Buttons.ChangeDms("Click here to change the conversation's active DMS"),
							AdaptiveCardHelper.Buttons.OpenUrl(new Uri("https://dataminer.services"), "dataminer.services"),
							// Example custom command from https://github.com/SkylineCommunications/ChatOps-Extensions/tree/main/CustomCommandExamples#input-parameter
							AdaptiveCardHelper.Buttons.RunCustomCommand(
								"Parameter Input Example",
								dataminerInfo.ID,
								dmsIdentity.OrganizationId,
								dmsIdentity.DmsId,
								new List<CustomCommandInput>()
								{
									new CustomCommandInput("Parameter Input", CustomCommandInputType.Parameter, "The overwritten default value"),
								},
								"Run Example Custom Command",
								skipConfirmation: false),
							AdaptiveCardHelper.Buttons.GetElement(1, dataminerInfo.ID, dmsIdentity.OrganizationId, dmsIdentity.DmsId, "Show element 1"),
							AdaptiveCardHelper.Buttons.GetAlarmsForElement(1, dataminerInfo.ID, "Fill in element name here", dmsIdentity.OrganizationId, dmsIdentity.DmsId, "Show alarms for element 1"),
							AdaptiveCardHelper.Buttons.GetAlarmsForView(1, "Fill in view name here", dmsIdentity.OrganizationId, dmsIdentity.DmsId, "Show alarms for view 1")
						}
					}
				};

				try
				{
					chatIntegrationHelper.Teams.TrySendChatNotification(chatIdParam.Value, adaptiveCardBody);
				}
				catch (TeamsChatIntegrationException e)
				{
					engine.ExitFail($"Couldn't send the notification card to the chat with ID {chatIdParam.Value} with error {e.Message}.");
					return;
				}

				engine.ExitSuccess($"The notification card was sent to the chat with ID {chatIdParam.Value}!");
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
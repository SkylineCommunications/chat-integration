<a name="readme-top"></a>
# Chat Integration / Dataminer Bot

This guideline is about how to integrate Dataminer into business communication platforms with automation scripts.

## Overview

There are two main parts to the Chat Integration for Teams:
 * Adding the automation scripts & memory files to your Dataminer System.
 * Granting admin consent in the DCP admin app.

## Getting Started

### Prerequisites

* The DataMiner System that integrates with chats must be connected to the DataMiner Cloud Platform ([DCP](https://docs.dataminer.services/user-guide/Cloud_Platform/CloudAdminApp/Managing_cloud-connected_nodes.html)).
* The [CloudGateway DxM](https://docs.dataminer.services/user-guide/Cloud_Platform/CloudAdminApp/Managing_cloud-connected_nodes.html) on your Dataminer System must be atleast version 2.9.0.
* The Dataminer App must be allowed in your [Teams](https://docs.microsoft.com/en-us/microsoftteams/manage-apps).

### Installing

* Deploy the [DMAPP]() to your Dataminer system. Double-click to install, this will add the automation scripts in the Automation module.

* Access the [DataMiner](https://docs.dataminer.services/user-guide/Getting_started/Accessing_DataMiner/Accessing_DataMiner.html) System and open the Automation module (via apps > Automation).

![Items](https://user-images.githubusercontent.com/109528797/186420636-61efa334-6041-44df-9056-11e6cf44da78.png)

* Open the "memory files" tab and create memory files named Teams, Channels and Chats. The purpose of these files is to persist the created Teams, Channels and Chats.

![Items](https://user-images.githubusercontent.com/109528797/186420786-f9b95bca-ee82-49d6-901a-5a19c4c14f43.png)

* Also create a memory file named YesOrNo with the following entries.

![Items](https://user-images.githubusercontent.com/109528797/186423143-bab59820-8e45-4315-a192-24b34398b502.png)

* Connect your DataMiner System to the cloud if it is not yet [Cloud Connected](https://docs.dataminer.services/user-guide/Cloud_Platform/AboutCloudPlatform/Connecting_your_DataMiner_System_to_the_cloud.html).

* Grant access with [Admin Consent](https://docs.dataminer.services/user-guide/Cloud_Platform/CloudAdminApp/Granting_admin_consent.html).



### Usage

* Open the Automation module (via apps > Automation) and click on the "automation scripts" tab and double click the "Create Team" automation script. Fill in the fields for creating a new Team, **the fields are case sensitive.**
* When pressing the "execute now" button, a Team is created in Microsoft Teams with the Dataminer bot.

<img src="https://user-images.githubusercontent.com/109528797/186440501-4850222e-c503-40fc-8f9a-003e8c7d5a30.png" width="684" height="757">


## Help

Any advise for common problems or issues.

## Contact

[@Jordy Ampe](https://github.com/JordyGit)  
[@RobbertVH](https://github.com/RobbertVH)

Project Link: [chat-integration](https://github.com/SkylineCommunications/chat-integration)

## Version History

* 0.2
   * Refactoring
   * See [commit change]() or See [release history]()
* 1.0
   * Initial Release

## License

This project is licensed under the [MIT License](https://github.com/SkylineCommunications/chat-integration/blob/main/LICENSE) - see the file for details
<p align="right">(<a href="#readme-top">back to top</a>)</p>

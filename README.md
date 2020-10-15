# Jira Connect

This application queries Jira issues with calculated flow metrics and exports to a csv.

## Getting Started

### Configuring & Running

Jira Connect will need to be configured with the following:
#### 1. Update the JiraConfig section with connection data to Jira cloud, including your organizations URL and credentials (API key).
  - **BaseURL** (required): Specify your organization's URL for Jira Cloud and append the path to Jira Cloud's REST API. For example: https://{your-org}.atlassian.net/rest/api/3 (replace *your-org*)
  - **ApiKey** (required): Generate an API Key to be used to authenticate with Jira Cloud. Instructions can be found [here](https://confluence.atlassian.com/cloud/api-tokens-938839638.html).

#### 2. Configuring the data to query and the format of the output for one or many organizations/teams. Update the "Mapping" section, adding one entry for each team or organization representation in Jira:
  - **TeamName**: Specify how the "Team" column in the csv export will be populated.
  - **OutputFileName**: Specify the path and filename for the exported data.
  - **JiraQuery**: Specify the query to be used for this team/organization. The query will include the Jira issues returned from the query in the export.
  - **Enabled**: Determines whether this configured mapping is processed when running the application. This allows setting up a team configuration for future/periodic use.
  - **Workflow**: Provide details how the Jira states should be represented. This is the equivalent of configuring and mapping statuses on a Jira board. Configure the workflow for each state with the following:
    - **IssueType**: Specify what Jira Issue type for which this state is used, such as "Story" or "Bug"
    - **JiraStates**: Specify one or multiple statuses in Jira that should map to this state configuration.
    - **MappedState**: Determines the name of the state that will be displayed in the export and also used for displaying time-in-state calculations. Mapped states should be the same for each issue type (see example below).
    - **StateType**: Specify whether this state is a "ToDo", "InProgress", or "Done" state (similar to StatusCategory in Jira. This is used in calculating cycle times.
    - **Sequence**: Allows ordering of states from earliest to latest. Numbers should be unique for each state and issue type combination (i.e. "To Do", "Doing", "Done" configuration for an issue type of "Story" could have a sequence of 1, 2, and 3. The same for an issue type of bug can also have a sequence of 1, 2, 3).
  
  Here is an example of a completed appsettings.json file with a mapping for a team's board and an organization's Epic board:
  
    {
      "AppSettings": {
        "JiraConfig": {
          "ApiKey": "{place api key here}",
          "BaseUrl": "https://{your-org}.atlassian.net/rest/api/3"
        },
        "Mapping": [
          {
            "TeamName": "Engineering",
            "OutputFileName": "Engineering_Epics.csv",
            "JiraQuery": "project = ENG AND issuetype = 'Epic' AND statusCategory != 'To Do'",
            "Enabled": true,
            "Workflow": [
              {
                "IssueType": "Epic",
                "JiraStates": [ "ToDo" ],
                "MappedState": "To Do",
                "StateType": "ToDo",
                "Sequence": 1
              },
              {
                "IssueType": "Epic",
                "JiraStates": [ "InProgress" ],
                "MappedState": "Building",
                "StateType": "InProgress",
                "Sequence": 2
              },
              {
                "IssueType": "Epic",
                "JiraStates": [ "Done" ],
                "MappedState": "Delivered",
                "StateType": "Done",
                "Sequence": 3
              }
            ]
          },
          {
            "TeamName": "Awesome Sauce",
            "OutputFileName": "AwesomeSauceTeam_Issues.csv",
            "JiraQuery": "project = AWS AND issuetype IN('Story','Bug') AND statusCategory != 'To Do'",
            "Enabled": true,
            "Workflow": [
              {
                "IssueType": "Bug",
                "JiraStates": [ "New" ],
                "MappedState": "To Do",
                "StateType": "ToDo",
                "Sequence": 1
              },
              {
                "IssueType": "Bug",
                "JiraStates": [ "In Progress" ],
                "MappedState": "In Progress",
                "StateType": "InProgress",
                "Sequence": 2
              },
              {
                "IssueType": "Bug",
                "JiraStates": [ "Closed" ],
                "MappedState": "Done",
                "StateType": "Done",
                "Sequence": 3
              },
              {
                "IssueType": "Story",
                "JiraStates": [ "Ready", "New" ],
                "MappedState": "To Do",
                "StateType": "ToDo",
                "Sequence": 1
              },
              {
                "IssueType": "Story",
                "JiraStates": [ "In Progress" ],
                "MappedState": "In Progress",
                "StateType": "InProgress",
                "Sequence": 2
              },
              {
                "IssueType": "Story",
                "JiraStates": [ "Accepted" ],
                "MappedState": "Done",
                "StateType": "Done",
                "Sequence": 3
              }
            ]
          }        
        ]
      }
    }
  
  Consider the following when setting up mappings for each team or organization:
  - Only map a single "To Do" status (likely the first state on Jira team board)
  - Only map a single "Done" status (likely the last state on Jira board)
  - Make sure all jira statuses are mapped to the same set of "MappedStates" (i.e. Team Awesome Sauce's example above maps to the same "To Do", "In Progress" and "Done" mapped states for both Bug and Story issue types).
  - Setting a proper sequence is important for Jira Connect to calculate the correct flow metrics. Typically, the "ToDo" state is the first in the sequence and "Done" should be the last.
  - Set up your configuration for each team/org once and enable/disable them as they are needed.

  
  ### Output
  
  Running Jira Connect will result in a csv file for each configured (and enabled) mapping. Each csv file will include the following data:
  
| Field/Heading           | Description                                                                                                                |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Key                     | The external identifier used for each issue in Jira                                                                        |
| Description             | The Summary field for the issue                                                                                            |
| Type                    | Jira issue type                                                                                                            |
| Team                    | The "TeamName" specified in the mapping                                                                                    |
| Status                  | The current status of the Jira issue. The raw value of the status field and not mapped using the configured workflow       |
| Labels                  | Labels associated with the Jira issue.                                                                                     |
| InProgressDate          | Calculated date for when the issue entered an "InProgress" state, based on the workflow configured in the mapping          |
| DoneDate                | Calculated date for when the issue entered a "Done" state, based on the workflow configured in the mapping                 |
| CycleTime               | The calculated number of days from "InProgressDate" and "DoneDate", excluding weekends                                     |
| {Flow State Fields}     | A column for each state configured in the workflow mapping, populated with the date that the issue entered the state       |
| {Time-in-State fields}  | A column for each state configured in the workflow mapping, populated with the calculated number of days spent in the state|

### Calculating Flow Metrics

Jira Connect will calculate flow metric data and include it in the export. The data calculated includes the following:
- **InProgressDate**: This is the date that the issue enters an "In Progress" state, as determined by the workflow mapping.
- **DoneDate**: This is the date that the issue enters a "Done" state, as determined by the workflow mapping. There should only be a single "Done" date mapped in the workflow; the date will only be calculated for the first "Done" state mapped in the workflow.
- **Flow States**: Each state configured in the workflow mapping will generate a column that displays the date when an issue enters that state.
- **Time-In-State**: Each state configured in the workflow mapping will generate a column that displays the amount of week days the issue was in that state.

> ### What happens if issues enters the same state multiple times?
>
> Even though we encourage preserving flow and not moving issues backwards, it can happen sometimes. Jira Connect will consider the following when determining which dates to use for each state:
> - Finds the last time it entered a mapped "ToDo" state.
> - Finds the first time it entered a "InProgress" state, given that it is after the previous state in the workflow sequence.
> - Finds the last time it entered a "Done" state.
>
> These considerations are used for all of the calculated flow metrics (InProgressDate, DoneDate, flow state and time-in-state fields).
>
> If we use our example above for team Awesome Sauce, Jira Connect will report on the last time a story entered a "Ready" or "New" status in Jira (which is mapped to our "To Do" state), the first time it entered a status of "In Progress" (as long as it was after the date for the "To Do" state), and the last time it entered the jira status of "Accepted" (mapped to "Done" in our workflow).
> Jira Connect will also account for issues that have moved all the way through our workflow to a "Done" state and back to a "To Do" state.

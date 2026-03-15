For execution: pnpm run dev

# Express - Typescript
--------------------------------------------------------------------------------------------------
| UC-## | Use Case Name                     | Primary Actor      | Module                | Status |
|-------|-----------------------------------|------------------  | --------------------- | ------ |
| UC-06	| Manage Collaborators              | Writer             | Management            |   ❌   |
| UC-07	| Configure Privacy                 | Writer             | Management            |   ❌   |
| UC-08	| Edit Manuscript (Rich Text)       | Editor / Writer    | Writing (MongoDB)     |   ❌   |
| UC-09	| Manage Wiki (Nodes)               | Editor / Writer    | Worldbuilding (Neo4j) |   ❌   |
| UC-10	| Visualize Relations (Graph)       | Reader / Editor    | Worldbuilding (Neo4j) |   ❌   |
| UC-15 | Manage Users (Ban/Roles)          | Administrator      | Admin                 |   ❌   |

## UC-06 Manage Collaborators
### Description
The Writer invites other users to participate in the work, assigning them specific roles (Editor) to enable co-authorship or proofreading.
### Preconditions
The user must be the Project Owner (Role Owner).
### Postconditions
POS-1: The guest user can now view and edit the project on their own Dashboard.
### Normal Flow
1.	The System displays the current list of collaborators.
2.	The Writer enters the email address of the user to invite.
3.	The System searches for the user in the identity database (SQL Server).
4.	The System validates that the user exists (FA-01).
5.	The Writer selects the role to assign (e.g., "Editor").
6.	The System registers the permission in the project's Roles table.
7.	The System sends a notification to the invited user.
8.	UC ends.

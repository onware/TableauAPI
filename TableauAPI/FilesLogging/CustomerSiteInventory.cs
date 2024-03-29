﻿using System;
using System.Collections.Generic;
using System.Linq;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.FilesLogging
{
    /// <summary>
    /// Management class for site inventory
    /// </summary>
    internal class CustomerSiteInventory : CsvDataGenerator
    {
        private const string ContentType = "content-type";
        private const string ContentUrl = "content-url";
        private const string ContentConnectionServer = "connection-server";
        private const string ContentConnectionType = "connection-type";
        private const string ContentConnectionPort = "connection-port";
        private const string ContentConnectionUserName = "connection-user-name";
        private const string ContentProjectId = "project-id";
        private const string ContentWorkbookId = "workbook-id";
        private const string ContentWorkbookName = "workbook-name";
        private const string ContentProjectName = "project-name";
        private const string ContentUserId = "user-id";
        private const string ContentUserName= "user-name";
        private const string ContentGroupId = "group-id";
        private const string ContentGroupName = "group-name";
        private const string ContentId = "id";
        private const string ContentName = "name";
        private const string ContentDescription = "description";
        private const string ContentOwnerId = "owner-id";
        private const string ContentOwnerName = "owner-name";
        private const string ContentTags = "tags";
        private const string WorkbookShowTabs = "workbook-show-tabs";
        private const string SiteRole = "user-role";
        private const string DeveloperNotes = "developer-notes";

        /// <summary>
        /// Efficent store for looking up user names
        /// </summary>
        private readonly KeyedLookup<SiteUser> _siteUserMapping;

        /// <summary>
        /// Status log data
        /// </summary>
        public readonly ITaskStatusLogger StatusLog;

        /// <summary>
        /// Constructor.  Builds the data for the CSV file
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="dataSources"></param>
        /// <param name="workbooks"></param>
        /// <param name="users"></param>
        /// <param name="groups"></param>
        /// <param name="statusLogger"></param>
        public CustomerSiteInventory(
            IEnumerable<SiteProject> projects, 
            IEnumerable<SiteDatasource> dataSources,
            IEnumerable<SiteWorkbook> workbooks,
            IEnumerable<SiteUser> users,
            IEnumerable<SiteGroup> groups,
            ITaskStatusLogger statusLogger)
        {
            //Somewhere to store status logs
            if (statusLogger == null)
            {
                statusLogger = new TaskStatusLogs();
            }
            StatusLog = statusLogger;

            //If we have a user-set, put it into a lookup class so we can quickly look up user names when we write out other data
            //that has user ids
            var siteUsers = users as IList<SiteUser> ?? users.ToList();
            if(users != null)
            {
                _siteUserMapping = new KeyedLookup<SiteUser>(siteUsers);
            }

            AddProjectsData(projects);
            AddDatasourcesData(dataSources);
            AddWorkbooksData(workbooks);
            AddUsersData(siteUsers);
            AddGroupsData(groups);
        }

        /// <summary>
        /// Attempt to look up the name of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string helper_AttemptUserNameLookup(string userId)
        {
            try
            {
                return helper_AttemptUserNameLookup_inner(userId);
            }
            catch(Exception ex)
            {
                StatusLog.AddError("Error looking up user id", ex);
                return "** Error in user lookup **"; //Continue onward
            }
        }

        /// <summary>
        /// Attempt to look up the name of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string helper_AttemptUserNameLookup_inner(string userId)
        {
            var userIdMap = _siteUserMapping;
            //If we have no user mapping, then return a blank
            if(userIdMap == null)
            {
                return "";
            }

            //We always expect to find the user
            var user = userIdMap.FindItem(userId);
            if(user == null)
            {
                throw new Exception("User ID cannot be mapped '" + userId + "'");
            }
            return user.Name;
        }


        /// <summary>
        /// Add CSV for all the data sources
        /// </summary>
        /// <param name="dataSources"></param>
        private void AddDatasourcesData(IEnumerable<SiteDatasource> dataSources)
        {
            //No data sources? Do nothing.
            if (dataSources == null) return;

            //Add each data source as a row in the CSV file we will generate
            foreach (var thisDatasource in dataSources)
            {
                //Attempt to look up the owner name.  This will be blank if we do not have a users list
                string ownerName = helper_AttemptUserNameLookup(thisDatasource.OwnerId);

                AddKeyValuePairs(
                    new[] { 
                        ContentType         //1
                        ,ContentId           //2
                        ,ContentName         //3
                        ,ContentProjectId    //4
                        ,ContentProjectName  //5
                        ,ContentOwnerId      //6
                        ,ContentOwnerName    //7
                        ,ContentTags         //8
                        ,DeveloperNotes      //9
                    },
                    new[] { 
                        "datasource"                //1
                        ,thisDatasource.Id           //2
                        ,thisDatasource.Name         //3
                        ,thisDatasource.ProjectId    //4
                        ,thisDatasource.ProjectName  //5
                        ,thisDatasource.OwnerId      //6
                        ,ownerName                   //7
                        ,thisDatasource.TagSetText   //8
                    });
            }
        }

        /// <summary>
        /// Add CSV for all the data sources
        /// </summary>
        /// <param name="workbooks"></param>
        private void AddWorkbooksData(IEnumerable<SiteWorkbook> workbooks)
        {
            //None? Do nothing.
            if (workbooks == null) return;

            //Add each data source as a row in the CSV file we will generate
            foreach (var thisWorkbook in workbooks)
            {
                //Attempt to look up the owner name.  This will be blank if we do not have a users list
                string ownerName = helper_AttemptUserNameLookup(thisWorkbook.OwnerId);

                AddKeyValuePairs(
                    new [] { 
                        ContentType            //1 
                        ,ContentId              //2
                        ,ContentName            //3
                        ,ContentWorkbookId      //4
                        ,ContentWorkbookName    //5
                        ,ContentUrl             //6
                        ,ContentProjectId       //7
                        ,ContentProjectName     //8
                        ,ContentOwnerId         //9
                        ,ContentOwnerName       //10
                        ,WorkbookShowTabs       //11
                        ,ContentTags            //12
                        ,DeveloperNotes         //13
                    },
                    new [] { 
                        "workbook"                //1 
                        ,thisWorkbook.Id          //2
                        ,thisWorkbook.Name        //3
                        ,thisWorkbook.Id          //4
                        ,thisWorkbook.Name        //5
                        ,thisWorkbook.ContentUrl  //6
                        ,thisWorkbook.ProjectId   //7
                        ,thisWorkbook.ProjectName //8
                        ,thisWorkbook.OwnerId     //9
                        ,ownerName                //10
                        ,XmlHelper.BoolToXmlText(thisWorkbook.ShowTabs) //11
                        ,thisWorkbook.TagSetText  //12
                        ,thisWorkbook.DeveloperNotes //13
                    });

                //If we have workbooks connections information then log that
                AddWorkbookConnectionData(thisWorkbook);
            }
        }

        /// <summary>
        /// Add data source connection data
        /// </summary>
        /// <param name="thisWorkbook"></param>
        private void AddWorkbookConnectionData(SiteWorkbook thisWorkbook)
        {
            var dataConnections = thisWorkbook.DataConnections;
            if(dataConnections == null)
            {
                return;
            }

            //Write out details for each data connection
            foreach (var thisConnection in dataConnections)
            {
                AddKeyValuePairs(
                    new[] { 
                        ContentType               //1 
                        ,ContentId                //2
                        ,ContentConnectionType    //3
                        ,ContentConnectionServer  //4
                        ,ContentConnectionPort    //5
                        ,ContentConnectionUserName//6
                        ,ContentWorkbookId        //7
                        ,ContentWorkbookName      //8
                        ,ContentProjectId         //9
                        ,ContentProjectName       //10
                        ,ContentOwnerId           //11
                        ,DeveloperNotes           //12
                    },
                    new[] { 
                        "data-connection"              //1 
                        ,thisConnection.Id             //2
                        ,thisConnection.ConnectionType //3
                        ,thisConnection.ServerAddress  //4
                        ,thisConnection.ServerPort     //5
                        ,thisConnection.UserName       //6
                        ,thisWorkbook.Id               //7
                        ,thisWorkbook.Name             //8
                        ,thisWorkbook.ProjectId        //9
                        ,thisWorkbook.ProjectName      //10
                        ,thisWorkbook.OwnerId          //11
                        ,thisWorkbook.DeveloperNotes   //12
                    });
            }
        }

        /// <summary>
        /// Add CSV rows for all the projects data
        /// </summary>
        /// <param name="projects"></param>
        private void AddProjectsData(IEnumerable<SiteProject> projects)
        {
            //No data to add? do nothing.
            if (projects == null) return;

            //Add each project as a row in the CSV file we will generate
            foreach (var thisProject in projects)
            {
                AddKeyValuePairs(
                    new[] { 
                        ContentType               //1
                        ,ContentId                 //2
                        ,ContentName               //3
                        ,ContentProjectId          //4
                        ,ContentProjectName        //5
                        ,ContentDescription        //6
                        ,DeveloperNotes            //7
                    },      
                    new[] { 
                        "project"                  //1
                        ,thisProject.Id             //2
                        ,thisProject.Name           //3
                        ,thisProject.Id             //4
                        ,thisProject.Name           //5
                        ,thisProject.Description    //6
                        ,thisProject.DeveloperNotes //7
                    });       
            } 
        }

        /// <summary>
        /// Add CSV rows for all the groups data
        /// </summary>
        /// <param name="groups"></param>
        private void AddGroupsData(IEnumerable<SiteGroup> groups)
        {
            //No data to add? do nothing.
            if (groups == null) return;

            //Add each project as a row in the CSV file we will generate
            foreach (var thisGroup in groups)
            {
                AddKeyValuePairs(
                    new[] { 
                        ContentType               //1
                        ,ContentId                 //2
                        ,ContentName               //3
                        ,ContentGroupId            //4
                        ,ContentGroupName          //5
                        ,DeveloperNotes            //6
                    },
                    new[] { 
                        "group"                   //1
                        ,thisGroup.Id              //2
                        ,thisGroup.Name            //3
                        ,thisGroup.Id              //4
                        ,thisGroup.Name            //5
                        ,thisGroup.DeveloperNotes  //6
                    });

                //-------------------------------------------------------------------------
                //Add the set of users that are members of the group
                //-------------------------------------------------------------------------
                AddGroupUsersData(thisGroup);
            }
        }

        /// <summary>
        /// CSV rows for each member of a group
        /// </summary>
        /// <param name="group"></param>
        private void AddGroupUsersData(SiteGroup group)
        {
            var usersInGroup = group.Users;
            //Nothing to add?
            if(usersInGroup == null)
            {
                return;
            }

            string groupId = group.Id;
            string groupName = group.Name;

            foreach(var thisUser in usersInGroup)
            {
                AddKeyValuePairs(
                    new[] { 
                        ContentType               //1
                        ,ContentId                 //2
                        ,ContentName               //3
                        ,SiteRole                  //4
                        ,ContentUserId             //5
                        ,ContentUserName           //6
                        ,ContentGroupId            //7
                        ,ContentGroupName          //8
                        ,DeveloperNotes            //9
                    },
                    new[] { 
                        "group-member"            //1
                        ,thisUser.Id               //2
                        ,thisUser.Name             //3
                        ,thisUser.SiteRole         //4
                        ,thisUser.Id               //5
                        ,thisUser.Name             //6
                        ,groupId                   //7
                        ,groupName                 //8
                        ,thisUser.DeveloperNotes   //9
                    });

            }
        }

        /// <summary>
        /// Add CSV rows for all the users data
        /// </summary>
        /// <param name="users"></param>
        private void AddUsersData(IEnumerable<SiteUser> users)
        {
            //No data to add? do nothing.
            if (users == null) return;

            //Add each project as a row in the CSV file we will generate
            foreach (var thisUser in users)
            {
                AddKeyValuePairs(
                    new[] { 
                        ContentType         //1 
                        ,ContentId          //2
                        ,ContentName        //3
                        ,SiteRole           //4
                        ,ContentUserId      //5
                        ,ContentUserName    //6
                        ,DeveloperNotes     //7
                    },
                    new[] { 
                        "user"                   //1
                        ,thisUser.Id             //2
                        ,thisUser.Name           //3
                        ,thisUser.SiteRole       //4
                        ,thisUser.Id             //5
                        ,thisUser.Name           //6
                        ,thisUser.DeveloperNotes //7
                    });
            }
        }
    }
}
 
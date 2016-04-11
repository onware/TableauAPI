using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Grou[ in a Server's site
    /// </summary>
    public class SiteGroup : IHasSiteItemId
    {
        /// <summary>
        /// Group ID
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Name of the Group
        /// </summary>
        public readonly string Name;

        
        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        private readonly List<SiteUser> _usersInGroup;
        /// <summary>
        /// Returns the list of users associated with this group
        /// </summary>
        public ICollection<SiteUser> Users => _usersInGroup.AsReadOnly();

        /// <summary>
        /// Constructs an instance of SiteGroup based on XML from the Tableau server,
        /// and associates a collection of SiteUsers with it
        /// </summary>
        /// <param name="projectNode"></param>
        /// <param name="usersToPlaceInGroup"></param>
        public SiteGroup(XmlNode projectNode, IEnumerable<SiteUser> usersToPlaceInGroup)
        {
            //If we were passed in a set of users, store them
            var usersList = new List<SiteUser>();
            if(usersToPlaceInGroup != null)
            {
                usersList.AddRange(usersToPlaceInGroup);
            }
            _usersInGroup = usersList;


            if(projectNode.Name.ToLower() != "group")
            {
                AppDiagnostics.Assert(false, "Not a group");
                throw new Exception("Unexpected content - not group");
            }

            Id = projectNode.Attributes?["id"].Value;
            Name = projectNode.Attributes?["name"].Value;
        }

        /// <summary>
        /// Name and ID of the Group
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Group: " + Name + "/" + Id;
        }

        /// <summary>
        /// Adds a set of users.  This is typically called when initializing this object.
        /// </summary>
        /// <param name="usersList"></param>
        internal void AddUsers(IEnumerable<SiteUser> usersList)
        {
            //Nothing to add?
            if (usersList == null)
            {
                return;
            }

            _usersInGroup.AddRange(usersList);
        }

        /// <summary>
        /// Group ID
        /// </summary>
        string IHasSiteItemId.Id => Id;
    }
}

using TableauAPI.ServerData;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Questions everything that manages a set of projects needs to be able to answer
    /// </summary>
    public interface IProjectsList
    {
        /// <summary>
        /// Find a Project based on its ID
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>A Tableau Site Project</returns>
        SiteProject FindProjectWithId(string projectId);

        /// <summary>
        /// Find a Project based on its name
        /// </summary>
        /// <param name="projectName">Project Name</param>
        /// <returns>A Tableau Site Project</returns>
        SiteProject FindProjectWithName(string projectName);
    }
}

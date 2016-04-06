namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// What is the behavior during uploads when we cannot find a matching pre-existing projec
    /// </summary>
    internal class UploadBehaviorProjects
    {
        public readonly bool AttemptProjectCreate;
        public readonly bool UseDefaultProjectIfNeeded;

        public UploadBehaviorProjects(bool attemptCreate, bool allowDefaultIfNeeded)
        {
            this.AttemptProjectCreate = attemptCreate;
            this.UseDefaultProjectIfNeeded = allowDefaultIfNeeded;
        }
    }
}

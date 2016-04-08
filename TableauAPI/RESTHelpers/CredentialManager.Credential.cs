namespace TableauAPI.RESTHelpers
{

    /// <summary>
    /// Manages credentials used to connect to the Tableau server.
    /// </summary>
    internal partial class CredentialManager
    {
        /// <summary>
        /// A client's credentials.
        /// </summary>
        public class Credential
        {
            /// <summary>
            /// The Tableau user name
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// The Tableau user's password
            /// </summary>
            public readonly string Password;

            /// <summary>
            /// true if the credentials are embedded; false otherwise
            /// </summary>
            public readonly bool IsEmbedded;

            /// <summary>
            /// Construct an instance of a Credential.
            /// </summary>
            /// <param name="name">The Tableau user name</param>
            /// <param name="password">The Tableau user's password</param>
            /// <param name="isEmbedded">true if the credentials are embedded; false otherwise</param>
            public Credential(string name, string password, bool isEmbedded)
            {
                Name = name;
                Password = password;
                IsEmbedded = isEmbedded;
            }
        }

    }
}

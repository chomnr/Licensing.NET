namespace License_Server.Services.User
{
    public class UserSession
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// After obtaining the user's information through their session id call this method
        /// UserSession. The Licensing system just needs their ID the username field can be nullable.
        /// </summary>
        /// <param name="id"></param>
        public UserSession(string id)
        {
            Id = id;
        }
    }
}

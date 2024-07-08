namespace www.backupserver.com.Service
{
    public class TokenStorage
    {
        private string _token;

        public string GetToken()
        {
            return _token;
        }

        public void SetToken(string token)
        {
            _token = token;
        }
    }

}

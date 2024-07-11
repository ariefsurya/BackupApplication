namespace Model
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public string Token { get; set; }

        public UserDTO UserMapToDto(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyId = user.CompanyId,
                Token = user.Token,
            };
        }
    }
}

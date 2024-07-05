using System;

namespace Model
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public UserDTO oUser { get; set; }


        private CompanyDTO MapToDto(Company oCompany, UserDTO oUserDTO)
        {
            return new CompanyDTO
            {
                Id = oCompany.Id,
                CompanyName = oCompany.CompanyName,
                oUser = oUserDTO
            };
        }
    }
}

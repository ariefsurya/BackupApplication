using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthoriazation.Models
{
    public class Claims
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TypeName { get; set; }
    }
}

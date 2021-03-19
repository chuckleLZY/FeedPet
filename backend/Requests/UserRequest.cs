using System.ComponentModel.DataAnnotations;

namespace SyaApi.Requests
{
    public class UserRequest
    {
        [Required]
        public bool gender { get; set; }
        [Required]
        public string avatar { get; set; }
        [Required]
        public string tel { get; set; }
        [Required]
        public string bank { get; set; }
    }

    public class ApplyUserRequest{
        [Required]
        public int user_id { get; set; }
    }
}

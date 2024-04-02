using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_TRADUCTOR.Models
{
    public class History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public DateTime RegDate { get; set; }
    }
}

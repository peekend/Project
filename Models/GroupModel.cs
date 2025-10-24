using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Diplom2.Models
{
    public class GroupModel
    {
        [Required(ErrorMessage = "Введите название группы")]
        [StringLength(120, ErrorMessage = "Название группы не должно превышать 120 символов")]
        [Display(Name = "Название группы")]
        public string? GroupName { get; set; }
    }
}

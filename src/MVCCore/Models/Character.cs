using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCore.Models
{
    public class Character
    {
        [Key]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; } = 10;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public List<QuestItem> Items { get; set; }
    }

    public class QuestItem
    {
        public string Name { get; set; }

        public int Level { get; set; }
    }
}

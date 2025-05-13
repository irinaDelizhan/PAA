using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("Section")]
public partial class Section
{
    [Key]
    [Column("sectionId")]
    public int SectionId { get; set; }

    [Column("sectionName")]
    [StringLength(50)]
    [Unicode(false)]
    public string SectionName { get; set; } = null!;

    [InverseProperty("Section")]
    public virtual ICollection<UserPaa> UserPaas { get; set; } = new List<UserPaa>();
}

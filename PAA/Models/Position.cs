using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("Position")]
public partial class Position
{
    [Key]
    [Column("positionId")]
    public int PositionId { get; set; }

    [Column("positionName")]
    [StringLength(50)]
    [Unicode(false)]
    public string PositionName { get; set; } = null!;

    [Column("salary", TypeName = "decimal(10, 2)")]
    public decimal Salary { get; set; }

    [InverseProperty("Position")]
    public virtual ICollection<UserPaa> UserPaas { get; set; } = new List<UserPaa>();
}

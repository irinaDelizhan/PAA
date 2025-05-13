using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("State")]
public partial class State
{
    [Key]
    [Column("stateId")]
    public int StateId { get; set; }

    [Column("description")]
    [StringLength(255)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [Column("projectId")]
    public int ProjectId { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [Column("date", TypeName = "datetime")]
    public DateTime Date { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("States")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("States")]
    public virtual UserPaa User { get; set; } = null!;
}

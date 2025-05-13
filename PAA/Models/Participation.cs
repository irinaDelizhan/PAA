using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("Participation")]
public partial class Participation
{
    [Key]
    [Column("participationId")]
    public int ParticipationId { get; set; }

    [Column("startDate", TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column("endDate", TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [Column("projectId")]
    public int ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Participations")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Participations")]
    public virtual UserPaa User { get; set; } = null!;
}

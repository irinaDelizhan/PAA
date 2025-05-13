using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("Project")]
public partial class Project
{
    [Key]
    [Column("projectId")]
    public int ProjectId { get; set; }

    [Column("projectName")]
    [StringLength(50)]
    [Unicode(false)]
    public string ProjectName { get; set; } = null!;

    [Column("headId")]
    public int HeadId { get; set; }

    [Column("startDate", TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column("expectedEndDate", TypeName = "datetime")]
    public DateTime ExpectedEndDate { get; set; }

    [Column("actualEndDate", TypeName = "datetime")]
    public DateTime? ActualEndDate { get; set; }

    [Column("executionStatus")]
    [StringLength(20)]
    [Unicode(false)]
    public string ExecutionStatus { get; set; } = null!;

    [ForeignKey("HeadId")]
    [InverseProperty("Projects")]
    public virtual UserPaa Head { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    [InverseProperty("Project")]
    public virtual ICollection<State> States { get; set; } = new List<State>();
}

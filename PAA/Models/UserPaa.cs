using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("UserPAA")]
[Index("Login", Name = "UQ__UserPAA__7838F27227747122", IsUnique = true)]
public partial class UserPaa
{
    [Key]
    [Column("userId")]
    public int UserId { get; set; }

    [Column("name")]
    [StringLength(20)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("surname")]
    [StringLength(20)]
    [Unicode(false)]
    public string Surname { get; set; } = null!;

    [Column("patronymic")]
    [StringLength(20)]
    [Unicode(false)]
    public string Patronymic { get; set; } = null!;

    [Column("login")]
    [StringLength(20)]
    [Unicode(false)]
    public string Login { get; set; } = null!;

    [Column("password")]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("address")]
    [StringLength(50)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [Column("sectionId")]
    public int SectionId { get; set; }

    [Column("positionId")]
    public int PositionId { get; set; }

    [Column("startDate", TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column("endDate", TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    [ForeignKey("PositionId")]
    [InverseProperty("UserPaas")]
    public virtual Position Position { get; set; } = null!;

    [InverseProperty("Head")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [ForeignKey("SectionId")]
    [InverseProperty("UserPaas")]
    public virtual Section Section { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<State> States { get; set; } = new List<State>();

    [InverseProperty("User")]
    public virtual ICollection<TransactionPaa> TransactionPaas { get; set; } = new List<TransactionPaa>();
}

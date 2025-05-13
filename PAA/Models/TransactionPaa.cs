using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

[Table("TransactionPAA")]
public partial class TransactionPaa
{
    [Key]
    [Column("transactionId")]
    public int TransactionId { get; set; }

    [Column("type")]
    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column("description")]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [Column("date", TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TransactionPaas")]
    public virtual UserPaa User { get; set; } = null!;
}

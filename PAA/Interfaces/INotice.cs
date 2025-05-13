using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAA.Interfaces
{
    public interface INotice
    {
        string? Description { get; set; }
        DateTime? Date {  get; set; }
    }
}

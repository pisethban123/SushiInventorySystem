using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiInventorySystem.Models
{
    public interface IReport
    {
        string GenerateSummary();
    }
}

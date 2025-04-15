using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Inventory
{
    public class InvoiceModel
    {

        public decimal Amount { get; set; }

        public string ClientName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime DueDate { get; set; }
    }
}

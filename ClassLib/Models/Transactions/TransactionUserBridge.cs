using ClassLib.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.Models.Transactions
{
    public class TransactionUserBridge
    {
        public required string Id { get; set; }
        public required string TransactionId { get; set; }
        public required string UserId { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public required UserDTO User { get; set; }
        public required TransactionDTO Transaction { get; set; }
    }
}

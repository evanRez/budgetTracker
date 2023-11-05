using ClassLib.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassLib.Models.Users
{
    public class UserDTO
    {
        public required string Id { get; set; }
        //public required string Auth0UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? Picture {  get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public Collection<TransactionDTO>? Transactions { get; set; }

    }
}

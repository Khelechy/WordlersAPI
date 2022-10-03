using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordlersAPI.Models.Core
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public bool InRound { get; set; }
        public virtual List<int> Users { get; set; }
        public string RoomId { get; set; }
        public int NumberOfRounds { get; set; }
        public int RoundDuration { get; set; }
        public int TimeInBetweenRound { get; set; }
        public virtual ICollection<UserGamePoint> UserGamePoints { get; set; }
    }
}

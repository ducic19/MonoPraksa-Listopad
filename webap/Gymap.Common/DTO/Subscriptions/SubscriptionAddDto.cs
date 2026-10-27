using System.ComponentModel.DataAnnotations;

namespace Gymap.Common.DTO.Subscriptions
{
    // Weekly = broj dolazaka tjedno (1-7), Duration = trajanje pretplate u tjednima.
    public class SubscriptionAddDto
    {
        [Range(1, 7, ErrorMessage = "Tjedni broj dolazaka mora biti između 1 i 7.")]
        public int Weekly { get; set; }

        [Range(1, 520, ErrorMessage = "Trajanje mora biti između 1 i 520 tjedana.")]
        public int Duration { get; set; }

        [Range(0, 100000, ErrorMessage = "Cijena mora biti između 0 i 100000.")]
        public decimal Price { get; set; }
    }
}

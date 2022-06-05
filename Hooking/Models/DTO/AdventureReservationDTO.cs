using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hooking.Models.DTO
{
    public class AdventureReservationDTO
    {
        // adventure
        public string AdventureId { get; set; }
        public string InstructorId { get; set; }
        [Display(Name = "Naziv avanture")]
        public string AdventureName { get; set; }

        // adventure realisation
        public string AdventureRealisationId { get; set; }
        [Display(Name = "Trajanje")]
        public string Duration { get; set; }
        [Display(Name = "Cena")]
        public string Price { get; set; }
        [Display(Name = "Datum početka")]
        public string StartDate { get; set; }

        // adventure reservation
        public string AdventureReservationId { get; set; }
        public string UserDetailsId { get; set; }
        [Display(Name = "Ime korisnika")]
        public string UserDetailsFirstName { get; set; }
        [Display(Name = "Prezime korisnika")]
        public string UserDetailsLastName { get; set; }
        public bool IsReviewed { get; set; }
        public AdventureReservationDTO(){}

        public AdventureReservationDTO(Adventure adventure, AdventureRealisation realisation, AdventureReservation reservation)
        {
            AdventureId = adventure.Id.ToString();
            InstructorId = adventure.InstructorId;
            AdventureName = adventure.Name;

            AdventureRealisationId = realisation.Id.ToString();
            Duration = realisation.Duration.ToString();
            Price = realisation.Price.ToString();
            StartDate = realisation.StartDate.ToString();

            AdventureReservationId = reservation.Id.ToString();
            UserDetailsId = reservation.UserDetailsId;
            IsReviewed = reservation.IsReviewed;
        }

    }
}

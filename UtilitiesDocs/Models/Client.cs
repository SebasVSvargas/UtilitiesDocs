using System;

namespace UtilitiesDocs.Models
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identificador único
        public string FirstName { get; set; } = string.Empty; // Nombres
        public string LastName { get; set; } = string.Empty; // Apellidos
        public string IdNumber { get; set; } = string.Empty; // Cédula
        public string PhoneNumber { get; set; } = string.Empty; // Celular
        public string Address { get; set; } = string.Empty; // Dirección
        public int BirthYear { get; set; } // Año de nacimiento

        public string FullName => $"{FirstName} {LastName}";
        public string FullDetails => $"{FullName} - {IdNumber}";
    }
}

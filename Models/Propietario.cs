using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Models;

[Index("Dni", Name = "UQ_Propietarios_Dni", IsUnique = true)]
public partial class Propietario
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Los nombres deben tener entre 3 y 100 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI solo debe contener números.")]
    [Unicode(false)]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(15, MinimumLength = 7, ErrorMessage = "El teléfono debe tener entre 7 y 15 caracteres.")]
    public string Telefono { get; set; } = null!;

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }

    // [JsonIgnore] evita el ciclo Propietario -> Mascotas -> Propietario al serializar.
    // Las mascotas de un propietario se consultan con GET api/propietarios/{id}/mascotas.
    [JsonIgnore]
    [InverseProperty("Propietario")]
    public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}

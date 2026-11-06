using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaAPI.Models;

namespace VeterinariaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly VeterinariaContext _context;

    public MascotasController(VeterinariaContext context) => _context = context;

    // GET api/mascotas?especie=perro&propietarioId=1&buscar=fir&edadMax=5
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mascota>>> Get(
        string? especie, int? propietarioId, string? buscar, int? edadMax)
    {
        var consulta = _context.Mascotas.Include(m => m.Propietario).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);
        if (propietarioId != null)
            consulta = consulta.Where(m => m.PropietarioId == propietarioId);
        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(m => EF.Functions.Collate(m.Nombre, "Latin1_General_CI_AI").Contains(buscar));
        if (edadMax != null)
            consulta = consulta.Where(m => m.Edad <= edadMax);

        var mascotas = await consulta.OrderBy(m => m.Nombre).ToListAsync();
        return Ok(mascotas);
    }

    // GET api/mascotas/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Mascota>> Get(int id)
    {
        var mascota = await _context.Mascotas
            .Include(m => m.Propietario)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mascota == null) return NotFound();
        return Ok(mascota);
    }

    // POST api/mascotas
    [HttpPost]
    public async Task<ActionResult<Mascota>> Post(Mascota mascota)
    {
        if (!await _context.Propietarios.AnyAsync(p => p.Id == mascota.PropietarioId))
        {
            ModelState.AddModelError(nameof(mascota.PropietarioId), "El propietario indicado no existe.");
            return ValidationProblem(ModelState);
        }

        // Regla de negocio 3: un propietario no puede tener dos mascotas con el mismo nombre
        if (await _context.Mascotas.AnyAsync(m => m.PropietarioId == mascota.PropietarioId && m.Nombre == mascota.Nombre))
            return Conflict(new { mensaje = $"El propietario ya tiene una mascota llamada {mascota.Nombre}." });

        mascota.Id = 0;
        mascota.Propietario = null;
        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = mascota.Id }, mascota);
    }

    // PUT api/mascotas/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Mascota mascota)
    {
        if (id != mascota.Id) return BadRequest();

        var actual = await _context.Mascotas.FindAsync(id);
        if (actual == null) return NotFound();

        if (!await _context.Propietarios.AnyAsync(p => p.Id == mascota.PropietarioId))
        {
            ModelState.AddModelError(nameof(mascota.PropietarioId), "El propietario indicado no existe.");
            return ValidationProblem(ModelState);
        }

        // Regla de negocio 3: un propietario no puede tener dos mascotas con el mismo nombre
        if (await _context.Mascotas.AnyAsync(m => m.PropietarioId == mascota.PropietarioId
                                                && m.Nombre == mascota.Nombre && m.Id != id))
            return Conflict(new { mensaje = $"El propietario ya tiene otra mascota llamada {mascota.Nombre}." });

        actual.Nombre = mascota.Nombre;
        actual.Especie = mascota.Especie;
        actual.Raza = mascota.Raza;
        actual.Edad = mascota.Edad;
        actual.Peso = mascota.Peso;
        actual.PropietarioId = mascota.PropietarioId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/mascotas/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota == null) return NotFound();

        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

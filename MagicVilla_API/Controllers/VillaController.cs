using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaController : ControllerBase
	{
		private readonly ILogger<VillaController> _logger;
		private readonly IVillaRepositorio _villaRepo;
		private readonly IMapper _mapper;
		protected APIResponse _apiResponse;

		public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo,IMapper mapper)
		{
			_logger = logger;
			_villaRepo = villaRepo;
			_mapper = mapper;
			_apiResponse = new();

		}

		[HttpGet]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			try
			{
				_logger.LogInformation("Obtener las villas");

				IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

				_apiResponse.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
				_apiResponse.statusCode = System.Net.HttpStatusCode.OK;

				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{
				_apiResponse.IsExitoso= false;
				_apiResponse.ErrorMessages = new List<string>() { ex.ToString() };

			}
			return _apiResponse;
			
		}

		[HttpGet("id:int",Name ="GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_logger.LogError("Error al traer Villa con Id " + id);
					_apiResponse.statusCode = System.Net.HttpStatusCode.BadRequest;
					_apiResponse.IsExitoso=false;	
					return BadRequest(_apiResponse);
				}
				//var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
				var villa = await _villaRepo.Obtener(x => x.Id == id);
				if (villa == null)
				{
					_apiResponse.statusCode= System.Net.HttpStatusCode.NotFound;
					_apiResponse.IsExitoso=false;
					return NotFound(_apiResponse);
				}

				_apiResponse.Resultado = _mapper.Map<VillaDto>(villa);
				_apiResponse.statusCode=System.Net.HttpStatusCode.OK;
				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{

				_apiResponse.IsExitoso= false;
				_apiResponse.ErrorMessages=new List<string> { ex.ToString() };

			}
			return _apiResponse;
			
			
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
				{
					ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
					return BadRequest(ModelState);
				}
				if (createDto == null)
				{
					return BadRequest();
				}

				Villa modelo = _mapper.Map<Villa>(createDto);

				modelo.FechaCreacion = DateTime.Now;
				modelo.FechaActualizacion=DateTime.Now;
				await _villaRepo.Crear(modelo);

				_apiResponse.Resultado = modelo;
				_apiResponse.statusCode = System.Net.HttpStatusCode.Created;
				return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _apiResponse);
			}
			catch (Exception ex)
			{

				_apiResponse.IsExitoso = false;
				_apiResponse.ErrorMessages=new List<string> { ex.ToString() };
			}
			return _apiResponse;
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_apiResponse.IsExitoso=false;
					_apiResponse.statusCode=System.Net.HttpStatusCode.BadRequest;
					return BadRequest(_apiResponse);
				}
				var villa = await _villaRepo.Obtener(v => v.Id == id);
				if (villa == null)
				{
					_apiResponse.IsExitoso=false;
					_apiResponse.statusCode= System.Net.HttpStatusCode.NotFound;
					return NotFound(_apiResponse);
				}
				await _villaRepo.Remover(villa);
				_apiResponse.statusCode= System.Net.HttpStatusCode.NoContent;
				return Ok(_apiResponse);
			}
			catch (Exception ex)
			{

				_apiResponse.IsExitoso = false;
				_apiResponse.ErrorMessages=new List<string> { ex.ToString() };
			}
			return BadRequest(_apiResponse);
		}

		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
		{
			if(updateDto == null || id!= updateDto.Id)
			{
				_apiResponse.IsExitoso = false;
				_apiResponse.statusCode=System.Net.HttpStatusCode.BadRequest;
				return BadRequest(_apiResponse);
			}
			

			Villa modelo = _mapper.Map<Villa>(updateDto);
			
			await _villaRepo.Actualizar(modelo);
			_apiResponse.statusCode = System.Net.HttpStatusCode.NoContent;
			return Ok(_apiResponse);
		}

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
		{
			if (patchDto == null || id ==0)
			{
				return BadRequest();
			}

			var villa = await _villaRepo.Obtener(v=> v.Id == id,tracked:false);

			VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);
			
			if(villa==null) return BadRequest();
			patchDto.ApplyTo(villaDto, ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			Villa modelo = _mapper.Map<Villa>(villaDto);
			
			await _villaRepo.Actualizar(modelo);
			_apiResponse.statusCode=System.Net.HttpStatusCode.NoContent;
			
			return Ok(_apiResponse);
		}
	}
}

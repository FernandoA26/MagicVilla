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
	public class NumeroVillaController : ControllerBase
	{
		private readonly ILogger<NumeroVillaController> _logger;
		private readonly IVillaRepositorio _villaRepo;
		private readonly INumeroVillaRepositorio _numeroRepo;
		private readonly IMapper _mapper;
		protected APIResponse _apiResponse;

		public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo,INumeroVillaRepositorio numeroRepo,IMapper mapper)
		{
			_logger = logger;
			_villaRepo = villaRepo;
			_numeroRepo = numeroRepo;
			_mapper = mapper;
			_apiResponse = new();

		}

		[HttpGet]
		public async Task<ActionResult<APIResponse>> GetNumeroVillas()
		{
			try
			{
				_logger.LogInformation("Obtener Numeros villas");

				IEnumerable<NumeroVilla> numerovillaList = await _numeroRepo.ObtenerTodos();

				_apiResponse.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numerovillaList);
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

		[HttpGet("id:int",Name ="GetNumeroVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_logger.LogError("Error al traer Numero Villa con Id " + id);
					_apiResponse.statusCode = System.Net.HttpStatusCode.BadRequest;
					_apiResponse.IsExitoso=false;	
					return BadRequest(_apiResponse);
				}
				//var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
				var numeroVilla = await _numeroRepo.Obtener(x => x.VillaNro == id);
				if (numeroVilla == null)
				{
					_apiResponse.statusCode= System.Net.HttpStatusCode.NotFound;
					_apiResponse.IsExitoso=false;
					return NotFound(_apiResponse);
				}

				_apiResponse.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
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
		public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				if (await _numeroRepo.Obtener(v => v.VillaNro == createDto.VillaNro) != null)
				{
					ModelState.AddModelError("NombreExiste", "El numero villa con ese nombre ya existe");
					return BadRequest(ModelState);
				}
				if(await _villaRepo.Obtener(v => v.Id == createDto.VillaId) == null)
				{
					ModelState.AddModelError("ClaveForanea", "El Id de la villa no existe!");
					return BadRequest(ModelState);
				}
				if (createDto == null)
				{
					return BadRequest();
				}

				NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);

				modelo.FechaCreacion = DateTime.Now;
				modelo.FechaActualizacion=DateTime.Now;
				await _numeroRepo.Crear(modelo);

				_apiResponse.Resultado = modelo;
				_apiResponse.statusCode = System.Net.HttpStatusCode.Created;
				return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNro }, _apiResponse);
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
		public async Task<IActionResult> DeleteNumeroVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_apiResponse.IsExitoso=false;
					_apiResponse.statusCode=System.Net.HttpStatusCode.BadRequest;
					return BadRequest(_apiResponse);
				}
				var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNro == id);
				if (numeroVilla == null)
				{
					_apiResponse.IsExitoso=false;
					_apiResponse.statusCode= System.Net.HttpStatusCode.NotFound;
					return NotFound(_apiResponse);
				}
				await _numeroRepo.Remover(numeroVilla);
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
		public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
		{
			if(updateDto == null || id!= updateDto.VillaNro)
			{
				_apiResponse.IsExitoso = false;
				_apiResponse.statusCode=System.Net.HttpStatusCode.BadRequest;
				return BadRequest(_apiResponse);
			}
			if(await _villaRepo.Obtener(v => v.Id == updateDto.VillaId) == null)
			{
				ModelState.AddModelError("ClaveForanea", "El Id de la villa no existe");
				return BadRequest(ModelState);
			}

			NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);
			
			await _numeroRepo.Actualizar(modelo);
			_apiResponse.statusCode = System.Net.HttpStatusCode.NoContent;
			return Ok(_apiResponse);
		}

		
	}
}

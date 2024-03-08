using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientsService _clientsService;
        private IUsersService _usersService;

        public ClientsController(IClientsService clientsService, IUsersService usersService)
        {
            _clientsService = clientsService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<ClientDTO> clientsDTO = _clientsService.GetAllClients();

                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                ClientDTO clientDTO = _clientsService.GetClientById(id);

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = _usersService.GetCurrentClientLoggedEmail(User);

                ClientDTO clientDTO = _clientsService.GetClientByEmail(email);

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost()]
        public IActionResult Post([FromBody] ClientInDTO userDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(userDTO.Email))
                    return StatusCode(403, "email inválido");

                if (string.IsNullOrEmpty(userDTO.Password))
                    return StatusCode(403, "contrseña inválida");

                if (string.IsNullOrEmpty(userDTO.FirstName))
                    return StatusCode(403, "nombre inválido");

                if (string.IsNullOrEmpty(userDTO.LastName))
                    return StatusCode(403, "apellido inválido");

                Client newClient = _clientsService.CreateClient(userDTO);
                
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post()
        {
            try
            {
                string email = _usersService.GetCurrentClientLoggedEmail(User);

                AccountDTO accountDTO = _clientsService.CreateAccount(email);

                return Created("", accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] CardInDTO cardInDTO)
        {
            try
            {
                string email = _usersService.GetCurrentClientLoggedEmail(User);

                CardOutDTO cardOutDTO = _clientsService.CreateCard(email, cardInDTO);

                return Created("", cardOutDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientAccounts()
        {
            try
            {
                string email = _usersService.GetCurrentClientLoggedEmail(User);

                IEnumerable<AccountDTO> accountsDTO = _clientsService.GetAccounts(email);

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientCards()
        {
            try
            {
                string email = _usersService.GetCurrentClientLoggedEmail(User);

                IEnumerable<CardOutDTO> cardsDTO = _clientsService.GetCards(email);

                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}